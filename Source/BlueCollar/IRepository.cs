//-----------------------------------------------------------------------
// <copyright file="IRepository.cs" company="Tasty Codes">
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
    /// Defines the interface for Blue Collar repositories.
    /// </summary>
    public interface IRepository : IDisposable
    {
        /// <summary>
        /// Gets the connection string used to connect to the repository, if applicable.
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// Attempts to obtain the lock for the given record ID.
        /// </summary>
        /// <param name="id">The ID of the queued record to obtain the lock for.</param>
        /// <param name="forceIfOlderThan">A date to compare the lock's last updated date with. If
        /// the lock is older than the given date, then it will be forced and acquired by the caller.</param>
        /// <param name="transaction">The transaction to use, if necessary.</param>
        /// <returns>True if the lock was acquired, false otherwise.</returns>
        bool AcquireQueuedLock(long id, DateTime forceIfOlderThan, IDbTransaction transaction);

        /// <summary>
        /// Attempts to obtain the lock for the given schedule ID.
        /// </summary>
        /// <param name="id">The ID of the schedule to obtain the lock for.</param>
        /// <param name="forceIfOlderThan">A date to compare the lock's last updated date with. If
        /// the lock is older than the given date, then it will be forced and acquired by the caller.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>True if the lock was obtained, false otherwise.</returns>
        bool AcquireScheduleLock(long id, DateTime forceIfOlderThan, IDbTransaction transaction);

        /// <summary>
        /// Attempts to obtain the lock for the given record ID.
        /// </summary>
        /// <param name="id">The ID of the worker record to obtain the lock for.</param>
        /// <param name="forceIfOlderThan">A date to compare the lock's last updated date with. If
        /// the lock is older than the give date, then it will be forced and acquired by the caller.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>True if the lock was acquired, false otherwise.</returns>
        bool AcquireWorkerLock(long id, DateTime forceIfOlderThan, IDbTransaction transaction);

        /// <summary>
        /// Attempts to obtain the lock for the given record ID.
        /// </summary>
        /// <param name="id">The ID of the working job record to obtain the lock for.</param>
        /// <param name="forceIfOlderThan">A date to compare the lock's last updated date with. If
        /// the lock is older than the give date, then it will be forced and acquired by the caller.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>True if the lock was acquired, false otherwise.</returns>
        bool AcquireWorkingLock(long id, DateTime forceIfOlderThan, IDbTransaction transaction);

        /// <summary>
        /// Begins a transaction.
        /// </summary>
        /// <returns>The transaction.</returns>
        IDbTransaction BeginTransaction();

        /// <summary>
        /// Begins a transaction.
        /// </summary>
        /// <param name="level">The isolation level to use for the transaction.</param>
        /// <returns>The transaction.</returns>
        IDbTransaction BeginTransaction(IsolationLevel level);

        /// <summary>
        /// Clears signals for the given worker and working job if applicable.
        /// </summary>
        /// <param name="workerId">The ID of the worker to clear the signal of.</param>
        /// <param name="workingId">The ID of the working job to clear the signal of, if applicable.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        void ClearWorkingSignalPair(long workerId, long? workingId, IDbTransaction transaction);

        /// <summary>
        /// Creates a history record.
        /// </summary>
        /// <param name="record">The record to create.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>The created record.</returns>
        HistoryRecord CreateHistory(HistoryRecord record, IDbTransaction transaction);

        /// <summary>
        /// Creates a queue record.
        /// </summary>
        /// <param name="record">The record to create.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>The created record.</returns>
        QueueRecord CreateQueued(QueueRecord record, IDbTransaction transaction);

        /// <summary>
        /// Creates the queue and history records for the given schedule.
        /// </summary>
        /// <param name="scheduleId">The ID of the schedule records are being created for.</param>
        /// <param name="scheduleDate">The schedule date records are being created for.</param>
        /// <param name="queued">The queued records to create.</param>
        /// <param name="history">The history records to create.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>The number of records created.</returns>
        int CreateQueuedAndHistoryForSchedule(long scheduleId, DateTime scheduleDate, IEnumerable<QueueRecord> queued, IEnumerable<HistoryRecord> history, IDbTransaction transaction);

        /// <summary>
        /// Creates a schedule record.
        /// </summary>
        /// <param name="record">The record to create.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>The created record.</returns>
        ScheduleRecord CreateSchedule(ScheduleRecord record, IDbTransaction transaction);

        /// <summary>
        /// Creates a scheduled job record.
        /// </summary>
        /// <param name="record">The record to create.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>The created record.</returns>
        ScheduledJobRecord CreateScheduledJob(ScheduledJobRecord record, IDbTransaction transaction);

        /// <summary>
        /// Creates a worker record.
        /// </summary>
        /// <param name="record">The record to create.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>The created record.</returns>
        WorkerRecord CreateWorker(WorkerRecord record, IDbTransaction transaction);

        /// <summary>
        /// Creates a working record.
        /// </summary>
        /// <param name="record">The working record to create.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>The created record.</returns>
        WorkingRecord CreateWorking(WorkingRecord record, IDbTransaction transaction);

        /// <summary>
        /// Deletes all data in the repository.
        /// </summary>
        /// <param name="applicationName">The name of the application to delete data for.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        void DeleteAll(string applicationName, IDbTransaction transaction);

        /// <summary>
        /// Deletes history older than the given date.
        /// </summary>
        /// <param name="applicationName">The name of the application to delete data for.</param>
        /// <param name="olderThan">The date to delete history older than.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        void DeleteHistory(string applicationName, DateTime olderThan, IDbTransaction transaction);

        /// <summary>
        /// Deletes the queued record with the given ID.
        /// </summary>
        /// <param name="id">The ID of the queued record to delete.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        void DeleteQueued(long id, IDbTransaction transaction);

        /// <summary>
        /// Deletes the schedule record with the given ID.
        /// </summary>
        /// <param name="id">The ID of the schedule to delete.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        void DeleteSchedule(long id, IDbTransaction transaction);

        /// <summary>
        /// Deletes the scheduled job record with the given ID.
        /// </summary>
        /// <param name="id">The ID of the scheduled job to delete.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        void DeleteScheduledJob(long id, IDbTransaction transaction);

        /// <summary>
        /// Deletes the worker record with the given ID.
        /// </summary>
        /// <param name="id">The ID of the worker to delete.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        void DeleteWorker(long id, IDbTransaction transaction);

        /// <summary>
        /// Deletes the working record with the given ID.
        /// </summary>
        /// <param name="id">The ID of the working record to delete.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        void DeleteWorking(long id, IDbTransaction transaction);

        /// <summary>
        /// Gets a set of counts for the given application.
        /// </summary>
        /// <param name="applicationName">The name of the application to get counts for.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A set of counts.</returns>
        CountsRecord GetCounts(string applicationName, IDbTransaction transaction);

        /// <summary>
        /// Gets a history details record.
        /// </summary>
        /// <param name="id">The ID of the record to get.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A history details record.</returns>
        HistoryDetailsRecord GetHistoryDetails(long id, IDbTransaction transaction);

        /// <summary>
        /// Gets a list of history records.
        /// </summary>
        /// <param name="applicationName">The name of the application to get the history list for.</param>
        /// <param name="search">The search query to filter the collection with.</param>
        /// <param name="limit">The paging limit to use.</param>
        /// <param name="offset">The paging offset to use.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A list of history records.</returns>
        RecordList<HistoryListRecord> GetHistoryList(string applicationName, string search, int limit, int offset, IDbTransaction transaction);

        /// <summary>
        /// Gets a queued record for the given application and queue names.
        /// </summary>
        /// <param name="applicationName">The name of the application to get the queued record for.</param>
        /// <param name="queueFilters">The queue filters to use when filtering the queues to read from.</param>
        /// <param name="queuedBefore">The date to filter on.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A queued record, or null if none was found.</returns>
        QueueRecord GetQueued(string applicationName, QueueNameFilters queueFilters, DateTime queuedBefore, IDbTransaction transaction);

        /// <summary>
        /// Gets a queued details record.
        /// </summary>
        /// <param name="id">The ID of the record to get.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A queued details record.</returns>
        QueueDetailsRecord GetQueuedDetails(long id, IDbTransaction transaction);

        /// <summary>
        /// Gets a list of queue records.
        /// </summary>
        /// <param name="applicationName">The name of the application to get the queue list for.</param>
        /// <param name="search">The search query to filter the collection with.</param>
        /// <param name="limit">The paging limit to use.</param>
        /// <param name="offset">The paging offset to use.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A collection of queue records.</returns>
        RecordList<QueueListRecord> GetQueuedList(string applicationName, string search, int limit, int offset, IDbTransaction transaction);

        /// <summary>
        /// Gets the schedule with the given ID, NOT including its related scheduled jobs.
        /// </summary>
        /// <param name="id">The ID of the schedule to get.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>The schedule, or null if none was found.</returns>
        ScheduleRecord GetSchedule(long id, IDbTransaction transaction);

        /// <summary>
        /// Gets a value indicating whether data exists for the given schedule ID and calculated schedule date.
        /// If it does, this indicates that jobs have already been enqueued for the schedule and should not
        /// be enqueued again until the next calculated schedule date.
        /// </summary>
        /// <param name="scheduleId">The ID of the schedule to check data for.</param>
        /// <param name="scheduleDate">The calculated schedule date to check data for.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>True if data already exists, false otherwise.</returns>
        bool GetScheduleDateExistsForSchedule(long scheduleId, DateTime scheduleDate, IDbTransaction transaction);

        /// <summary>
        /// Gets a schedule and its related scheduled jobs, filtered by the given list parameters.
        /// </summary>
        /// <param name="applicationName">The name of the application to get the scheduled job list for.</param>
        /// <param name="id">The ID of the schedule to get.</param>
        /// <param name="search">The search query to filter the related job collection with.</param>
        /// <param name="limit">The paging limit to use.</param>
        /// <param name="offset">The paging offset to use.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A schedule, or null if none was found.</returns>
        ScheduledJobRecordList GetScheduledJobList(string applicationName, long id, string search, int limit, int offset, IDbTransaction transaction);

        /// <summary>
        /// Gets a list of schedule records.
        /// </summary>
        /// <param name="applicationName">The name of the application to get the schedule list for.</param>
        /// <param name="search">The search query to filter the collection with.</param>
        /// <param name="limit">The paging limit to use.</param>
        /// <param name="offset">The paging offset to use.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A collection of schedules.</returns>
        RecordList<ScheduleListRecord> GetScheduleList(string applicationName, string search, int limit, int offset, IDbTransaction transaction);

        /// <summary>
        /// Gets a collection of schedules and their related scheduled jobs for the given application name.
        /// </summary>
        /// <param name="applicationName">The name of the application to get schedules for.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A collection of schedules.</returns>
        IEnumerable<ScheduleRecord> GetSchedules(string applicationName, IDbTransaction transaction);

        /// <summary>
        /// Gets a set of system statistics for the given application name and date ranges.
        /// </summary>
        /// <param name="applicationName">The name of the application to get system statistics for.</param>
        /// <param name="recentBeginDate">The begin date of the recent period to get statistics for.</param>
        /// <param name="distantBeginDate">The begin date of the distant period to get statistics for.</param>
        /// <param name="endDate">The end date of the distant period to get statistics for.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A set of system statistics.</returns>
        StatisticsRecord GetStatistics(string applicationName, DateTime recentBeginDate, DateTime distantBeginDate, DateTime endDate, IDbTransaction transaction);

        /// <summary>
        /// Gets the worker record with the given ID.
        /// </summary>
        /// <param name="id">The ID of the worker record to get.</param>
        /// <param name="transaction">The transaction to use.</param>
        /// <returns>A worker record.</returns>
        WorkerRecord GetWorker(long id, IDbTransaction transaction);

        /// <summary>
        /// Gets a list of worker records.
        /// </summary>
        /// <param name="applicationName">The application name to get records for.</param>
        /// <param name="search">The search query to filter the collection with.</param>
        /// <param name="limit">The paging limit to use.</param>
        /// <param name="offset">The paging offset to use.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A collection of worker records.</returns>
        RecordList<WorkerRecord> GetWorkerList(string applicationName, string search, int limit, int offset, IDbTransaction transaction);

        /// <summary>
        /// Gets the worker collection for the given machine.
        /// </summary>
        /// <param name="applicationName">The application name to get workers for.</param>
        /// <param name="machineAddress">The address of the machine to get workers for.</param>
        /// <param name="machineName">The name of the machine to get workers for.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A collection of worker records.</returns>
        IEnumerable<WorkerRecord> GetWorkers(string applicationName, string machineAddress, string machineName, IDbTransaction transaction);

        /// <summary>
        /// Gets the working record with the given ID.
        /// </summary>
        /// <param name="id">The ID of the record to get.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A working record, or null if none was found.</returns>
        WorkingRecord GetWorking(long id, IDbTransaction transaction);

        /// <summary>
        /// Gets a working details record.
        /// </summary>
        /// <param name="id">The ID of the record to get.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A working details record, or null if none was found.</returns>
        WorkingDetailsRecord GetWorkingDetails(long id, IDbTransaction transaction);

        /// <summary>
        /// Gets a collection of working records that belong to the given worker ID.
        /// </summary>
        /// <param name="workerId">The ID of the worker to get working records for.</param>
        /// <param name="excludingId">The ID of the working record to exclude, if applicable.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A collection of working records.</returns>
        IEnumerable<WorkingRecord> GetWorkingForWorker(long workerId, long? excludingId, IDbTransaction transaction);

        /// <summary>
        /// Gets a list of working records.
        /// </summary>
        /// <param name="applicationName">The application name to get records for.</param>
        /// <param name="search">The search query to filter the collection with.</param>
        /// <param name="limit">The paging limit to use.</param>
        /// <param name="offset">The paging offset to use.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A collection of working records.</returns>
        RecordList<WorkingListRecord> GetWorkingList(string applicationName, string search, int limit, int offset, IDbTransaction transaction);

        /// <summary>
        /// Gets the current signals set for a worker and a working job, if applicable.
        /// </summary>
        /// <param name="workerId">The ID of the worker to get a signal for.</param>
        /// <param name="workingId">The ID of the working job to get a signal for, if applicable.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A signals record.</returns>
        SignalsRecord GetWorkingSignals(long workerId, long? workingId, IDbTransaction transaction);

        /// <summary>
        /// Releases the lock for the given queued job ID.
        /// </summary>
        /// <param name="id">The ID of the record to release the lock for.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        void ReleaseQueuedLock(long id, IDbTransaction transaction);

        /// <summary>
        /// Releases the lock for the given schedule ID.
        /// </summary>
        /// <param name="id">The ID of the record to release the lock for.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        void ReleaseScheduleLock(long id, IDbTransaction transaction);

        /// <summary>
        /// Releases the lock for the given worker ID.
        /// </summary>
        /// <param name="id">The ID of the record to release the lock for.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        void ReleaseWorkerLock(long id, IDbTransaction transaction);

        /// <summary>
        /// Releases the lock for the given working job ID.
        /// </summary>
        /// <param name="id">The ID of the record to release the lock for.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        void ReleaseWorkingLock(long id, IDbTransaction transaction);

        /// <summary>
        /// Signals all workers for the given application name.
        /// </summary>
        /// <param name="applicationName">The application name to signal workers for.</param>
        /// <param name="signal">The signal to set.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        void SignalWorkers(string applicationName, WorkerSignal signal, IDbTransaction transaction);

        /// <summary>
        /// Updates the given schedule.
        /// </summary>
        /// <param name="record">The schedule record to update.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>The updated record.</returns>
        ScheduleRecord UpdateSchedule(ScheduleRecord record, IDbTransaction transaction);

        /// <summary>
        /// Updates the given scheduled job.
        /// </summary>
        /// <param name="record">The scheduled job record to update.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>The updated record.</returns>
        ScheduledJobRecord UpdateScheduledJob(ScheduledJobRecord record, IDbTransaction transaction);

        /// <summary>
        /// Updates the scheduled job's order number.
        /// </summary>
        /// <param name="record">The scheduled job order record identifying the scheduled job to update.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        void UpdateScheduledJobOrder(ScheduledJobOrderRecord record, IDbTransaction transaction);

        /// <summary>
        /// Updates the given worker.
        /// </summary>
        /// <param name="record">The worker record to update.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>The updated worker.</returns>
        WorkerRecord UpdateWorker(WorkerRecord record, IDbTransaction transaction);

        /// <summary>
        /// Updates the status of the worker with the given ID.
        /// </summary>
        /// <param name="id">The ID of the worker to update status for.</param>
        /// <param name="status">The status to update.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        void UpdateWorkerStatus(long id, WorkerStatus status, IDbTransaction transaction);

        /// <summary>
        /// Updates the given working record.
        /// </summary>
        /// <param name="record">The working record to update.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>The updated working record.</returns>
        WorkingRecord UpdateWorking(WorkingRecord record, IDbTransaction transaction);
    }
}
