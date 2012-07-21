EXECUTE sp_rename N'[dbo].[BlueCollarScheduledJob].[Properties]', N'Data', 'COLUMN' 
GO

CREATE TABLE #BCSJ
(
	[Id] bigint,
	[ScheduleId] bigint,
	[JobType] varchar(256),
	[Data] text
)

INSERT INTO #BCSJ([Id], [ScheduleId], [JobType], [Data])
SELECT [Id], [ScheduleId], [JobType], [Data]
FROM [BlueCollarScheduledJob]

DROP TABLE [BlueCollarScheduledJob]
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

SET IDENTITY_INSERT [BlueCollarScheduledJob] ON

INSERT INTO [BlueCollarScheduledJob]([Id], [ScheduleId], [Number], [JobType], [Data])
SELECT 
	[Id], 
	[ScheduleId], 
	RANK() OVER(PARTITION BY [ScheduleId] ORDER BY [JobType]), 
	[JobType], 
	[Data]
FROM #BCSJ

SET IDENTITY_INSERT [BlueCollarScheduledJob] OFF

DROP TABLE #BCSJ
GO