//-----------------------------------------------------------------------
// <copyright file="SQLiteRepository.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SQLite;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Dapper;

    /// <summary>
    /// Implements <see cref="IRepository"/> using SQLite.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The spelling and casing are correct according to convention.")]
    public class SQLiteRepository : IRepository
    {
        private const string CountsSql =
@"SELECT CAST(COUNT([Id]) AS bigint)
FROM [BlueCollarHistory]
WHERE
    [ApplicationName] = @ApplicationName;

SELECT CAST(COUNT([Id]) AS bigint)
FROM [BlueCollarQueue]
WHERE
    [ApplicationName] = @ApplicationName;

SELECT CAST(COUNT([Id]) AS bigint)
FROM [BlueCollarSchedule]
WHERE
    [ApplicationName] = @ApplicationName;

SELECT CAST(COUNT([Id]) AS bigint)
FROM [BlueCollarWorker]
WHERE
    [ApplicationName] = @ApplicationName;

SELECT CAST(COUNT([Id]) AS bigint)
FROM [BlueCollarWorking]
WHERE
    [ApplicationName] = @ApplicationName;";

        private IDbConnection connection;
        private IsolationLevel defaultIsolationLevel;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the SQLiteRepository class.
        /// </summary>
        /// <param name="connectionString">The connection string to use when connecting to the database.</param>
        public SQLiteRepository(string connectionString)
        {
            connectionString = (connectionString ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = "data source=BlueCollar.sqlite;journal mode=Off;synchronous=Off;version=3";
            }

            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder(connectionString);
            builder.DataSource = BlueCollarSection.Section.ResolvePath(builder.DataSource);
            builder.DateTimeKind = DateTimeKind.Utc;

            EnsureDatabase(builder.DataSource);
            this.ConnectionString = builder.ConnectionString;

            try
            {
                this.defaultIsolationLevel = builder.DefaultIsolationLevel;
            }
            catch (NullReferenceException)
            {
                this.defaultIsolationLevel = IsolationLevel.Serializable;
            }

            this.connection = new SQLiteConnection(this.ConnectionString);
            this.connection.Open();
        }

        /// <summary>
        /// Finalizes an instance of the SQLiteRepository class.
        /// </summary>
        ~SQLiteRepository()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets the connection string used to connect to this instance's database.
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// Begins a transaction.
        /// </summary>
        /// <returns>The transaction.</returns>
        public IDbTransaction BeginTransaction()
        {
            return this.connection.BeginTransaction();
        }

        /// <summary>
        /// Begins a transaction.
        /// </summary>
        /// <param name="level">The isolation level to use for the transaction.</param>
        /// <returns>The transaction.</returns>
        public IDbTransaction BeginTransaction(IsolationLevel level)
        {
            switch (level)
            {
                case IsolationLevel.Chaos:
                case IsolationLevel.ReadUncommitted:
                case IsolationLevel.Unspecified:
                    level = this.defaultIsolationLevel;
                    break;
                case IsolationLevel.RepeatableRead:
                case IsolationLevel.Serializable:
                case IsolationLevel.Snapshot:
                    level = IsolationLevel.Serializable;
                    break;
                case IsolationLevel.ReadCommitted:
                    break;
                default:
                    throw new NotImplementedException();
            }

            return this.connection.BeginTransaction(level);
        }

        /// <summary>
        /// Clears signals for the given worker and working job if applicable.
        /// </summary>
        /// <param name="workerId">The ID of the worker to clear the signal of.</param>
        /// <param name="workingId">The ID of the working job to clear the signal of, if applicable.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        public void ClearWorkingSignalPair(long workerId, long? workingId, IDbTransaction transaction)
        {
            StringBuilder sb = new StringBuilder(
@"UPDATE [BlueCollarWorker] 
SET 
    [Signal] = @WorkerSignal,
    [UpdatedOn] = @Now
WHERE
    [Id] = @WorkerId;");

            if (workingId != null)
            {
                sb.Append("\n");
                sb.Append(
@"UPDATE [BlueCollarWorking]
SET
    [Signal] = @WorkingSignal
WHERE
    [Id] = @WorkingId;");
            }

            this.connection.Execute(
                sb.ToString(),
                new
                {
                    Now = DateTime.UtcNow,
                    WorkerId = workerId,
                    WorkerSignal = WorkerSignal.None.ToString(),
                    WorkingId = workingId,
                    WorkingSignal = WorkingSignal.None.ToString()
                },
                transaction,
                null,
                null);
        }

        /// <summary>
        /// Creates a history record.
        /// </summary>
        /// <param name="record">The record to create.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>The created record.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "IRepository methods are designed to be called by validated code paths only.")]
        public HistoryRecord CreateHistory(HistoryRecord record, IDbTransaction transaction)
        {
            const string Sql =
@"INSERT INTO [BlueCollarHistory]([ApplicationName],[WorkerId],[ScheduleId],[QueueName],[JobName],[JobType],[Data],[QueuedOn],[TryNumber],[StartedOn],[Status],[Exception],[FinishedOn])
VALUES(@ApplicationName,@WorkerId,@ScheduleId,@QueueName,@JobName,@JobType,@Data,@QueuedOn,@TryNumber,@StartedOn,@StatusString,@Exception,@FinishedOn);
SELECT last_insert_rowid();";

            record.Id = this.connection.Query<long>(
                Sql,
                record,
                transaction,
                true,
                null,
                null).First();

            return record;
        }

        /// <summary>
        /// Creates a queue record.
        /// </summary>
        /// <param name="record">The record to create.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>The created record.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "IRepository methods are designed to be called by validated code paths only.")]
        public QueueRecord CreateQueued(QueueRecord record, IDbTransaction transaction)
        {
            const string Sql =
@"INSERT INTO [BlueCollarQueue]([ApplicationName],[ScheduleId],[QueueName],[JobName],[JobType],[Data],[QueuedOn],[TryNumber])
VALUES(@ApplicationName,@ScheduleId,@QueueName,@JobName,@JobType,@Data,@QueuedOn,@TryNumber);
SELECT last_insert_rowid();";

            record.Id = this.connection.Query<long>(
                Sql,
                record,
                transaction,
                true,
                null,
                null).First();

            return record;
        }

        /// <summary>
        /// Creates the queue and history records for the given schedule.
        /// </summary>
        /// <param name="scheduleId">The ID of the schedule records are being created for.</param>
        /// <param name="scheduleDate">The schedule date records are being created for.</param>
        /// <param name="queued">The queued records to create.</param>
        /// <param name="history">The history records to create.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>The number of records created.</returns>
        public int CreateQueuedAndHistoryForSchedule(long scheduleId, DateTime scheduleDate, IEnumerable<QueueRecord> queued, IEnumerable<HistoryRecord> history, IDbTransaction transaction)
        {
            int created = 0;
            bool commitRollback = false;

            if (transaction == null)
            {
                commitRollback = true;
                transaction = this.BeginTransaction();
            }

            try
            {
                const string InsertQueuedSql =
@"INSERT INTO [BlueCollarQueue]([ApplicationName],[ScheduleId],[QueueName],[JobName],[JobType],[Data],[QueuedOn],[TryNumber])
VALUES(@ApplicationName,@ScheduleId,@QueueName,@JobName,@JobType,@Data,@QueuedOn,@TryNumber);";

                const string InsertHistorySql =
@"INSERT INTO [BlueCollarHistory]([ApplicationName],[WorkerId],[ScheduleId],[QueueName],[JobName],[JobType],[Data],[QueuedOn],[TryNumber],[StartedOn],[Status],[Exception],[FinishedOn])
VALUES (@ApplicationName,@WorkerId,@ScheduleId,@QueueName,@JobName,@JobType,@Data,@QueuedOn,@TryNumber,@StartedOn,@StatusString,@Exception,@FinishedOn);";

                if (queued != null && queued.Count() > 0)
                {
                    created += this.connection.Execute(InsertQueuedSql, queued, transaction, null, null);
                }

                if (history != null && history.Count() > 0)
                {
                    created += this.connection.Execute(InsertHistorySql, history, transaction, null, null);
                }

                if (commitRollback)
                {
                    transaction.Commit();
                }
            }
            catch
            {
                if (commitRollback)
                {
                    transaction.Rollback();
                }

                throw;
            }
            finally
            {
                if (commitRollback)
                {
                    transaction.Dispose();
                }
            }

            return created;
        }

        /// <summary>
        /// Creates a schedule record.
        /// </summary>
        /// <param name="record">The record to create.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>The created record.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "IRepository methods are designed to be called by validated code paths only.")]
        public ScheduleRecord CreateSchedule(ScheduleRecord record, IDbTransaction transaction)
        {
            const string Sql =
@"INSERT INTO [BlueCollarSchedule]([ApplicationName],[QueueName],[Name],[StartOn],[EndOn],[RepeatType],[RepeatValue],[Enabled],[Enqueueing],[EnqueueingUpdatedOn])
VALUES(@ApplicationName,@QueueName,@Name,@StartOn,@EndOn,@RepeatTypeString,@RepeatValue,@Enabled,@Enqueueing,@EnqueueingUpdatedOn);
SELECT last_insert_rowid();";

            record.Id = this.connection.Query<long>(
                Sql,
                record,
                transaction,
                true,
                null,
                null).First();

            return record;
        }

        /// <summary>
        /// Creates a scheduled job record.
        /// </summary>
        /// <param name="record">The record to create.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>The created record.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "IRepository methods are designed to be called by validated code paths only.")]
        public ScheduledJobRecord CreateScheduledJob(ScheduledJobRecord record, IDbTransaction transaction)
        {
            const string Sql =
@"INSERT INTO [BlueCollarScheduledJob]([ScheduleId],[JobType],[Properties])
VALUES(@ScheduleId,@JobType,@Properties);
SELECT last_insert_rowid();";

            record.Id = this.connection.Query<long>(
                Sql,
                record,
                transaction,
                true,
                null,
                null).First();

            return record;
        }

        /// <summary>
        /// Creates a worker record.
        /// </summary>
        /// <param name="record">The record to create.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>The created record.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "IRepository methods are designed to be called by validated code paths only.")]
        public WorkerRecord CreateWorker(WorkerRecord record, IDbTransaction transaction)
        {
            const string Sql =
@"INSERT INTO [BlueCollarWorker]([ApplicationName],[Name],[MachineName],[MachineAddress],[QueueNames],[Status],[Signal],[Startup],[UpdatedOn])
VALUES(@ApplicationName,@Name,@MachineName,@MachineAddress,@QueueNames,@StatusString,@SignalString,@StartupString,@UpdatedOn);
SELECT last_insert_rowid();";

            record.Id = this.connection.Query<long>(
                Sql,
                record,
                transaction,
                true,
                null,
                null).First();

            return record;
        }

        /// <summary>
        /// Creates a working record.
        /// </summary>
        /// <param name="record">The working record to create.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>The created record.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "IRepository methods are designed to be called by validated code paths only.")]
        public WorkingRecord CreateWorking(WorkingRecord record, IDbTransaction transaction)
        {
            const string Sql =
@"INSERT INTO [BlueCollarWorking]([ApplicationName],[WorkerId],[ScheduleId],[QueueName],[JobName],[JobType],[Data],[QueuedOn],[TryNumber],[StartedOn],[Signal])
VALUES(@ApplicationName,@WorkerId,@ScheduleId,@QueueName,@JobName,@JobType,@Data,@QueuedOn,@TryNumber,@StartedOn,@SignalString);
SELECT last_insert_rowid();";

            record.Id = this.connection.Query<long>(
                Sql,
                record,
                transaction,
                true,
                null,
                null).First();

            return record;
        }

        /// <summary>
        /// Deletes all data in the repository.
        /// </summary>
        /// <param name="applicationName">The name of the application to delete data for.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        public void DeleteAll(string applicationName, IDbTransaction transaction)
        {
            const string Sql =
@"DELETE FROM [BlueCollarHistory] WHERE [ApplicationName] = @ApplicationName;
DELETE FROM [BlueCollarWorking] WHERE [ApplicationName] = @ApplicationName;
DELETE FROM [BlueCollarWorker] WHERE [ApplicationName] = @ApplicationName;
DELETE FROM [BlueCollarQueue] WHERE [ApplicationName] = @ApplicationName;
DELETE FROM [BlueCollarScheduledJob]
WHERE
	[ScheduleId] IN
	(
		SELECT [Id]
		FROM [BlueCollarSchedule]
		WHERE
			[ApplicationName] = @ApplicationName
	);
DELETE FROM [BlueCollarSchedule] WHERE [ApplicationName] = @ApplicationName;";

            this.connection.Execute(
                Sql,
                new { ApplicationName = applicationName },
                transaction,
                null,
                null);
        }

        /// <summary>
        /// Deletes history older than the given date.
        /// </summary>
        /// <param name="applicationName">The name of the application to delete data for.</param>
        /// <param name="olderThan">The date to delete history older than.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        public void DeleteHistory(string applicationName, DateTime olderThan, IDbTransaction transaction)
        {
            const string Sql =
@"DELETE FROM [BlueCollarHistory]
WHERE
    [ApplicationName] = @ApplicationName
    AND [QueuedOn] < @OlderThan;";

            this.connection.Execute(
                Sql,
                new { ApplicationName = @applicationName, OlderThan = olderThan },
                transaction,
                null,
                null);
        }

        /// <summary>
        /// Deletes the queued record with the given ID.
        /// </summary>
        /// <param name="id">The ID of the queued record to delete.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        public void DeleteQueued(long id, IDbTransaction transaction)
        {
            this.connection.Execute(
                "DELETE FROM [BlueCollarQueue] WHERE [Id] = @Id;",
                new { Id = id },
                transaction,
                null,
                null);
        }

        /// <summary>
        /// Deletes the schedule record with the given ID.
        /// </summary>
        /// <param name="id">The ID of the schedule to delete.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        public void DeleteSchedule(long id, IDbTransaction transaction)
        {
            const string Sql =
@"UPDATE [BlueCollarQueue]
SET
    [ScheduleId] = NULL
WHERE
    [ScheduleId] = @Id;

UPDATE [BlueCollarWorking]
SET
    [ScheduleId] = NULL
WHERE
    [ScheduleId] = @Id;

UPDATE [BlueCollarHistory]
SET
    [ScheduleId] = NULL
WHERE
    [ScheduleId] = @Id;

DELETE FROM [BlueCollarScheduledJob]
WHERE
    [ScheduleId] = @Id;
DELETE FROM [BlueCollarSchedule]
WHERE
    [Id] = @Id;";

            this.connection.Execute(Sql, new { Id = id }, transaction, null, null);
        }

        /// <summary>
        /// Deletes the scheduled job record with the given ID.
        /// </summary>
        /// <param name="id">The ID of the scheduled job to delete.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        public void DeleteScheduledJob(long id, IDbTransaction transaction)
        {
            this.connection.Execute(
                "DELETE FROM [BlueCollarScheduledJob] WHERE [Id] = @Id;",
                new { Id = id },
                transaction,
                null,
                null);
        }

        /// <summary>
        /// Deletes the worker record with the given ID.
        /// </summary>
        /// <param name="id">The ID of the worker to delete.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        public void DeleteWorker(long id, IDbTransaction transaction)
        {
            const string Sql =
@"DELETE FROM [BlueCollarWorking]
WHERE
    [WorkerId] = @Id;

DELETE FROM [BlueCollarHistory]
WHERE
    [WorkerId] = @Id;

DELETE FROM [BlueCollarWorker]
WHERE
    [Id] = @Id;";

            this.connection.Execute(
                Sql,
                new { Id = id },
                transaction,
                null,
                null);
        }

        /// <summary>
        /// Deletes the working record with the given ID.
        /// </summary>
        /// <param name="id">The ID of the working record to delete.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        public void DeleteWorking(long id, IDbTransaction transaction)
        {
            this.connection.Execute(
                "DELETE FROM [BlueCollarWorking] WHERE [Id] = @Id;",
                new { Id = id },
                transaction,
                null,
                null);
        }

        /// <summary>
        /// Disposes of resoures used by this instance.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets a set of counts for the given application.
        /// </summary>
        /// <param name="applicationName">The name of the application to get counts for.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A set of counts.</returns>
        public CountsRecord GetCounts(string applicationName, IDbTransaction transaction)
        {
            using (var multi = this.connection.QueryMultiple(CountsSql, new { ApplicationName = applicationName }, transaction, null, null))
            {
                return CreateCounts(multi);
            }
        }

        /// <summary>
        /// Gets a history details record.
        /// </summary>
        /// <param name="id">The ID of the record to get.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A history details record.</returns>
        public HistoryDetailsRecord GetHistoryDetails(long id, IDbTransaction transaction)
        {
            const string Sql =
@"SELECT 
    h.[Id],
    h.[Data], 
    h.[Exception], 
    h.[QueuedOn],
    w.[MachineAddress] AS [WorkerMachineAddress],
    w.[MachineName] AS [WorkerMachineName],
    w.[Name] AS [WorkerName]
FROM [BlueCollarHistory] h
    INNER JOIN [BlueCollarWorker] w ON h.[WorkerId] = w.[Id]
WHERE
    h.[Id] = @Id;";

            return this.connection.Query<HistoryDetailsRecord>(
                Sql,
                new { Id = id },
                transaction,
                true,
                null,
                null).FirstOrDefault();
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
            StringBuilder cb = new StringBuilder(
@"SELECT CAST(COUNT(h.[Id]) AS bigint)
FROM [BlueCollarHistory] h
    LEFT OUTER JOIN [BlueCollarSchedule] s ON h.[ScheduleId] = s.[Id]
WHERE
    h.[ApplicationName] = @ApplicationName");

            StringBuilder sb = new StringBuilder(
@"SELECT 
    h.[Id], 
    h.[QueueName],
    h.[JobName],
    h.[JobType],
    h.[QueuedOn],
    h.[TryNumber],
    h.[StartedOn],
    h.[Status],
    h.[FinishedOn],
    s.[Name] AS [ScheduleName]
FROM [BlueCollarHistory] h
    LEFT OUTER JOIN [BlueCollarSchedule] s ON h.[ScheduleId] = s.[Id]
WHERE
    h.[ApplicationName] = @ApplicationName");

            if (!string.IsNullOrEmpty(search))
            {
                const string Search = @"
    AND
    (
        h.[QueueName] LIKE @Search
        OR h.[JobName] LIKE @Search
        OR h.[JobType] LIKE @Search
        OR h.[Status] LIKE @Search
        OR s.[Name] LIKE @Search
    )";

                cb.Append(Search);
                sb.Append(Search);
            }

            cb.Append(";\n\n");

            sb.Append("\n");
            sb.Append(
@"ORDER BY h.[QueuedOn] DESC, h.[JobName] ASC, h.[TryNumber] DESC
LIMIT @Limit OFFSET @Offset;");

            sb.Append("\n");
            sb.Append(CountsSql);

            var p = new
            {
                ApplicationName = applicationName,
                Search = !string.IsNullOrEmpty(search) ? string.Concat("%", search, "%") : null,
                Limit = limit,
                Offset = offset
            };

            var list = new RecordList<HistoryListRecord>();

            using (var multi = this.connection.QueryMultiple(cb.ToString() + sb.ToString(), p, transaction, null, null))
            {
                list.SetPaging(multi.Read<long>().First(), limit, offset);

                foreach (var record in multi.Read<HistoryListRecord>())
                {
                    list.Records.Add(record);
                }

                list.Counts = CreateCounts(multi);
            }

            return list;
        }

        /// <summary>
        /// Gets a queued record for the given application and queue names.
        /// </summary>
        /// <param name="applicationName">The name of the application to get the queued record for.</param>
        /// <param name="queueFilters">The queue filters to use when filtering the queues to read from.</param>
        /// <param name="queuedBefore">The date to filter on.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A queued record, or null if none was found.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "IRepository methods are designed to be called by validated code paths only.")]
        public QueueRecord GetQueued(string applicationName, QueueNameFilters queueFilters, DateTime queuedBefore, IDbTransaction transaction)
        {
            StringBuilder sb = new StringBuilder(
@"SELECT *
FROM [BlueCollarQueue]
WHERE
    [ApplicationName] = @ApplicationName
    AND [QueuedOn] <= @QueuedBefore");

            if (!queueFilters.IncludesAllQueues)
            {
                if (queueFilters.Include.Count() > 0)
                {
                    sb.Append("\n    AND [QueueName] IN @IncludeQueueNames");
                }

                if (queueFilters.Exclude.Count() > 0)
                {
                    sb.Append("\n    AND [QueueName] NOT IN @ExcludeQueueNames");
                }
            }

            sb.Append("\n");
            sb.Append(
@"ORDER BY [QueuedOn] ASC
LIMIT 1;");

            return this.connection.Query<QueueRecord>(
                sb.ToString(),
                new
                {
                    ApplicationName = applicationName,
                    ExcludeQueueNames = queueFilters.Exclude.ToArray(),
                    IncludeQueueNames = queueFilters.Include.ToArray(),
                    QueuedBefore = queuedBefore
                },
                transaction,
                true,
                null,
                null).FirstOrDefault();
        }

        /// <summary>
        /// Gets a queued details record.
        /// </summary>
        /// <param name="id">The ID of the record to get.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A queued details record.</returns>
        public QueueDetailsRecord GetQueuedDetails(long id, IDbTransaction transaction)
        {
            const string Sql =
@"SELECT 
    q.[Id],
    q.[QueueName],
    q.[JobName],
    q.[JobType],
    q.[Data],
    q.[QueuedOn],
    q.[TryNumber], 
    s.[Name] AS [ScheduleName]
FROM [BlueCollarQueue] q
    LEFT OUTER JOIN [BlueCollarSchedule] s ON q.[ScheduleId] = s.[Id]
WHERE
    q.[Id] = @Id;";

            return this.connection.Query<QueueDetailsRecord>(
                Sql,
                new { Id = id },
                transaction,
                true,
                null,
                null).FirstOrDefault();
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
            StringBuilder cb = new StringBuilder(
@"SELECT CAST(COUNT(q.[Id]) AS bigint)
FROM [BlueCollarQueue] q
    LEFT OUTER JOIN [BlueCollarSchedule] s ON q.[ScheduleId] = s.[Id]
WHERE
    q.[ApplicationName] = @ApplicationName");

            StringBuilder sb = new StringBuilder(
@"SELECT 
    q.[Id],
    q.[QueueName],
    q.[JobName],
    q.[JobType],
    q.[QueuedOn],
    q.[TryNumber], 
    s.[Name] AS [ScheduleName]
FROM [BlueCollarQueue] q
    LEFT OUTER JOIN [BlueCollarSchedule] s ON q.[ScheduleId] = s.[Id]
WHERE
    q.[ApplicationName] = @ApplicationName");

            if (!string.IsNullOrEmpty(search))
            {
                const string Search = @"
    AND
    (
        q.[QueueName] LIKE @Search
        OR q.[JobName] LIKE @Search
        OR q.[JobType] LIKE @Search
        OR s.[Name] LIKE @Search
    )";

                cb.Append(Search);
                sb.Append(Search);
            }

            cb.Append(";\n\n");

            sb.Append("\n");
            sb.Append(
@"ORDER BY q.[QueuedOn] ASC, q.[JobName] ASC
LIMIT @Limit OFFSET @Offset;");

            sb.Append("\n");
            sb.Append(CountsSql);

            var p = new
            {
                ApplicationName = applicationName,
                Search = !string.IsNullOrEmpty(search) ? string.Concat("%", search, "%") : null,
                Limit = limit,
                Offset = offset
            };

            var list = new RecordList<QueueListRecord>();

            using (var multi = this.connection.QueryMultiple(cb.ToString() + sb.ToString(), p, transaction, null, null))
            {
                list.SetPaging(multi.Read<long>().First(), limit, offset);

                foreach (var record in multi.Read<QueueListRecord>())
                {
                    list.Records.Add(record);
                }

                list.Counts = CreateCounts(multi);
            }

            return list;
        }

        /// <summary>
        /// Gets the schedule with the given ID, NOT including its related scheduled jobs.
        /// </summary>
        /// <param name="id">The ID of the schedule to get.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>The schedule, or null if none was found.</returns>
        public ScheduleRecord GetSchedule(long id, IDbTransaction transaction)
        {
            const string Sql =
@"SELECT *
FROM [BlueCollarSchedule]
WHERE
    [Id] = @Id;";

            return this.connection.Query<ScheduleRecord>(
                Sql,
                new { Id = id },
                transaction,
                true,
                null,
                null).FirstOrDefault();
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
            const string Sql = @"SELECT CAST(COUNT([Id]) AS bigint)
FROM [BlueCollarHistory]
WHERE
    [ScheduleId] = @ScheduleId
    AND datetime([QueuedOn]) = datetime(@ScheduleDate);

SELECT CAST(COUNT([Id]) AS bigint)
FROM [BlueCollarQueue]
WHERE
    [ScheduleId] = @ScheduleId
    AND datetime([QueuedOn]) = datetime(@ScheduleDate);

SELECT CAST(COUNT([Id]) AS bigint)
FROM [BlueCollarWorking]
WHERE
    [ScheduleId] = @ScheduleId
    AND datetime([QueuedOn]) = datetime(@ScheduleDate);";

            long count = 0;

            using (var multi = this.connection.QueryMultiple(Sql, new { ScheduleId = scheduleId, ScheduleDate = scheduleDate }, transaction, null, null))
            {
                count = multi.Read<long>().First()
                    + multi.Read<long>().First()
                    + multi.Read<long>().First();
            }

            return count > 0;
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
            StringBuilder cb = new StringBuilder(
@"SELECT CAST(COUNT(j.[Id]) AS bigint)
FROM [BlueCollarScheduledJob] j
WHERE
    j.[ScheduleId] = @Id");

            StringBuilder sb = new StringBuilder(
@"SELECT s.[Id], s.[Name], j.*
FROM [BlueCollarSchedule] s
    LEFT OUTER JOIN [BlueCollarScheduledJob] j ON s.[Id] = j.[ScheduleId]");

            if (!string.IsNullOrEmpty(search))
            {
                cb.Append("\n    AND j.[JobType] LIKE @Search");
                sb.Append(" AND j.[JobType] LIKE @Search");
            }

            cb.Append(";\n\n");
            sb.Append(@"
WHERE
    s.[Id] = @Id
ORDER BY s.[Name], j.[JobType] ASC
LIMIT @Limit OFFSET @Offset;");

            sb.Append("\n");
            sb.Append(CountsSql);

            var p = new
            {
                ApplicationName = applicationName,
                Id = id,
                Search = !string.IsNullOrEmpty(search) ? string.Concat("%", search, "%") : null,
                Limit = limit,
                Offset = offset
            };

            ScheduledJobRecordList list = new ScheduledJobRecordList();

            using (var multi = this.connection.QueryMultiple(cb.ToString() + sb.ToString(), p, transaction, null, null))
            {
                bool readSchedule = false;
                list.SetPaging(multi.Read<long>().First(), limit, offset);

                var records = multi.Read<ScheduleRecord, ScheduledJobRecord, ScheduledJobRecord>(
                    (s, sj) =>
                    {
                        if (!readSchedule)
                        {
                            list.Id = s.Id.Value;
                            list.Name = s.Name;
                        }

                        return sj;
                    },
                    "Id").Where(r => r != null);

                foreach (var record in records)
                {
                    list.Records.Add(record);
                }

                list.Counts = CreateCounts(multi);
            }

            return list;
        }

        /// <summary>
        /// Attempts to obtain the enqueueing lock for the given schedule ID.
        /// </summary>
        /// <param name="scheduleId">The ID of the schedule to obtain the schedule enqueueing lock for.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>True if the enqueueing lock was obtained, false otherwise.</returns>
        public bool GetScheduleEnqueueingLock(long scheduleId, IDbTransaction transaction)
        {
            const string SelectSql =
@"SELECT [Enqueueing] 
FROM [BlueCollarSchedule] 
WHERE 
    [Id] = @Id;";

            const string UpdateSql =
@"UPDATE [BlueCollarSchedule] 
SET 
    [Enqueueing] = @Enqueueing,
    [EnqueueingUpdatedOn] = @EnqueueingUpdatedOn
WHERE 
    [Id] = @Id;";

            bool obtained = false, commitRollback = false;

            if (transaction == null)
            {
                transaction = this.BeginTransaction(IsolationLevel.Serializable);
                commitRollback = true;
            }

            try
            {
                obtained = !this.connection.Query<bool>(
                     SelectSql,
                     new { Id = scheduleId },
                     transaction,
                     true,
                     null,
                     null).First();

                if (obtained)
                {
                    this.connection.Execute(
                        UpdateSql,
                        new { Id = scheduleId, Enqueueing = true, EnqueueingUpdatedOn = DateTime.UtcNow },
                        transaction,
                        null,
                        null);
                }

                if (commitRollback)
                {
                    transaction.Commit();
                }
            }
            catch
            {
                if (commitRollback)
                {
                    transaction.Rollback();
                }

                throw;
            }

            return obtained;
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
            StringBuilder cb = new StringBuilder(
@"SELECT CAST(COUNT(DISTINCT s.[Id]) AS bigint)
FROM [BlueCollarSchedule] s
    LEFT OUTER JOIN [BlueCollarScheduledJob] j ON s.[Id] = j.[ScheduleId]
WHERE
    s.[ApplicationName] = @ApplicationName");

            StringBuilder sb = new StringBuilder(
@"SELECT DISTINCT s.*,
(
    SELECT CAST(COUNT(sj.[Id]) AS bigint)
    FROM [BlueCollarScheduledJob] sj
    WHERE
        sj.[ScheduleId] = s.[Id]
) AS [JobCount]
FROM [BlueCollarSchedule] s
    LEFT OUTER JOIN [BlueCollarScheduledJob] j ON s.[Id] = j.[ScheduleId]
WHERE
    s.[ApplicationName] = @ApplicationName");

            if (!string.IsNullOrEmpty(search))
            {
                const string Search = @"
    AND
    (
        s.[QueueName] LIKE @Search
        OR s.[Name] LIKE @Search
        OR s.[RepeatType] LIKE @Search
        OR j.[JobType] LIKE @Search
    )";

                cb.Append(Search);
                sb.Append(Search);
            }

            cb.Append(";\n\n");

            sb.Append("\n");
            sb.Append(
@"ORDER BY s.[Name] ASC
LIMIT @Limit OFFSET @Offset;");

            sb.Append("\n");
            sb.Append(CountsSql);

            var p = new
            {
                ApplicationName = applicationName,
                Search = !string.IsNullOrEmpty(search) ? string.Concat("%", search, "%") : null,
                Limit = limit,
                Offset = offset
            };

            var list = new RecordList<ScheduleListRecord>();

            using (var multi = this.connection.QueryMultiple(cb.ToString() + sb.ToString(), p, transaction, null, null))
            {
                list.SetPaging(multi.Read<long>().First(), limit, offset);

                foreach (var record in multi.Read<ScheduleListRecord>())
                {
                    list.Records.Add(record);
                }

                list.Counts = CreateCounts(multi);
            }

            return list;
        }

        /// <summary>
        /// Gets a collection of schedules and their related scheduled jobs for the given application name.
        /// </summary>
        /// <param name="applicationName">The name of the application to get schedules for.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A collection of schedules.</returns>
        public IEnumerable<ScheduleRecord> GetSchedules(string applicationName, IDbTransaction transaction)
        {
            const string Sql =
@"SELECT s.*, sj.*
FROM [BlueCollarSchedule] s
    LEFT OUTER JOIN [BlueCollarScheduledJob] sj ON s.[Id] = sj.[ScheduleId]
WHERE
    s.[ApplicationName] = @ApplicationName;";

            Dictionary<long, ScheduleRecord> lookup = new Dictionary<long, ScheduleRecord>();
            List<ScheduleRecord> schedules = new List<ScheduleRecord>();

            this.connection.Query<ScheduleRecord, ScheduledJobRecord, ScheduleRecord>(
                Sql,
                (s, sj) => 
                {
                    ScheduleRecord schedule;

                    if (!lookup.ContainsKey(s.Id.Value))
                    {
                        lookup[s.Id.Value] = s;
                        schedules.Add(s);
                        schedule = s;
                    }
                    else
                    {
                        schedule = lookup[s.Id.Value];
                    }

                    if (sj != null) 
                    { 
                        schedule.ScheduledJobs.Add(sj);
                        sj.Schedule = schedule; 
                    }

                    return schedule; 
                },
                new { ApplicationName = applicationName },
                transaction,
                true,
                "Id",
                null,
                null);

            return schedules;
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
            const string HistoryStatusSql =
@"SELECT CAST(COUNT([Id]) AS bigint)
FROM [BlueCollarHistory]
WHERE
    [ApplicationName] = @ApplicationName
    AND [FinishedOn] > {0}
    AND [FinishedOn] <= @End;

SELECT CAST(COUNT([Id]) AS bigint)
FROM [BlueCollarHistory]
WHERE
    [ApplicationName] = @ApplicationName
    AND [FinishedOn] > {0}
    AND [FinishedOn] <= @End
    AND [Status] = @Succeeded;

SELECT CAST(COUNT([Id]) AS bigint)
FROM [BlueCollarHistory]
WHERE
    [ApplicationName] = @ApplicationName
    AND [FinishedOn] > {0}
    AND [FinishedOn] <= @End
    AND [Status] = @Failed;

SELECT CAST(COUNT([Id]) AS bigint)
FROM [BlueCollarHistory]
WHERE
    [ApplicationName] = @ApplicationName
    AND [FinishedOn] > {0}
    AND [FinishedOn] <= @End
    AND [Status] = @Canceled;

SELECT CAST(COUNT([Id]) AS bigint)
FROM [BlueCollarHistory]
WHERE
    [ApplicationName] = @ApplicationName
    AND [FinishedOn] > {0}
    AND [FinishedOn] <= @End
    AND [Status] = @TimedOut;

SELECT CAST(COUNT([Id]) AS bigint)
FROM [BlueCollarHistory]
WHERE
    [ApplicationName] = @ApplicationName
    AND [FinishedOn] > {0}
    AND [FinishedOn] <= @End
    AND [Status] = @Interrupted;";

            // QueueName needs "AS QueueName" for some reason; the projection is naming the column
            // key as literally "[QueueName]" (meaning, with brackets) otherwise.
            const string JobsPerHourSql =
@"SELECT
	datetime([Day]) AS [Day], 
	[QueueName] AS [QueueName],
	CAST(AVG([Count]) AS bigint) AS [JobsPerHour]
FROM
(
	SELECT 
		date([Hour], 'start of day') AS [Day],
		[QueueName],
		[Count]
	FROM
	(
		SELECT 
			[Hour], 
			[QueueName],
			COUNT([Id]) AS [Count]
		FROM
		(
			SELECT 
				[Id],
				[QueueName],
				strftime('%Y-%m-%d %H:00:00', [FinishedOn]) AS [Hour]
			FROM [BlueCollarHistory]
            WHERE
                [ApplicationName] = @ApplicationName
                AND [FinishedOn] > @DistantBegin
                AND [FinishedOn] <= @End
		) t1
		GROUP BY [Hour], [QueueName]
	) t2
) t3
GROUP BY [Day], [QueueName];";

            const string JobsPerWorkerSql =
@"SELECT 
    w.[Name], 
    w.[MachineName], 
    w.[MachineAddress], 
    CAST(COUNT(h.[Id]) AS bigint) AS [Count]
FROM [BlueCollarHistory] h
    LEFT OUTER JOIN [BlueCollarWorker] w ON h.[WorkerId] = w.[Id]
WHERE
    h.[ApplicationName] = @ApplicationName
    AND h.[FinishedOn] > @DistantBegin
    AND h.[FinishedOn] <= @End
GROUP BY w.[Name], w.[MachineName], w.[MachineAddress];";

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(CultureInfo.InvariantCulture, HistoryStatusSql, "@DistantBegin");
            sb.Append("\n\n");
            sb.AppendFormat(CultureInfo.InvariantCulture, HistoryStatusSql, "@RecentBegin");
            sb.Append("\n\n");
            sb.Append(JobsPerHourSql);
            sb.Append("\n\n");
            sb.Append(JobsPerWorkerSql);
            sb.Append("\n\n");
            sb.Append(CountsSql);

            var p = new
            {
                ApplicationName = applicationName,
                DistantBegin = distantBeginDate,
                RecentBegin = recentBeginDate,
                End = endDate,
                Succeeded = HistoryStatus.Succeeded.ToString(),
                Failed = HistoryStatus.Failed.ToString(),
                Canceled = HistoryStatus.Canceled.ToString(),
                TimedOut = HistoryStatus.TimedOut.ToString(),
                Interrupted = HistoryStatus.Interrupted.ToString()
            };

            StatisticsRecord stats = new StatisticsRecord();

            using (var multi = this.connection.QueryMultiple(sb.ToString(), p, transaction, null, null))
            {
                stats.HistoryStatusDistant = CreateHistoryStatusCounts(multi);
                stats.HistoryStatusRecent = CreateHistoryStatusCounts(multi);

                foreach (var record in multi.Read<JobsPerHourByDayRecord>().OrderBy(r => r.Date))
                {
                    stats.JobsPerHourByDay.Add(record);
                }

                foreach (var record in multi.Read<JobsPerWorkerRecord>())
                {
                    stats.JobsPerWorker.Add(record);
                }

                stats.Counts = CreateCounts(multi);
            }

            return stats;
        }

        /// <summary>
        /// Gets the worker record with the given ID.
        /// </summary>
        /// <param name="id">The ID of the worker record to get.</param>
        /// <param name="transaction">The transaction to use.</param>
        /// <returns>A worker record.</returns>
        public WorkerRecord GetWorker(long id, IDbTransaction transaction)
        {
            const string Sql =
@"SELECT *
FROM [BlueCollarWorker]
WHERE
    [Id] = @Id;";

            return this.connection.Query<WorkerRecord>(
                Sql,
                new { Id = id },
                transaction,
                true,
                null,
                null).FirstOrDefault();
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
            StringBuilder cb = new StringBuilder(
@"SELECT CAST(COUNT([Id]) AS bigint)
FROM [BlueCollarWorker]
WHERE
    [ApplicationName] = @ApplicationName");

            StringBuilder sb = new StringBuilder(
@"SELECT *
FROM [BlueCollarWorker]
WHERE
    [ApplicationName] = @ApplicationName");

            if (!string.IsNullOrEmpty(search))
            {
                const string Search = @"
    AND
    (
        [Name] LIKE @Search
        OR [MachineName] LIKE @Search
        OR [MachineAddress] LIKE @Search
        OR [QueueNames] LIKE @Search
        OR [Status] LIKE @Search
        OR [Signal] LIKE @Search
        OR [Startup] LIKE @Search
    )";

                cb.Append(Search);
                sb.Append(Search);
            }

            cb.Append(";\n\n");

            sb.Append("\n");
            sb.Append(
@"ORDER BY [Name] ASC
LIMIT @Limit OFFSET @Offset;");

            sb.Append("\n");
            sb.Append(CountsSql);

            var p = new
            {
                ApplicationName = applicationName,
                Search = !string.IsNullOrEmpty(search) ? string.Concat("%", search, "%") : null,
                Limit = limit,
                Offset = offset
            };

            var list = new RecordList<WorkerRecord>();

            using (var multi = this.connection.QueryMultiple(cb.ToString() + sb.ToString(), p, transaction, null, null))
            {
                list.SetPaging(multi.Read<long>().First(), limit, offset);

                foreach (var record in multi.Read<WorkerRecord>())
                {
                    list.Records.Add(record);
                }

                list.Counts = CreateCounts(multi);
            }

            return list;
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
            const string Sql =
@"SELECT *
FROM [BlueCollarWorker]
WHERE
    [ApplicationName] = @ApplicationName
    AND ([MachineName] IS NULL OR [MachineName] = '' OR [MachineName] = @MachineName)
    AND ([MachineAddress] IS NULL OR [MachineAddress] = '' OR [MachineAddress] = @MachineAddress);";

            return this.connection.Query<WorkerRecord>(
                Sql,
                new { ApplicationName = applicationName, MachineName = machineName, MachineAddress = machineAddress },
                transaction,
                true,
                null,
                null);
        }

        /// <summary>
        /// Gets the working record with the given ID.
        /// </summary>
        /// <param name="id">The ID of the record to get.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A working record, or null if none was found.</returns>
        public WorkingRecord GetWorking(long id, IDbTransaction transaction)
        {
            const string Sql =
@"SELECT *
FROM [BlueCollarWorking]
WHERE
    [Id] = @Id;";

            return this.connection.Query<WorkingRecord>(
                Sql,
                new { Id = id },
                transaction,
                true,
                null,
                null).FirstOrDefault();
        }

        /// <summary>
        /// Gets a working details record.
        /// </summary>
        /// <param name="id">The ID of the record to get.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>A working details record, or null if none was found.</returns>
        public WorkingDetailsRecord GetWorkingDetails(long id, IDbTransaction transaction)
        {
            const string Sql =
@"SELECT 
    wg.[Id],
    wg.[JobName],
    wg.[JobType],
    wg.[Data],
    wg.[QueuedOn],
    wg.[QueueName],
    wg.[Signal],
    wg.[StartedOn],
    wg.[TryNumber],
    w.[Name] AS [WorkerName], 
    w.[MachineAddress] AS [WorkerMachineAddress], 
    w.[MachineName] AS [WorkerMachineName], 
    s.[Name] AS [ScheduleName]
FROM [BlueCollarWorking] wg
    INNER JOIN [BlueCollarWorker] w ON wg.[WorkerId] = w.[Id]
    LEFT OUTER JOIN [BlueCollarSchedule] s ON wg.[ScheduleId] = s.[Id]
WHERE
    wg.[Id] = @Id;";

            return this.connection.Query<WorkingDetailsRecord>(
                Sql,
                new { Id = id },
                transaction,
                true,
                null,
                null).FirstOrDefault();
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
            StringBuilder sb = new StringBuilder(
@"SELECT *
FROM [BlueCollarWorking]
WHERE
    [WorkerId] = @WorkerId");

            if (excludingId != null)
            {
                sb.Append("\n    AND [Id] != @ExcludingId");
            }

            sb.Append(";");

            return this.connection.Query<WorkingRecord>(
                sb.ToString(),
                new { WorkerId = workerId, ExcludingId = excludingId },
                transaction,
                true,
                null,
                null);
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
            StringBuilder cb = new StringBuilder(
@"SELECT CAST(COUNT(wg.[Id]) AS bigint)
FROM [BlueCollarWorking] wg
    INNER JOIN [BlueCollarWorker] w ON wg.[WorkerId] = w.[Id]
    LEFT OUTER JOIN [BlueCollarSchedule] s ON wg.[ScheduleId] = s.[Id]
WHERE
    wg.[ApplicationName] = @ApplicationName");

            StringBuilder sb = new StringBuilder(
@"SELECT 
    wg.[Id],
    wg.[JobName],
    wg.[JobType],
    wg.[QueuedOn],
    wg.[QueueName],
    wg.[Signal],
    wg.[StartedOn],
    wg.[TryNumber],
    w.[Name] AS [WorkerName], 
    w.[MachineAddress] AS [WorkerMachineAddress], 
    w.[MachineName] AS [WorkerMachineName], 
    s.[Name] AS [ScheduleName]
FROM [BlueCollarWorking] wg
    INNER JOIN [BlueCollarWorker] w ON wg.[WorkerId] = w.[Id]
    LEFT OUTER JOIN [BlueCollarSchedule] s ON wg.[ScheduleId] = s.[Id]
WHERE
    wg.[ApplicationName] = @ApplicationName");

            if (!string.IsNullOrEmpty(search))
            {
                const string Search = @"
    AND
    (
        wg.[QueueName] LIKE @Search
        OR wg.[JobType] LIKE @Search
        OR wg.[JobName] LIKE @Search
        OR w.[Name] LIKE @Search
        OR w.[MachineName] LIKE @Search
        OR s.[Name] LIKE @Search
    )";
                
                cb.Append(Search);
                sb.Append(Search);
            }

            cb.Append(";\n\n");

            sb.Append("\n");
            sb.Append(
@"ORDER BY [QueuedOn] DESC
LIMIT @Limit OFFSET @Offset;");

            sb.Append("\n");
            sb.Append(CountsSql);

            var p = new
            {
                ApplicationName = applicationName,
                Search = !string.IsNullOrEmpty(search) ? string.Concat("%", search, "%") : null,
                Limit = limit,
                Offset = offset
            };

            var list = new RecordList<WorkingListRecord>();

            using (var multi = this.connection.QueryMultiple(cb.ToString() + sb.ToString(), p, transaction, null, null))
            {
                list.SetPaging(multi.Read<long>().First(), limit, offset);
                
                foreach (WorkingListRecord record in multi.Read<WorkingListRecord>())
                {
                    list.Records.Add(record);
                }

                list.Counts = CreateCounts(multi);
            }

            return list;
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
            const string Sql =
@"SELECT w.[Signal] AS [WorkerSignal], w.[QueueNames], wg.[Signal] AS [WorkingSignal]
FROM [BlueCollarWorker] w
    LEFT OUTER JOIN [BlueCollarWorking] wg ON w.[Id] = wg.[WorkerId] AND wg.[Id] = @WorkingId
WHERE
    w.[Id] = @WorkerId;";

            return this.connection.Query<SignalsRecord>(
                Sql,
                new
                {
                    WorkerId = workerId,
                    WorkingId = workingId
                },
                transaction,
                true,
                null,
                null).FirstOrDefault();
        }

        /// <summary>
        /// Releases the enqueueing lock for the schedule with the given ID.
        /// </summary>
        /// <param name="scheduleId">The ID of the schedule to release the enqueuing lock for.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        public void ReleaseScheduleEnqueueingLock(long scheduleId, IDbTransaction transaction)
        {
            const string Sql =
@"UPDATE [BlueCollarSchedule]
SET
    [Enqueueing] = @Enqueueing,
    [EnqueueingUpdatedOn] = @EnqueueingUpdatedOn
WHERE
    [Id] = @Id;";

            this.connection.Execute(
                Sql,
                new { Id = scheduleId, Enqueueing = false, EnqueueingUpdatedOn = DateTime.UtcNow },
                transaction,
                null,
                null);
        }

        /// <summary>
        /// Signals all workers for the given application name.
        /// </summary>
        /// <param name="applicationName">The application name to signal workers for.</param>
        /// <param name="signal">The signal to set.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        public void SignalWorkers(string applicationName, WorkerSignal signal, IDbTransaction transaction)
        {
            const string Sql =
@"UPDATE [BlueCollarWorker]
SET
    [Signal] = @Signal
WHERE
    [ApplicationName] = @ApplicationName;";

            this.connection.Execute(
                Sql,
                new { ApplicationName = applicationName, Signal = signal.ToString() },
                transaction,
                null,
                null);
        }

        /// <summary>
        /// Updates the given schedule.
        /// </summary>
        /// <param name="record">The schedule record to update.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>The updated record.</returns>
        public ScheduleRecord UpdateSchedule(ScheduleRecord record, IDbTransaction transaction)
        {
            const string Sql =
@"UPDATE [BlueCollarSchedule]
SET
    [QueueName] = @QueueName,
    [Name] = @Name,
    [StartOn] = @StartOn,
    [EndOn] = @EndOn,
    [RepeatType] = @RepeatTypeString,
    [Repeatvalue] = @RepeatValue,
    [Enabled] = @Enabled,
    [Enqueueing] = @Enqueueing,
    [EnqueueingUpdatedOn] = @EnqueueingUpdatedOn
WHERE
    [Id] = @Id;";

            this.connection.Execute(
                Sql,
                record,
                transaction,
                null,
                null);

            return record;
        }

        /// <summary>
        /// Updates the given scheduled job.
        /// </summary>
        /// <param name="record">The scheduled job record to update.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>The updated record.</returns>
        public ScheduledJobRecord UpdateScheduledJob(ScheduledJobRecord record, IDbTransaction transaction)
        {
            const string Sql =
@"UPDATE [BlueCollarScheduledJob]
SET
    [JobType] = @JobType,
    [Properties] = @Properties
WHERE
    [Id] = @Id;";

            this.connection.Execute(
                Sql,
                record,
                transaction,
                null,
                null);

            return record;
        }

        /// <summary>
        /// Updates the given worker.
        /// </summary>
        /// <param name="record">The worker record to update.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>The updated worker.</returns>
        public WorkerRecord UpdateWorker(WorkerRecord record, IDbTransaction transaction)
        {
            const string Sql =
@"UPDATE [BlueCollarWorker]
SET
    [Name] = @Name,
    [MachineName] = @MachineName,
    [MachineAddress] = @MachineAddress,
    [QueueNames] = @QueueNames,
    [Status] = @StatusString,
    [Signal] = @SignalString,
    [Startup] = @StartupString,
    [UpdatedOn] = @UpdatedOn
WHERE
    [Id] = @Id;";

            this.connection.Execute(
                Sql,
                record,
                transaction,
                null,
                null);

            return record;
        }

        /// <summary>
        /// Updates the status of the worker with the given ID.
        /// </summary>
        /// <param name="id">The ID of the worker to update status for.</param>
        /// <param name="status">The status to update.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        public void UpdateWorkerStatus(long id, WorkerStatus status, IDbTransaction transaction)
        {
            const string Sql =
@"UPDATE [BlueCollarWorker]
SET
    [Status] = @Status,
    [UpdatedOn] = @Now
WHERE
    [Id] = @Id;";

            this.connection.Execute(
                Sql,
                new
                {
                    Id = id,
                    Now = DateTime.UtcNow,
                    Status = status.ToString()
                },
                transaction,
                null,
                null);
        }

        /// <summary>
        /// Updates the given working record.
        /// </summary>
        /// <param name="record">The working record to update.</param>
        /// <param name="transaction">The transaction to use, if applicable.</param>
        /// <returns>The updated working record.</returns>
        public WorkingRecord UpdateWorking(WorkingRecord record, IDbTransaction transaction)
        {
            const string Sql =
@"UPDATE [BlueCollarWorking]
SET
    [WorkerId] = @WorkerId,
    [ScheduleId] = @ScheduleId,
    [QueueName] = @QueueName,
    [JobName] = @JobName,
    [JobType] = @JobType,
    [Data] = @Data,
    [QueuedOn] = @QueuedOn,
    [TryNumber] = @TryNumber,
    [StartedOn] = @StartedOn,
    [Signal] = @Signal
WHERE
    [Id] = @Id;";

            this.connection.Execute(
                Sql,
                record,
                transaction,
                null,
                null);

            return record;
        }

        /// <summary>
        /// Ensures that the SQLite database at the given path exists, creating it if it doesn't.
        /// </summary>
        /// <param name="path">The path of the SQLite database to ensure.</param>
        protected static void EnsureDatabase(string path)
        {
            if (!File.Exists(path))
            {
                string sql;
                Stream stream = null;

                try
                {
                    stream = typeof(SQLiteRepository).Assembly.GetManifestResourceStream("BlueCollar.SQLite.Schema.sql");

                    using (StreamReader reader = new StreamReader(stream))
                    {
                        stream = null;
                        sql = reader.ReadToEnd();
                    }
                }
                finally
                {
                    if (stream != null)
                    {
                        stream.Dispose();
                    }
                }

                using (SQLiteConnection connection = new SQLiteConnection(string.Concat("data source=", path, ";journal mode=Off;synchronous=Off;version=3")))
                {
                    connection.Open();
                    connection.Execute(sql, null);
                }
            }
        }

        /// <summary>
        /// Disposes of resources used by this instance.
        /// </summary>
        /// <param name="disposing">A value indicating whether to dispose of managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (this.connection != null)
                {
                    this.connection.Dispose();
                    this.connection = null;
                }

                this.disposed = true;
            }
        }

        /// <summary>
        /// Creates a new <see cref="CountsRecord"/> by reading from the given multi-mapped reader.
        /// </summary>
        /// <param name="multi">The multi-mapped reader to read from.</param>
        /// <returns>A new <see cref="CountsRecord"/>.</returns>
        private static CountsRecord CreateCounts(SqlMapper.GridReader multi)
        {
            return new CountsRecord()
            {
                HistoryCount = multi.Read<long>().FirstOrDefault(),
                QueueCount = multi.Read<long>().FirstOrDefault(),
                ScheduleCount = multi.Read<long>().FirstOrDefault(),
                WorkerCount = multi.Read<long>().FirstOrDefault(),
                WorkingCount = multi.Read<long>().FirstOrDefault()
            };
        }

        /// <summary>
        /// Creates a new <see cref="HistoryStatusCountsRecord"/> by reading from the given multi-mapped reader.
        /// </summary>
        /// <param name="multi">The multi-mapped reader to read from.</param>
        /// <returns>A new <see cref="HistoryStatusCountsRecord"/>.</returns>
        private static HistoryStatusCountsRecord CreateHistoryStatusCounts(SqlMapper.GridReader multi)
        {
            return new HistoryStatusCountsRecord()
            {
                TotalCount = multi.Read<long>().First(),
                SucceededCount = multi.Read<long>().First(),
                FailedCount = multi.Read<long>().First(),
                CanceledCount = multi.Read<long>().First(),
                TimedOutCount = multi.Read<long>().First(),
                InterruptedCount = multi.Read<long>().First()
            };
        }
    }
}
