CREATE TABLE [runtime].[ScheduleTaskState] (
    [TaskId]            UNIQUEIDENTIFIER NOT NULL,
    [OldTaskId]         INT              NULL,
    [Status]            INT              NOT NULL,
    [LastRunTime]       DATETIME         NULL,
    [NextRunTime]       DATETIME         NULL,
    [LockedByProcessID] INT              NULL,
    [ExecutionInfo]     NVARCHAR (MAX)   NULL,
    [timestamp]         ROWVERSION       NOT NULL,
    CONSTRAINT [PK_ScheduleTaskState_1] PRIMARY KEY CLUSTERED ([TaskId] ASC)
);




GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_ScheduleTaskState_TaskId]
    ON [runtime].[ScheduleTaskState]([OldTaskId] ASC);



