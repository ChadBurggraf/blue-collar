IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[BlueCollarHistory]'))
	DROP TABLE [BlueCollarHistory]
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[BlueCollarWorking]'))
	DROP TABLE [BlueCollarWorking]
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[BlueCollarWorker]'))
	DROP TABLE [BlueCollarWorker]
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[BlueCollarQueue]'))
	DROP TABLE [BlueCollarQueue]
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[BlueCollarScheduledJob]'))
	DROP TABLE [BlueCollarScheduledJob]
GO

IF EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[BlueCollarSchedule]'))
	DROP TABLE [BlueCollarSchedule]
GO

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
	[Enqueueing] bit NOT NULL,
	[EnqueueingUpdatedOn] datetime NULL
)
GO

CREATE TABLE [BlueCollarScheduledJob]
(
	[Id] bigint NOT NULL PRIMARY KEY IDENTITY(1, 1),
	[ScheduleId] bigint NOT NULL,
	[Number] bigint NOT NULL,
	[JobType] varchar(256) NOT NULL,
	[Data] text NULL
)
GO

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
	[TryNumber] bigint NOT NULL
)
GO

CREATE INDEX [IX_BlueCollarHistory_QueuedOn_JobName_TryNumber_ApplicationName] 
ON [BlueCollarHistory] 
(
	[QueuedOn] DESC,
	[JobName] ASC,
	[TryNumber] DESC,
	[ApplicationName] ASC
)
INCLUDE 
( 
	[Id],
	[ScheduleId],
	[QueueName],
	[JobType],
	[StartedOn],
	[Status],
	[FinishedOn]
)
GO 

CREATE INDEX [IX_BlueCollarHistory_FinishedOn_ApplicationName_Status] 
ON [BlueCollarHistory] 
(
	[FinishedOn] ASC,
	[ApplicationName] ASC,
	[Status] ASC
)
INCLUDE
(
	[WorkerId],
	[QueueName]
)
GO

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
	[UpdatedOn] datetime NOT NULL
)
GO

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
	[Signal] varchar(12) NULL
)
GO

CREATE TABLE [BlueCollarHistory]
(
	[Id] bigint NOT NULL PRIMARY KEY IDENTITY(1, 1),
	[ApplicationName] varchar(64) NOT NULL,
	[WorkerId] bigint NOT NULL,
	[ScheduleId] bigint NULL,
	[QueueName] varchar(24) NULL,
	[JobName] varchar(64) NULL,
	[JobType] varchar(256) NOT NULL,
	[Data] text NULL,
	[QueuedOn] datetime NULL,
	[TryNumber] bigint NOT NULL,
	[StartedOn] datetime NOT NULL,
	[Status] varchar(12) NOT NULL,
	[Exception] text NULL,
	[FinishedOn] datetime NOT NULL
)
GO

CREATE INDEX [IX_BlueCollarHistory_QueuedOn]
ON [BlueCollarHistory]([QueuedOn] DESC)
GO