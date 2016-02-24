CREATE TABLE [genericdata].[DataRecordStorage] (
    [ID]               INT             IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (1000) NOT NULL,
    [DataRecordTypeID] INT             NOT NULL,
    [DataStoreID]      INT             NOT NULL,
    [Settings]         NVARCHAR (MAX)  NOT NULL,
    [State]            NVARCHAR (MAX)  NULL,
    [CreatedTime]      DATETIME        CONSTRAINT [DF_DataRecordStorage_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]        ROWVERSION      NULL,
    CONSTRAINT [PK_DataRecordStorage] PRIMARY KEY CLUSTERED ([ID] ASC)
);

