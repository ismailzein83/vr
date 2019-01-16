CREATE TABLE [runtime].[SchedulerTaskActionType] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             VARCHAR (50)     NOT NULL,
    [ActionTypeInfo]   VARCHAR (MAX)    NOT NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_SchedulerTaskActionType_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_SchedulerTaskActionType_LastModifiedTime] DEFAULT (getdate()) NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK_SchedulerTaskActionType] PRIMARY KEY CLUSTERED ([ID] ASC)
);









