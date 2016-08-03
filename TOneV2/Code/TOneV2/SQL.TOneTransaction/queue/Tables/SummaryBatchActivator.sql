CREATE TABLE [queue].[SummaryBatchActivator] (
    [QueueID]     INT              NOT NULL,
    [BatchStart]  DATETIME         NOT NULL,
    [ActivatorID] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_SummaryBatchActivator] PRIMARY KEY CLUSTERED ([QueueID] ASC, [BatchStart] ASC)
);

