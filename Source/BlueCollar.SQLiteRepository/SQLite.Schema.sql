﻿CREATE TABLE IF NOT EXISTS [BlueCollarSchedule]
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
	[Enqueueing] BOOLEAN NOT NULL,
	[EnqueueingUpdatedOn] DATETIME NULL
);

CREATE TABLE IF NOT EXISTS [BlueCollarScheduledJob]
(
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[ScheduleId] INTEGER NOT NULL,
	[Number] INTEGER NOT NULL,
	[JobType] VARCHAR(256) NOT NULL,
	[Data] TEXT NULL
);

CREATE TABLE IF NOT EXISTS [BlueCollarQueue]
(
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[ApplicationName] VARCHAR(64) NOT NULL,
	[ScheduleId] INTEGER NULL,
	[QueueName] VARCHAR(24) NULL,
	[JobName] VARCHAR(64) NOT NULL,
	[JobType] VARCHAR(256) NOT NULL,
	[Data] TEXT NOT NULL,
	[QueuedOn] DATETIME NOT NULL,
	[TryNumber] INTEGER NOT NULL
);

CREATE INDEX IF NOT EXISTS [IX_BlueCollarQueue_QueuedOn]
ON [BlueCollarQueue]([QueuedOn] ASC);

CREATE TABLE IF NOT EXISTS [BlueCollarWorker]
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
	[UpdatedOn] DATETIME NOT NULL
);

CREATE TABLE IF NOT EXISTS [BlueCollarWorking]
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
	[Signal] VARCHAR(12) NULL
);

CREATE TABLE IF NOT EXISTS [BlueCollarHistory]
(
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[ApplicationName] VARCHAR(64) NOT NULL,
	[WorkerId] INTEGER NOT NULL,
	[ScheduleId] INTEGER NULL,
	[QueueName] VARCHAR(24) NULL,
	[JobName] VARCHAR(64) NULL,
	[JobType] VARCHAR(256) NOT NULL,
	[Data] TEXT NULL,
	[QueuedOn] DATETIME NULL,
	[TryNumber] INTEGER NOT NULL,
	[StartedOn] DATETIME NOT NULL,
	[Status] VARCHAR(12) NOT NULL,
	[Exception] TEXT NULL,
	[FinishedOn] DATETIME NOT NULL
);

CREATE INDEX IF NOT EXISTS [IX_BlueCollarHistory_QueuedOn_JobName_TryNumber_ApplicationName]
ON [BlueCollarHistory]
(
	[QueuedOn] DESC,
	[JobName] ASC,
	[TryNumber] DESC,
	[ApplicationName] ASC
);

CREATE INDEX IF NOT EXISTS [IX_BlueCollarHistory_FinishedOn_ApplicationName_Status] 
ON [BlueCollarHistory] 
(
	[FinishedOn] ASC,
	[ApplicationName] ASC,
	[Status] ASC
);