CREATE TABLE [runtime].[SchedulerTaskActionType] (
    [ID]             UNIQUEIDENTIFIER NOT NULL,
    [Name]           VARCHAR (50)     NOT NULL,
    [ActionTypeInfo] VARCHAR (MAX)    NOT NULL,
    [timestamp]      ROWVERSION       NULL,
    CONSTRAINT [PK_SchedulerTaskActionType] PRIMARY KEY CLUSTERED ([ID] ASC)
);







