CREATE TABLE [runtime].[SchedulerTaskTriggerType] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             VARCHAR (50)     NOT NULL,
    [TriggerTypeInfo]  VARCHAR (MAX)    NOT NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_SchedulerTaskTriggerType_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_SchedulerTaskTriggerType_LastModifiedTime] DEFAULT (getdate()) NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK_SchedulerTaskTriggerTemplate] PRIMARY KEY CLUSTERED ([ID] ASC)
);













