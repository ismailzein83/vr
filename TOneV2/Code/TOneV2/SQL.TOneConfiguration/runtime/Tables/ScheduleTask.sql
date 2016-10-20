CREATE TABLE [runtime].[ScheduleTask] (
    [Name]             VARCHAR (255)    NOT NULL,
    [IsEnabled]        BIT              NOT NULL,
    [TaskType]         INT              NOT NULL,
    [TriggerTypeId]    UNIQUEIDENTIFIER NOT NULL,
    [OldTriggerTypeId] INT              NULL,
    [ActionTypeId]     UNIQUEIDENTIFIER NOT NULL,
    [OldActionTypeId]  INT              NULL,
    [TaskSettings]     VARCHAR (MAX)    NOT NULL,
    [OwnerId]          INT              NULL,
    [timestamp]        ROWVERSION       NULL,
    [OldId]            INT              NULL,
    [Id]               UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    CONSTRAINT [pk_ScheduleTask] PRIMARY KEY CLUSTERED ([Id] ASC)
);







