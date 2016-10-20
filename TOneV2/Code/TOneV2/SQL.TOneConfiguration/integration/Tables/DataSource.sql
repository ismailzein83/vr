CREATE TABLE [integration].[DataSource] (
    [Name]         VARCHAR (100)    NOT NULL,
    [AdapterState] VARCHAR (1000)   NULL,
    [OldTaskId]    INT              NOT NULL,
    [Settings]     VARCHAR (MAX)    NOT NULL,
    [CreatedTime]  DATETIME         CONSTRAINT [DF_DataSource_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]    ROWVERSION       NULL,
    [OldID]        INT              NULL,
    [ID]           UNIQUEIDENTIFIER CONSTRAINT [DF__DataSource__ID__485B9C89] DEFAULT (newid()) NOT NULL,
    [OldAdapterID] INT              NULL,
    [AdapterID]    UNIQUEIDENTIFIER NULL,
    [TaskId]       UNIQUEIDENTIFIER NULL,
    CONSTRAINT [pk_DataSource] PRIMARY KEY CLUSTERED ([ID] ASC)
);



