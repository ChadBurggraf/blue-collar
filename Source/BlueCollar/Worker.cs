//-----------------------------------------------------------------------
// <copyright file="Worker.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents an individual worker that dequeues jobs and performs work.
    /// </summary>
    public sealed class Worker : IDisposable
    {
        #region Private Fields

        private readonly object runLocker = new object();
        private readonly object statusLocker = new object();
        private Thread signalThread, runThread;
        private IScheduler scheduler;
        private WorkingRecord currentRecord;
        private bool schedulerEnabled, disposed;
        private string applicationName, name;
        private long id;
        private QueueNameFilters queueFilters;
        private int heartbeat;
        private IRepositoryFactory repositoryFactory;
        private ILogger logger;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the Worker class.
        /// </summary>
        /// <param name="applicationName">The name of the application the worker belongs to.</param>
        /// <param name="id">The ID of the worker in the repository.</param>
        /// <param name="name">The name of the worker.</param>
        /// <param name="queueFilters">The queue name filters the worker should use while processing queues..</param>
        /// <param name="heartbeat">The number of seconds between poll intervals.</param>
        /// <param name="schedulerEnabled">A value indicating whether the scheduler is enabled.</param>
        /// <param name="repositoryFactory">The repository factory to use when accessing data.</param>
        /// <param name="logger">The logger to use when logging messages.</param>
        public Worker(string applicationName, long id, string name, QueueNameFilters queueFilters, int heartbeat, bool schedulerEnabled, IRepositoryFactory repositoryFactory, ILogger logger)
            : this(applicationName, id, name, queueFilters, heartbeat, schedulerEnabled, repositoryFactory, logger, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Worker class.
        /// </summary>
        /// <param name="applicationName">The name of the application the worker belongs to.</param>
        /// <param name="id">The ID of the worker in the repository.</param>
        /// <param name="name">The name of the worker.</param>
        /// <param name="queueFilters">The queue name filters the worker should use while processing queues..</param>
        /// <param name="heartbeat">The number of seconds between poll intervals.</param>
        /// <param name="schedulerEnabled">A value indicating whether the scheduler is enabled.</param>
        /// <param name="repositoryFactory">The repository factory to use when accessing data.</param>
        /// <param name="logger">The logger to use when logging messages.</param>
        /// <param name="scheduler">The scheduler to use when managing schedules and enqueueing scheduled jobs.</param>
        public Worker(string applicationName, long id, string name, QueueNameFilters queueFilters, int heartbeat, bool schedulerEnabled, IRepositoryFactory repositoryFactory, ILogger logger, IScheduler scheduler)
        {
            if (string.IsNullOrEmpty(applicationName))
            {
                throw new ArgumentNullException("applicationName", "applicationName must contain a value.");
            }

            if (id < 1)
            {
                throw new ArgumentOutOfRangeException("id", "id must be greater than 0.");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name", "name must contain a value.");
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

            this.applicationName = applicationName;
            this.id = id;
            this.name = name;
            this.heartbeat = heartbeat * 1000;
            this.schedulerEnabled = schedulerEnabled;
            this.repositoryFactory = repositoryFactory;
            this.logger = logger;
            this.queueFilters = queueFilters ?? QueueNameFilters.Any();
            this.scheduler = scheduler ?? new Scheduler(id, applicationName, heartbeat, repositoryFactory, logger);

            this.signalThread = new Thread(this.SignalLoop);
            this.signalThread.Name = "BlueCollar Signal Thread";
            this.signalThread.Start();
        }

        /// <summary>
        /// Finalizes an instance of the Worker class.
        /// </summary>
        ~Worker()
        {
            this.Dispose(false);
        }

        #endregion

        #region Events and Properties

        /// <summary>
        /// Event raised when this instance is being disposed.
        /// </summary>
        public event EventHandler Disposing;

        /// <summary>
        /// Gets this instance's database ID.
        /// </summary>
        public long Id
        {
            get { return this.id; }
        }

        /// <summary>
        /// Gets a value indicating whether the scheduler is enabled.
        /// </summary>
        public bool SchedulerEnabled
        {
            get { return this.schedulerEnabled; }
        }

        /// <summary>
        /// Gets the worker's current status.
        /// </summary>
        public WorkerStatus Status { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this isntance's loop threads are alive.
        /// </summary>
        internal bool LoopThreadsAreAlive
        {
            get
            {
                return (this.runThread != null && this.runThread.IsAlive)
                    || (this.signalThread != null && this.signalThread.IsAlive);
            }
        }

        #endregion

        #region Public Instance Methods

        /// <summary>
        /// Disposes of resources used by this instance.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Starts the worker.
        /// </summary>
        /// <returns>True if the worker is started after this method completes, false otherwise.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We don't care if Thread.Abort() fails for any reason.")]
        public bool Start()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("Worker");
            }

            bool started = false;

            lock (this.statusLocker)
            {
                if (this.Status == WorkerStatus.Stopped)
                {
                    lock (this.runLocker)
                    {
                        this.runThread = new Thread(this.RunLoop);
                        this.runThread.Name = "BlueCollar Run Thread";
                        this.runThread.Start();
                    }

                    this.SetStatus(WorkerStatus.Working);

                    started = true;
                    this.logger.Info("Worker {0} ({1}) has started.", this.name, this.id);
                }
                else if (this.Status == WorkerStatus.Working)
                {
                    started = true;
                    this.logger.Info("Worker {0} ({1}) was already working.", this.name, this.id);
                }
                else
                {
                    this.logger.Info("Worker {0} ({1}) was not started because it is currently stopping.", this.name, this.id);
                }
            }

            return started;
        }

        /// <summary>
        /// Stops the worker.
        /// </summary>
        /// <param name="force">A value indicating whether to force the worker to stop, even if work will be abandoned.</param>
        public void Stop(bool force)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("Worker");
            }

            bool isWorking = false, isAlive = false;

            lock (this.statusLocker)
            {
                if (this.Status == WorkerStatus.Working)
                {
                    isWorking = true;
                    this.SetStatus(WorkerStatus.Stopping);
                }
            }

            lock (this.runLocker)
            {
                isAlive = this.runThread != null && this.runThread.IsAlive;
            }

            if (isWorking && isAlive)
            {
                if (!force)
                {
                    try
                    {
                        this.runThread.Join();
                        this.runThread = null;
                    }
                    catch (NullReferenceException)
                    {
                    }
                }
                else
                {
                    this.KillRunThread();
                }
            }

            lock (this.statusLocker)
            {
                lock (this.runLocker)
                {
                    WorkingRecord record = this.currentRecord;
                    
                    using (IRepository repository = this.repositoryFactory.Create())
                    {
                        using (IDbTransaction transaction = repository.BeginTransaction())
                        {
                            if (record != null)
                            {
                                HistoryRecord history = CreateHistory(record, HistoryStatus.Interrupted);
                                repository.DeleteWorking(record.Id.Value, transaction);
                                history = repository.CreateHistory(history, transaction);
                            }

                            this.SetStatus(WorkerStatus.Stopped, repository, transaction);
                            transaction.Commit();
                        }
                    }

                    this.currentRecord = null;
                }
            }

            if (isWorking)
            {
                this.logger.Info("Worker {0} ({1}) has stopped.", this.name, this.id);
            }
        }

        #endregion

        #region Internal Static Methods

        /// <summary>
        /// Creates a history record from the given working record and status.
        /// </summary>
        /// <param name="working">The working record to create the history record for.</param>
        /// <param name="status">The status to create the history record with.</param>
        /// <returns>A history record.</returns>
        internal static HistoryRecord CreateHistory(WorkingRecord working, HistoryStatus status)
        {
            return new HistoryRecord()
            {
                ApplicationName = working.ApplicationName,
                Data = working.Data,
                FinishedOn = DateTime.UtcNow,
                JobName = working.JobName,
                JobType = working.JobType,
                QueuedOn = working.QueuedOn,
                QueueName = working.QueueName,
                ScheduleId = working.ScheduleId,
                StartedOn = working.StartedOn,
                Status = status,
                TryNumber = working.TryNumber,
                WorkerId = working.WorkerId
            };
        }

        /// <summary>
        /// Creates a queue retry record from the given working record.
        /// </summary>
        /// <param name="working">The working record to create the queue record from.</param>
        /// <returns>A queue record.</returns>
        internal static QueueRecord CreateQueueRetry(WorkingRecord working)
        {
            return new QueueRecord()
            {
                ApplicationName = working.ApplicationName,
                Data = working.Data,
                JobName = working.JobName,
                JobType = working.JobType,
                QueuedOn = DateTime.UtcNow,
                QueueName = working.QueueName,
                ScheduleId = working.ScheduleId,
                TryNumber = working.TryNumber + 1
            };
        }

        /// <summary>
        /// Creates a working record from the given queued record.
        /// </summary>
        /// <param name="queued">The queued record to create the working record for.</param>
        /// <param name="workerId">The ID of the worker to create the record for.</param>
        /// <param name="scheduleId">The ID of the schedule to create the record for.</param>
        /// <param name="startedOn">The start date to create the record with.</param>
        /// <returns>A working record.</returns>
        internal static WorkingRecord CreateWorking(QueueRecord queued, long workerId, long? scheduleId, DateTime startedOn)
        {
            return new WorkingRecord()
            {
                ApplicationName = queued.ApplicationName,
                Data = queued.Data,
                JobName = queued.JobName,
                JobType = queued.JobType,
                QueuedOn = queued.QueuedOn,
                QueueName = queued.QueueName,
                ScheduleId = scheduleId,
                StartedOn = startedOn,
                TryNumber = queued.TryNumber,
                WorkerId = workerId
            };
        }

        #endregion

        #region Internal Instance Methods

        /// <summary>
        /// Cancels the current job.
        /// </summary>
        internal void CancelCurrent()
        {
            lock (this.runLocker)
            {
                this.KillRunThread();

                if (this.currentRecord != null)
                {
                    this.logger.Info("Worker {0} ({1}) canceled '{2}'.", this.name, this.id, this.currentRecord.JobName);

                    using (IRepository repository = this.repositoryFactory.Create())
                    {
                        using (IDbTransaction transaction = repository.BeginTransaction())
                        {
                            HistoryRecord history = CreateHistory(this.currentRecord, HistoryStatus.Canceled);
                            repository.DeleteWorking(this.currentRecord.Id.Value, transaction);
                            history = repository.CreateHistory(history, transaction);
                            transaction.Commit();
                        }
                    }

                    this.currentRecord = null;
                }

                this.runThread = new Thread(this.RunLoop);
                this.runThread.Name = "BlueCollar Run Thread";
                this.runThread.Start();
            }
        }

        /// <summary>
        /// Dequeues a job to do work on.
        /// </summary>
        /// <returns>A working record representing the dequeued job.</returns>
        internal WorkingRecord DequeueRecord()
        {
            WorkingRecord working = null;
            QueueNameFilters queues = null;
            
            lock (this.runLocker)
            {
                queues = new QueueNameFilters(this.queueFilters.Include, this.queueFilters.Exclude);
            }

            using (IRepository repository = this.repositoryFactory.Create())
            {
                using (IDbTransaction transaction = repository.BeginTransaction(IsolationLevel.RepeatableRead))
                {
                    QueueRecord queued = repository.GetQueued(this.applicationName, queues, DateTime.UtcNow, transaction);

                    if (queued != null)
                    {
                        working = CreateWorking(queued, this.id, queued.ScheduleId, DateTime.UtcNow);

                        repository.DeleteQueued(queued.Id.Value, transaction);
                        working = repository.CreateWorking(working, transaction);

                        this.logger.Info("Worker {0} ({1}) dequeued '{2}'.", this.name, this.id, queued.JobName);
                    }

                    transaction.Commit();
                }
            }

            return working;
        }

        /// <summary>
        /// Executes the given job.
        /// </summary>
        /// <param name="job">The job to execute.</param>
        /// <param name="ex">Contains the execution exception, if applicable.</param>
        /// <returns>True if the job executed successfully, false otherwise.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We're returning any exceptions encountered back to the caller.")]
        internal bool ExecuteJob(IJob job, out Exception ex)
        {
            bool success = false;
            ex = null;

            try
            {
                // Execute the job, with a timeout if necessary.
                int timeout = job.Timeout < 0 ? 60000 : job.Timeout;

                if (timeout > 0)
                {
                    this.logger.Info("Worker {0} ({1}) is executing job '{2}' with timeout {3}.", this.name, this.id, job.Name, job.Timeout);
                    new Action(job.Execute).InvokeWithTimeout(timeout);
                }
                else
                {
                    this.logger.Info("Worker {0} ({1}) is executing job '{2}' with no timeout.", this.name, this.id, job.Name);
                    job.Execute();
                }

                success = true;
            }
            catch (Exception tx)
            {
                ex = tx;
            }

            return success;
        }

        /// <summary>
        /// Prunes orphaned jobs assigned to this worker that for one reason or another didn't get marked as interrupted.
        /// </summary>
        internal void PruneOrphans()
        {
            long? currentId = null;

            lock (this.runLocker)
            {
                currentId = this.currentRecord != null ? this.currentRecord.Id : null;
            }

            using (IRepository repository = this.repositoryFactory.Create())
            {
                using (IDbTransaction transaction = repository.BeginTransaction())
                {
                    foreach (WorkingRecord working in repository.GetWorkingForWorker(this.Id, currentId, transaction))
                    {
                        if (working.Id != currentId)
                        {
                            HistoryRecord history = CreateHistory(working, HistoryStatus.Interrupted);
                            repository.DeleteWorking(working.Id.Value, transaction);
                            repository.CreateHistory(history, transaction);
                        }
                    }

                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Implements the worker run loop.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We want the run loop to continue through any exception except for ThreadAbortException.")]
        internal void RunLoop()
        {
            while (true)
            {
                bool working = false, needsRest = true;
                WorkingRecord record = null;
                IJob job = null;
                Exception ex = null;

                try
                {
                    // Find out if we're supposed to be doing work.
                    lock (this.statusLocker)
                    {
                        working = this.Status == WorkerStatus.Working;
                    }

                    if (working)
                    {
                        // Dequeue a new job to work on.
                        record = this.DequeueRecord();

                        // If a record exists, we have work to do.
                        if (record != null)
                        {
                            // Try to de-serialize a job.
                            try
                            {
                                job = JobSerializer.Deserialize(record.JobType, record.Data);
                                this.logger.Debug("Worker {0} ({1}) de-serialized a job instance for '{2}'.", this.name, this.id, record.JobType);
                            }
                            catch (Exception sx)
                            {
                                ex = sx;
                                this.logger.Warn("Worker {0} ({1}) failed to de-serialize a job instane for '{2}'.", this.name, this.id, record.JobType);
                            }

                            // If we failed to de-serialize, fail the job.
                            if (job == null)
                            {
                                HistoryRecord history = CreateHistory(record, HistoryStatus.Failed);

                                if (ex != null)
                                {
                                    history.Exception = new ExceptionXElement(ex).ToString();
                                }

                                using (IRepository repository = this.repositoryFactory.Create())
                                {
                                    using (IDbTransaction transaction = repository.BeginTransaction())
                                    {
                                        repository.DeleteWorking(record.Id.Value, transaction);
                                        history = repository.CreateHistory(history, transaction);
                                        transaction.Commit();
                                    }
                                }
                            }
                            else
                            {
                                // Update this instance's current record so we can interrupt
                                // execution if necessary.
                                lock (this.runLocker)
                                {
                                    this.currentRecord = record;
                                }

                                // Execute the job.
                                bool success = this.ExecuteJob(job, out ex);

                                // Acquire the run lock and move the job from the working
                                // state to the history state, including the execution results.
                                lock (this.runLocker)
                                {
                                    HistoryStatus status = HistoryStatus.Succeeded;
                                    string exceptionString = null;

                                    if (success)
                                    {
                                        this.logger.Info("Worker {0} ({1}) executed '{2}' successfully.", this.name, this.id, this.currentRecord.JobName);
                                    }
                                    else
                                    {
                                        if (ex as TimeoutException != null)
                                        {
                                            status = HistoryStatus.TimedOut;
                                            this.logger.Warn("Worker {0} ({1}) timed out '{2}'.", this.name, this.id, this.currentRecord.JobName);
                                        }
                                        else
                                        {
                                            status = HistoryStatus.Failed;

                                            if (ex != null)
                                            {
                                                exceptionString = new ExceptionXElement(ex).ToString();
                                            }

                                            this.logger.Warn("Worker {0} ({1}) encountered an exception during execution of '{2}'.", this.name, this.id, this.currentRecord.JobName);
                                        }
                                    }

                                    HistoryRecord history = CreateHistory(this.currentRecord, status);
                                    history.Exception = exceptionString;

                                    using (IRepository repository = this.repositoryFactory.Create())
                                    {
                                        using (IDbTransaction transaction = repository.BeginTransaction())
                                        {
                                            repository.DeleteWorking(this.currentRecord.Id.Value, transaction);
                                            history = repository.CreateHistory(history, transaction);

                                            // Re-try?
                                            if ((status == HistoryStatus.Failed
                                                || status == HistoryStatus.Interrupted
                                                || status == HistoryStatus.TimedOut)
                                                && (job.Retries == 0 || job.Retries >= this.currentRecord.TryNumber))
                                            {
                                                repository.CreateQueued(CreateQueueRetry(this.currentRecord), transaction);
                                            }

                                            transaction.Commit();
                                        }
                                    }

                                    this.currentRecord = null;
                                }
                            }

                            needsRest = false;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Exception rx)
                {
                    this.logger.Error(rx, "Exception thrown during the run loop for worker {0} ({1}).", this.name, this.id);
                }

                if (working)
                {
                    // Take a breather real quick.
                    if (needsRest)
                    {
                        this.logger.Debug("Worker {0} ({1}) is resting before trying to de-queue another job.", this.name, this.id);
                        Thread.Sleep(this.heartbeat.Randomize());
                    }
                    else
                    {
                        this.logger.Debug("Worker {0} ({1}) will immediately try to de-queue another job.", this.name, this.id);
                    }
                }
            }
        }

        /// <summary>
        /// Implements the worker signal loop.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We want the signal loop to continue through any exception except for ThreadAbortException.")]
        internal void SignalLoop()
        {
            while (true)
            {
                try
                {
                    // Prun any orphans that were abandoned for any reason.
                    this.PruneOrphans();

                    WorkerStatus status;
                    long? recordId = null;

                    lock (this.statusLocker)
                    {
                        status = this.Status;

                        lock (this.runLocker)
                        {
                            if (status == WorkerStatus.Working && this.currentRecord != null)
                            {
                                recordId = this.currentRecord.Id;
                            }
                        }
                    }

                    SignalsRecord signals;

                    // Load the current signals from the repository.
                    using (IRepository repository = this.repositoryFactory.Create())
                    {
                        using (IDbTransaction transaction = repository.BeginTransaction())
                        {
                            signals = repository.GetWorkingSignals(this.id, recordId, transaction);
                            repository.ClearWorkingSignalPair(this.id, recordId, transaction);
                            transaction.Commit();
                        }
                    }

                    if (signals != null)
                    {
                        bool refreshQueues = false;

                        // Refresh the queues we're processing.
                        lock (this.runLocker)
                        {
                            QueueNameFilters filters = QueueNameFilters.Parse(signals.QueueNames);

                            if (!this.queueFilters.Equals(filters))
                            {
                                this.queueFilters = QueueNameFilters.Parse(signals.QueueNames);
                                refreshQueues = true;
                            }
                        }

                        // Perform the signalled operation, if applicable.
                        if (signals.WorkerSignal == WorkerSignal.Stop)
                        {
                            this.logger.Debug("Worker {0} ({1}) received a signal to stop.", this.name, this.id);
                            this.Stop(false);
                        }
                        else
                        {
                            if (signals.WorkingSignal == WorkingSignal.Cancel)
                            {
                                this.logger.Debug("Worker {0} ({1}) received a signal to cancel its current job.", this.name, this.id);
                                this.CancelCurrent();
                            }

                            if (signals.WorkerSignal == WorkerSignal.Start)
                            {
                                this.logger.Debug("Worker {0} ({1}) received a signal to start.", this.name, this.id);
                                this.Start();
                            }

                            lock (this.statusLocker)
                            {
                                status = this.Status;
                            }

                            if (status == WorkerStatus.Working)
                            {
                                if (this.SchedulerEnabled)
                                {
                                    if (refreshQueues)
                                    {
                                        this.scheduler.QueueFilters = new QueueNameFilters(this.queueFilters.Include, this.queueFilters.Exclude);
                                    }

                                    if (signals.WorkerSignal == WorkerSignal.RefreshSchedules)
                                    {
                                        this.logger.Debug("Worker {0} ({1}) received a signal to refresh its schedules.", this.name, this.id);
                                        this.scheduler.RefreshSchedules();
                                    }

                                    this.scheduler.EnqueueScheduledJobs();
                                }
                            }
                        }
                    }
                    else
                    {
                        // If no signals were returned, it means that this worker
                        // is orphaned.
                        this.Stop(false);
                        break;
                    }
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    this.logger.Error(ex, "Exception thrown during the signal loop for worker {0} ({1}).", this.name, this.id);
                }

                Thread.Sleep(this.heartbeat.Randomize());
            }
        }

        /// <summary>
        /// Sets this worker's status.
        /// </summary>
        /// <param name="status">The status to set.</param>
        internal void SetStatus(WorkerStatus status)
        {
            using (IRepository repository = this.repositoryFactory.Create())
            {
                this.SetStatus(status, repository, null);
            }
        }

        /// <summary>
        /// Sets this worker's status.
        /// </summary>
        /// <param name="status">The status to set.</param>
        /// <param name="repository">The repository to use when accessing data.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        internal void SetStatus(WorkerStatus status, IRepository repository, IDbTransaction transaction)
        {
            WorkerStatus oldStatus;

            lock (this.statusLocker)
            {
                oldStatus = this.Status;

                if (oldStatus != status)
                {
                    repository.UpdateWorkerStatus(this.id, status, transaction);
                    this.Status = status;
                }
            }

            this.logger.Debug("Updated status of worker {0} ({1}) from '{2}' to '{3}'.", this.name, this.id, oldStatus, status);
        }

        #endregion

        #region Private Instance Methods

        /// <summary>
        /// Disposes of resources used by this instance.
        /// </summary>
        /// <param name="disposing">A value indicating whether to dispose of managed resources.</param>
        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (this.Disposing != null)
                {
                    this.Disposing(this, EventArgs.Empty);
                }

                if (disposing)
                {
                    this.Stop(true);

                    lock (this.statusLocker)
                    {
                        lock (this.runLocker)
                        {
                            this.KillRunThread();
                            this.KillSignalThread();
                        }
                    }

                    this.logger.Debug("Worker {0} ({1}) has been disposed.", this.name, this.id);
                }
                else
                {
                    this.KillRunThread();
                    this.KillSignalThread();
                }

                this.disposed = true;
            }
        }

        /// <summary>
        /// Kills the run thread.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We don't care if Thread.Abort() fails for any reason.")]
        private void KillRunThread()
        {
            try
            {
                if (this.runThread != null && this.runThread.IsAlive)
                {
                    this.runThread.Abort();
                    this.runThread = null;
                }
            }
            catch (Exception ex)
            {
                this.logger.Error(ex);
            }
        }

        /// <summary>
        /// Kills the signal thread.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We don't care if Thread.Abort() fails for any reason.")]
        private void KillSignalThread()
        {
            try
            {
                if (this.signalThread != null && this.signalThread.IsAlive)
                {
                    this.signalThread.Abort();
                    this.signalThread = null;
                }
            }
            catch (Exception ex)
            {
                this.logger.Error(ex);
            }
        }

        #endregion
    }
}
