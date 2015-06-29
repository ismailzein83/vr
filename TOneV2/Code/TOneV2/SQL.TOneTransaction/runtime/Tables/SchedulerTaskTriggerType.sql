CREATE TABLE [runtime].[SchedulerTaskTriggerType] (
    [ID]              INT           IDENTITY (1, 1) NOT NULL,
    [Name]            VARCHAR (50)  NOT NULL,
    [TriggerTypeInfo] VARCHAR (255) NOT NULL,
    CONSTRAINT [PK_SchedulerTaskTriggerTemplate] PRIMARY KEY CLUSTERED ([ID] ASC)
);

