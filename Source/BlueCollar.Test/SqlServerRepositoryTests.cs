//-----------------------------------------------------------------------
// <copyright file="SqlServerRepositoryTests.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Test
{
    using System;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// SQLServerRepository tests.
    /// </summary>
    [TestClass]
    public sealed class SqlServerRepositoryTests : RepositoryTests
    {
        private string connectionString;
        private SqlServerRepository repository;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the SqlServerRepositoryTests class.
        /// </summary>
        public SqlServerRepositoryTests()
        {
            var css = ConfigurationManager.ConnectionStrings["SqlServerRepository"];

            if (css != null)
            {
                this.connectionString = css.ConnectionString;
            }
        }

        /// <summary>
        /// Gets the repository to use for the current test.
        /// </summary>
        protected override IRepository Repository
        {
            get
            {
                if (this.repository == null && !string.IsNullOrEmpty(this.connectionString))
                {
                    this.repository = new SqlServerRepository(this.connectionString);
                }

                return this.repository;
            }
        }

        /// <summary>
        /// Acquire queued lock tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryAcquireQueuedLock()
        {
            AcquireQueuedLock();
        }

        /// <summary>
        /// Acquire queued lock forced tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryAcquireQueuedLockForced()
        {
            AcquireQueuedLockForced();
        }

        /// <summary>
        /// Acquire schedule lock tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryAcquireScheduleLock()
        {
            AcquireScheduleLock();
        }

        /// <summary>
        /// Acquire schedule lock forced tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryAcquireScheduleLockForced()
        {
            AcquireScheduleLockForced();
        }

        /// <summary>
        /// Acquire worker lock tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryAcquireWorkerLock()
        {
            AcquireWorkerLock();
        }

        /// <summary>
        /// Acquire worker lock forced tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryAcquireWorkerLockForced()
        {
            AcquireWorkerLockForced();
        }

        /// <summary>
        /// Acquire working lock tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryAcquireWorkingLock()
        {
            AcquireWorkingLock();
        }

        /// <summary>
        /// Acquire working lock forced tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryAcquireWorkingLockForced()
        {
            AcquireWorkingLockForced();
        }

        /// <summary>
        /// Begin transaction tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryBeginTransaction()
        {
            BeginTransaction();
        }

        /// <summary>
        /// Clear working signal pair tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryClearWorkingSignalPair()
        {
            ClearWorkingSignalPair();
        }

        /// <summary>
        /// Create history tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryCreateHistory()
        {
            CreateHistory();
        }

        /// <summary>
        /// Create queued tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryCreateQueued()
        {
            CreateQueued();
        }

        /// <summary>
        /// Create queued and history for schedule tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryCreateQueuedAndHistoryForSchedule()
        {
            CreateQueuedAndHistoryForSchedule();
        }

        /// <summary>
        /// Create schedule tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryCreateSchedule()
        {
            CreateSchedule();
        }

        /// <summary>
        /// Create scheduled job tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryCreateScheduledJob()
        {
            CreateScheduledJob();
        }

        /// <summary>
        /// Create worker tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryCreateWorker()
        {
            CreateWorker();
        }

        /// <summary>
        /// Create working tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryCreateWorking()
        {
            CreateWorking();
        }

        /// <summary>
        /// Date tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryDates()
        {
            Dates();
        }

        /// <summary>
        /// Delete all tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryDeleteAll()
        {
            DeleteAll();
        }

        /// <summary>
        /// Delete history tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryDeleteHistory()
        {
            DeleteHistory();
        }

        /// <summary>
        /// Delete queued tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryDeleteQueued()
        {
            DeleteQueued();
        }

        /// <summary>
        /// Delete schedule tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryDeleteSchedule()
        {
            DeleteSchedule();
        }

        /// <summary>
        /// Delete scheduled job tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryDeleteScheduledJob()
        {
            DeleteScheduledJob();
        }

        /// <summary>
        /// Delete worker tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryDeleteWorker()
        {
            DeleteWorker();
        }

        /// <summary>
        /// Delete working tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryDeleteWorking()
        {
            DeleteWorking();
        }

        /// <summary>
        /// Get counts tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryGetCounts()
        {
            GetCounts();
        }

        /// <summary>
        /// Get history details tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryGetHistoryDetails()
        {
            GetHistoryDetails();
        }

        /// <summary>
        /// Get history list tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryGetHistoryList()
        {
            GetHistoryList();
        }

        /// <summary>
        /// Get queued tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryGetQueued()
        {
            GetQueued();
        }

        /// <summary>
        /// Get queued details tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryGetQueuedDetails()
        {
            GetQueuedDetails();
        }

        /// <summary>
        /// Get queued list tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryGetQueuedList()
        {
            GetQueuedList();
        }

        /// <summary>
        /// Get schedule tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryGetSchedule()
        {
            GetSchedule();
        }

        /// <summary>
        /// Get schedule date exists for schedule.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryGetScheduleDateExistsForSchedule()
        {
            GetScheduleDateExistsForSchedule();
        }

        /// <summary>
        /// Get scheduled job list tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryGetScheduledJobList()
        {
            GetScheduledJobList();
        }

        /// <summary>
        /// Get schedule list tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryGetScheduleList()
        {
            GetScheduleList();
        }

        /// <summary>
        /// Get schedules tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryGetSchedules()
        {
            GetSchedules();
        }

        /// <summary>
        /// Get statistics tests tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryGetStatistics()
        {
            GetStatistics();
        }

        /// <summary>
        /// Get worker tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryGetWorker()
        {
            GetWorker();
        }

        /// <summary>
        /// Get worker list tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryGetWorkerList()
        {
            GetWorkerList();
        }

        /// <summary>
        /// Get workers tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryGetWorkers()
        {
            GetWorkers();
        }

        /// <summary>
        /// Get working tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryGetWorking()
        {
            GetWorking();
        }

        /// <summary>
        /// Get working details tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryGetWorkingDetails()
        {
            GetWorkingDetails();
        }

        /// <summary>
        /// Get working for worker tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryGetWorkingForWorker()
        {
            GetWorkingForWorker();
        }

        /// <summary>
        /// Get working list tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryGetWorkingList()
        {
            GetWorkingList();
        }

        /// <summary>
        /// Get working signals tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryGetWorkingSignals()
        {
            GetWorkingSignals();
        }

        /// <summary>
        /// Release schedule enqueueing lock tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Enqueueing", Justification = "The spelling is correct.")]
        public void SqlServerRepositoryReleaseScheduleEnqueueingLock()
        {
            ReleaseScheduleEnqueueingLock();
        }

        /// <summary>
        /// Update schedule tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryUpdateSchedule()
        {
            UpdateSchedule();
        }

        /// <summary>
        /// Update scheduled job tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryUpdateScheduledJob()
        {
            UpdateScheduledJob();
        }

        /// <summary>
        /// Update scheduled job order tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryUpdateScheduledJobOrder()
        {
            UpdateScheduledJobOrder();
        }

        /// <summary>
        /// Update worker tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryUpdateWorker()
        {
            UpdateWorker();
        }

        /// <summary>
        /// Update worker status tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryUpdateWorkerStatus()
        {
            UpdateWorkerStatus();
        }

        /// <summary>
        /// Update working tests.
        /// </summary>
        [TestMethod]
        public void SqlServerRepositoryUpdateWorking()
        {
            UpdateWorking();
        }

        /// <summary>
        /// Disposes of resources used by this instance.
        /// </summary>
        /// <param name="disposing">A value indicating whether to dispose of managed resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (this.repository != null)
                {
                    this.repository.Dispose();
                    this.repository = null;
                }

                this.disposed = true;
            }
        }
    }
}
