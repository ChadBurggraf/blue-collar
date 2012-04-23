//-----------------------------------------------------------------------
// <copyright file="TestNoOpRepository.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Test
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    /// <summary>
    /// Empty implementation of <see cref="IRepository"/>.
    /// </summary>
    internal class TestNoOpRepository : IRepository
    {
        /// <summary>
        /// Initializes a new instance of the TestNoOpRepository class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public TestNoOpRepository(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// Begins a transaction.
        /// </summary>
        /// <returns>The transaction.</returns>
        public IDbTransaction BeginTransaction()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Begins a transaction.
        /// </summary>
        /// <param name="level">The isolation level to use for the transaction.</param>
        /// <returns>The transaction.</returns>
        public IDbTransaction BeginTransaction(IsolationLevel level)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Clears signals for the given worker and working job if applicable.
        /// </summary>
        /// <param name="workerId">The ID of the worker to clear the signal of.</param>
        /// <param name="workingId">The ID of the working job to clear the signal of, if applicable.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        public void ClearWorkingSignalPair(long workerId, long? workingId, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a history record.
        /// </summary>
        /// <param name="record">The record to create.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>The created record.</returns>
        public HistoryRecord CreateHistory(HistoryRecord record, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a queue record.
        /// </summary>
        /// <param name="record">The record to create.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>The created record.</returns>
        public QueueRecord CreateQueued(QueueRecord record, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates the queue and history records for the given schedule. This method should ensure that no
        /// records have already been created for the given schedule and schedule date before creating
        /// the records given.
        /// </summary>
        /// <param name="scheduleId">The ID of the schedule records are being created for.</param>
        /// <param name="scheduleDate">The schedule date records are being created for.</param>
        /// <param name="queued">The queued records to create.</param>
        /// <param name="history">The history records to create.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>The number of records created.</returns>
        public int CreateQueuedAndHistoryForSchedule(long scheduleId, DateTime scheduleDate, IEnumerable<QueueRecord> queued, IEnumerable<HistoryRecord> history, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a schedule record.
        /// </summary>
        /// <param name="record">The record to create.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>The created record.</returns>
        public ScheduleRecord CreateSchedule(ScheduleRecord record, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a scheduled job record.
        /// </summary>
        /// <param name="record">The record to create.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>The created record.</returns>
        public ScheduledJobRecord CreateScheduledJob(ScheduledJobRecord record, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a worker record.
        /// </summary>
        /// <param name="record">The record to create.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>The created record.</returns>
        public WorkerRecord CreateWorker(WorkerRecord record, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a working record.
        /// </summary>
        /// <param name="record">The working record to create.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>The created record.</returns>
        public WorkingRecord CreateWorking(WorkingRecord record, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes all data in the repository.
        /// </summary>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        public void DeleteAll(IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes the queued record with the given ID.
        /// </summary>
        /// <param name="id">The ID of the queued record to delete.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        public void DeleteQueued(long id, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes the schedule record with the given ID.
        /// </summary>
        /// <param name="id">The ID of the schedule to delete.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        public void DeleteSchedule(long id, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes the scheduled job record with the given ID.
        /// </summary>
        /// <param name="id">The ID of the scheduled job to delete.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        public void DeleteScheduledJob(long id, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes the worker record with the given ID.
        /// </summary>
        /// <param name="id">The ID of the worker to delete.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        public void DeleteWorker(long id, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes the working record with the given ID.
        /// </summary>
        /// <param name="id">The ID of the working record to delete.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        public void DeleteWorking(long id, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Disposes of resources used by this instance.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Gets a set of counts for the given application.
        /// </summary>
        /// <param name="applicationName">The name of the application to get counts for.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A set of counts.</returns>
        public CountsRecord GetCounts(string applicationName, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a history details record.
        /// </summary>
        /// <param name="id">The ID of the record to get.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A history details record.</returns>
        public HistoryDetailsRecord GetHistoryDetails(long id, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a list of history records.
        /// </summary>
        /// <param name="applicationName">The name of the application to get the history list for.</param>
        /// <param name="search">The search query to filter the collection with.</param>
        /// <param name="limit">The paging limit to use.</param>
        /// <param name="offset">The paging offset to use.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A list of history records.</returns>
        public RecordList<HistoryListRecord> GetHistoryList(string applicationName, string search, int limit, int offset, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a queued record for the given application and queue names.
        /// </summary>
        /// <param name="applicationName">The name of the application to get the queued record for.</param>
        /// <param name="queueFilters">The queue filters to use when filtering the queues to read from.</param>
        /// <param name="queuedBefore">The date to filter on.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A queued record, or null if none was found.</returns>
        public QueueRecord GetQueued(string applicationName, QueueNameFilters queueFilters, DateTime queuedBefore, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a queued details record.
        /// </summary>
        /// <param name="id">The ID of the record to get.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A queued details record.</returns>
        public QueueDetailsRecord GetQueuedDetails(long id, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a list of queue records.
        /// </summary>
        /// <param name="applicationName">The name of the application to get the queue list for.</param>
        /// <param name="search">The search query to filter the collection with.</param>
        /// <param name="limit">The paging limit to use.</param>
        /// <param name="offset">The paging offset to use.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A collection of queue records.</returns>
        public RecordList<QueueListRecord> GetQueuedList(string applicationName, string search, int limit, int offset, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the schedule with the given ID, NOT including its related scheduled jobs.
        /// </summary>
        /// <param name="id">The ID of the schedule to get.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>The schedule, or null if none was found.</returns>
        public ScheduleRecord GetSchedule(long id, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a value indicating whether data exists for the given schedule ID and calculated schedule date.
        /// If it does, this indicates that jobs have already been enqueued for the schedule and should not
        /// be enqueued again until the next calculated schedule date.
        /// </summary>
        /// <param name="scheduleId">The ID of the schedule to check data for.</param>
        /// <param name="scheduleDate">The calcualted schedule date to check data for.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>True if data already exists, false otherwise.</returns>
        public bool GetScheduleDateExistsForSchedule(long scheduleId, DateTime scheduleDate, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a schedule and its related scheduled jobs, filtered by the given list parameters.
        /// </summary>
        /// <param name="applicationName">The name of the application to get the scheduled job list for.</param>
        /// <param name="id">The ID of the schedule to get.</param>
        /// <param name="search">The search query to filtere the related job collection with.</param>
        /// <param name="limit">The paging limit to use.</param>
        /// <param name="offset">The paging offset to use.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A schedul, or null if none was found.</returns>
        public ScheduledJobRecordList GetScheduledJobList(string applicationName, long id, string search, int limit, int offset, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Attempts to obtain the enqueueing lock for the given schedule ID.
        /// </summary>
        /// <param name="scheduleId">The ID of the schedule to obtain the schedule enqueueing lock for.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>True if the enqueueing lock was obtained, false otherwise.</returns>
        public bool GetScheduleEnqueueingLock(long scheduleId, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a list of schedule records.
        /// </summary>
        /// <param name="applicationName">The name of the application to get the schedule list for.</param>
        /// <param name="search">The search query to filter the collection with.</param>
        /// <param name="limit">The paging limit to use.</param>
        /// <param name="offset">The paging offset to use.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A collection of schedules.</returns>
        public RecordList<ScheduleListRecord> GetScheduleList(string applicationName, string search, int limit, int offset, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a collection of schedules and their related scheduled jobs for the given application name.
        /// </summary>
        /// <param name="applicationName">The name of the application to get schedules for.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A collection of schedules.</returns>
        public IEnumerable<ScheduleRecord> GetSchedules(string applicationName, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a set of system statistics for the given application name and date ranges.
        /// </summary>
        /// <param name="applicationName">The name of the application to get system statistics for.</param>
        /// <param name="recentBeginDate">The begin date of the recent period to get statistics for.</param>
        /// <param name="distantBeginDate">The begin date of the distant period to get statistics for.</param>
        /// <param name="endDate">The end date of the distant period to get statistics for.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A set of system statistics.</returns>
        public StatisticsRecord GetStatistics(string applicationName, DateTime recentBeginDate, DateTime distantBeginDate, DateTime endDate, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the worker record with the given ID.
        /// </summary>
        /// <param name="id">The ID of the worker record to get.</param>
        /// <param name="transaction">The transaction to use.</param>
        /// <returns>A worker record.</returns>
        public WorkerRecord GetWorker(long id, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a list of worker records.
        /// </summary>
        /// <param name="applicationName">The application name to get records for.</param>
        /// <param name="search">The search query to filter the collection with.</param>
        /// <param name="limit">The paging limit to use.</param>
        /// <param name="offset">The paging offset to use.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A collection of worer records.</returns>
        public RecordList<WorkerRecord> GetWorkerList(string applicationName, string search, int limit, int offset, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the worker collection for the given machine.
        /// </summary>
        /// <param name="applicationName">The application name to get workers for.</param>
        /// <param name="machineAddress">The address of the machine to get workers for.</param>
        /// <param name="machineName">The name of the machine to get workers for.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A collection of worker records.</returns>
        public IEnumerable<WorkerRecord> GetWorkers(string applicationName, string machineAddress, string machineName, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the working record with the given ID.
        /// </summary>
        /// <param name="id">The ID of the record to get.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A working record, or null if none was found.</returns>
        public WorkingRecord GetWorking(long id, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a working details record.
        /// </summary>
        /// <param name="id">The ID of the record to get.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A working details record, or null if none was found.</returns>
        public WorkingDetailsRecord GetWorkingDetails(long id, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a collection of working records that belong to the given worker ID.
        /// </summary>
        /// <param name="workerId">The ID of the worker to get working records for.</param>
        /// <param name="excludingId">The ID of the working record to exclude, if applicable.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A collection of working records.</returns>
        public IEnumerable<WorkingRecord> GetWorkingForWorker(long workerId, long? excludingId, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a list of working records.
        /// </summary>
        /// <param name="applicationName">The application name to get records for.</param>
        /// <param name="search">The search query to filter the collection with.</param>
        /// <param name="limit">The paging limit to use.</param>
        /// <param name="offset">The paging offset to use.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A collection of working records.</returns>
        public RecordList<WorkingListRecord> GetWorkingList(string applicationName, string search, int limit, int offset, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the current signals set for a worker and a working job, if applicable.
        /// </summary>
        /// <param name="workerId">The ID of the worker to get a signal for.</param>
        /// <param name="workingId">The ID of the working job to get a signal for, if applicable.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A signals record.</returns>
        public SignalsRecord GetWorkingSignals(long workerId, long? workingId, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Releases the enqueueing lock for the schedule with the given ID.
        /// </summary>
        /// <param name="scheduleId">The ID of the schedule to release the enqueuing lock for.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        public void ReleaseScheduleEnqueueingLock(long scheduleId, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Signals all workers for the given application name.
        /// </summary>
        /// <param name="applicationName">The application name to signal workers for.</param>
        /// <param name="signal">The signal to set.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        public void SignalWorkers(string applicationName, WorkerSignal signal, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates the given schedule.
        /// </summary>
        /// <param name="record">The schedule record to update.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>The updated record.</returns>
        public ScheduleRecord UpdateSchedule(ScheduleRecord record, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates the given scheduled job.
        /// </summary>
        /// <param name="record">The scheduled job record to update.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>The updated record.</returns>
        public ScheduledJobRecord UpdateScheduledJob(ScheduledJobRecord record, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates the given worker.
        /// </summary>
        /// <param name="record">The worker record to update.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>The updated worker.</returns>
        public WorkerRecord UpdateWorker(WorkerRecord record, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates the status of the worker with the given ID.
        /// </summary>
        /// <param name="id">The ID of the worker to update status for.</param>
        /// <param name="status">The status to update.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        public void UpdateWorkerStatus(long id, WorkerStatus status, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates the given working record.
        /// </summary>
        /// <param name="record">The working record to update.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>The updated working record.</returns>
        public WorkingRecord UpdateWorking(WorkingRecord record, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }
    }
}
