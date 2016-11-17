CREATE TABLE [genericdata].[DataRecordStorage] (
    [ID]                  UNIQUEIDENTIFIER NOT NULL,
    [OldID]               INT              NULL,
    [Name]                NVARCHAR (1000)  NOT NULL,
    [OldDataRecordTypeID] INT              NULL,
    [DataRecordTypeID]    UNIQUEIDENTIFIER NOT NULL,
    [DataStoreID]         UNIQUEIDENTIFIER NOT NULL,
    [OldDataStoreID]      INT              NULL,
    [Settings]            NVARCHAR (MAX)   NOT NULL,
    [State]               NVARCHAR (MAX)   NULL,
    [CreatedTime]         DATETIME         CONSTRAINT [DF_DataRecordStorage_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]           ROWVERSION       NULL,
    CONSTRAINT [PK_DataRecordStorage] PRIMARY KEY CLUSTERED ([ID] ASC)
);







