CREATE TABLE [runtime].[TransactionLock] (
    [ID]                    UNIQUEIDENTIFIER NOT NULL,
    [TransactionUniqueName] NVARCHAR (255)   NOT NULL,
    [ProcessID]             INT              NOT NULL,
    [CreatedTime]           DATETIME         CONSTRAINT [DF_TransactionLock_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_TransactionLock] PRIMARY KEY CLUSTERED ([ID] ASC)
);

