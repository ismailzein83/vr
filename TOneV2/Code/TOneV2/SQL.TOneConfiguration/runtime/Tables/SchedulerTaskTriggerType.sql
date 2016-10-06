CREATE TABLE [runtime].[SchedulerTaskTriggerType] (
    [ID]              UNIQUEIDENTIFIER NOT NULL,
    [OldID]           INT              NULL,
    [Name]            VARCHAR (50)     NOT NULL,
    [TriggerTypeInfo] VARCHAR (255)    NOT NULL,
    CONSTRAINT [PK_SchedulerTaskTriggerTemplate] PRIMARY KEY CLUSTERED ([ID] ASC)
);



