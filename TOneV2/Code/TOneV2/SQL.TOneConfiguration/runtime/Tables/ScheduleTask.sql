CREATE TABLE [runtime].[ScheduleTask] (
    [ID]               INT              IDENTITY (1, 1) NOT NULL,
    [Name]             VARCHAR (255)    NOT NULL,
    [IsEnabled]        BIT              NOT NULL,
    [TaskType]         INT              NOT NULL,
    [TriggerTypeId]    UNIQUEIDENTIFIER NULL,
    [OldTriggerTypeId] INT              NOT NULL,
    [ActionTypeId]     UNIQUEIDENTIFIER NULL,
    [OldActionTypeId]  INT              NOT NULL,
    [TaskSettings]     VARCHAR (MAX)    NOT NULL,
    [OwnerId]          INT              NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK_ScheduleTask] PRIMARY KEY CLUSTERED ([ID] ASC)
);



