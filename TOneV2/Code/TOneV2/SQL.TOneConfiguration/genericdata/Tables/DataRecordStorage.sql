CREATE TABLE [genericdata].[DataRecordStorage] (
    [ID]                  UNIQUEIDENTIFIER NOT NULL,
    [OldID]               INT              NULL,
    [Name]                NVARCHAR (1000)  NOT NULL,
    [OldDataRecordTypeID] INT              NOT NULL,
    [DataRecordTypeID]    UNIQUEIDENTIFIER NULL,
    [DataStoreID]         UNIQUEIDENTIFIER NULL,
    [OldDataStoreID]      INT              NOT NULL,
    [Settings]            NVARCHAR (MAX)   NOT NULL,
    [State]               NVARCHAR (MAX)   NULL,
    [CreatedTime]         DATETIME         CONSTRAINT [DF_DataRecordStorage_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]           ROWVERSION       NULL,
    CONSTRAINT [PK_DataRecordStorage] PRIMARY KEY CLUSTERED ([ID] ASC)
);





