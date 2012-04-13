//-----------------------------------------------------------------------
// <copyright file="SchedulerTests.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Test
{
    using System;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// Scheduler tests.
    /// </summary>
    [TestClass]
    public sealed class SchedulerTests
    {
        /// <summary>
        /// Can schedule be enqueued tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling is correct.")]
        public void SchedulerCanScheduleBeEnqueued()
        {
            DateTime? scheduleDate;
            DateTime now = DateTime.UtcNow.FloorWithSeconds();

            ScheduleRecord schedule = new ScheduleRecord()
            {
                RepeatType = ScheduleRepeatType.Days,
                RepeatValue = 1,
                StartOn = now
            };

            DateTime begin = now.AddSeconds(-1);
            DateTime end = now;
            Assert.IsTrue(Scheduler.CanScheduleBeEnqueued(schedule, begin, end, null, out scheduleDate));
            Assert.IsNotNull(scheduleDate);

            begin = end;
            end = begin.AddSeconds(1);
            Assert.IsFalse(Scheduler.CanScheduleBeEnqueued(schedule, begin, end, null, out scheduleDate));
            Assert.IsNull(scheduleDate);

            begin = now.AddSeconds(-2);
            end = now.AddSeconds(-1);
            Assert.IsFalse(Scheduler.CanScheduleBeEnqueued(schedule, begin, end, null, out scheduleDate));
            Assert.IsNull(scheduleDate);

            begin = now.AddDays(1).AddSeconds(-1);
            end = now.AddDays(1);
            Assert.IsTrue(Scheduler.CanScheduleBeEnqueued(schedule, begin, end, null, out scheduleDate));
            Assert.IsNotNull(scheduleDate);

            schedule.RepeatType = ScheduleRepeatType.None;
            Assert.IsFalse(Scheduler.CanScheduleBeEnqueued(schedule, begin, end, null, out scheduleDate));
            Assert.IsNull(scheduleDate);
        }

        /// <summary>
        /// Refresh schedules tests.
        /// </summary>
        [TestMethod]
        public void SchedulerRefreshSchedules()
        {
            ScheduleRecord schedule = new ScheduleRecord()
            {
                ApplicationName = "/test",
                Enabled = true,
                Id = 1,
                Name = "Test",
                QueueName = "*",
                RepeatType = ScheduleRepeatType.Days,
                RepeatValue = 1,
                StartOn = DateTime.UtcNow.FloorWithSeconds()
            };

            ScheduledJobRecord scheduledJob = new ScheduledJobRecord()
            {
                Id = 1,
                JobType = JobSerializer.GetTypeName(typeof(TestScheduledJob)),
                Properties = @"{""A"":""Hello, world!""}",
                Schedule = schedule,
                ScheduleId = 1
            };

            schedule.ScheduledJobs.Add(scheduledJob);

            var transaction = new Mock<IDbTransaction>();

            var repository = new Mock<IRepository>();
            repository.Setup(r => r.BeginTransaction()).Returns(transaction.Object);
            repository.Setup(r => r.GetSchedules("/test", It.IsAny<IDbTransaction>())).Returns(new ScheduleRecord[] { schedule });

            var factory = new Mock<IRepositoryFactory>();
            factory.Setup(f => f.Create()).Returns(repository.Object);

            var logger = new Mock<ILogger>();

            Scheduler scheduler = new Scheduler(1, "/test", 1, factory.Object, logger.Object);
            Assert.AreEqual(0, scheduler.Schedules.Count());

            scheduler.RefreshSchedules();
            Assert.AreEqual(1, scheduler.Schedules.Count());
        }
    }
}
