IF NOT EXISTS (SELECT * FROM sys.columns WHERE [object_id] = OBJECT_ID(N'[BlueCollarSchedule]') AND [name] = 'Locked')
BEGIN
	BEGIN TRANSACTION
	
	CREATE TABLE #S
	(
		[Id] bigint NOT NULL,
		[ApplicationName] varchar(64) NOT NULL,
		[QueueName] varchar(24) NULL,
		[Name] varchar(24) NOT NULL,
		[StartOn] datetime NOT NULL,
		[EndOn] datetime NULL,
		[RepeatType] varchar(12) NOT NULL,
		[RepeatValue] bigint NULL,
		[Enabled] bit NOT NULL,
		[Enqueueing] bit NOT NULL,
		[EnqueueingUpdatedOn] datetime NULL
	)

	INSERT INTO #S
	SELECT *
	FROM [BlueCollarSchedule]

	DROP TABLE [BlueCollarSchedule]

	CREATE TABLE [BlueCollarSchedule]
	(
		[Id] bigint NOT NULL PRIMARY KEY IDENTITY(1, 1),
		[ApplicationName] varchar(64) NOT NULL,
		[QueueName] varchar(24) NULL,
		[Name] varchar(24) NOT NULL,
		[StartOn] datetime NOT NULL,
		[EndOn] datetime NULL,
		[RepeatType] varchar(12) NOT NULL,
		[RepeatValue] bigint NULL,
		[Enabled] bit NOT NULL,
		[Locked] bit NOT NULL,
		[LockedUpdatedOn] datetime NULL
	)
	
	SET IDENTITY_INSERT [BlueCollarSchedule] ON
	
	INSERT INTO [BlueCollarSchedule]
	SELECT *
	FROM #S
	
	SET IDENTITY_INSERT [BlueCollarSchedule] OFF
	
	DROP TABLE #S
	
	COMMIT
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE [object_id] = OBJECT_ID(N'[BlueCollarQueue]') AND [name] = 'Locked')
BEGIN
	BEGIN TRANSACTION
	
	CREATE TABLE #Q
	(
		[Id] bigint NOT NULL,
		[ApplicationName] varchar(64) NOT NULL,
		[ScheduleId] bigint NULL,
		[QueueName] varchar(24) NULL,
		[JobName] varchar(64) NOT NULL,
		[JobType] varchar(256) NOT NULL,
		[Data] text NOT NULL,
		[QueuedOn] datetime NOT NULL,
		[TryNumber] bigint NOT NULL
	)
	
	INSERT INTO #Q
	SELECT *
	FROM [BlueCollarQueue]
	
	DROP TABLE [BlueCollarQueue]
	
	CREATE TABLE [BlueCollarQueue]
	(
		[Id] bigint NOT NULL PRIMARY KEY IDENTITY(1, 1),
		[ApplicationName] varchar(64) NOT NULL,
		[ScheduleId] bigint NULL,
		[QueueName] varchar(24) NULL,
		[JobName] varchar(64) NOT NULL,
		[JobType] varchar(256) NOT NULL,
		[Data] text NOT NULL,
		[QueuedOn] datetime NOT NULL,
		[TryNumber] bigint NOT NULL,
		[Locked] bit NOT NULL,
		[LockedUpdatedOn] datetime NULL
	)
	
	SET IDENTITY_INSERT [BlueCollarQueue] ON
	
	INSERT INTO [BlueCollarQueue]
	SELECT
		[Id],
		[ApplicationName],
		[ScheduleId],
		[QueueName],
		[JobName],
		[JobType],
		[Data],
		[QueuedOn],
		[TryNumber],
		0,
		NULL
	FROM #Q
	
	SET IDENTITY_INSERT [BlueCollarQueue] OFF
	
	DROP TABLE #Q
	
	COMMIT
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE [object_id] = OBJECT_ID(N'[BlueCollarWorker]') AND [name] = 'Locked')
BEGIN
	BEGIN TRANSACTION
	
	CREATE TABLE #W
	(
		[Id] bigint NOT NULL,
		[ApplicationName] varchar(64) NOT NULL,
		[Name] varchar(64) NOT NULL,
		[MachineName] varchar(128) NULL,
		[MachineAddress] varchar(64) NULL,
		[QueueNames] text NULL,
		[Status] varchar(12) NOT NULL,
		[Signal] varchar(24) NULL,
		[Startup] varchar(12) NOT NULL,
		[UpdatedOn] datetime NOT NULL
	)
	
	INSERT INTO #W
	SELECT * FROM [BlueCollarWorker]
	
	DROP TABLE [BlueCollarWorker]
	
	CREATE TABLE [BlueCollarWorker]
	(
		[Id] bigint NOT NULL PRIMARY KEY IDENTITY(1, 1),
		[ApplicationName] varchar(64) NOT NULL,
		[Name] varchar(64) NOT NULL,
		[MachineName] varchar(128) NULL,
		[MachineAddress] varchar(64) NULL,
		[QueueNames] text NULL,
		[Status] varchar(12) NOT NULL,
		[Signal] varchar(24) NULL,
		[Startup] varchar(12) NOT NULL,
		[UpdatedOn] datetime NOT NULL,
		[Locked] bit NOT NULL,
		[LockedUpdatedOn] datetime NULL
	)
	
	SET IDENTITY_INSERT [BlueCollarWorker] ON
	
	INSERT INTO [BlueCollarWorker]
	SELECT
		[Id],
		[ApplicationName],
		[Name],
		[MachineName],
		[MachineAddress],
		[QueueNames],
		[Status],
		[Signal],
		[Startup],
		[UpdatedOn],
		0,
		NULL
	FROM #W
	
	SET IDENTITY_INSERT [BlueCollarWorker] OFF
	
	DROP TABLE #W

	COMMIT
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE [object_id] = OBJECT_ID(N'[BlueCollarWorking]') AND [name] = 'Locked')
BEGIN
	BEGIN TRANSACTION
	
	CREATE TABLE #W
	(
		[Id] bigint NOT NULL,
		[ApplicationName] varchar(64) NOT NULL,
		[WorkerId] bigint NOT NULL,
		[ScheduleId] bigint NULL,
		[QueueName] varchar(24) NULL,
		[JobName] varchar(64) NOT NULL,
		[JobType] varchar(256) NOT NULL,
		[Data] text NOT NULL,
		[QueuedOn] datetime NOT NULL,
		[TryNumber] bigint NOT NULL,
		[StartedOn] datetime NOT NULL,
		[Signal] varchar(12) NULL
	)
	
	INSERT INTO #W
	SELECT *
	FROM [BlueCollarWorking]
	
	DROP TABLE [BlueCollarWorking]

	CREATE TABLE [BlueCollarWorking]
	(
		[Id] bigint NOT NULL PRIMARY KEY IDENTITY(1, 1),
		[ApplicationName] varchar(64) NOT NULL,
		[WorkerId] bigint NOT NULL,
		[ScheduleId] bigint NULL,
		[QueueName] varchar(24) NULL,
		[JobName] varchar(64) NOT NULL,
		[JobType] varchar(256) NOT NULL,
		[Data] text NOT NULL,
		[QueuedOn] datetime NOT NULL,
		[TryNumber] bigint NOT NULL,
		[StartedOn] datetime NOT NULL,
		[Signal] varchar(12) NULL,
		[Locked] bit NOT NULL,
		[LockedUpdatedOn] datetime NULL
	)
	
	SET IDENTITY_INSERT [BlueCollarWorking] ON
	
	INSERT INTO [BlueCollarWorking]
	SELECT
		[Id],
		[ApplicationName],
		[WorkerId],
		[ScheduleId],
		[QueueName],
		[JobName],
		[JobType],
		[Data],
		[QueuedOn],
		[TryNumber],
		[StartedOn],
		[Signal],
		0,
		NULL
	FROM #W
	
	SET IDENTITY_INSERT [BlueCollarWorking] OFF
	
	DROP TABLE #W
	
	COMMIT
END
GO