CREATE TABLE [runtime].[SchedulerTaskTriggerType] (
    [ID]              UNIQUEIDENTIFIER NOT NULL,
    [Name]            VARCHAR (50)     NOT NULL,
    [TriggerTypeInfo] VARCHAR (300)    NOT NULL,
    [timestamp]       ROWVERSION       NULL,
    CONSTRAINT [PK_SchedulerTaskTriggerTemplate] PRIMARY KEY CLUSTERED ([ID] ASC)
);









