//-----------------------------------------------------------------------
// <copyright file="RepositoryTests.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Test
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;

    /// <summary>
    /// Base class for <see cref="IRepository"/> tests.
    /// </summary>
    [TestClass]
    public abstract class RepositoryTests : IDisposable
    {
        /// <summary>
        /// Finalizes an instance of the RepositoryTests class.
        /// </summary>
        ~RepositoryTests()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets the repository to use for the current test.
        /// </summary>
        protected abstract IRepository Repository { get; }

        /// <summary>
        /// Disposes of resources used by this instance.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Test cleanup.
        /// </summary>
        [TestCleanup]
        public virtual void TestCleanup()
        {
            if (this.Repository != null)
            {
                this.Repository.Dispose();
            }
        }

        /// <summary>
        /// Test initialization.
        /// </summary>
        [TestInitialize]
        public virtual void TestInitialize()
        {
            if (this.Repository != null)
            {
                this.Repository.DeleteAll(BlueCollarSection.Section.ApplicationName, null);
            }
        }

        /// <summary>
        /// Begin transaction tests.
        /// </summary>
        protected void BeginTransaction()
        {
            if (this.Repository != null)
            {
                using (var transaction = this.Repository.BeginTransaction())
                {
                    Assert.IsNotNull(transaction);
                }

                using (var transaction = this.Repository.BeginTransaction(IsolationLevel.RepeatableRead))
                {
                    Assert.IsNotNull(transaction);
                }
            }
        }

        /// <summary>
        /// Clear working signal pair tests.
        /// </summary>
        protected void ClearWorkingSignalPair()
        {
            if (this.Repository != null)
            {
                IJob job = new TestJob() { Id = Guid.NewGuid() };

                WorkerRecord workerRecord = new WorkerRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    MachineAddress = Machine.Address,
                    MachineName = Machine.Name,
                    Name = "Test Worker",
                    QueueNames = "*",
                    Signal = WorkerSignal.Stop,
                    Status = WorkerStatus.Working,
                    Startup = WorkerStartupType.Automatic,
                    UpdatedOn = DateTime.UtcNow
                };

                this.Repository.CreateWorker(workerRecord, null);

                WorkingRecord workingRecord = new WorkingRecord()
                {
                    ApplicationName = workerRecord.ApplicationName,
                    Data = JsonConvert.SerializeObject(job),
                    JobName = job.Name,
                    JobType = job.GetType().FullName + ", " + job.GetType().Assembly.GetName().Name,
                    QueueName = "*",
                    QueuedOn = DateTime.UtcNow,
                    Signal = WorkingSignal.Cancel,
                    StartedOn = DateTime.UtcNow,
                    TryNumber = 1,
                    WorkerId = workerRecord.Id.Value
                };

                this.Repository.CreateWorking(workingRecord, null);
                this.Repository.ClearWorkingSignalPair(workerRecord.Id.Value, workingRecord.Id, null);

                SignalsRecord signals = this.Repository.GetWorkingSignals(workerRecord.Id.Value, workingRecord.Id, null);
                Assert.AreEqual(WorkerSignal.None, signals.WorkerSignal);
                Assert.AreEqual(WorkingSignal.None, signals.WorkingSignal);

                workerRecord.Id = null;
                workerRecord.Name = "Test Worker 2";

                this.Repository.CreateWorker(workerRecord, null);
                this.Repository.ClearWorkingSignalPair(workerRecord.Id.Value, null, null);

                signals = this.Repository.GetWorkingSignals(workerRecord.Id.Value, null, null);
                Assert.AreEqual(WorkerSignal.None, signals.WorkerSignal);
                Assert.AreEqual(WorkingSignal.None, signals.WorkingSignal);
            }
        }

        /// <summary>
        /// Create history tests.
        /// </summary>
        protected void CreateHistory()
        {
            if (this.Repository != null)
            {
                IJob job = new TestJob() { Id = Guid.NewGuid() };

                WorkerRecord workerRecord = new WorkerRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    MachineAddress = Machine.Address,
                    MachineName = Machine.Name,
                    Name = "Test Worker",
                    QueueNames = "*",
                    Signal = WorkerSignal.Stop,
                    Status = WorkerStatus.Working,
                    Startup = WorkerStartupType.Automatic,
                    UpdatedOn = DateTime.UtcNow
                };

                this.Repository.CreateWorker(workerRecord, null);

                WorkingRecord workingRecord = new WorkingRecord()
                {
                    ApplicationName = workerRecord.ApplicationName,
                    Data = JsonConvert.SerializeObject(job),
                    JobName = job.Name,
                    JobType = job.GetType().FullName + ", " + job.GetType().Assembly.GetName().Name,
                    QueueName = "*",
                    QueuedOn = DateTime.UtcNow,
                    Signal = WorkingSignal.Cancel,
                    StartedOn = DateTime.UtcNow,
                    TryNumber = 1,
                    WorkerId = workerRecord.Id.Value
                };

                this.Repository.CreateWorking(workingRecord, null);

                HistoryRecord historyRecord = new HistoryRecord()
                {
                    ApplicationName = workingRecord.ApplicationName,
                    Data = workingRecord.Data,
                    FinishedOn = DateTime.UtcNow,
                    JobName = workingRecord.JobName,
                    JobType = workingRecord.JobType,
                    QueuedOn = workingRecord.QueuedOn,
                    QueueName = workingRecord.QueueName,
                    StartedOn = workingRecord.StartedOn,
                    Status = HistoryStatus.Succeeded,
                    TryNumber = workingRecord.TryNumber,
                    WorkerId = workingRecord.WorkerId
                };

                this.Repository.CreateHistory(historyRecord, null);
                Assert.IsNotNull(historyRecord.Id);
            }
        }

        /// <summary>
        /// Create queued tests.
        /// </summary>
        protected void CreateQueued()
        {
            if (this.Repository != null)
            {
                IJob job = new TestJob() { Id = Guid.NewGuid() };

                WorkerRecord workerRecord = new WorkerRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    MachineAddress = Machine.Address,
                    MachineName = Machine.Name,
                    Name = "Test Worker",
                    QueueNames = "*",
                    Signal = WorkerSignal.Stop,
                    Status = WorkerStatus.Working,
                    Startup = WorkerStartupType.Automatic,
                    UpdatedOn = DateTime.UtcNow
                };

                this.Repository.CreateWorker(workerRecord, null);

                QueueRecord queueRecord = new QueueRecord()
                {
                    ApplicationName = workerRecord.ApplicationName,
                    Data = JsonConvert.SerializeObject(job),
                    JobName = job.Name,
                    JobType = job.GetType().FullName + ", " + job.GetType().Assembly.GetName().Name,
                    QueuedOn = DateTime.UtcNow,
                    QueueName = "*",
                    TryNumber = 1
                };

                this.Repository.CreateQueued(queueRecord, null);
                Assert.IsNotNull(queueRecord.Id);
            }
        }

        /// <summary>
        /// Create queued and history for schedule tests.
        /// </summary>
        protected void CreateQueuedAndHistoryForSchedule()
        {
            if (this.Repository != null)
            {
                WorkerRecord workerRecord = new WorkerRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    MachineAddress = Machine.Address,
                    MachineName = Machine.Name,
                    Name = "Test Worker",
                    QueueNames = "*",
                    Signal = WorkerSignal.Stop,
                    Status = WorkerStatus.Working,
                    Startup = WorkerStartupType.Automatic,
                    UpdatedOn = DateTime.UtcNow
                };

                this.Repository.CreateWorker(workerRecord, null);

                ScheduleRecord scheduleRecord = new ScheduleRecord()
                {
                    ApplicationName = workerRecord.ApplicationName,
                    Name = "Test",
                    QueueName = "*",
                    RepeatType = ScheduleRepeatType.Days,
                    RepeatValue = 1,
                    StartOn = DateTime.UtcNow.FloorWithSeconds()
                };

                this.Repository.CreateSchedule(scheduleRecord, null);

                List<QueueRecord> queued = new List<QueueRecord>();
                List<HistoryRecord> history = new List<HistoryRecord>();

                queued.Add(
                    new QueueRecord()
                    {
                        ApplicationName = workerRecord.ApplicationName,
                        Data = JobSerializer.Serialize(new TestJob() { Id = Guid.NewGuid() }),
                        JobName = new TestJob().Name,
                        JobType = JobSerializer.GetTypeName(typeof(TestJob)),
                        QueuedOn = scheduleRecord.StartOn,
                        QueueName = "*",
                        TryNumber = 1
                    });

                queued.Add(
                    new QueueRecord()
                    {
                        ApplicationName = workerRecord.ApplicationName,
                        Data = JobSerializer.Serialize(new TestJob() { Id = Guid.NewGuid() }),
                        JobName = new TestJob().Name,
                        JobType = JobSerializer.GetTypeName(typeof(TestJob)),
                        QueuedOn = scheduleRecord.StartOn,
                        QueueName = "*",
                        TryNumber = 1
                    });

                history.Add(
                    new HistoryRecord()
                    {
                        ApplicationName = workerRecord.ApplicationName,
                        Data = JobSerializer.Serialize(new TestJob() { Id = Guid.NewGuid() }),
                        Exception = new ExceptionXElement(new InvalidOperationException()).ToString(),
                        FinishedOn = scheduleRecord.StartOn,
                        JobName = new TestJob().Name,
                        JobType = JobSerializer.GetTypeName(typeof(TestJob)),
                        QueuedOn = scheduleRecord.StartOn,
                        QueueName = "*",
                        ScheduleId = scheduleRecord.Id,
                        StartedOn = scheduleRecord.StartOn,
                        Status = HistoryStatus.Failed,
                        TryNumber = 1,
                        WorkerId = workerRecord.Id.Value
                    });

                Assert.AreEqual(3, this.Repository.CreateQueuedAndHistoryForSchedule(scheduleRecord.Id.Value, scheduleRecord.StartOn, queued, history, null));
            }
        }

        /// <summary>
        /// Create schedule tests.
        /// </summary>
        protected void CreateSchedule()
        {
            if (this.Repository != null)
            {
                ScheduleRecord scheduleRecord = new ScheduleRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    Enabled = true,
                    Name = "Test",
                    QueueName = "*",
                    RepeatType = ScheduleRepeatType.Days,
                    RepeatValue = 1,
                    StartOn = DateTime.UtcNow.FloorWithSeconds()
                };

                this.Repository.CreateSchedule(scheduleRecord, null);
                Assert.IsNotNull(scheduleRecord.Id);
            }
        }

        /// <summary>
        /// Create scheduled job tests.
        /// </summary>
        protected void CreateScheduledJob()
        {
            if (this.Repository != null)
            {
                ScheduleRecord scheduleRecord = new ScheduleRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    Name = "Test",
                    QueueName = "*",
                    RepeatType = ScheduleRepeatType.Days,
                    RepeatValue = 1,
                    StartOn = DateTime.UtcNow.FloorWithSeconds()
                };

                this.Repository.CreateSchedule(scheduleRecord, null);

                ScheduledJobRecord scheduledJobRecord = new ScheduledJobRecord()
                {
                    JobType = "BlueCollar.TestScheduledJob, BlueCollar",
                    Properties = "{}",
                    ScheduleId = scheduleRecord.Id.Value
                };

                this.Repository.CreateScheduledJob(scheduledJobRecord, null);
                Assert.IsNotNull(scheduledJobRecord.Id);
            }
        }

        /// <summary>
        /// Create worker tests.
        /// </summary>
        protected void CreateWorker()
        {
            if (this.Repository != null)
            {
                WorkerRecord workerRecord = new WorkerRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    MachineAddress = Machine.Address,
                    MachineName = Machine.Name,
                    Name = "Test Worker",
                    QueueNames = "*",
                    Signal = WorkerSignal.Stop,
                    Status = WorkerStatus.Working,
                    Startup = WorkerStartupType.Automatic,
                    UpdatedOn = DateTime.UtcNow
                };

                this.Repository.CreateWorker(workerRecord, null);
                Assert.IsNotNull(workerRecord.Id);
            }
        }

        /// <summary>
        /// Create working tests.
        /// </summary>
        protected void CreateWorking()
        {
            if (this.Repository != null)
            {
                IJob job = new TestJob() { Id = Guid.NewGuid() };

                WorkerRecord workerRecord = new WorkerRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    MachineAddress = Machine.Address,
                    MachineName = Machine.Name,
                    Name = "Test Worker",
                    QueueNames = "*",
                    Signal = WorkerSignal.Stop,
                    Status = WorkerStatus.Working,
                    Startup = WorkerStartupType.Automatic,
                    UpdatedOn = DateTime.UtcNow
                };

                this.Repository.CreateWorker(workerRecord, null);

                WorkingRecord workingRecord = new WorkingRecord()
                {
                    ApplicationName = workerRecord.ApplicationName,
                    Data = JsonConvert.SerializeObject(job),
                    JobName = job.Name,
                    JobType = job.GetType().FullName + ", " + job.GetType().Assembly.GetName().Name,
                    QueueName = "*",
                    QueuedOn = DateTime.UtcNow,
                    Signal = WorkingSignal.Cancel,
                    StartedOn = DateTime.UtcNow,
                    TryNumber = 1,
                    WorkerId = workerRecord.Id.Value
                };

                this.Repository.CreateWorking(workingRecord, null);
                Assert.IsNotNull(workingRecord.Id);
            }
        }

        /// <summary>
        /// Date tests.
        /// </summary>
        protected void Dates()
        {
            if (this.Repository != null)
            {
                WorkerRecord workerRecord = new WorkerRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    MachineAddress = Machine.Address,
                    MachineName = Machine.Name,
                    Name = "Test Worker",
                    QueueNames = "*",
                    Signal = WorkerSignal.Stop,
                    Status = WorkerStatus.Working,
                    Startup = WorkerStartupType.Automatic,
                    UpdatedOn = DateTime.UtcNow.FloorWithSeconds()
                };

                this.Repository.CreateWorker(workerRecord, null);
                WorkerRecord compareRecord = this.Repository.GetWorker(workerRecord.Id.Value, null);
                Assert.AreEqual(workerRecord.UpdatedOn, compareRecord.UpdatedOn);
            }
        }

        /// <summary>
        /// Delete all tests.
        /// </summary>
        protected void DeleteAll()
        {
            if (this.Repository != null)
            {
                IJob job = new TestJob() { Id = Guid.NewGuid() };

                WorkerRecord workerRecord = new WorkerRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    MachineAddress = Machine.Address,
                    MachineName = Machine.Name,
                    Name = "Test Worker",
                    QueueNames = "*",
                    Signal = WorkerSignal.Stop,
                    Status = WorkerStatus.Working,
                    Startup = WorkerStartupType.Automatic,
                    UpdatedOn = DateTime.UtcNow
                };

                this.Repository.CreateWorker(workerRecord, null);

                QueueRecord queueRecord = new QueueRecord()
                {
                    ApplicationName = workerRecord.ApplicationName,
                    Data = JsonConvert.SerializeObject(job),
                    JobName = job.Name,
                    JobType = job.GetType().FullName + ", " + job.GetType().Assembly.GetName().Name,
                    QueuedOn = DateTime.UtcNow,
                    QueueName = "*",
                    TryNumber = 1
                };

                this.Repository.CreateQueued(queueRecord, null);

                ScheduleRecord scheduleRecord = new ScheduleRecord()
                {
                    ApplicationName = workerRecord.ApplicationName,
                    Name = "Test",
                    RepeatType = ScheduleRepeatType.Days,
                    RepeatValue = 1,
                    StartOn = DateTime.UtcNow
                };

                this.Repository.CreateSchedule(scheduleRecord, null);

                ScheduledJobRecord scheduledJobRecord = new ScheduledJobRecord()
                {
                    JobType = JobSerializer.GetTypeName(job.GetType()),
                    ScheduleId = scheduleRecord.Id.Value
                };

                this.Repository.CreateScheduledJob(scheduledJobRecord, null);

                WorkingRecord workingRecord = new WorkingRecord()
                {
                    ApplicationName = workerRecord.ApplicationName,
                    Data = JobSerializer.Serialize(job),
                    JobName = job.Name,
                    JobType = JobSerializer.GetTypeName(job.GetType()),
                    QueuedOn = DateTime.UtcNow,
                    QueueName = null,
                    ScheduleId = scheduleRecord.Id,
                    StartedOn = DateTime.UtcNow,
                    TryNumber = 1,
                    WorkerId = workerRecord.Id.Value
                };

                this.Repository.CreateWorking(workingRecord, null);

                HistoryRecord historyRecord = new HistoryRecord()
                {
                    ApplicationName = workerRecord.ApplicationName,
                    Data = JobSerializer.Serialize(job),
                    FinishedOn = DateTime.UtcNow,
                    JobName = job.Name,
                    JobType = JobSerializer.GetTypeName(job.GetType()),
                    QueuedOn = DateTime.UtcNow,
                    ScheduleId = scheduleRecord.Id,
                    StartedOn = DateTime.UtcNow,
                    Status = HistoryStatus.Succeeded,
                    TryNumber = 1,
                    WorkerId = workerRecord.Id.Value
                };

                this.Repository.CreateHistory(historyRecord, null);

                this.Repository.DeleteAll(BlueCollarSection.Section.ApplicationName, null);

                Assert.AreEqual(0, this.Repository.GetHistoryList(workerRecord.ApplicationName, null, 100, 0, null).TotalCount);
                Assert.AreEqual(0, this.Repository.GetWorkingList(workerRecord.ApplicationName, null, 100, 0, null).TotalCount);
                Assert.AreEqual(0, this.Repository.GetWorkerList(workerRecord.ApplicationName, null, 100, 0, null).TotalCount);
                Assert.AreEqual(0, this.Repository.GetQueuedList(workerRecord.ApplicationName, null, 100, 0, null).TotalCount);
                Assert.AreEqual(0, this.Repository.GetScheduledJobList(workerRecord.ApplicationName, scheduleRecord.Id.Value, null, 0, 100, null).TotalCount);
                Assert.AreEqual(0, this.Repository.GetScheduleList(workerRecord.ApplicationName, null, 100, 0, null).TotalCount);
            }
        }

        /// <summary>
        /// Delete history tests.
        /// </summary>
        protected void DeleteHistory()
        {
            if (this.Repository != null)
            {
                IJob job = new TestJob() { Id = Guid.NewGuid() };
                DateTime now = DateTime.UtcNow.FloorWithSeconds();

                WorkerRecord workerRecord = new WorkerRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    MachineAddress = Machine.Address,
                    MachineName = Machine.Name,
                    Name = "Test Worker",
                    QueueNames = "*",
                    Signal = WorkerSignal.Stop,
                    Status = WorkerStatus.Working,
                    Startup = WorkerStartupType.Automatic,
                    UpdatedOn = now
                };

                this.Repository.CreateWorker(workerRecord, null);

                HistoryRecord historyRecord = new HistoryRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    Data = JobSerializer.Serialize(job),
                    FinishedOn = now,
                    JobName = job.Name,
                    JobType = JobSerializer.GetTypeName(job),
                    QueuedOn = now,
                    QueueName = QueueNameFilters.Any().ToString(),
                    StartedOn = now,
                    Status = HistoryStatus.Succeeded,
                    TryNumber = 1,
                    WorkerId = workerRecord.Id.Value
                };

                this.Repository.CreateHistory(historyRecord, null);

                historyRecord = new HistoryRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    Data = JobSerializer.Serialize(job),
                    FinishedOn = now.AddDays(-2),
                    JobName = job.Name,
                    JobType = JobSerializer.GetTypeName(job),
                    QueuedOn = now.AddDays(-2),
                    QueueName = QueueNameFilters.Any().ToString(),
                    StartedOn = now.AddDays(-2),
                    Status = HistoryStatus.Succeeded,
                    TryNumber = 1,
                    WorkerId = workerRecord.Id.Value
                };

                this.Repository.CreateHistory(historyRecord, null);
                Assert.AreEqual(2, this.Repository.GetHistoryList(BlueCollarSection.Section.ApplicationName, null, 100, 0, null).Records.Count);

                this.Repository.DeleteHistory(BlueCollarSection.Section.ApplicationName, now.AddDays(-1), null);
                Assert.AreEqual(1, this.Repository.GetHistoryList(BlueCollarSection.Section.ApplicationName, null, 100, 0, null).Records.Count);
            }
        }

        /// <summary>
        /// Delete queued tests.
        /// </summary>
        protected void DeleteQueued()
        {
            if (this.Repository != null)
            {
                IJob job = new TestJob() { Id = Guid.NewGuid() };

                WorkerRecord workerRecord = new WorkerRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    MachineAddress = Machine.Address,
                    MachineName = Machine.Name,
                    Name = "Test Worker",
                    QueueNames = "*",
                    Signal = WorkerSignal.Stop,
                    Status = WorkerStatus.Working,
                    Startup = WorkerStartupType.Automatic,
                    UpdatedOn = DateTime.UtcNow
                };

                this.Repository.CreateWorker(workerRecord, null);

                QueueRecord queueRecord = new QueueRecord()
                {
                    ApplicationName = workerRecord.ApplicationName,
                    Data = JsonConvert.SerializeObject(job),
                    JobName = job.Name,
                    JobType = job.GetType().FullName + ", " + job.GetType().Assembly.GetName().Name,
                    QueuedOn = DateTime.UtcNow,
                    QueueName = "*",
                    TryNumber = 1
                };

                this.Repository.CreateQueued(queueRecord, null);
                this.Repository.DeleteQueued(queueRecord.Id.Value, null);

                Assert.IsNull(this.Repository.GetQueued(queueRecord.ApplicationName, QueueNameFilters.Any(), DateTime.UtcNow, null));
            }
        }

        /// <summary>
        /// Delete schedule tests.
        /// </summary>
        protected void DeleteSchedule()
        {
            if (this.Repository != null)
            {
                ScheduleRecord scheduleRecord = new ScheduleRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    Name = "Test",
                    RepeatType = ScheduleRepeatType.Days,
                    RepeatValue = 1,
                    StartOn = DateTime.UtcNow.FloorWithSeconds()
                };

                this.Repository.CreateSchedule(scheduleRecord, null);
                this.Repository.DeleteSchedule(scheduleRecord.Id.Value, null);

                Assert.IsNull(this.Repository.GetScheduledJobList(BlueCollarSection.Section.ApplicationName, scheduleRecord.Id.Value, null, 1, 0, null).Id);
            }
        }

        /// <summary>
        /// Delete scheduled job tests.
        /// </summary>
        protected void DeleteScheduledJob()
        {
            if (this.Repository != null)
            {
                ScheduleRecord scheduleRecord = new ScheduleRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    Name = "Test",
                    RepeatType = ScheduleRepeatType.Days,
                    RepeatValue = 1,
                    StartOn = DateTime.UtcNow.FloorWithSeconds()
                };

                this.Repository.CreateSchedule(scheduleRecord, null);

                ScheduledJobRecord scheduledJobRecord = new ScheduledJobRecord()
                {
                    ScheduleId = scheduleRecord.Id.Value,
                    JobType = "BlueCollar.Test.TestJob, BlueCollar.Test",
                    Properties = "{}"
                };

                this.Repository.CreateScheduledJob(scheduledJobRecord, null);
                this.Repository.DeleteScheduledJob(scheduledJobRecord.Id.Value, null);
                Assert.AreEqual(0, this.Repository.GetScheduledJobList(BlueCollarSection.Section.ApplicationName, scheduleRecord.Id.Value, null, 100, 0, null).TotalCount);
            }
        }

        /// <summary>
        /// Delete worker tests.
        /// </summary>
        protected void DeleteWorker()
        {
            if (this.Repository != null)
            {
                WorkerRecord workerRecord = new WorkerRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    MachineAddress = Machine.Address,
                    MachineName = Machine.Name,
                    Name = "Test Worker",
                    QueueNames = "*",
                    Signal = WorkerSignal.Stop,
                    Status = WorkerStatus.Working,
                    Startup = WorkerStartupType.Automatic,
                    UpdatedOn = DateTime.UtcNow
                };

                this.Repository.CreateWorker(workerRecord, null);
                this.Repository.DeleteWorker(workerRecord.Id.Value, null);
                Assert.AreEqual(0, this.Repository.GetWorkerList(workerRecord.ApplicationName, null, 100, 0, null).TotalCount);
            }
        }

        /// <summary>
        /// Delete working tests.
        /// </summary>
        protected void DeleteWorking()
        {
            if (this.Repository != null)
            {
                IJob job = new TestJob() { Id = Guid.NewGuid() };

                WorkerRecord workerRecord = new WorkerRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    MachineAddress = Machine.Address,
                    MachineName = Machine.Name,
                    Name = "Test Worker",
                    QueueNames = "*",
                    Signal = WorkerSignal.Stop,
                    Status = WorkerStatus.Working,
                    Startup = WorkerStartupType.Automatic,
                    UpdatedOn = DateTime.UtcNow
                };

                this.Repository.CreateWorker(workerRecord, null);

                WorkingRecord workingRecord = new WorkingRecord()
                {
                    ApplicationName = workerRecord.ApplicationName,
                    Data = JsonConvert.SerializeObject(job),
                    JobName = job.Name,
                    JobType = job.GetType().FullName + ", " + job.GetType().Assembly.GetName().Name,
                    QueueName = "*",
                    QueuedOn = DateTime.UtcNow,
                    Signal = WorkingSignal.Cancel,
                    StartedOn = DateTime.UtcNow,
                    TryNumber = 1,
                    WorkerId = workerRecord.Id.Value
                };

                this.Repository.CreateWorking(workingRecord, null);
                this.Repository.DeleteWorking(workingRecord.Id.Value, null);
            }
        }

        /// <summary>
        /// Disposes of resources used by this instance.
        /// </summary>
        /// <param name="disposing">A value indicating whether to dispose of managed resources.</param>
        protected abstract void Dispose(bool disposing);

        /// <summary>
        /// Get counts tests.
        /// </summary>
        protected void GetCounts()
        {
            if (this.Repository != null)
            {
                IJob job = new TestJob() { Id = Guid.NewGuid() };
                string jobData = JobSerializer.Serialize(job);
                string typeName = JobSerializer.GetTypeName(job.GetType());

                WorkerRecord workerRecord = new WorkerRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    MachineAddress = Machine.Address,
                    MachineName = Machine.Name,
                    Name = "Test Worker",
                    QueueNames = "*",
                    Signal = WorkerSignal.Stop,
                    Status = WorkerStatus.Working,
                    Startup = WorkerStartupType.Automatic,
                    UpdatedOn = DateTime.UtcNow
                };

                this.Repository.CreateWorker(workerRecord, null);

                QueueRecord queueRecord = new QueueRecord()
                {
                    ApplicationName = workerRecord.ApplicationName,
                    Data = jobData,
                    JobName = job.Name,
                    JobType = typeName,
                    QueuedOn = DateTime.UtcNow,
                    QueueName = "*",
                    TryNumber = 1
                };

                this.Repository.CreateQueued(queueRecord, null);

                for (int i = 0; i < 10; i++)
                {
                    HistoryRecord historyRecord = new HistoryRecord()
                    {
                        ApplicationName = workerRecord.ApplicationName,
                        Data = jobData,
                        FinishedOn = DateTime.UtcNow,
                        JobName = job.Name,
                        JobType = typeName,
                        QueuedOn = DateTime.UtcNow,
                        QueueName = "*",
                        StartedOn = DateTime.UtcNow,
                        Status = HistoryStatus.Succeeded,
                        TryNumber = 1,
                        WorkerId = workerRecord.Id.Value
                    };

                    this.Repository.CreateHistory(historyRecord, null);
                }

                ScheduleRecord scheduleRecord = new ScheduleRecord()
                {
                    ApplicationName = workerRecord.ApplicationName,
                    Name = "Test Schedule 1",
                    RepeatType = ScheduleRepeatType.Days,
                    RepeatValue = 1,
                    StartOn = DateTime.UtcNow
                };

                this.Repository.CreateSchedule(scheduleRecord, null);

                scheduleRecord.Id = null;
                scheduleRecord.Name = "Test Schedule 2";
                this.Repository.CreateSchedule(scheduleRecord, null);

                CountsRecord counts = this.Repository.GetCounts(workerRecord.ApplicationName, null);
                Assert.IsNotNull(counts);
                Assert.AreEqual(10, counts.HistoryCount);
                Assert.AreEqual(1, counts.QueueCount);
                Assert.AreEqual(2, counts.ScheduleCount);
                Assert.AreEqual(1, counts.WorkerCount);
                Assert.AreEqual(0, counts.WorkingCount);
            }
        }

        /// <summary>
        /// Get history details tests.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Exception is used for testing purposes only; it is not thrown.")]
        protected void GetHistoryDetails()
        {
            if (this.Repository != null)
            {
                IJob job = new TestJob() { Id = Guid.NewGuid() };

                WorkerRecord workerRecord = new WorkerRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    MachineAddress = Machine.Address,
                    MachineName = Machine.Name,
                    Name = "Test Worker",
                    QueueNames = "*",
                    Signal = WorkerSignal.Stop,
                    Status = WorkerStatus.Working,
                    Startup = WorkerStartupType.Automatic,
                    UpdatedOn = DateTime.UtcNow
                };

                this.Repository.CreateWorker(workerRecord, null);

                HistoryRecord historyRecord = new HistoryRecord()
                {
                    ApplicationName = workerRecord.ApplicationName,
                    Data = JobSerializer.Serialize(job),
                    Exception = new ExceptionXElement(new Exception()).ToString(),
                    FinishedOn = DateTime.UtcNow,
                    JobName = job.Name,
                    JobType = JobSerializer.GetTypeName(job.GetType()),
                    QueuedOn = DateTime.UtcNow,
                    StartedOn = DateTime.UtcNow,
                    Status = HistoryStatus.Succeeded,
                    TryNumber = 1,
                    WorkerId = workerRecord.Id.Value
                };

                this.Repository.CreateHistory(historyRecord, null);

                HistoryDetailsRecord historyDetailsRecord = this.Repository.GetHistoryDetails(historyRecord.Id.Value, null);
                Assert.IsNotNull(historyDetailsRecord);
                Assert.AreEqual(historyDetailsRecord.Id, historyRecord.Id);
                Assert.IsFalse(string.IsNullOrEmpty(historyDetailsRecord.Data));
                Assert.IsFalse(string.IsNullOrEmpty(historyDetailsRecord.Exception));
                Assert.IsFalse(string.IsNullOrEmpty(historyDetailsRecord.WorkerMachineAddress) && string.IsNullOrEmpty(historyDetailsRecord.WorkerMachineName));
                Assert.IsFalse(string.IsNullOrEmpty(historyDetailsRecord.WorkerName));
            }
        }

        /// <summary>
        /// Get history tests.
        /// </summary>
        protected void GetHistoryList()
        {
            if (this.Repository != null)
            {
                IJob job = new TestJob() { Id = Guid.NewGuid() };
                DateTime now = DateTime.UtcNow.FloorWithSeconds();

                WorkerRecord workerRecord = new WorkerRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    MachineAddress = Machine.Address,
                    MachineName = Machine.Name,
                    Name = "Test Worker",
                    QueueNames = "*",
                    Signal = WorkerSignal.Stop,
                    Status = WorkerStatus.Working,
                    Startup = WorkerStartupType.Automatic,
                    UpdatedOn = now
                };

                this.Repository.CreateWorker(workerRecord, null);

                HistoryRecord historyRecord = new HistoryRecord()
                {
                    ApplicationName = workerRecord.ApplicationName,
                    Data = JobSerializer.Serialize(job),
                    FinishedOn = now,
                    JobName = job.Name,
                    JobType = JobSerializer.GetTypeName(job.GetType()),
                    QueuedOn = now,
                    StartedOn = now,
                    Status = HistoryStatus.Succeeded,
                    TryNumber = 1,
                    WorkerId = workerRecord.Id.Value
                };

                this.Repository.CreateHistory(historyRecord, null);

                var list = this.Repository.GetHistoryList(workerRecord.ApplicationName, null, 100, 0, null);
                Assert.IsNotNull(list);
                Assert.AreEqual(1, list.TotalCount);
                Assert.AreEqual(1, list.Records.Count);

                list = this.Repository.GetHistoryList(workerRecord.ApplicationName, "boo", 100, 0, null);
                Assert.IsNotNull(list);
                Assert.AreEqual(0, list.TotalCount);
                Assert.AreEqual(0, list.Records.Count);

                ScheduleRecord scheduleRecord = new ScheduleRecord()
                {
                    ApplicationName = workerRecord.ApplicationName,
                    Enabled = true,
                    Name = "Test",
                    QueueName = "scheduled",
                    RepeatType = ScheduleRepeatType.None,
                    StartOn = now
                };

                this.Repository.CreateSchedule(scheduleRecord, null);

                historyRecord = new HistoryRecord()
                {
                    ApplicationName = workerRecord.ApplicationName,
                    Data = JobSerializer.Serialize(job),
                    FinishedOn = now,
                    JobName = job.Name,
                    JobType = JobSerializer.GetTypeName(job.GetType()),
                    QueuedOn = now,
                    ScheduleId = scheduleRecord.Id.Value,
                    StartedOn = now,
                    Status = HistoryStatus.Succeeded,
                    TryNumber = 1,
                    WorkerId = workerRecord.Id.Value
                };

                this.Repository.CreateHistory(historyRecord, null);

                list = this.Repository.GetHistoryList(workerRecord.ApplicationName, null, 100, 0, null);
                Assert.IsNotNull(list);
                Assert.AreEqual(2, list.TotalCount);
                Assert.AreEqual(2, list.Records.Count);
                Assert.IsTrue(list.Records.Where(h => h.ScheduleName == "Test").Count() == 1);
            }
        }

        /// <summary>
        /// Get queue tests.
        /// </summary>
        protected void GetQueued()
        {
            if (this.Repository != null)
            {
                IJob job = new TestJob() { Id = Guid.NewGuid() };
                DateTime now = DateTime.UtcNow.FloorWithSeconds().AddSeconds(-1);

                WorkerRecord workerRecord = new WorkerRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    MachineAddress = Machine.Address,
                    MachineName = Machine.Name,
                    Name = "Test Worker",
                    QueueNames = "*",
                    Signal = WorkerSignal.Stop,
                    Status = WorkerStatus.Working,
                    Startup = WorkerStartupType.Automatic,
                    UpdatedOn = now
                };

                this.Repository.CreateWorker(workerRecord, null);

                QueueRecord queueRecord = new QueueRecord()
                {
                    ApplicationName = workerRecord.ApplicationName,
                    Data = JobSerializer.Serialize(job),
                    JobName = job.Name,
                    JobType = JobSerializer.GetTypeName(job.GetType()),
                    QueuedOn = now,
                    QueueName = "*",
                    TryNumber = 1
                };

                this.Repository.CreateQueued(queueRecord, null);
                Assert.IsNotNull(this.Repository.GetQueued(workerRecord.ApplicationName, QueueNameFilters.Any(), DateTime.UtcNow, null));
            }
        }

        /// <summary>
        /// Get queued details tests.
        /// </summary>
        protected void GetQueuedDetails()
        {
            if (this.Repository != null)
            {
                IJob job = new TestJob() { Id = Guid.NewGuid() };

                WorkerRecord workerRecord = new WorkerRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    MachineAddress = Machine.Address,
                    MachineName = Machine.Name,
                    Name = "Test Worker",
                    QueueNames = "*",
                    Signal = WorkerSignal.Stop,
                    Status = WorkerStatus.Working,
                    Startup = WorkerStartupType.Automatic,
                    UpdatedOn = DateTime.UtcNow
                };

                this.Repository.CreateWorker(workerRecord, null);

                QueueRecord queueRecord = new QueueRecord()
                {
                    ApplicationName = workerRecord.ApplicationName,
                    Data = JobSerializer.Serialize(job),
                    JobName = job.Name,
                    JobType = JobSerializer.GetTypeName(job.GetType()),
                    QueuedOn = DateTime.UtcNow,
                    QueueName = "*",
                    TryNumber = 1
                };

                this.Repository.CreateQueued(queueRecord, null);

                QueueDetailsRecord queueDetailsRecord = this.Repository.GetQueuedDetails(queueRecord.Id.Value, null);
                Assert.IsNotNull(queueDetailsRecord);
                Assert.AreEqual(queueRecord.Id, queueDetailsRecord.Id);
                Assert.IsFalse(string.IsNullOrEmpty(queueDetailsRecord.Data));
            }
        }

        /// <summary>
        /// Get queued list tests.
        /// </summary>
        protected void GetQueuedList()
        {
            if (this.Repository != null)
            {
                IJob job = new TestJob() { Id = Guid.NewGuid() };

                WorkerRecord workerRecord = new WorkerRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    MachineAddress = Machine.Address,
                    MachineName = Machine.Name,
                    Name = "Test Worker",
                    QueueNames = "*",
                    Signal = WorkerSignal.Stop,
                    Status = WorkerStatus.Working,
                    Startup = WorkerStartupType.Automatic,
                    UpdatedOn = DateTime.UtcNow
                };

                this.Repository.CreateWorker(workerRecord, null);

                QueueRecord queueRecord = new QueueRecord()
                {
                    ApplicationName = workerRecord.ApplicationName,
                    Data = JobSerializer.Serialize(job),
                    JobName = job.Name,
                    JobType = JobSerializer.GetTypeName(job.GetType()),
                    QueuedOn = DateTime.UtcNow,
                    QueueName = "*",
                    TryNumber = 1
                };

                this.Repository.CreateQueued(queueRecord, null);

                var list = this.Repository.GetQueuedList(workerRecord.ApplicationName, null, 100, 0, null);
                Assert.IsNotNull(list);
                Assert.AreEqual(1, list.TotalCount);
                Assert.AreEqual(1, list.Records.Count);

                list = this.Repository.GetQueuedList(workerRecord.ApplicationName, "boo", 100, 0, null);
                Assert.IsNotNull(list);
                Assert.AreEqual(0, list.TotalCount);
                Assert.AreEqual(0, list.Records.Count);
            }
        }

        /// <summary>
        /// Get schedule tsts.
        /// </summary>
        protected void GetSchedule()
        {
            if (this.Repository != null)
            {
                Assert.IsNull(this.Repository.GetSchedule(12, null));

                ScheduleRecord scheduleRecord = new ScheduleRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    Name = "Test",
                    RepeatType = ScheduleRepeatType.Days,
                    RepeatValue = 1,
                    StartOn = DateTime.UtcNow
                };

                this.Repository.CreateSchedule(scheduleRecord, null);
                Assert.IsNotNull(this.Repository.GetSchedule(scheduleRecord.Id.Value, null));
            }
        }

        /// <summary>
        /// Get schedule date exists for schedule tests.
        /// </summary>
        protected void GetScheduleDateExistsForSchedule()
        {
            if (this.Repository != null)
            {
                DateTime now = DateTime.UtcNow.FloorWithSeconds();
                TestJob job = new TestJob();

                ScheduleRecord scheduleRecord = new ScheduleRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    Name = "Test",
                    RepeatType = ScheduleRepeatType.Days,
                    RepeatValue = 1,
                    StartOn = now.AddDays(-1)
                };

                this.Repository.CreateSchedule(scheduleRecord, null);

                ScheduledJobRecord scheduledJobRecord = new ScheduledJobRecord()
                {
                    JobType = JobSerializer.GetTypeName(job),
                    ScheduleId = scheduleRecord.Id.Value
                };

                this.Repository.CreateScheduledJob(scheduledJobRecord, null);
                Assert.IsFalse(this.Repository.GetScheduleDateExistsForSchedule(scheduleRecord.Id.Value, now, null));

                QueueRecord queueRecord = new QueueRecord()
                {
                    ApplicationName = scheduleRecord.ApplicationName,
                    Data = JobSerializer.Serialize(job),
                    JobName = job.Name,
                    JobType = JobSerializer.GetTypeName(job),
                    QueuedOn = now,
                    ScheduleId = scheduleRecord.Id,
                    TryNumber = 1
                };

                this.Repository.CreateQueued(queueRecord, null);
                Assert.IsTrue(this.Repository.GetScheduleDateExistsForSchedule(scheduleRecord.Id.Value, now, null));
            }
        }

        /// <summary>
        /// Get scheduled job list tests.
        /// </summary>
        protected void GetScheduledJobList()
        {
            if (this.Repository != null)
            {
                ScheduleRecord scheduleRecord = new ScheduleRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    Name = "Test",
                    RepeatType = ScheduleRepeatType.Days,
                    RepeatValue = 1,
                    StartOn = DateTime.UtcNow.FloorWithSeconds()
                };

                this.Repository.CreateSchedule(scheduleRecord, null);

                ScheduledJobRecordList list = this.Repository.GetScheduledJobList(BlueCollarSection.Section.ApplicationName, scheduleRecord.Id.Value, null, 1, 0, null);
                Assert.IsNotNull(list.Id);
                Assert.AreEqual(0, list.Records.Count);

                list = this.Repository.GetScheduledJobList(BlueCollarSection.Section.ApplicationName, scheduleRecord.Id.Value, "boo", 50, 0, null);
                Assert.IsNotNull(list.Id);
                Assert.AreEqual(0, list.Records.Count);
            }
        }

        /// <summary>
        /// Get schedule enqueueing lock tests.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Enqueueing", Justification = "The spelling is correct.")]
        protected void GetScheduleEnqueueingLock()
        {
            if (this.Repository != null)
            {
                ScheduleRecord scheduleRecord = new ScheduleRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    Name = "Nightly",
                    QueueName = "schedules",
                    RepeatType = ScheduleRepeatType.Days,
                    RepeatValue = 1,
                    StartOn = DateTime.UtcNow.FloorWithSeconds()
                };

                this.Repository.CreateSchedule(scheduleRecord, null);

                Assert.IsTrue(this.Repository.GetScheduleEnqueueingLock(scheduleRecord.Id.Value, null));
                Assert.IsFalse(this.Repository.GetScheduleEnqueueingLock(scheduleRecord.Id.Value, null));
            }
        }

        /// <summary>
        /// Get schedule list tests.
        /// </summary>
        protected void GetScheduleList()
        {
            if (this.Repository != null)
            {
                ScheduleRecord scheduleRecord = new ScheduleRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    Name = "Nightly",
                    QueueName = "schedules",
                    RepeatType = ScheduleRepeatType.Days,
                    RepeatValue = 1,
                    StartOn = DateTime.UtcNow.FloorWithSeconds()
                };

                this.Repository.CreateSchedule(scheduleRecord, null);

                ScheduledJobRecord scheduledJobRecord = new ScheduledJobRecord()
                {
                    ScheduleId = scheduleRecord.Id.Value,
                    JobType = "BlueCollar.TestScheduledJob, BlueCollar",
                    Properties = "{}"
                };

                this.Repository.CreateScheduledJob(scheduledJobRecord, null);

                scheduleRecord.Id = null;
                scheduleRecord.Name = "Weekly";
                scheduleRecord.RepeatType = ScheduleRepeatType.Weeks;
                this.Repository.CreateSchedule(scheduleRecord, null);

                scheduledJobRecord = new ScheduledJobRecord()
                {
                    ScheduleId = scheduleRecord.Id.Value,
                    JobType = "BlueCollar.TestScheduledJob2, BlueCollar",
                    Properties = "{}"
                };

                this.Repository.CreateScheduledJob(scheduledJobRecord, null);

                scheduledJobRecord = new ScheduledJobRecord()
                {
                    ScheduleId = scheduleRecord.Id.Value,
                    JobType = "BlueCollar.TestScheduledJob3, BlueCollar",
                    Properties = "{}"
                };

                this.Repository.CreateScheduledJob(scheduledJobRecord, null);

                RecordList<ScheduleListRecord> schedules = this.Repository.GetScheduleList(BlueCollarSection.Section.ApplicationName, null, 100, 0, null);
                Assert.IsNotNull(schedules);
                Assert.AreEqual(2, schedules.TotalCount);
                Assert.AreEqual(2, schedules.Records.Count);
                Assert.AreEqual("Nightly", schedules.Records[0].Name);
                Assert.AreEqual(1, schedules.Records[0].JobCount);
                Assert.AreEqual("Weekly", schedules.Records[1].Name);
                Assert.AreEqual(2, schedules.Records[1].JobCount);
            }
        }

        /// <summary>
        /// Get schedules tests.
        /// </summary>
        protected void GetSchedules()
        {
            if (this.Repository != null)
            {
                ScheduleRecord scheduleRecord = new ScheduleRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    Name = "Nightly",
                    QueueName = "schedules",
                    RepeatType = ScheduleRepeatType.Days,
                    RepeatValue = 1,
                    StartOn = DateTime.UtcNow.FloorWithSeconds()
                };

                this.Repository.CreateSchedule(scheduleRecord, null);

                ScheduledJobRecord scheduledJobRecord = new ScheduledJobRecord()
                {
                    ScheduleId = scheduleRecord.Id.Value,
                    JobType = "BlueCollar.TestScheduledJob, BlueCollar",
                    Properties = "{}"
                };

                this.Repository.CreateScheduledJob(scheduledJobRecord, null);

                scheduleRecord.Id = null;
                scheduleRecord.Name = "Weekly";
                scheduleRecord.RepeatType = ScheduleRepeatType.Weeks;
                this.Repository.CreateSchedule(scheduleRecord, null);

                scheduledJobRecord = new ScheduledJobRecord()
                {
                    ScheduleId = scheduleRecord.Id.Value,
                    JobType = "BlueCollar.TestScheduledJob2, BlueCollar",
                    Properties = "{}"
                };

                this.Repository.CreateScheduledJob(scheduledJobRecord, null);

                scheduledJobRecord = new ScheduledJobRecord()
                {
                    ScheduleId = scheduleRecord.Id.Value,
                    JobType = "BlueCollar.TestScheduledJob3, BlueCollar",
                    Properties = "{}"
                };

                this.Repository.CreateScheduledJob(scheduledJobRecord, null);

                var schedules = this.Repository.GetSchedules(scheduleRecord.ApplicationName, null);
                Assert.AreEqual(2, schedules.Count());
                Assert.AreEqual(1, schedules.ElementAt(0).ScheduledJobs.Count);
                Assert.AreEqual(2, schedules.ElementAt(1).ScheduledJobs.Count);
            }
        }

        /// <summary>
        /// Get statistics tests.
        /// </summary>
        protected void GetStatistics()
        {
            if (this.Repository != null)
            {
                IJob job = new TestJob() { Id = Guid.NewGuid() };
                string jobData = JobSerializer.Serialize(job);
                string typeName = JobSerializer.GetTypeName(job.GetType());
                DateTime date = new DateTime(2011, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                DateTime now = DateTime.UtcNow.FloorWithSeconds();
                HistoryRecord historyRecord;

                WorkerRecord workerRecord = new WorkerRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    MachineAddress = Machine.Address,
                    MachineName = Machine.Name,
                    Name = "Test Worker",
                    QueueNames = "*",
                    Signal = WorkerSignal.Stop,
                    Status = WorkerStatus.Working,
                    Startup = WorkerStartupType.Automatic,
                    UpdatedOn = DateTime.UtcNow
                };

                this.Repository.CreateWorker(workerRecord, null);

                StatisticsRecord stats = this.Repository.GetStatistics(workerRecord.ApplicationName, date.AddDays(-1), date.AddDays(-14), date, null);
                Assert.IsNotNull(stats);

                /*
                 * Dequeued per hour by day.
                 */

                for (int i = 0; i < 14; i++)
                {
                    for (int j = 0; j < 24; j++)
                    {
                        for (int k = 0; k < 10; k++)
                        {
                            DateTime finished = date.AddDays(i).AddHours(j);

                            historyRecord = new HistoryRecord()
                            {
                                ApplicationName = workerRecord.ApplicationName,
                                Data = jobData,
                                FinishedOn = finished,
                                JobName = job.Name,
                                JobType = typeName,
                                QueuedOn = finished.AddSeconds(-1),
                                QueueName = "*",
                                StartedOn = finished.AddSeconds(-11),
                                Status = HistoryStatus.Succeeded,
                                TryNumber = 1,
                                WorkerId = workerRecord.Id.Value
                            };

                            this.Repository.CreateHistory(historyRecord, null);
                        }
                    }
                }

                stats = this.Repository.GetStatistics(workerRecord.ApplicationName, date.AddDays(13), date, date.AddDays(14), null);
                Assert.AreEqual(14, stats.JobsPerHourByDay.Count);

                for (int i = 0; i < stats.JobsPerHourByDay.Count; i++)
                {
                    Assert.AreEqual(date.AddDays(i), stats.JobsPerHourByDay[i].Date);
                    Assert.AreEqual(10, stats.JobsPerHourByDay[i].JobsPerHour);
                }

                /*
                 * Dequeued per hour by day - multiple queues.
                 */

                this.Repository.DeleteAll(BlueCollarSection.Section.ApplicationName, null);

                workerRecord.Id = null;
                this.Repository.CreateWorker(workerRecord, null);

                historyRecord = new HistoryRecord()
                {
                    ApplicationName = workerRecord.ApplicationName,
                    Data = jobData,
                    FinishedOn = now.AddHours(-1),
                    JobName = job.Name,
                    JobType = typeName,
                    QueuedOn = now.AddHours(-1),
                    QueueName = "*",
                    StartedOn = now.AddHours(-1),
                    Status = HistoryStatus.Succeeded,
                    TryNumber = 1,
                    WorkerId = workerRecord.Id.Value
                };

                this.Repository.CreateHistory(historyRecord, null);

                historyRecord = new HistoryRecord()
                {
                    ApplicationName = workerRecord.ApplicationName,
                    Data = jobData,
                    FinishedOn = now.AddHours(-1),
                    JobName = job.Name,
                    JobType = typeName,
                    QueuedOn = now.AddHours(-1),
                    QueueName = "test",
                    StartedOn = now.AddHours(-1),
                    Status = HistoryStatus.Succeeded,
                    TryNumber = 1,
                    WorkerId = workerRecord.Id.Value
                };

                this.Repository.CreateHistory(historyRecord, null);

                stats = this.Repository.GetStatistics(workerRecord.ApplicationName, now.AddDays(-1), now.AddDays(-14), now, null);
                Assert.AreEqual(2, stats.JobsPerHourByDay.Count);
                Assert.AreEqual("*", stats.JobsPerHourByDay[0].QueueName);
                Assert.AreEqual("test", stats.JobsPerHourByDay[1].QueueName);

                /*
                 * History status counts.
                 */

                this.Repository.DeleteAll(BlueCollarSection.Section.ApplicationName, null);

                Func<int, DateTime> getDate = (int i) =>
                {
                    if (i % 3 == 0)
                    {
                        return now.AddDays(-30);
                    }
                    else if (i % 3 == 1)
                    {
                        return now.AddDays(-10);
                    }
                    else if (i % 3 == 2)
                    {
                        return now.AddHours(-13);
                    }
                    else
                    {
                        throw new InvalidOperationException("Loop count must be a multiple of 3.");
                    }
                };

                Func<int, HistoryStatus> getStatus = (int i) =>
                {
                    if (i % 5 == 0)
                    {
                        return HistoryStatus.Succeeded;
                    }
                    else if (i % 5 == 1)
                    {
                        return HistoryStatus.Failed;
                    }
                    else if (i % 5 == 2)
                    {
                        return HistoryStatus.Canceled;
                    }
                    else if (i % 5 == 3)
                    {
                        return HistoryStatus.TimedOut;
                    }
                    else if (i % 5 == 4)
                    {
                        return HistoryStatus.Interrupted;
                    }
                    else
                    {
                        throw new InvalidOperationException("Loop count must be a multiple of 5.");
                    }
                };

                for (int i = 0; i < 150; i++)
                {
                    DateTime indexDate = getDate(i);

                    historyRecord = new HistoryRecord()
                    {
                        ApplicationName = workerRecord.ApplicationName,
                        Data = jobData,
                        FinishedOn = indexDate,
                        JobName = job.Name,
                        JobType = typeName,
                        QueuedOn = date.AddSeconds(-2),
                        QueueName = "*",
                        StartedOn = date.AddSeconds(-1),
                        Status = getStatus(i),
                        TryNumber = 1,
                        WorkerId = workerRecord.Id.Value
                    };

                    this.Repository.CreateHistory(historyRecord, null);
                }

                stats = this.Repository.GetStatistics(workerRecord.ApplicationName, now.AddHours(-24), now.AddDays(-14), now, null);
                Assert.IsNotNull(stats.HistoryStatusRecent);
                Assert.AreEqual(10, stats.HistoryStatusRecent.SucceededCount);
                Assert.AreEqual(10, stats.HistoryStatusRecent.FailedCount);
                Assert.AreEqual(10, stats.HistoryStatusRecent.CanceledCount);
                Assert.AreEqual(10, stats.HistoryStatusRecent.TimedOutCount);
                Assert.AreEqual(10, stats.HistoryStatusRecent.InterruptedCount);
                Assert.AreEqual(50, stats.HistoryStatusRecent.TotalCount);

                Assert.IsNotNull(stats.HistoryStatusDistant);
                Assert.AreEqual(20, stats.HistoryStatusDistant.SucceededCount);
                Assert.AreEqual(20, stats.HistoryStatusDistant.FailedCount);
                Assert.AreEqual(20, stats.HistoryStatusDistant.CanceledCount);
                Assert.AreEqual(20, stats.HistoryStatusDistant.TimedOutCount);
                Assert.AreEqual(20, stats.HistoryStatusDistant.InterruptedCount);
                Assert.AreEqual(100, stats.HistoryStatusDistant.TotalCount);

                stats = this.Repository.GetStatistics(workerRecord.ApplicationName, now.AddDays(-31), now.AddDays(-60), now.AddDays(-30), null);
                Assert.AreEqual(10, stats.HistoryStatusDistant.SucceededCount);
                Assert.AreEqual(10, stats.HistoryStatusDistant.FailedCount);
                Assert.AreEqual(10, stats.HistoryStatusDistant.CanceledCount);
                Assert.AreEqual(10, stats.HistoryStatusDistant.TimedOutCount);
                Assert.AreEqual(10, stats.HistoryStatusDistant.InterruptedCount);
                Assert.AreEqual(50, stats.HistoryStatusDistant.TotalCount);
            }
        }

        /// <summary>
        /// Get worker tests.
        /// </summary>
        protected void GetWorker()
        {
            if (this.Repository != null)
            {
                WorkerRecord workerRecord = new WorkerRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    MachineAddress = Machine.Address,
                    MachineName = Machine.Name,
                    Name = "Test Worker",
                    QueueNames = "*",
                    Signal = WorkerSignal.Stop,
                    Status = WorkerStatus.Working,
                    Startup = WorkerStartupType.Automatic,
                    UpdatedOn = DateTime.UtcNow
                };

                this.Repository.CreateWorker(workerRecord, null);
                Assert.IsNotNull(this.Repository.GetWorker(workerRecord.Id.Value, null));
            }
        }

        /// <summary>
        /// Get worker list tests.
        /// </summary>
        protected void GetWorkerList()
        {
            if (this.Repository != null)
            {
                WorkerRecord workerRecord = new WorkerRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    Name = "Test",
                    MachineName = "PRODUCTION",
                    MachineAddress = "127.0.0.1",
                    QueueNames = "*",
                    Status = WorkerStatus.Working,
                    Signal = WorkerSignal.None,
                    Startup = WorkerStartupType.Automatic,
                    UpdatedOn = DateTime.UtcNow
                };

                this.Repository.CreateWorker(workerRecord, null);

                RecordList<WorkerRecord> list = this.Repository.GetWorkerList(BlueCollarSection.Section.ApplicationName, null, 1, 0, null);
                Assert.IsNotNull(list);
                Assert.AreEqual(1, list.Records.Count);

                list = this.Repository.GetWorkerList(BlueCollarSection.Section.ApplicationName, "boo", 50, 0, null);
                Assert.IsNotNull(list);
                Assert.AreEqual(0, list.Records.Count);
            }
        }

        /// <summary>
        /// Get workers tests.
        /// </summary>
        protected void GetWorkers()
        {
            if (this.Repository != null)
            {
                WorkerRecord workerRecord = new WorkerRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    MachineAddress = Machine.Address,
                    MachineName = Machine.Name,
                    Name = "Test Worker",
                    QueueNames = "*",
                    Signal = WorkerSignal.Stop,
                    Status = WorkerStatus.Working,
                    Startup = WorkerStartupType.Automatic,
                    UpdatedOn = DateTime.UtcNow
                };

                WorkerRecord workerRecord2 = new WorkerRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    MachineAddress = Machine.Address,
                    MachineName = Machine.Name,
                    Name = "Test Worker 2",
                    QueueNames = "schedules",
                    Signal = WorkerSignal.Stop,
                    Status = WorkerStatus.Working,
                    Startup = WorkerStartupType.Automatic,
                    UpdatedOn = DateTime.UtcNow
                };

                WorkerRecord workerRecord3 = new WorkerRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    MachineAddress = "1.2.3.4.5",
                    MachineName = "DIFFERENTMACHINE",
                    Name = "Test Worker",
                    QueueNames = "*",
                    Signal = WorkerSignal.Stop,
                    Status = WorkerStatus.Working,
                    Startup = WorkerStartupType.Automatic,
                    UpdatedOn = DateTime.UtcNow
                };

                WorkerRecord workerRecord4 = new WorkerRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    MachineAddress = "1.2.3.4.5",
                    MachineName = null,
                    Name = "Test Worker 4",
                    QueueNames = "*",
                    Signal = WorkerSignal.Stop,
                    Status = WorkerStatus.Working,
                    Startup = WorkerStartupType.Automatic,
                    UpdatedOn = DateTime.UtcNow
                };

                WorkerRecord workerRecord5 = new WorkerRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    MachineAddress = null,
                    MachineName = "DIFFERENTMACHINE",
                    Name = "Test Worker 5",
                    QueueNames = "*",
                    Signal = WorkerSignal.Stop,
                    Status = WorkerStatus.Working,
                    Startup = WorkerStartupType.Automatic,
                    UpdatedOn = DateTime.UtcNow
                };

                this.Repository.CreateWorker(workerRecord, null);
                this.Repository.CreateWorker(workerRecord2, null);
                this.Repository.CreateWorker(workerRecord3, null);
                this.Repository.CreateWorker(workerRecord4, null);
                this.Repository.CreateWorker(workerRecord5, null);

                Assert.AreEqual(2, this.Repository.GetWorkers(BlueCollarSection.Section.ApplicationName, Machine.Address, Machine.Name, null).Count());
                Assert.AreEqual(3, this.Repository.GetWorkers(BlueCollarSection.Section.ApplicationName, "1.2.3.4.5", "DIFFERENTMACHINE", null).Count());
            }
        }

        /// <summary>
        /// Get working tests.
        /// </summary>
        protected void GetWorking()
        {
            if (this.Repository != null)
            {
                IJob job = new TestJob() { Id = Guid.NewGuid() };

                WorkerRecord workerRecord = new WorkerRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    MachineAddress = Machine.Address,
                    MachineName = Machine.Name,
                    Name = "Test Worker",
                    QueueNames = "*",
                    Signal = WorkerSignal.Stop,
                    Status = WorkerStatus.Working,
                    Startup = WorkerStartupType.Automatic,
                    UpdatedOn = DateTime.UtcNow
                };

                this.Repository.CreateWorker(workerRecord, null);

                WorkingRecord workingRecord = new WorkingRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    Data = JobSerializer.Serialize(job),
                    JobName = job.Name,
                    JobType = JobSerializer.GetTypeName(job.GetType()),
                    QueuedOn = DateTime.UtcNow,
                    QueueName = "*",
                    Signal = WorkingSignal.None,
                    StartedOn = DateTime.UtcNow,
                    TryNumber = 1,
                    WorkerId = workerRecord.Id.Value
                };

                this.Repository.CreateWorking(workingRecord, null);
                Assert.IsNotNull(this.Repository.GetWorking(workingRecord.Id.Value, null));
            }
        }

        /// <summary>
        /// Get working details tests.
        /// </summary>
        protected void GetWorkingDetails()
        {
            if (this.Repository != null)
            {
                IJob job = new TestJob() { Id = Guid.NewGuid() };

                WorkerRecord workerRecord = new WorkerRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    MachineAddress = Machine.Address,
                    MachineName = Machine.Name,
                    Name = "Test Worker",
                    QueueNames = "*",
                    Signal = WorkerSignal.Stop,
                    Status = WorkerStatus.Working,
                    Startup = WorkerStartupType.Automatic,
                    UpdatedOn = DateTime.UtcNow
                };

                this.Repository.CreateWorker(workerRecord, null);

                WorkingRecord workingRecord = new WorkingRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    Data = JobSerializer.Serialize(job),
                    JobName = job.Name,
                    JobType = JobSerializer.GetTypeName(job.GetType()),
                    QueuedOn = DateTime.UtcNow,
                    QueueName = "*",
                    Signal = WorkingSignal.None,
                    StartedOn = DateTime.UtcNow,
                    TryNumber = 1,
                    WorkerId = workerRecord.Id.Value
                };

                this.Repository.CreateWorking(workingRecord, null);

                WorkingDetailsRecord workingDetailsRecord = this.Repository.GetWorkingDetails(workingRecord.Id.Value, null);
                Assert.IsNotNull(workingDetailsRecord);
                Assert.IsFalse(string.IsNullOrEmpty(workingDetailsRecord.Data));
            }
        }

        /// <summary>
        /// Get working for worker tests.
        /// </summary>
        protected void GetWorkingForWorker()
        {
            if (this.Repository != null)
            {
                IJob job = new TestJob() { Id = Guid.NewGuid() };

                WorkerRecord workerRecord = new WorkerRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    MachineAddress = Machine.Address,
                    MachineName = Machine.Name,
                    Name = "Test Worker",
                    QueueNames = "*",
                    Signal = WorkerSignal.Stop,
                    Status = WorkerStatus.Working,
                    Startup = WorkerStartupType.Automatic,
                    UpdatedOn = DateTime.UtcNow
                };

                this.Repository.CreateWorker(workerRecord, null);

                WorkingRecord workingRecord = new WorkingRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    Data = JobSerializer.Serialize(job),
                    JobName = job.Name,
                    JobType = JobSerializer.GetTypeName(job.GetType()),
                    QueuedOn = DateTime.UtcNow,
                    QueueName = "*",
                    Signal = WorkingSignal.None,
                    StartedOn = DateTime.UtcNow,
                    TryNumber = 1,
                    WorkerId = workerRecord.Id.Value
                };

                this.Repository.CreateWorking(workingRecord, null);

                WorkerRecord workerRecord2 = new WorkerRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    MachineAddress = Machine.Address,
                    MachineName = Machine.Name,
                    Name = "Test Worker 2",
                    QueueNames = "*",
                    Signal = WorkerSignal.Stop,
                    Status = WorkerStatus.Working,
                    Startup = WorkerStartupType.Automatic,
                    UpdatedOn = DateTime.UtcNow
                };

                this.Repository.CreateWorker(workerRecord2, null);

                WorkingRecord workingRecord2 = new WorkingRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    Data = JobSerializer.Serialize(job),
                    JobName = job.Name,
                    JobType = JobSerializer.GetTypeName(job.GetType()),
                    QueuedOn = DateTime.UtcNow,
                    QueueName = "*",
                    Signal = WorkingSignal.None,
                    StartedOn = DateTime.UtcNow,
                    TryNumber = 1,
                    WorkerId = workerRecord2.Id.Value
                };

                this.Repository.CreateWorking(workingRecord2, null);

                WorkingRecord workingRecord3 = new WorkingRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    Data = JobSerializer.Serialize(job),
                    JobName = job.Name,
                    JobType = JobSerializer.GetTypeName(job.GetType()),
                    QueuedOn = DateTime.UtcNow,
                    QueueName = "*",
                    Signal = WorkingSignal.None,
                    StartedOn = DateTime.UtcNow,
                    TryNumber = 1,
                    WorkerId = workerRecord2.Id.Value
                };

                this.Repository.CreateWorking(workingRecord3, null);

                var working = this.Repository.GetWorkingForWorker(workerRecord.Id.Value, null, null);
                Assert.AreEqual(1, working.Count());
                Assert.AreEqual(workingRecord.Id, working.ElementAt(0).Id);

                working = this.Repository.GetWorkingForWorker(workerRecord2.Id.Value, null, null);
                Assert.AreEqual(2, working.Count());
                Assert.AreEqual(workingRecord2.Id, working.ElementAt(0).Id);
                Assert.AreEqual(workingRecord3.Id, working.ElementAt(1).Id);

                working = this.Repository.GetWorkingForWorker(workerRecord2.Id.Value, workingRecord2.Id, null);
                Assert.AreEqual(1, working.Count());
                Assert.AreEqual(workingRecord3.Id, working.ElementAt(0).Id);
            }
        }

        /// <summary>
        /// Get working list tests.
        /// </summary>
        protected void GetWorkingList()
        {
            if (this.Repository != null)
            {
                IJob job = new TestJob() { Id = Guid.NewGuid() };

                WorkerRecord workerRecord = new WorkerRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    MachineAddress = Machine.Address,
                    MachineName = Machine.Name,
                    Name = "Test Worker",
                    QueueNames = "*",
                    Signal = WorkerSignal.Stop,
                    Status = WorkerStatus.Working,
                    Startup = WorkerStartupType.Automatic,
                    UpdatedOn = DateTime.UtcNow
                };

                this.Repository.CreateWorker(workerRecord, null);

                WorkingRecord workingRecord = new WorkingRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    Data = JobSerializer.Serialize(job),
                    JobName = job.Name,
                    JobType = JobSerializer.GetTypeName(job.GetType()),
                    QueuedOn = DateTime.UtcNow,
                    QueueName = "*",
                    Signal = WorkingSignal.None,
                    StartedOn = DateTime.UtcNow,
                    TryNumber = 1,
                    WorkerId = workerRecord.Id.Value
                };

                this.Repository.CreateWorking(workingRecord, null);
                Assert.AreEqual(1, this.Repository.GetWorkingList(BlueCollarSection.Section.ApplicationName, null, 100, 0, null).TotalCount);
                Assert.AreEqual(1, this.Repository.GetWorkingList(BlueCollarSection.Section.ApplicationName, "TestJob", 100, 0, null).TotalCount);
                Assert.AreEqual(0, this.Repository.GetWorkingList(BlueCollarSection.Section.ApplicationName, "not found search", 100, 0, null).TotalCount);

                WorkingRecord workingRecord2 = new WorkingRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    Data = JobSerializer.Serialize(job),
                    JobName = job.Name,
                    JobType = JobSerializer.GetTypeName(job.GetType()),
                    QueuedOn = DateTime.UtcNow,
                    QueueName = "*",
                    Signal = WorkingSignal.None,
                    StartedOn = DateTime.UtcNow,
                    TryNumber = 1,
                    WorkerId = workerRecord.Id.Value
                };

                this.Repository.CreateWorking(workingRecord2, null);
                Assert.AreEqual(2, this.Repository.GetWorkingList(BlueCollarSection.Section.ApplicationName, null, 100, 0, null).TotalCount);
                Assert.AreEqual(1, this.Repository.GetWorkingList(BlueCollarSection.Section.ApplicationName, null, 1, 0, null).Records.Count);
                Assert.AreEqual(1, this.Repository.GetWorkingList(BlueCollarSection.Section.ApplicationName, null, 100, 1, null).Records.Count);
            }
        }

        /// <summary>
        /// Get working signals tests.
        /// </summary>
        protected void GetWorkingSignals()
        {
            if (this.Repository != null)
            {
                IJob job = new TestJob() { Id = Guid.NewGuid() };

                WorkerRecord workerRecord = new WorkerRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    MachineAddress = Machine.Address,
                    MachineName = Machine.Name,
                    Name = "Test Worker",
                    QueueNames = "*",
                    Signal = WorkerSignal.Stop,
                    Status = WorkerStatus.Working,
                    Startup = WorkerStartupType.Automatic,
                    UpdatedOn = DateTime.UtcNow
                };

                this.Repository.CreateWorker(workerRecord, null);

                SignalsRecord signals = this.Repository.GetWorkingSignals(workerRecord.Id.Value, null, null);
                Assert.IsNotNull(signals);
                Assert.IsFalse(string.IsNullOrEmpty(signals.QueueNames));
                Assert.AreEqual(WorkingSignal.None, signals.WorkingSignal);

                WorkingRecord workingRecord = new WorkingRecord()
                {
                    ApplicationName = workerRecord.ApplicationName,
                    Data = JsonConvert.SerializeObject(job),
                    JobName = job.Name,
                    JobType = job.GetType().FullName + ", " + job.GetType().Assembly.GetName().Name,
                    QueueName = "*",
                    QueuedOn = DateTime.UtcNow,
                    Signal = WorkingSignal.Cancel,
                    StartedOn = DateTime.UtcNow,
                    TryNumber = 1,
                    WorkerId = workerRecord.Id.Value
                };

                this.Repository.CreateWorking(workingRecord, null);

                signals = this.Repository.GetWorkingSignals(workerRecord.Id.Value, workingRecord.Id.Value, null);
                Assert.IsNotNull(signals);
                Assert.AreEqual(WorkingSignal.Cancel, signals.WorkingSignal);
            }
        }

        /// <summary>
        /// Release schedule enqueueing lock tests.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Enqueueing", Justification = "The spelling is correct.")]
        protected void ReleaseScheduleEnqueueingLock()
        {
            ScheduleRecord scheduleRecord = new ScheduleRecord()
            {
                ApplicationName = BlueCollarSection.Section.ApplicationName,
                Enabled = false,
                Name = "Nightly",
                QueueName = "schedules",
                RepeatType = ScheduleRepeatType.Days,
                RepeatValue = 1,
                StartOn = DateTime.UtcNow.FloorWithSeconds()
            };

            this.Repository.CreateSchedule(scheduleRecord, null);
            Assert.IsTrue(this.Repository.GetScheduleEnqueueingLock(scheduleRecord.Id.Value, null));

            Assert.IsFalse(this.Repository.GetScheduleEnqueueingLock(scheduleRecord.Id.Value, null));

            this.Repository.ReleaseScheduleEnqueueingLock(scheduleRecord.Id.Value, null);
            Assert.IsTrue(this.Repository.GetScheduleEnqueueingLock(scheduleRecord.Id.Value, null));
        }

        /// <summary>
        /// Update schedule tests.
        /// </summary>
        protected void UpdateSchedule()
        {
            if (this.Repository != null)
            {
                ScheduleRecord scheduleRecord = new ScheduleRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    Enabled = false,
                    Name = "Nightly",
                    QueueName = "schedules",
                    RepeatType = ScheduleRepeatType.Days,
                    RepeatValue = 1,
                    StartOn = DateTime.UtcNow.FloorWithSeconds()
                };

                this.Repository.CreateSchedule(scheduleRecord, null);

                scheduleRecord.Enabled = true;
                scheduleRecord.Name = "Bi-Weekly";
                scheduleRecord.QueueName = null;
                scheduleRecord.RepeatType = ScheduleRepeatType.Weeks;
                scheduleRecord.RepeatValue = 2;
                scheduleRecord.StartOn = new DateTime(2011, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                this.Repository.UpdateSchedule(scheduleRecord, null);

                ScheduleRecord updatedRecord = this.Repository.GetSchedules(BlueCollarSection.Section.ApplicationName, null).Where(s => s.Id == scheduleRecord.Id).First();

                Assert.AreEqual(scheduleRecord.Enabled, updatedRecord.Enabled);
                Assert.AreEqual(scheduleRecord.Name, updatedRecord.Name);
                Assert.AreEqual(scheduleRecord.QueueName, updatedRecord.QueueName);
                Assert.AreEqual(scheduleRecord.RepeatType, updatedRecord.RepeatType);
                Assert.AreEqual(scheduleRecord.RepeatValue, updatedRecord.RepeatValue);
                Assert.AreEqual(scheduleRecord.StartOn, updatedRecord.StartOn);
            }
        }

        /// <summary>
        /// Update scheduled job tests.
        /// </summary>
        protected void UpdateScheduledJob()
        {
            if (this.Repository != null)
            {
                ScheduleRecord scheduleRecord = new ScheduleRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    Name = "Nightly",
                    QueueName = "schedules",
                    RepeatType = ScheduleRepeatType.Days,
                    RepeatValue = 1,
                    StartOn = DateTime.UtcNow.FloorWithSeconds()
                };

                this.Repository.CreateSchedule(scheduleRecord, null);

                ScheduledJobRecord jobRecord = new ScheduledJobRecord()
                {
                    ScheduleId = scheduleRecord.Id.Value,
                    JobType = "BlueCollar.Test.TestJob, BlueCollar.Test",
                    Properties = "{}"
                };

                this.Repository.CreateScheduledJob(jobRecord, null);

                jobRecord.JobType = "BlueCollar.Test.UpdatedJob, BlueCollar.Test";
                jobRecord.Properties = "{\"One\":null, \"Two\":\"Three\"}";
                this.Repository.UpdateScheduledJob(jobRecord, null);

                ScheduledJobRecord updatedJob = this.Repository.GetScheduledJobList(BlueCollarSection.Section.ApplicationName, scheduleRecord.Id.Value, null, 100, 0, null).Records[0];
                Assert.AreEqual(jobRecord.JobType, updatedJob.JobType);
                Assert.AreEqual(jobRecord.Properties, updatedJob.Properties);
            }
        }

        /// <summary>
        /// Update worker tests.
        /// </summary>
        protected void UpdateWorker()
        {
            if (this.Repository != null)
            {
                WorkerRecord workerRecord = new WorkerRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    MachineAddress = Machine.Address,
                    MachineName = Machine.Name,
                    Name = "Test Worker",
                    QueueNames = "*",
                    Signal = WorkerSignal.Stop,
                    Status = WorkerStatus.Working,
                    Startup = WorkerStartupType.Automatic,
                    UpdatedOn = new DateTime(2011, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                };

                this.Repository.CreateWorker(workerRecord, null);

                workerRecord.MachineAddress = "0.0.0.0";
                workerRecord.MachineName = "TEST";
                workerRecord.Name = "Updated Test Worker";
                workerRecord.QueueNames = "test";
                workerRecord.Signal = WorkerSignal.Start;
                workerRecord.Status = WorkerStatus.Stopped;
                workerRecord.Startup = WorkerStartupType.Manual;
                workerRecord.UpdatedOn = DateTime.UtcNow.FloorWithSeconds();
                this.Repository.UpdateWorker(workerRecord, null);

                WorkerRecord updated = this.Repository.GetWorker(workerRecord.Id.Value, null);
                Assert.AreEqual(workerRecord.MachineAddress, updated.MachineAddress);
                Assert.AreEqual(workerRecord.MachineName, updated.MachineName);
                Assert.AreEqual(workerRecord.Name, updated.Name);
                Assert.AreEqual(workerRecord.QueueNames, updated.QueueNames);
                Assert.AreEqual(workerRecord.Signal, updated.Signal);
                Assert.AreEqual(workerRecord.Startup, updated.Startup);
                Assert.AreEqual(workerRecord.Status, updated.Status);
                Assert.AreEqual(workerRecord.UpdatedOn, updated.UpdatedOn);
            }
        }

        /// <summary>
        /// Update worker status tests.
        /// </summary>
        protected void UpdateWorkerStatus()
        {
            if (this.Repository != null)
            {
                WorkerRecord workerRecord = new WorkerRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    MachineAddress = Machine.Address,
                    MachineName = Machine.Name,
                    Name = "Test Worker",
                    QueueNames = "*",
                    Signal = WorkerSignal.Stop,
                    Status = WorkerStatus.Working,
                    Startup = WorkerStartupType.Automatic,
                    UpdatedOn = DateTime.UtcNow
                };

                this.Repository.CreateWorker(workerRecord, null);
                this.Repository.UpdateWorkerStatus(workerRecord.Id.Value, WorkerStatus.Stopped, null);

                Assert.AreEqual(WorkerStatus.Stopped, this.Repository.GetWorker(workerRecord.Id.Value, null).Status);
            }
        }

        /// <summary>
        /// Update working tests.
        /// </summary>
        protected void UpdateWorking()
        {
            if (this.Repository != null)
            {
                IJob job = new TestJob() { Id = Guid.NewGuid() };

                WorkerRecord workerRecord = new WorkerRecord()
                {
                    ApplicationName = BlueCollarSection.Section.ApplicationName,
                    MachineAddress = Machine.Address,
                    MachineName = Machine.Name,
                    Name = "Test Worker",
                    QueueNames = "*",
                    Signal = WorkerSignal.Stop,
                    Status = WorkerStatus.Working,
                    Startup = WorkerStartupType.Automatic,
                    UpdatedOn = DateTime.UtcNow
                };

                this.Repository.CreateWorker(workerRecord, null);

                WorkingRecord workingRecord = new WorkingRecord()
                {
                    ApplicationName = workerRecord.ApplicationName,
                    Data = JobSerializer.Serialize(job),
                    JobName = job.Name,
                    JobType = JobSerializer.GetTypeName(job),
                    QueueName = "*",
                    QueuedOn = DateTime.UtcNow,
                    Signal = WorkingSignal.Cancel,
                    StartedOn = DateTime.UtcNow,
                    TryNumber = 1,
                    WorkerId = workerRecord.Id.Value
                };

                this.Repository.CreateWorking(workingRecord, null);

                job = new TestJob() { Id = Guid.NewGuid() };
                workingRecord.Data = JobSerializer.Serialize(job);
                workingRecord.JobName = "Updated";
                workingRecord.JobType = "Updated";
                workingRecord.QueuedOn = new DateTime(2011, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                workingRecord.QueueName = "test";
                workingRecord.Signal = WorkingSignal.None;
                workingRecord.StartedOn = new DateTime(2011, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                workingRecord.TryNumber = 2;
                this.Repository.UpdateWorking(workingRecord, null);

                WorkingRecord updated = this.Repository.GetWorking(workingRecord.Id.Value, null);
                Assert.AreEqual(workingRecord.Data, updated.Data);
                Assert.AreEqual(workingRecord.JobName, updated.JobName);
                Assert.AreEqual(workingRecord.JobType, updated.JobType);
                Assert.AreEqual(workingRecord.QueuedOn, updated.QueuedOn);
                Assert.AreEqual(workingRecord.QueueName, updated.QueueName);
                Assert.AreEqual(workingRecord.Signal, updated.Signal);
                Assert.AreEqual(workingRecord.StartedOn, updated.StartedOn);
                Assert.AreEqual(workingRecord.TryNumber, updated.TryNumber);
            }
        }
    }
}