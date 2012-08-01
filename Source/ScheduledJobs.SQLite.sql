DROP TABLE IF EXISTS [Tmp_BlueCollarScheduledJob];

CREATE TEMP TABLE [Tmp_BlueCollarScheduledJob]
(
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[ScheduleId] INTEGER NOT NULL,
	[JobType] VARCHAR(256) NOT NULL,
	[Data] TEXT NULL
);

INSERT INTO [Tmp_BlueCollarScheduledJob]([Id], [ScheduleId], [JobType], [Data])
SELECT [Id], [ScheduleId], [JobType], [Properties]
FROM [BlueCollarScheduledJob];

DROP TABLE [BlueCollarScheduledJob];

CREATE TABLE [BlueCollarScheduledJob]
(
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[ScheduleId] INTEGER NOT NULL,
	[Number] INTEGER NOT NULL,
	[JobType] VARCHAR(256) NOT NULL,
	[Data] TEXT NULL
);

INSERT INTO [BlueCollarScheduledJob]
SELECT
	[Id],
	[ScheduleId],
	(
		SELECT 
		(
			SELECT COUNT(*)
			FROM [Tmp_BlueCollarScheduledJob] bsj3
			WHERE
				bsj2.[ScheduleId] = bsj3.[ScheduleId]
				AND bsj3.[JobType] <= bsj2.[JobType]
		) AS [Rank]
		FROM [Tmp_BlueCollarScheduledJob] bsj2
		WHERE
			bsj.[Id] = bsj2.[Id]
	) AS [Number],
	[JobType],
	[Data]
FROM [Tmp_BlueCollarScheduledJob] bsj;

DROP TABLE [Tmp_BlueCollarScheduledJob];