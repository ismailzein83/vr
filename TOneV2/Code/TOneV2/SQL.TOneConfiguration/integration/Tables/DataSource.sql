CREATE TABLE [integration].[DataSource] (
    [ID]           INT            IDENTITY (1, 1) NOT NULL,
    [Name]         VARCHAR (100)  NOT NULL,
    [AdapterID]    INT            NOT NULL,
    [AdapterState] VARCHAR (1000) NULL,
    [TaskId]       INT            NOT NULL,
    [Settings]     VARCHAR (MAX)  NOT NULL,
    [CreatedTime]  DATETIME       CONSTRAINT [DF_DataSource_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]    ROWVERSION     NULL,
    CONSTRAINT [PK_DataSource] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_DataSource_AdapterType] FOREIGN KEY ([AdapterID]) REFERENCES [integration].[AdapterType] ([ID]) ON DELETE CASCADE
);

