//-----------------------------------------------------------------------
// <copyright file="SQLiteRepositoryTests.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Test
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using BlueCollar;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;

    /// <summary>
    /// SQLiteRepository tests.
    /// </summary>
    [TestClass]
    [DeploymentItem(@"x86\SQLite.Interop.dll", "x86")]
    [DeploymentItem(@"x64\SQLite.Interop.dll", "x64")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
    public sealed class SQLiteRepositoryTests : RepositoryTests
    {
        private string connectionString;
        private SQLiteRepository repository;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the SQLiteRepositoryTests class.
        /// </summary>
        public SQLiteRepositoryTests()
        {
            string path = Path.GetFullPath(Guid.NewGuid().ToString() + ".sqlite");
            this.connectionString = string.Concat("data source=", path, ";journal mode=Off;synchronous=Off;version=3");
        }

        /// <summary>
        /// Gets or sets the current test context.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Gets the repository to use for the current test.
        /// </summary>
        protected override IRepository Repository
        {
            get { return this.repository ?? (this.repository = new SQLiteRepository(this.connectionString)); }
        }

        /// <summary>
        /// Acquire queued lock tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryAcquireQueuedLock()
        {
            AcquireQueuedLock();
        }

        /// <summary>
        /// Acquire queued lock forced tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryAcquireQueuedLockForced()
        {
            AcquireQueuedLockForced();
        }

        /// <summary>
        /// Acquire schedule lock tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryAcquireScheduleLock()
        {
            AcquireScheduleLock();
        }

        /// <summary>
        /// Acquire schedule lock forced tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryAcquireScheduleLockForced()
        {
            AcquireScheduleLockForced();
        }

        /// <summary>
        /// Acquire worker lock tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryAcquireWorkerLock()
        {
            AcquireWorkerLock();
        }

        /// <summary>
        /// Acquire worker lock forced tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryAcquireWorkerLockForced()
        {
            AcquireWorkerLockForced();
        }

        /// <summary>
        /// Acquire working lock tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryAcquireWorkingLock()
        {
            AcquireWorkingLock();
        }

        /// <summary>
        /// Acquire working lock forced tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryAcquireWorkingLockForced()
        {
            AcquireWorkingLockForced();
        }

        /// <summary>
        /// Begin transaction tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryBeginTransaction()
        {
            BeginTransaction();
        }

        /// <summary>
        /// Clear working signal pair tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryClearWorkingSignalPair()
        {
            ClearWorkingSignalPair();
        }

        /// <summary>
        /// Create history tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryCreateHistory()
        {
            CreateHistory();
        }

        /// <summary>
        /// Create queued tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryCreateQueued()
        {
            CreateQueued();
        }

        /// <summary>
        /// Create queued and history for schedule tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryCreateQueuedAndHistoryForSchedule()
        {
            CreateQueuedAndHistoryForSchedule();
        }

        /// <summary>
        /// Create schedule tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryCreateSchedule()
        {
            CreateSchedule();
        }

        /// <summary>
        /// Create scheduled job tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryCreateScheduledJob()
        {
            CreateScheduledJob();
        }

        /// <summary>
        /// Create worker tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryCreateWorker()
        {
            CreateWorker();
        }

        /// <summary>
        /// Create working tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryCreateWorking()
        {
            CreateWorking();
        }

        /// <summary>
        /// Date tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryDates()
        {
            Dates();
        }

        /// <summary>
        /// Delete all tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryDeleteAll()
        {
            DeleteAll();
        }

        /// <summary>
        /// Delete history tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryDeleteHistory()
        {
            DeleteHistory();
        }

        /// <summary>
        /// Delete queued tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryDeleteQueued()
        {
            DeleteQueued();
        }

        /// <summary>
        /// Delete schedule tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryDeleteSchedule()
        {
            DeleteSchedule();
        }

        /// <summary>
        /// Delete scheduled job tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryDeleteScheduledJob()
        {
            DeleteScheduledJob();
        }

        /// <summary>
        /// Delete worker tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryDeleteWorker()
        {
            DeleteWorker();
        }

        /// <summary>
        /// Delete working tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryDeleteWorking()
        {
            DeleteWorking();
        }

        /// <summary>
        /// Ensure database tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryEnsureDatabase()
        {
            string path = Path.GetFullPath(Guid.NewGuid().ToString() + ".sqlite");

            using (SQLiteRepository repo = new SQLiteRepository("data source=" + path))
            {
            }

            Assert.IsTrue(File.Exists(path));
        }

        /// <summary>
        /// Get counts tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryGetCounts()
        {
            GetCounts();
        }

        /// <summary>
        /// Get history details tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryGetHistoryDetails()
        {
            GetHistoryDetails();
        }

        /// <summary>
        /// Get history list tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryGetHistoryList()
        {
            GetHistoryList();
        }

        /// <summary>
        /// Get queued tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryGetQueued()
        {
            GetQueued();
        }

        /// <summary>
        /// Get queued details tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryGetQueuedDetails()
        {
            GetQueuedDetails();
        }

        /// <summary>
        /// Get queued list tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryGetQueuedList()
        {
            GetQueuedList();
        }

        /// <summary>
        /// Get schedule tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryGetSchedule()
        {
            GetSchedule();
        }

        /// <summary>
        /// Get schedule date exists for schedule.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryGetScheduleDateExistsForSchedule()
        {
            GetScheduleDateExistsForSchedule();
        }

        /// <summary>
        /// Get scheduled job list tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryGetScheduledJobList()
        {
            GetScheduledJobList();
        }

        /// <summary>
        /// Get schedule list tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryGetScheduleList()
        {
            GetScheduleList();
        }

        /// <summary>
        /// Get schedules tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryGetSchedules()
        {
            GetSchedules();
        }

        /// <summary>
        /// Get statistics tests tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryGetStatistics()
        {
            GetStatistics();
        }

        /// <summary>
        /// Get worker tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryGetWorker()
        {
            GetWorker();
        }

        /// <summary>
        /// Get worker list tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryGetWorkerList()
        {
            GetWorkerList();
        }

        /// <summary>
        /// Get workers tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryGetWorkers()
        {
            GetWorkers();
        }

        /// <summary>
        /// Get working tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryGetWorking()
        {
            GetWorking();
        }

        /// <summary>
        /// Get working details tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryGetWorkingDetails()
        {
            GetWorkingDetails();
        }

        /// <summary>
        /// Get working for worker tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryGetWorkingForWorker()
        {
            GetWorkingForWorker();
        }

        /// <summary>
        /// Get working list tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryGetWorkingList()
        {
            GetWorkingList();
        }

        /// <summary>
        /// Get working signals tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryGetWorkingSignals()
        {
            GetWorkingSignals();
        }

        /// <summary>
        /// Release queued lock tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryReleaseQueuedLock()
        {
            ReleaseQueuedLock();
        }

        /// <summary>
        /// Release schedule lock tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryReleaseScheduleLock()
        {
            ReleaseScheduleLock();
        }

        /// <summary>
        /// Release worker lock tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryReleaseWorkerLock()
        {
            ReleaseWorkerLock();
        }

        /// <summary>
        /// Release working lock tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryReleaseWorkingLock()
        {
            ReleaseWorkingLock();
        }

        /// <summary>
        /// Update schedule tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryUpdateSchedule()
        {
            UpdateSchedule();
        }

        /// <summary>
        /// Update scheduled job tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryUpdateScheduledJob()
        {
            UpdateScheduledJob();
        }

        /// <summary>
        /// Update scheduled job order tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryUpdateScheduledJobOrder()
        {
            UpdateScheduledJobOrder();
        }

        /// <summary>
        /// Update worker tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryUpdateWorker()
        {
            UpdateWorker();
        }

        /// <summary>
        /// Update worker status tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryUpdateWorkerStatus()
        {
            UpdateWorkerStatus();
        }

        /// <summary>
        /// Update working tests.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
        public void SQLiteRepositoryUpdateWorking()
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
