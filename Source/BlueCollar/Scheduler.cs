//-----------------------------------------------------------------------
// <copyright file="Scheduler.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// Provides scheduling services for scheduled jobs.
    /// </summary>
    public sealed class Scheduler : IScheduler
    {
        private long workerId;
        private string applicationName;
        private int heartbeat;
        private DateTime? lastRefreshOn;
        private List<ScheduleRecord> schedules;
        private QueueNameFilters queueFilters;
        private IRepositoryFactory repositoryFactory;
        private ILogger logger;
        
        /// <summary>
        /// Initializes a new instance of the Scheduler class.
        /// </summary>
        /// <param name="workerId">The ID of the worker the scheduler is scheduling for.</param>
        /// <param name="applicationName">The name of the application to schedule jobs for.</param>
        /// <param name="heartbeat">The heartbeat, in seconds, the system is polled for updates. Used to calculate scheduling windows.</param>
        /// <param name="repositoryFactory">The repository factory to use when accessing data.</param>
        /// <param name="logger">The logger to use when logging messages.</param>
        public Scheduler(long workerId, string applicationName, int heartbeat, IRepositoryFactory repositoryFactory, ILogger logger)
        {
            if (workerId < 1)
            {
                throw new ArgumentOutOfRangeException("workerId", "workerId must be greater than 0.");
            }

            if (string.IsNullOrEmpty(applicationName))
            {
                throw new ArgumentNullException("applicationName", "applicationName must contain a value.");
            }

            if (heartbeat < 1)
            {
                throw new ArgumentOutOfRangeException("heartbeat", "heartbeat must be greater than 0.");
            }

            if (repositoryFactory == null)
            {
                throw new ArgumentNullException("repositoryFactory", "repositoryFactory cannot be null.");
            }

            if (logger == null)
            {
                throw new ArgumentNullException("logger", "logger cannot be null.");
            }

            this.workerId = workerId;
            this.applicationName = applicationName;
            this.heartbeat = heartbeat;
            this.repositoryFactory = repositoryFactory;
            this.logger = logger;
            this.schedules = new List<ScheduleRecord>();
            this.queueFilters = new QueueNameFilters(null);
        }

        /// <summary>
        /// Gets the date the scheduler last performed an equeue operation.
        /// </summary>
        public DateTime? LastEnqueuedOn { get; private set; }

        /// <summary>
        /// Gets or sets the queues to process schedules for.
        /// </summary>
        public QueueNameFilters QueueFilters
        {
            get
            {
                lock (this.queueFilters)
                {
                    return this.queueFilters;
                }
            }

            set
            {
                lock (this.queueFilters)
                {
                    this.queueFilters = value ?? new QueueNameFilters(null);
                }
            }
        }

        /// <summary>
        /// Gets the collection of schedules the scheduler is operating on.
        /// </summary>
        internal IEnumerable<ScheduleRecord> Schedules
        {
            get { return this.schedules; }
        }

        /// <summary>
        /// Gets a value indicating whether a schedule can be enqueued for the given time window.
        /// The schedule can be enqueued if it contains an execution date that falls within the window.
        /// </summary>
        /// <param name="schedule">The schedule to test.</param>
        /// <param name="windowBegin">The begin date of the time window to check.</param>
        /// <param name="windowEnd">The end date of the time window to check.</param>
        /// <param name="scheduleDate">Contains the actual date the schedule falls on within the given window.</param>
        /// <returns>True if the schedule can be enqueued, false otherwise.</returns>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "This is the most elegate design and is similar to the TryParse etc. APIs.")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling is correct.")]
        public bool CanScheduleBeEnqueued(ScheduleRecord schedule, DateTime windowBegin, DateTime windowEnd, out DateTime? scheduleDate)
        {
            if (schedule == null)
            {
                throw new ArgumentNullException("schedule", "schedule cannot be null.");
            }

            bool can = false;

            scheduleDate = null;
            windowBegin = windowBegin.FloorWithSeconds();
            windowEnd = windowEnd.FloorWithSeconds();

            DateTime sd = schedule.StartOn.FloorWithSeconds();
            DateTime ed = (schedule.EndOn ?? DateTime.MaxValue).FloorWithSeconds();

            if (ed > windowEnd)
            {
                int repeatSeconds;

                switch (schedule.RepeatType)
                {
                    case ScheduleRepeatType.None:
                        repeatSeconds = 0;
                        break;
                    case ScheduleRepeatType.Seconds:
                        repeatSeconds = 1;
                        break;
                    case ScheduleRepeatType.Minutes:
                        repeatSeconds = 60;
                        break;
                    case ScheduleRepeatType.Hours:
                        repeatSeconds = 3600;
                        break;
                    case ScheduleRepeatType.Days:
                        repeatSeconds = 86400;
                        break;
                    case ScheduleRepeatType.Weeks:
                        repeatSeconds = 604800;
                        break;
                    default:
                        throw new NotImplementedException();
                }

                Action log = () =>
                {
                    if (this.logger != null)
                    {
                        this.logger.Debug(
                            "Schedule can be enqueued with start date of {0:yyyy-MM-dd HH:mm:ss}, repeat type of {1} and repeat value of {2}. Window: {3:yyyy-MM-dd HH:mm:ss} - {4:yyyy-MM-dd HH:mm:ss}; calculated schedule date: {5:yyyy-MM-dd HH:mm:ss}",
                            schedule.StartOn,
                            schedule.RepeatType,
                            schedule.RepeatValue ?? 0,
                            windowBegin,
                            windowEnd,
                            sd);
                    }
                };

                if (sd < windowBegin && schedule.RepeatValue != null && schedule.RepeatValue > 0)
                {
                    long totalRepeatSeconds = repeatSeconds * schedule.RepeatValue.Value;
                    double diffSeconds = windowBegin.Subtract(sd).TotalSeconds;
                    long repeatCount = (long)Math.Ceiling(diffSeconds / totalRepeatSeconds);
                    sd = sd.AddSeconds(repeatCount * totalRepeatSeconds);

                    if (sd > windowBegin && sd <= windowEnd)
                    {
                        scheduleDate = sd;
                        can = true;
                    }
                }
                else if (sd > windowBegin && sd <= windowEnd)
                {
                    scheduleDate = sd;
                    can = true;
                }

                if (can)
                {
                    using (IRepository repository = this.repositoryFactory.Create())
                    {
                        can = !repository.GetScheduleDateExistsForSchedule(schedule.Id.Value, scheduleDate.Value, null);
                    }

                    if (can)
                    {
                        log();
                    }
                    else
                    {
                        scheduleDate = null;
                    }
                }
            }

            return can;
        }

        /// <summary>
        /// Enqueues scheduled jobs that are due for execution.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We don't care why job de-serialization failed. We want to log it no matter what.")]
        public void EnqueueScheduledJobs()
        {
            // Create the schedule window.
            DateTime end = DateTime.UtcNow.FloorWithSeconds();
            DateTime begin = this.LastEnqueuedOn != null ? this.LastEnqueuedOn.Value : end.AddSeconds(-1 * this.heartbeat);
            this.LastEnqueuedOn = end;

            using (IRepository repository = this.repositoryFactory.Create())
            {
                // Ensure the schedules have been loaded.
                if (this.lastRefreshOn == null)
                {
                    this.RefreshSchedules(repository, null);
                }
            }

            foreach (ScheduleRecord schedule in this.Schedules)
            {
                bool hasEnqueueingLock = false;

                using (IRepository repository = this.repositoryFactory.Create())
                {
                    using (IDbTransaction transaction = repository.BeginTransaction(IsolationLevel.RepeatableRead))
                    {
                        try
                        {
                            hasEnqueueingLock = repository.GetScheduleEnqueueingLock(schedule.Id.Value, transaction);
                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }

                if (hasEnqueueingLock)
                {
                    DateTime? scheduleDate;

                    try
                    {
                        if (this.CanScheduleBeEnqueued(schedule, begin, end, out scheduleDate))
                        {
                            List<QueueRecord> queues = new List<QueueRecord>();
                            List<HistoryRecord> histories = new List<HistoryRecord>();

                            foreach (ScheduledJobRecord scheduledJob in schedule.ScheduledJobs)
                            {
                                IJob job = null;
                                Exception ex = null;

                                // Try do de-serialize the job. We do this because we need the job name,
                                // and also just to save the queue bandwidth in case in can't be de-serialized.
                                try
                                {
                                    job = JobSerializer.Deserialize(scheduledJob.JobType, scheduledJob.Data);
                                    this.logger.Debug("Scheduler de-serialized scheduled job instance for '{0}' for schedule '{1}'.", scheduledJob.JobType, schedule.Name);
                                }
                                catch (Exception sx)
                                {
                                    ex = sx;
                                    this.logger.Warn("Scheduler failed to de-serialize scheduled job instance for '{0}' for schedule '{1}'.", scheduledJob.JobType, schedule.Name);
                                }

                                if (job != null)
                                {
                                    queues.Add(
                                        new QueueRecord()
                                        {
                                            ApplicationName = this.applicationName,
                                            Data = scheduledJob.Data,
                                            JobName = job.Name,
                                            JobType = scheduledJob.JobType,
                                            QueuedOn = scheduleDate.Value,
                                            QueueName = schedule.QueueName,
                                            ScheduleId = schedule.Id,
                                            TryNumber = 1
                                        });
                                }
                                else
                                {
                                    histories.Add(
                                        new HistoryRecord()
                                        {
                                            ApplicationName = this.applicationName,
                                            Data = scheduledJob.Data,
                                            Exception = ex != null ? new ExceptionXElement(ex).ToString() : null,
                                            FinishedOn = scheduleDate.Value,
                                            JobName = null,
                                            JobType = scheduledJob.JobType,
                                            QueuedOn = scheduleDate.Value,
                                            QueueName = schedule.QueueName,
                                            ScheduleId = schedule.Id,
                                            StartedOn = scheduleDate.Value,
                                            Status = HistoryStatus.Failed,
                                            TryNumber = 1,
                                            WorkerId = this.workerId
                                        });
                                }
                            }

                            if (queues.Count > 0 || histories.Count > 0)
                            {
                                using (IRepository repository = this.repositoryFactory.Create())
                                {
                                    repository.CreateQueuedAndHistoryForSchedule(schedule.Id.Value, scheduleDate.Value, queues, histories, null);
                                }

                                this.logger.Debug("Scheduler created {0} queued jobs and {1} failed history jobs for schedule '{2}'.", queues.Count, histories.Count, schedule.Name);
                            }
                        }
                    }
                    finally
                    {
                        using (IRepository repository = this.repositoryFactory.Create())
                        {
                            repository.ReleaseScheduleEnqueueingLock(schedule.Id.Value, null);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Refreshes this instance's schedules with the latest data from the repository.
        /// </summary>
        public void RefreshSchedules()
        {
            using (IRepository repository = this.repositoryFactory.Create())
            {
                this.RefreshSchedules(repository, null);
            }

            this.lastRefreshOn = DateTime.UtcNow;
        }

        /// <summary>
        /// Refreshes this instance's schedules with the latest data from the repository.
        /// </summary>
        /// <param name="repository">The repository to use when accessing data.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        internal void RefreshSchedules(IRepository repository, IDbTransaction transaction)
        {
            var sch = repository.GetSchedules(this.applicationName, transaction);

            lock (this.queueFilters)
            {
                this.schedules.Clear();
                this.schedules.AddRange(sch.Where(s => s.Enabled && this.queueFilters.Includes(s.QueueName)));
                this.logger.Debug("Scheduler refreshed {0} schedules from the repository.", this.Schedules.Count());
            }

            this.lastRefreshOn = DateTime.UtcNow;
        }
    }
}
