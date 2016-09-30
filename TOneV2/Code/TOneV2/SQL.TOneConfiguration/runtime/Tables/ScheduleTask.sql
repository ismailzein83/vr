CREATE TABLE [runtime].[ScheduleTask] (
    [ID]            INT           IDENTITY (1, 1) NOT NULL,
    [Name]          VARCHAR (255) NOT NULL,
    [IsEnabled]     BIT           NOT NULL,
    [TaskType]      INT           NOT NULL,
    [TriggerTypeId] INT           NOT NULL,
    [ActionTypeId]  INT           NOT NULL,
    [TaskSettings]  VARCHAR (MAX) NOT NULL,
    [OwnerId]       INT           NULL,
    [timestamp]     ROWVERSION    NULL,
    CONSTRAINT [PK_ScheduleTask] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_ScheduleTask_SchedulerTaskActionType] FOREIGN KEY ([ActionTypeId]) REFERENCES [runtime].[SchedulerTaskActionType] ([ID]),
    CONSTRAINT [FK_ScheduleTask_SchedulerTaskTriggerType] FOREIGN KEY ([TriggerTypeId]) REFERENCES [runtime].[SchedulerTaskTriggerType] ([ID])
);

