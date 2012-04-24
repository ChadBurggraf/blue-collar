//-----------------------------------------------------------------------
// <copyright file="WorkerTests.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Test
{
    using System;
    using System.Collections.Generic;
    using System.Data;
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
        public void WorkerDequeue()
        {
            IJob job = new TestJob() { Id = Guid.NewGuid() };
            SignalsRecord signals = new SignalsRecord() { QueueNames = "*", WorkerSignal = WorkerSignal.None, WorkingSignal = WorkingSignal.None };

            QueueRecord queued = new QueueRecord() 
            {
                Id = 12,
                ApplicationName = "/test",
                Data = JsonConvert.SerializeObject(job),
                JobName = job.Name,
                JobType = JobSerializer.GetTypeName(job.GetType()),
                QueuedOn = DateTime.UtcNow,
                QueueName = "*",
                TryNumber = 1
            };

            var transaction = new Mock<IDbTransaction>(); 

            var repository = new Mock<IRepository>();
            repository.Setup(r => r.BeginTransaction()).Returns(transaction.Object);
            repository.Setup(r => r.BeginTransaction(It.IsAny<IsolationLevel>())).Returns(transaction.Object);
            repository.Setup(r => r.GetQueued(It.IsAny<string>(), It.IsAny<QueueNameFilters>(), It.IsAny<DateTime>(), It.IsAny<IDbTransaction>())).Returns(queued);
            repository.Setup(r => r.GetWorkingSignals(It.IsAny<long>(), It.IsAny<long?>(), It.IsAny<IDbTransaction>())).Returns(signals);

            var factory = new Mock<IRepositoryFactory>();
            factory.Setup(f => f.Create()).Returns(repository.Object);

            var logger = new Mock<ILogger>();

            using (Worker worker = new Worker("/test", 1, "Test Worker", null, 1, false, factory.Object, logger.Object))
            {
                worker.Start();
                Thread.Sleep(1500);
            }

            repository.Verify(r => r.GetQueued("/test", It.IsAny<QueueNameFilters>(), It.IsAny<DateTime>(), It.IsAny<IDbTransaction>()));
            repository.Verify(r => r.DeleteQueued(12, It.IsAny<IDbTransaction>()));
            repository.Verify(r => r.CreateWorking(It.Is<WorkingRecord>(w => w.ApplicationName == "/test" && w.WorkerId == 1), It.IsAny<IDbTransaction>()));
        }

        /// <summary>
        /// Dispose tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "Code needs to operate on the object after it is disposed.")]
        public void WorkerDispose()
        {
            IJob job = new TestJob() { Id = Guid.NewGuid() };
            SignalsRecord signals = new SignalsRecord() { QueueNames = "*", WorkerSignal = WorkerSignal.None, WorkingSignal = WorkingSignal.None };

            QueueRecord queued = new QueueRecord()
            {
                Id = 12,
                ApplicationName = "/test",
                Data = JsonConvert.SerializeObject(job),
                JobName = job.Name,
                JobType = JobSerializer.GetTypeName(job.GetType()),
                QueuedOn = DateTime.UtcNow,
                QueueName = "*",
                TryNumber = 1
            };

            var transaction = new Mock<IDbTransaction>();

            var repository = new Mock<IRepository>();
            repository.Setup(r => r.BeginTransaction()).Returns(transaction.Object);
            repository.Setup(r => r.BeginTransaction(It.IsAny<IsolationLevel>())).Returns(transaction.Object);
            repository.Setup(r => r.GetQueued(It.IsAny<string>(), It.IsAny<QueueNameFilters>(), It.IsAny<DateTime>(), It.IsAny<IDbTransaction>())).Returns(queued);
            repository.Setup(r => r.GetWorkingSignals(It.IsAny<long>(), It.IsAny<long?>(), It.IsAny<IDbTransaction>())).Returns(signals);

            var factory = new Mock<IRepositoryFactory>();
            factory.Setup(f => f.Create()).Returns(repository.Object);

            var logger = new Mock<ILogger>();
            Worker worker = null;

            try
            {
                worker = new Worker("/test", 1, "Test Worker", null, 1, false, factory.Object, logger.Object);
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
        public void WorkerExecute()
        {
            IJob job = new TestJob() { Id = Guid.NewGuid() };
            SignalsRecord signals = new SignalsRecord() { QueueNames = "*", WorkerSignal = WorkerSignal.None, WorkingSignal = WorkingSignal.None };

            QueueRecord queued = new QueueRecord()
            {
                Id = 12,
                ApplicationName = "/test",
                Data = JsonConvert.SerializeObject(job),
                JobName = job.Name,
                JobType = JobSerializer.GetTypeName(job.GetType()),
                QueuedOn = DateTime.UtcNow,
                QueueName = "*",
                TryNumber = 1
            };

            WorkingRecord working = Worker.CreateWorking(queued, 1, null, DateTime.UtcNow);
            working.Id = 13;

            var transaction = new Mock<IDbTransaction>();

            var repository = new Mock<IRepository>();
            repository.Setup(r => r.BeginTransaction()).Returns(transaction.Object);
            repository.Setup(r => r.BeginTransaction(It.IsAny<IsolationLevel>())).Returns(transaction.Object);
            repository.Setup(r => r.CreateWorking(It.IsAny<WorkingRecord>(), It.IsAny<IDbTransaction>())).Returns(working);
            repository.Setup(r => r.GetQueued(It.IsAny<string>(), It.IsAny<QueueNameFilters>(), It.IsAny<DateTime>(), It.IsAny<IDbTransaction>())).Returns(queued);
            repository.Setup(r => r.GetWorkingSignals(It.IsAny<long>(), It.IsAny<long?>(), It.IsAny<IDbTransaction>())).Returns(signals);

            var factory = new Mock<IRepositoryFactory>();
            factory.Setup(f => f.Create()).Returns(repository.Object);

            var logger = new Mock<ILogger>();

            using (Worker worker = new Worker("/test", 1, "Test Worker", null, 1, false, factory.Object, logger.Object))
            {
                worker.Start();
                Thread.Sleep(1500);
            }

            repository.Verify(r => r.CreateHistory(It.Is<HistoryRecord>(h => h.Status == HistoryStatus.Succeeded), It.IsAny<IDbTransaction>()));
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
                ApplicationName = "/test",
                Data = JsonConvert.SerializeObject(job),
                JobName = job.Name,
                JobType = JobSerializer.GetTypeName(job.GetType()),
                QueuedOn = DateTime.UtcNow,
                QueueName = "*",
                TryNumber = 1
            };

            WorkingRecord working = Worker.CreateWorking(queued, 1, null, DateTime.UtcNow);
            working.Id = 13;

            var transaction = new Mock<IDbTransaction>();

            var repository = new Mock<IRepository>();
            repository.Setup(r => r.BeginTransaction()).Returns(transaction.Object);
            repository.Setup(r => r.BeginTransaction(It.IsAny<IsolationLevel>())).Returns(transaction.Object);
            repository.Setup(r => r.CreateWorking(It.IsAny<WorkingRecord>(), It.IsAny<IDbTransaction>())).Returns(working);
            repository.Setup(r => r.GetQueued(It.IsAny<string>(), It.IsAny<QueueNameFilters>(), It.IsAny<DateTime>(), It.IsAny<IDbTransaction>())).Returns(queued);
            repository.Setup(r => r.GetWorkingSignals(It.IsAny<long>(), It.IsAny<long?>(), It.IsAny<IDbTransaction>())).Returns(signals);

            var factory = new Mock<IRepositoryFactory>();
            factory.Setup(f => f.Create()).Returns(repository.Object);

            var logger = new Mock<ILogger>();

            using (Worker worker = new Worker("/test", 1, "Test Worker", null, 1, false, factory.Object, logger.Object))
            {
                worker.Start();
                Thread.Sleep(1500);
                signals.WorkingSignal = WorkingSignal.Cancel;
                Thread.Sleep(1500);
            }

            repository.Verify(r => r.CreateHistory(It.Is<HistoryRecord>(h => h.Status == HistoryStatus.Canceled), It.IsAny<IDbTransaction>()));
        }

        /// <summary>
        /// Execute exception tests.
        /// </summary>
        [TestMethod]
        public void WorkerExecuteException()
        {
            IJob job = new TestJob() { ThrowException = true };
            SignalsRecord signals = new SignalsRecord() { QueueNames = "*", WorkerSignal = WorkerSignal.None, WorkingSignal = WorkingSignal.None };

            QueueRecord queued = new QueueRecord()
            {
                Id = 12,
                ApplicationName = "/test",
                Data = JsonConvert.SerializeObject(job),
                JobName = job.Name,
                JobType = JobSerializer.GetTypeName(job.GetType()),
                QueuedOn = DateTime.UtcNow,
                QueueName = "*",
                TryNumber = 1
            };

            WorkingRecord working = Worker.CreateWorking(queued, 1, null, DateTime.UtcNow);
            working.Id = 13;

            var transaction = new Mock<IDbTransaction>();

            var repository = new Mock<IRepository>();
            repository.Setup(r => r.BeginTransaction()).Returns(transaction.Object);
            repository.Setup(r => r.BeginTransaction(It.IsAny<IsolationLevel>())).Returns(transaction.Object);
            repository.Setup(r => r.CreateWorking(It.IsAny<WorkingRecord>(), It.IsAny<IDbTransaction>())).Returns(working);
            repository.Setup(r => r.GetQueued(It.IsAny<string>(), It.IsAny<QueueNameFilters>(), It.IsAny<DateTime>(), It.IsAny<IDbTransaction>())).Returns(queued);
            repository.Setup(r => r.GetWorkingSignals(It.IsAny<long>(), It.IsAny<long?>(), It.IsAny<IDbTransaction>())).Returns(signals);

            var factory = new Mock<IRepositoryFactory>();
            factory.Setup(f => f.Create()).Returns(repository.Object);

            var logger = new Mock<ILogger>();

            using (Worker worker = new Worker("/test", 1, "Test Worker", null, 1, false, factory.Object, logger.Object))
            {
                worker.Start();
                Thread.Sleep(1500);
                Assert.AreEqual(WorkerStatus.Working, worker.Status);
            }

            repository.Verify(r => r.CreateHistory(It.Is<HistoryRecord>(h => h.Status == HistoryStatus.Failed), It.IsAny<IDbTransaction>()));
        }

        /// <summary>
        /// Execute retry tests.
        /// </summary>
        [TestMethod]
        public void WorkerExecuteRetry()
        {
            IJob job = new TestJob() { ThrowException = true, Retries = 1 };
            SignalsRecord signals = new SignalsRecord() { QueueNames = "*", WorkerSignal = WorkerSignal.None, WorkingSignal = WorkingSignal.None };

            QueueRecord queued = new QueueRecord()
            {
                Id = 12,
                ApplicationName = "/test",
                Data = JsonConvert.SerializeObject(job),
                JobName = job.Name,
                JobType = JobSerializer.GetTypeName(job.GetType()),
                QueuedOn = DateTime.UtcNow,
                QueueName = "*",
                TryNumber = 1
            };

            WorkingRecord working = Worker.CreateWorking(queued, 1, null, DateTime.UtcNow);
            working.Id = 13;

            var transaction = new Mock<IDbTransaction>();

            var repository = new Mock<IRepository>();
            repository.Setup(r => r.BeginTransaction()).Returns(transaction.Object);
            repository.Setup(r => r.BeginTransaction(It.IsAny<IsolationLevel>())).Returns(transaction.Object);
            repository.Setup(r => r.CreateWorking(It.IsAny<WorkingRecord>(), It.IsAny<IDbTransaction>())).Returns(working);
            repository.Setup(r => r.GetQueued(It.IsAny<string>(), It.IsAny<QueueNameFilters>(), It.IsAny<DateTime>(), It.IsAny<IDbTransaction>())).Returns(queued);
            repository.Setup(r => r.GetWorkingSignals(It.IsAny<long>(), It.IsAny<long?>(), It.IsAny<IDbTransaction>())).Returns(signals);

            var factory = new Mock<IRepositoryFactory>();
            factory.Setup(f => f.Create()).Returns(repository.Object);

            var logger = new Mock<ILogger>();

            using (Worker worker = new Worker("/test", 1, "Test Worker", null, 1, false, factory.Object, logger.Object))
            {
                worker.Start();
                Thread.Sleep(1500);
                Assert.AreEqual(WorkerStatus.Working, worker.Status);
            }

            repository.Verify(r => r.CreateHistory(It.Is<HistoryRecord>(h => h.Status == HistoryStatus.Failed), It.IsAny<IDbTransaction>()));
            repository.Verify(r => r.CreateQueued(It.Is<QueueRecord>(q => q.TryNumber == 2), It.IsAny<IDbTransaction>()));
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
                ApplicationName = "/test",
                Data = JsonConvert.SerializeObject(job),
                JobName = job.Name,
                JobType = JobSerializer.GetTypeName(job.GetType()),
                QueuedOn = DateTime.UtcNow,
                QueueName = "*",
                TryNumber = 1
            };

            WorkingRecord working = Worker.CreateWorking(queued, 1, null, DateTime.UtcNow);
            working.Id = 13;

            var transaction = new Mock<IDbTransaction>();

            var repository = new Mock<IRepository>();
            repository.Setup(r => r.BeginTransaction()).Returns(transaction.Object);
            repository.Setup(r => r.BeginTransaction(It.IsAny<IsolationLevel>())).Returns(transaction.Object);
            repository.Setup(r => r.CreateWorking(It.IsAny<WorkingRecord>(), It.IsAny<IDbTransaction>())).Returns(working);
            repository.Setup(r => r.GetQueued(It.IsAny<string>(), It.IsAny<QueueNameFilters>(), It.IsAny<DateTime>(), It.IsAny<IDbTransaction>())).Returns(queued);
            repository.Setup(r => r.GetWorkingSignals(It.IsAny<long>(), It.IsAny<long?>(), It.IsAny<IDbTransaction>())).Returns(signals);

            var factory = new Mock<IRepositoryFactory>();
            factory.Setup(f => f.Create()).Returns(repository.Object);

            var logger = new Mock<ILogger>();

            using (Worker worker = new Worker("/test", 1, "Test Worker", null, 1, false, factory.Object, logger.Object))
            {
                worker.Start();
                Thread.Sleep(1500);
            }

            repository.Verify(r => r.CreateHistory(It.Is<HistoryRecord>(h => h.Status == HistoryStatus.TimedOut), It.IsAny<IDbTransaction>()));
        }

        /// <summary>
        /// Refresh schedules with signal tests.
        /// </summary>
        [TestMethod]
        public void WorkerRefreshSchedulesWithSignal()
        {
            SignalsRecord signals = new SignalsRecord() { QueueNames = "*", WorkerSignal = WorkerSignal.None, WorkingSignal = WorkingSignal.None };

            var transaction = new Mock<IDbTransaction>();

            var repository = new Mock<IRepository>();
            repository.Setup(r => r.BeginTransaction()).Returns(transaction.Object);
            repository.Setup(r => r.BeginTransaction(It.IsAny<IsolationLevel>())).Returns(transaction.Object);
            repository.Setup(r => r.GetWorkingSignals(It.IsAny<long>(), It.IsAny<long?>(), It.IsAny<IDbTransaction>())).Returns(signals);

            var factory = new Mock<IRepositoryFactory>();
            factory.Setup(f => f.Create()).Returns(repository.Object);

            var logger = new Mock<ILogger>();
            var scheduler = new Mock<IScheduler>();

            using (Worker worker = new Worker("/test", 1, "Test Worker", null, 1, true, factory.Object, logger.Object, scheduler.Object))
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
            var transaction = new Mock<IDbTransaction>(); 
            var repository = new Mock<IRepository>();
            repository.Setup(r => r.BeginTransaction()).Returns(transaction.Object);
            repository.Setup(r => r.BeginTransaction(It.IsAny<IsolationLevel>())).Returns(transaction.Object);
            var factory = new Mock<IRepositoryFactory>();
            factory.Setup(f => f.Create()).Returns(repository.Object);
            var logger = new Mock<ILogger>();

            using (Worker worker = new Worker("/test", 1, "Test Worker", null, 1, false, factory.Object, logger.Object))
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
            var transaction = new Mock<IDbTransaction>();
            var repository = new Mock<IRepository>();
            repository.Setup(r => r.BeginTransaction()).Returns(transaction.Object);
            repository.Setup(r => r.BeginTransaction(It.IsAny<IsolationLevel>())).Returns(transaction.Object);
            var factory = new Mock<IRepositoryFactory>();
            factory.Setup(f => f.Create()).Returns(repository.Object);
            var logger = new Mock<ILogger>();

            using (Worker worker = new Worker("/test", 1, "Test Worker", null, 1, false, factory.Object, logger.Object))
            {
                worker.Start();
                worker.Stop(false);
                Thread.Sleep(1);
                Assert.AreEqual(WorkerStatus.Stopped, worker.Status);
            }
        }
    }
}
