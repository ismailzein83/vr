CREATE TABLE [runtime].[ScheduleTask] (
    [Id]               UNIQUEIDENTIFIER CONSTRAINT [DF__ScheduleTask__Id__46735417] DEFAULT (newid()) NOT NULL,
    [Name]             VARCHAR (255)    NOT NULL,
    [IsEnabled]        BIT              NOT NULL,
    [TaskType]         INT              NOT NULL,
    [TriggerTypeId]    UNIQUEIDENTIFIER NOT NULL,
    [ActionTypeId]     UNIQUEIDENTIFIER NOT NULL,
    [TaskSettings]     VARCHAR (MAX)    NOT NULL,
    [OwnerId]          INT              NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_ScheduleTask_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_ScheduleTask_LastModifiedTime] DEFAULT (getdate()) NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [pk_ScheduleTask] PRIMARY KEY CLUSTERED ([Id] ASC)
);













