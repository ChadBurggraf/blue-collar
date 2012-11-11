DROP TABLE IF EXISTS [Tmp_BlueCollarSchedule];

CREATE TEMP TABLE [Tmp_BlueCollarSchedule]
(
	[Id] INTEGER NOT NULL,
	[ApplicationName] VARCHAR(64) NOT NULL,
	[QueueName] VARCHAR(24) NULL,
	[Name] VARCHAR(24) NOT NULL,
	[StartOn] DATETIME NOT NULL,
	[EndOn] DATETIME NULL,
	[RepeatType] VARCHAR(12) NOT NULL,
	[RepeatValue] INTEGER NULL,
	[Enabled] BOOLEAN NOT NULL,
	[Enqueueing] BOOLEAN NOT NULL,
	[EnqueueingUpdatedOn] DATETIME NULL
);

INSERT INTO [Tmp_BlueCollarSchedule]([Id],[ApplicationName],[QueueName],[Name],[StartOn],[EndOn],[RepeatType],[RepeatValue],[Enabled],[Enqueueing],[EnqueueingUpdatedOn])
SELECT [Id], [ApplicationName], [QueueName], [Name], [StartOn], [EndOn], [RepeatType], [RepeatValue], [Enabled], [Enqueueing], [EnqueueingUpdatedOn]
FROM [BlueCollarSchedule];

DROP TABLE [BlueCollarSchedule];

CREATE TABLE [BlueCollarSchedule]
(
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[ApplicationName] VARCHAR(64) NOT NULL,
	[QueueName] VARCHAR(24) NULL,
	[Name] VARCHAR(24) NOT NULL,
	[StartOn] DATETIME NOT NULL,
	[EndOn] DATETIME NULL,
	[RepeatType] VARCHAR(12) NOT NULL,
	[RepeatValue] INTEGER NULL,
	[Enabled] BOOLEAN NOT NULL,
	[Locked] BOOLEAN NOT NULL,
	[LockedUpdatedOn] DATETIME NULL
);

INSERT INTO [BlueCollarSchedule]([Id],[ApplicationName],[QueueName],[Name],[StartOn],[EndOn],[RepeatType],[RepeatValue],[Enabled],[Locked],[LockedUpdatedOn])
SELECT [Id], [ApplicationName], [QueueName], [Name], [StartOn], [EndOn], [RepeatType], [RepeatValue], [Enabled], [Enqueueing], [EnqueueingUpdatedOn]
FROM [Tmp_BlueCollarSchedule]; 

DROP TABLE [Tmp_BlueCollarSchedule];

DROP TABLE IF EXISTS [Tmp_BlueCollarQueue];

CREATE TEMP TABLE [Tmp_BlueCollarQueue]
(
	[Id] INTEGER NOT NULL,
	[ApplicationName] VARCHAR(64) NOT NULL,
	[ScheduleId] INTEGER NULL,
	[QueueName] VARCHAR(24) NULL,
	[JobName] VARCHAR(64) NOT NULL,
	[JobType] VARCHAR(256) NOT NULL,
	[Data] TEXT NOT NULL,
	[QueuedOn] DATETIME NOT NULL,
	[TryNumber] INTEGER NOT NULL
);

INSERT INTO [Tmp_BlueCollarQueue]([Id],[ApplicationName],[ScheduleId],[QueueName],[JobName],[JobType],[Data],[QueuedOn],[QueueName])
SELECT [Id], [ApplicationName], [ScheduleId], [QueueName], [JobName], [JobType], [Data], [QueuedOn], [QueueName]
FROM [BlueCollarQueue];

DROP TABLE [BlueCollarQueue];

CREATE TABLE [BlueCollarQueue]
(
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[ApplicationName] VARCHAR(64) NOT NULL,
	[ScheduleId] INTEGER NULL,
	[QueueName] VARCHAR(24) NULL,
	[JobName] VARCHAR(64) NOT NULL,
	[JobType] VARCHAR(256) NOT NULL,
	[Data] TEXT NOT NULL,
	[QueuedOn] DATETIME NOT NULL,
	[TryNumber] INTEGER NOT NULL,
	[Locked] BOOLEAN NOT NULL,
	[LockedUpdatedOn] DATETIME NULL
);

INSERT INTO [BlueCollarQueue]([Id],[ApplicationName],[ScheduleId],[QueueName],[JobName],[JobType],[Data],[QueuedOn],[QueueName],[Locked],[LockedUpdatedOn])
SELECT [Id], [ApplicationName], [ScheduleId], [QueueName], [JobName], [JobType], [Data], [QueuedOn], [QueueName], 0, NULL
FROM [Tmp_BlueCollarQueue];

DROP TABLE [Tmp_BlueCollarQueue];

DROP TABLE IF EXISTS [Tmp_BlueCollarWorker];

CREATE TEMP TABLE [Tmp_BlueCollarWorker]
(
	[Id] INTEGER NOT NULL,
	[ApplicationName] VARCHAR(64) NOT NULL,
	[Name] VARCHAR(64) NOT NULL,
	[MachineName] VARCHAR(128) NULL,
	[MachineAddress] VARCHAR(64) NULL,
	[QueueNames] TEXT NULL,
	[Status] VARCHAR(12) NOT NULL,
	[Signal] VARCHAR(24) NULL,
	[Startup] VARCHAR(12) NOT NULL,
	[UpdatedOn] DATETIME NOT NULL
);

INSERT INTO [Tmp_BlueCollarWorker]([Id],[ApplicationName],[Name],[MachineName],[MachineAddress],[QueueNames],[Status],[Signal],[Startup],[UpdatedOn])
SELECT [Id], [ApplicationName], [Name], [MachineName], [MachineAddress], [QueueNames], [Status], [Signal], [Startup], [UpdatedOn]
FROM [BlueCollarWorker];

DROP TABLE [BlueCollarWorker];

CREATE TABLE [BlueCollarWorker]
(
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[ApplicationName] VARCHAR(64) NOT NULL,
	[Name] VARCHAR(64) NOT NULL,
	[MachineName] VARCHAR(128) NULL,
	[MachineAddress] VARCHAR(64) NULL,
	[QueueNames] TEXT NULL,
	[Status] VARCHAR(12) NOT NULL,
	[Signal] VARCHAR(24) NULL,
	[Startup] VARCHAR(12) NOT NULL,
	[UpdatedOn] DATETIME NOT NULL,
	[Locked] BOOLEAN NOT NULL,
	[LockedUpdatedOn] DATETIME NULL
);

INSERT INTO [BlueCollarWorker]([Id],[ApplicationName],[Name],[MachineName],[MachineAddress],[QueueNames],[Status],[Signal],[Startup],[UpdatedOn],[Locked],[LockedUpdatedOn])
SELECT [Id], [ApplicationName], [Name], [MachineName], [MachineAddress], [QueueNames], [Status], [Signal], [Startup], [UpdatedOn], 0, NULL
FROM [Tmp_BlueCollarWorker];

DROP TABLE [Tmp_BlueCollarWorker];

DROP TABLE IF EXISTS [Tmp_BlueCollarWorking];

CREATE TEMP TABLE [Tmp_BlueCollarWorking]
(
	[Id] INTEGER NOT NULL,
	[ApplicationName] VARCHAR(64) NOT NULL,
	[WorkerId] INTEGER NOT NULL,
	[ScheduleId] INTEGER NULL,
	[QueueName] VARCHAR(24) NULL,
	[JobName] VARCHAR(64) NOT NULL,
	[JobType] VARCHAR(256) NOT NULL,
	[Data] TEXT NOT NULL,
	[QueuedOn] DATETIME NOT NULL,
	[TryNumber] INTEGER NOT NULL,
	[StartedOn] DATETIME NOT NULL,
	[Signal] VARCHAR(12) NULL
);

INSERT INTO [Tmp_BlueCollarWorking]([Id],[ApplicationName],[WorkerId],[ScheduleId],[QueueName],[JobName],[JobType],[Data],[QueuedOn],[TryNumber],[StartedOn],[Signal])
SELECT [Id], [ApplicationName], [WorkerId], [ScheduleId], [QueueName], [JobName], [JobType], [Data], [QueuedOn], [TryNumber], [StartedOn], [Signal]
FROM [BlueCollarWorking];

DROP TABLE [BlueCollarWorking];

CREATE TABLE [BlueCollarWorking]
(
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[ApplicationName] VARCHAR(64) NOT NULL,
	[WorkerId] INTEGER NOT NULL,
	[ScheduleId] INTEGER NULL,
	[QueueName] VARCHAR(24) NULL,
	[JobName] VARCHAR(64) NOT NULL,
	[JobType] VARCHAR(256) NOT NULL,
	[Data] TEXT NOT NULL,
	[QueuedOn] DATETIME NOT NULL,
	[TryNumber] INTEGER NOT NULL,
	[StartedOn] DATETIME NOT NULL,
	[Signal] VARCHAR(12) NULL,
	[Locked] BOOLEAN NOT NULL,
	[LockedUpdatedOn] DATETIME NULL
);

INSERT INTO [BlueCollarWorking]([Id],[ApplicationName],[WorkerId],[ScheduleId],[QueueName],[JobName],[JobType],[Data],[QueuedOn],[TryNumber],[StartedOn],[Signal],[Locked],[LockedUpdatedOn])
SELECT [Id], [ApplicationName], [WorkerId], [ScheduleId], [QueueName], [JobName], [JobType], [Data], [QueuedOn], [TryNumber], [StartedOn], [Signal], 0, NULL
FROM [Tmp_BlueCollarWorking];

DROP TABLE [Tmp_BlueCollarWorking];