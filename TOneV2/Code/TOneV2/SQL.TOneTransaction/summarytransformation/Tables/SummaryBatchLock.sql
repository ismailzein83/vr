CREATE TABLE [summarytransformation].[SummaryBatchLock] (
    [TypeID]            INT      NOT NULL,
    [BatchStart]        DATETIME NOT NULL,
    [LockedByProcessID] INT      NULL,
    CONSTRAINT [PK_SummaryBatchLock] PRIMARY KEY CLUSTERED ([TypeID] ASC, [BatchStart] ASC)
);

