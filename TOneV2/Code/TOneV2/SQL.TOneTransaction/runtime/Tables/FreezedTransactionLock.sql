CREATE TABLE [runtime].[FreezedTransactionLock] (
    [ID]                     BIGINT        IDENTITY (1, 1) NOT NULL,
    [TransactionLockItemIds] VARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_FreezedTransactionLock] PRIMARY KEY CLUSTERED ([ID] ASC)
);

