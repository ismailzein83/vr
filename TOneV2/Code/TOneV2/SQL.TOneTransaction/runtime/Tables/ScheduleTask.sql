CREATE TABLE [runtime].[ScheduleTask] (
    [ID]            INT            IDENTITY (1, 1) NOT NULL,
    [Name]          VARCHAR (255)  NOT NULL,
    [IsEnabled]     BIT            NOT NULL,
    [TaskType]      INT            NOT NULL,
    [Status]        INT            NOT NULL,
    [LastRunTime]   DATETIME       NULL,
    [NextRunTime]   DATETIME       NULL,
    [TriggerTypeId] INT            NOT NULL,
    [TaskTrigger]   VARCHAR (1000) NOT NULL,
    [ActionTypeId]  INT            NULL,
    [TaskAction]    VARCHAR (1000) NOT NULL,
    CONSTRAINT [PK_ScheduleTask] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_ScheduleTask_SchedulerTaskTriggerType] FOREIGN KEY ([TriggerTypeId]) REFERENCES [runtime].[SchedulerTaskTriggerType] ([ID])
);









