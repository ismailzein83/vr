CREATE TABLE [runtime].[ScheduleTask] (
    [Id]               UNIQUEIDENTIFIER CONSTRAINT [DF__ScheduleTask__Id__46735417] DEFAULT (newid()) NOT NULL,
    [OldId]            INT              NULL,
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
    CONSTRAINT [pk_ScheduleTask] PRIMARY KEY CLUSTERED ([Id] ASC)
);









