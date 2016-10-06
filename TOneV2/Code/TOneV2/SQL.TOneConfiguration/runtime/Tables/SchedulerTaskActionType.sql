CREATE TABLE [runtime].[SchedulerTaskActionType] (
    [ID]             UNIQUEIDENTIFIER NOT NULL,
    [OldID]          INT              NULL,
    [Name]           VARCHAR (50)     NOT NULL,
    [ActionTypeInfo] VARCHAR (MAX)    NOT NULL,
    CONSTRAINT [PK_SchedulerTaskActionType] PRIMARY KEY CLUSTERED ([ID] ASC)
);



