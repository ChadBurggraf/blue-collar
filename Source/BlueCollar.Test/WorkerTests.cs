//-----------------------------------------------------------------------
// <copyright file="WorkerTests.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using BlueCollar;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Newtonsoft.Json;

    /// <summary>
    /// Worker tests.
    /// </summary>
    [TestClass]
    public sealed class WorkerTests
    {
        /// <summary>
        /// Worker dequeue tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "We're going to ignore this in tests for now.")]
        public void WorkerDequeue()
        {
            IJob job = new TestJob() { Id = Guid.NewGuid() };
            SignalsRecord signals = new SignalsRecord() { QueueNames = "*", WorkerSignal = WorkerSignal.None, WorkingSignal = WorkingSignal.None };

            QueueRecord queued = new QueueRecord() 
            {
                Id = 12,
                ApplicationName = BlueCollarSection.Section.ApplicationName,
                Data = JsonConvert.SerializeObject(job),
                JobName = job.Name,
                JobType = JobSerializer.GetTypeName(job.GetType()),
                QueuedOn = DateTime.UtcNow,
                QueueName = "*",
                TryNumber = 1
            };

            var repository = new Mock<IRepository>();
            repository.Setup(r => r.AcquireQueuedLock(queued.Id.Value, It.IsAny<DateTime>())).Returns(true);
            repository.Setup(r => r.AcquireWorkerLock(1, It.IsAny<DateTime>())).Returns(true);
            repository.Setup(r => r.GetQueued(It.IsAny<string>(), It.IsAny<QueueNameFilters>(), It.IsAny<DateTime>())).Returns(queued);
            repository.Setup(r => r.GetWorkingSignals(It.IsAny<long>(), It.IsAny<long?>())).Returns(signals);

            var factory = new Mock<IRepositoryFactory>();
            factory.Setup(f => f.Create()).Returns(repository.Object);

            var logger = new Mock<ILogger>();

            using (Worker worker = new Worker(BlueCollarSection.Section.ApplicationName, 1, "Test Worker", null, 1, false, factory.Object, logger.Object))
            {
                worker.Start();
                Thread.Sleep(1500);
            }

            repository.Verify(r => r.GetQueued(BlueCollarSection.Section.ApplicationName, It.IsAny<QueueNameFilters>(), It.IsAny<DateTime>()));
            repository.Verify(r => r.DeleteQueued(12));
            repository.Verify(r => r.CreateWorking(It.Is<WorkingRecord>(w => w.ApplicationName == BlueCollarSection.Section.ApplicationName && w.WorkerId == 1)));
        }

        /// <summary>
        /// Dispose tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "We're going to ignore this in tests for now.")]
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "Code needs to operate on the object after it is disposed.")]
        public void WorkerDispose()
        {
            IJob job = new TestJob() { Id = Guid.NewGuid() };
            SignalsRecord signals = new SignalsRecord() { QueueNames = "*", WorkerSignal = WorkerSignal.None, WorkingSignal = WorkingSignal.None };

            QueueRecord queued = new QueueRecord()
            {
                Id = 12,
                ApplicationName = BlueCollarSection.Section.ApplicationName,
                Data = JsonConvert.SerializeObject(job),
                JobName = job.Name,
                JobType = JobSerializer.GetTypeName(job.GetType()),
                QueuedOn = DateTime.UtcNow,
                QueueName = "*",
                TryNumber = 1
            };

            var repository = new Mock<IRepository>();
            repository.Setup(r => r.AcquireWorkerLock(1, It.IsAny<DateTime>())).Returns(true);
            repository.Setup(r => r.GetQueued(It.IsAny<string>(), It.IsAny<QueueNameFilters>(), It.IsAny<DateTime>())).Returns(queued);
            repository.Setup(r => r.GetWorkingSignals(It.IsAny<long>(), It.IsAny<long?>())).Returns(signals);

            var factory = new Mock<IRepositoryFactory>();
            factory.Setup(f => f.Create()).Returns(repository.Object);

            var logger = new Mock<ILogger>();
            Worker worker = null;

            try
            {
                worker = new Worker(BlueCollarSection.Section.ApplicationName, 1, "Test Worker", null, 1, false, factory.Object, logger.Object);
                worker.Start();
                Thread.Sleep(1500);
                worker.Stop(false);
                worker.Dispose();

                Assert.IsFalse(worker.LoopThreadsAreAlive);
                worker = null;
            }
            finally
            {
                if (worker != null)
                {
                    worker.Dispose();
                }
            }
        }

        /// <summary>
        /// Execute tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "We're going to ignore this in tests for now.")]
        public void WorkerExecute()
        {
            IJob job = new TestJob() { Id = Guid.NewGuid() };
            SignalsRecord signals = new SignalsRecord() { QueueNames = "*", WorkerSignal = WorkerSignal.None, WorkingSignal = WorkingSignal.None };

            QueueRecord queued = new QueueRecord()
            {
                Id = 12,
                ApplicationName = BlueCollarSection.Section.ApplicationName,
                Data = JsonConvert.SerializeObject(job),
                JobName = job.Name,
                JobType = JobSerializer.GetTypeName(job.GetType()),
                QueuedOn = DateTime.UtcNow,
                QueueName = "*",
                TryNumber = 1
            };

            WorkingRecord working = Worker.CreateWorking(queued, 1, null, DateTime.UtcNow);
            working.Id = 13;

            var repository = new Mock<IRepository>();
            repository.Setup(r => r.AcquireQueuedLock(queued.Id.Value, It.IsAny<DateTime>())).Returns(true);
            repository.Setup(r => r.AcquireWorkerLock(1, It.IsAny<DateTime>())).Returns(true);
            repository.Setup(r => r.AcquireWorkingLock(working.Id.Value, It.IsAny<DateTime>())).Returns(true);
            repository.Setup(r => r.CreateWorking(It.IsAny<WorkingRecord>())).Returns(working);
            repository.Setup(r => r.GetQueued(It.IsAny<string>(), It.IsAny<QueueNameFilters>(), It.IsAny<DateTime>())).Returns(queued);
            repository.Setup(r => r.GetWorkingSignals(It.IsAny<long>(), It.IsAny<long?>())).Returns(signals);

            var factory = new Mock<IRepositoryFactory>();
            factory.Setup(f => f.Create()).Returns(repository.Object);

            var logger = new Mock<ILogger>();

            using (Worker worker = new Worker(BlueCollarSection.Section.ApplicationName, 1, "Test Worker", null, 1, false, factory.Object, logger.Object))
            {
                worker.Start();
                Thread.Sleep(1500);
            }

            repository.Verify(r => r.CreateHistory(It.Is<HistoryRecord>(h => h.Status == HistoryStatus.Succeeded)));
        }

        /// <summary>
        /// Execute cancel tests.
        /// </summary>
        [TestMethod]
        public void WorkerExecuteCancel()
        {
            IJob job = new TestJob() { SleepDuration = 10000 };
            SignalsRecord signals = new SignalsRecord() { QueueNames = "*", WorkerSignal = WorkerSignal.None, WorkingSignal = WorkingSignal.None };

            QueueRecord queued = new QueueRecord()
            {
                Id = 12,
                ApplicationName = BlueCollarSection.Section.ApplicationName,
                Data = JsonConvert.SerializeObject(job),
                JobName = job.Name,
                JobType = JobSerializer.GetTypeName(job.GetType()),
                QueuedOn = DateTime.UtcNow,
                QueueName = "*",
                TryNumber = 1
            };

            WorkingRecord working = Worker.CreateWorking(queued, 1, null, DateTime.UtcNow);
            working.Id = 13;

            var repository = new Mock<IRepository>();
            repository.Setup(r => r.AcquireQueuedLock(queued.Id.Value, It.IsAny<DateTime>())).Returns(true);
            repository.Setup(r => r.AcquireWorkerLock(1, It.IsAny<DateTime>())).Returns(true);
            repository.Setup(r => r.AcquireWorkingLock(working.Id.Value, It.IsAny<DateTime>())).Returns(true);
            repository.Setup(r => r.CreateHistory(It.IsAny<HistoryRecord>())).Returns((HistoryRecord r) => r);
            repository.Setup(r => r.CreateWorking(It.IsAny<WorkingRecord>())).Returns(working);
            repository.Setup(r => r.GetQueued(It.IsAny<string>(), It.IsAny<QueueNameFilters>(), It.IsAny<DateTime>())).Returns(queued);
            repository.Setup(r => r.GetWorkingSignals(It.IsAny<long>(), It.IsAny<long?>())).Returns(signals);
            
            var factory = new Mock<IRepositoryFactory>();
            factory.Setup(f => f.Create()).Returns(repository.Object);

            var logger = new Mock<ILogger>();

            using (Worker worker = new Worker(BlueCollarSection.Section.ApplicationName, 1, "Test Worker", null, 1, false, factory.Object, logger.Object))
            {
                worker.Start();
                Thread.Sleep(1500);
                signals.WorkingSignal = WorkingSignal.Cancel;
                Thread.Sleep(1500);
            }

            repository.Verify(r => r.CreateHistory(It.Is<HistoryRecord>(h => h.Status == HistoryStatus.Canceled)));
        }

        /// <summary>
        /// Execute exception tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "We're going to ignore this in tests for now.")]
        public void WorkerExecuteException()
        {
            IJob job = new TestJob() { ThrowException = true };
            SignalsRecord signals = new SignalsRecord() { QueueNames = "*", WorkerSignal = WorkerSignal.None, WorkingSignal = WorkingSignal.None };

            QueueRecord queued = new QueueRecord()
            {
                Id = 12,
                ApplicationName = BlueCollarSection.Section.ApplicationName,
                Data = JsonConvert.SerializeObject(job),
                JobName = job.Name,
                JobType = JobSerializer.GetTypeName(job.GetType()),
                QueuedOn = DateTime.UtcNow,
                QueueName = "*",
                TryNumber = 1
            };

            WorkingRecord working = Worker.CreateWorking(queued, 1, null, DateTime.UtcNow);
            working.Id = 13;

            var repository = new Mock<IRepository>();
            repository.Setup(r => r.AcquireQueuedLock(queued.Id.Value, It.IsAny<DateTime>())).Returns(true);
            repository.Setup(r => r.AcquireWorkerLock(1, It.IsAny<DateTime>())).Returns(true);
            repository.Setup(r => r.AcquireWorkingLock(working.Id.Value, It.IsAny<DateTime>())).Returns(true);
            repository.Setup(r => r.CreateWorking(It.IsAny<WorkingRecord>())).Returns(working);
            repository.Setup(r => r.GetQueued(It.IsAny<string>(), It.IsAny<QueueNameFilters>(), It.IsAny<DateTime>())).Returns(queued);
            repository.Setup(r => r.GetWorkingSignals(It.IsAny<long>(), It.IsAny<long?>())).Returns(signals);

            var factory = new Mock<IRepositoryFactory>();
            factory.Setup(f => f.Create()).Returns(repository.Object);

            var logger = new Mock<ILogger>();

            using (Worker worker = new Worker(BlueCollarSection.Section.ApplicationName, 1, "Test Worker", null, 1, false, factory.Object, logger.Object))
            {
                worker.Start();
                Thread.Sleep(1500);
                Assert.AreEqual(WorkerStatus.Working, worker.Status);
            }

            repository.Verify(r => r.CreateHistory(It.Is<HistoryRecord>(h => h.Status == HistoryStatus.Failed)));
        }

        /// <summary>
        /// Execute retry tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "We're going to ignore this in tests for now.")]
        public void WorkerExecuteRetry()
        {
            IJob job = new TestJob() { ThrowException = true, Retries = 1 };
            SignalsRecord signals = new SignalsRecord() { QueueNames = "*", WorkerSignal = WorkerSignal.None, WorkingSignal = WorkingSignal.None };

            QueueRecord queued = new QueueRecord()
            {
                Id = 12,
                ApplicationName = BlueCollarSection.Section.ApplicationName,
                Data = JsonConvert.SerializeObject(job),
                JobName = job.Name,
                JobType = JobSerializer.GetTypeName(job.GetType()),
                QueuedOn = DateTime.UtcNow,
                QueueName = "*",
                TryNumber = 1
            };

            WorkingRecord working = Worker.CreateWorking(queued, 1, null, DateTime.UtcNow);
            working.Id = 13;

            var repository = new Mock<IRepository>();
            repository.Setup(r => r.AcquireQueuedLock(queued.Id.Value, It.IsAny<DateTime>())).Returns(true);
            repository.Setup(r => r.AcquireWorkerLock(1, It.IsAny<DateTime>())).Returns(true);
            repository.Setup(r => r.AcquireWorkingLock(working.Id.Value, It.IsAny<DateTime>())).Returns(true);
            repository.Setup(r => r.CreateWorking(It.IsAny<WorkingRecord>())).Returns(working);
            repository.Setup(r => r.GetQueued(It.IsAny<string>(), It.IsAny<QueueNameFilters>(), It.IsAny<DateTime>())).Returns(queued);
            repository.Setup(r => r.GetWorkingSignals(It.IsAny<long>(), It.IsAny<long?>())).Returns(signals);

            var factory = new Mock<IRepositoryFactory>();
            factory.Setup(f => f.Create()).Returns(repository.Object);

            var logger = new Mock<ILogger>();

            using (Worker worker = new Worker(BlueCollarSection.Section.ApplicationName, 1, "Test Worker", null, 1, false, factory.Object, logger.Object))
            {
                worker.Start();
                Thread.Sleep(1500);
                Assert.AreEqual(WorkerStatus.Working, worker.Status);
            }

            repository.Verify(r => r.CreateHistory(It.Is<HistoryRecord>(h => h.Status == HistoryStatus.Failed)));
            repository.Verify(r => r.CreateQueued(It.Is<QueueRecord>(q => q.TryNumber == 2)));
        }

        /// <summary>
        /// Execute timeout tests.
        /// </summary>
        [TestMethod]
        public void WorkerExecuteTimeout()
        {
            IJob job = new TestJob() { SleepDuration = 100, Timeout = 10 };
            SignalsRecord signals = new SignalsRecord() { QueueNames = "*", WorkerSignal = WorkerSignal.None, WorkingSignal = WorkingSignal.None };

            QueueRecord queued = new QueueRecord()
            {
                Id = 12,
                ApplicationName = BlueCollarSection.Section.ApplicationName,
                Data = JsonConvert.SerializeObject(job),
                JobName = job.Name,
                JobType = JobSerializer.GetTypeName(job.GetType()),
                QueuedOn = DateTime.UtcNow,
                QueueName = "*",
                TryNumber = 1
            };

            WorkingRecord working = Worker.CreateWorking(queued, 1, null, DateTime.UtcNow);
            working.Id = 13;

            var repository = new Mock<IRepository>();
            repository.Setup(r => r.AcquireQueuedLock(queued.Id.Value, It.IsAny<DateTime>())).Returns(true);
            repository.Setup(r => r.AcquireWorkerLock(1, It.IsAny<DateTime>())).Returns(true);
            repository.Setup(r => r.AcquireWorkingLock(working.Id.Value, It.IsAny<DateTime>())).Returns(true);
            repository.Setup(r => r.CreateWorking(It.IsAny<WorkingRecord>())).Returns(working);
            repository.Setup(r => r.GetQueued(It.IsAny<string>(), It.IsAny<QueueNameFilters>(), It.IsAny<DateTime>())).Returns(queued);
            repository.Setup(r => r.GetWorkingSignals(It.IsAny<long>(), It.IsAny<long?>())).Returns(signals);

            var factory = new Mock<IRepositoryFactory>();
            factory.Setup(f => f.Create()).Returns(repository.Object);

            var logger = new Mock<ILogger>();

            using (Worker worker = new Worker(BlueCollarSection.Section.ApplicationName, 1, "Test Worker", null, 1, false, factory.Object, logger.Object))
            {
                worker.Start();
                Thread.Sleep(1500);
            }

            repository.Verify(r => r.CreateHistory(It.Is<HistoryRecord>(h => h.Status == HistoryStatus.TimedOut)));
        }

        /// <summary>
        /// Refresh schedules with signal tests.
        /// </summary>
        [TestMethod]
        public void WorkerRefreshSchedulesWithSignal()
        {
            SignalsRecord signals = new SignalsRecord() { QueueNames = "*", WorkerSignal = WorkerSignal.None, WorkingSignal = WorkingSignal.None };

            var repository = new Mock<IRepository>();
            repository.Setup(r => r.AcquireWorkerLock(1, It.IsAny<DateTime>())).Returns(true);
            repository.Setup(r => r.GetWorkingSignals(It.IsAny<long>(), It.IsAny<long?>())).Returns(signals);

            var factory = new Mock<IRepositoryFactory>();
            factory.Setup(f => f.Create()).Returns(repository.Object);

            var logger = new Mock<ILogger>();
            var scheduler = new Mock<IScheduler>();

            using (Worker worker = new Worker(BlueCollarSection.Section.ApplicationName, 1, "Test Worker", null, 1, true, factory.Object, logger.Object, scheduler.Object))
            {
                worker.Start();
                signals.WorkerSignal = WorkerSignal.RefreshSchedules;
                Thread.Sleep(1500);
            }

            scheduler.Verify(s => s.RefreshSchedules(), Times.AtLeastOnce());
        }

        /// <summary>
        /// Start tests.
        /// </summary>
        [TestMethod]
        public void WorkerStart()
        {
            var repository = new Mock<IRepository>();
            repository.Setup(r => r.AcquireWorkerLock(1, It.IsAny<DateTime>())).Returns(true);
            var factory = new Mock<IRepositoryFactory>();
            factory.Setup(f => f.Create()).Returns(repository.Object);
            var logger = new Mock<ILogger>();

            using (Worker worker = new Worker(BlueCollarSection.Section.ApplicationName, 1, "Test Worker", null, 1, false, factory.Object, logger.Object))
            {
                Assert.AreEqual(WorkerStatus.Stopped, worker.Status);

                worker.Start();
                Assert.AreEqual(WorkerStatus.Working, worker.Status);
            }
        }

        /// <summary>
        /// Stop tests.
        /// </summary>
        [TestMethod]
        public void WorkerStop()
        {
            var repository = new Mock<IRepository>();
            repository.Setup(r => r.AcquireWorkerLock(1, It.IsAny<DateTime>())).Returns(true);
            var factory = new Mock<IRepositoryFactory>();
            factory.Setup(f => f.Create()).Returns(repository.Object);
            var logger = new Mock<ILogger>();

            using (Worker worker = new Worker(BlueCollarSection.Section.ApplicationName, 1, "Test Worker", null, 1, false, factory.Object, logger.Object))
            {
                worker.Start();
                worker.Stop(false);
                Thread.Sleep(1);
                Assert.AreEqual(WorkerStatus.Stopped, worker.Status);
            }
        }
    }
}
