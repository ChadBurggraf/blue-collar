//-----------------------------------------------------------------------
// <copyright file="Benchmarks.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Test
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// Executes benchmarks as unit tests.
    /// </summary>
    [TestClass]
    public sealed class Benchmarks
    {
        /// <summary>
        /// Gets or sets the test context.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Dequeue and execute 1,000 jobs.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This is a naive and mostly useless benchmark; I'll delete it if I have a problem maintaining it.")]
        public void BenchmarkDequeueAndExecute1000Jobs()
        {
            ManualResetEvent handle = new ManualResetEvent(false);
            Queue<QueueRecord> queue = new Queue<QueueRecord>();
            TestJob job = new TestJob() { SleepDuration = 10 };
            string typeName = JobSerializer.GetTypeName(job.GetType());
            
            for (int i = 0; i < 1000; i++)
            {
                job.Id = Guid.NewGuid();

                queue.Enqueue(
                    new QueueRecord()
                    {
                        Id = i + 1,
                        ApplicationName = BlueCollarSection.Section.ApplicationName,
                        Data = JobSerializer.Serialize(job),
                        JobName = job.Name,
                        JobType = typeName,
                        QueuedOn = DateTime.UtcNow,
                        QueueName = "*",
                        TryNumber = 1
                    });
            }

            SignalsRecord signals = new SignalsRecord() { QueueNames = "*", WorkerSignal = WorkerSignal.None, WorkingSignal = WorkingSignal.None };

            var transaction = new Mock<IDbTransaction>();

            var repository = new Mock<IRepository>();
            repository.Setup(r => r.AcquireQueuedLock(It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<IDbTransaction>())).Returns(true);
            repository.Setup(r => r.AcquireWorkerLock(1, It.IsAny<DateTime>(), It.IsAny<IDbTransaction>())).Returns(true);
            repository.Setup(r => r.AcquireWorkingLock(It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<IDbTransaction>())).Returns(true);
            repository.Setup(r => r.CreateWorking(It.IsAny<WorkingRecord>(), It.IsAny<IDbTransaction>())).Returns((WorkingRecord r, IDbTransaction t) => { r.Id = 1; return r; });
            repository.Setup(r => r.GetWorkingSignals(It.IsAny<long>(), It.IsAny<long?>(), It.IsAny<IDbTransaction>())).Returns(signals);
            repository.Setup(r => r.GetQueued(It.IsAny<string>(), It.IsAny<QueueNameFilters>(), It.IsAny<DateTime>(), It.IsAny<IDbTransaction>()))
                .Returns(
                    () =>
                    {
                        var r = queue.Dequeue();

                        if (queue.Count == 0)
                        {
                            handle.Set();
                        }

                        return r;
                    });

            var factory = new Mock<IRepositoryFactory>();
            factory.Setup(f => f.Create()).Returns(repository.Object);

            var logger = new Mock<ILogger>();

            Stopwatch stopwatch = new Stopwatch();

            using (Worker worker = new Worker(BlueCollarSection.Section.ApplicationName, 1, "Test Worker", QueueNameFilters.Any(), 1, false, factory.Object, logger.Object))
            {
                stopwatch.Start();

                worker.Start();
                handle.WaitOne();
                worker.Stop(false);

                stopwatch.Stop();
            }

            this.TestContext.WriteLine("1,000 jobs with 10ms execution times were dequeued and executed in {0:N3}s.", stopwatch.Elapsed.TotalSeconds);
        }
    }
}
