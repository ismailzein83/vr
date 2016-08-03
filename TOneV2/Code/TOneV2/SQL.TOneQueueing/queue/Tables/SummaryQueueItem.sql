CREATE TABLE [queue].[SummaryQueueItem] (
    [ID]                         BIGINT          NOT NULL,
    [QueueID]                    INT             NOT NULL,
    [Content]                    VARBINARY (MAX) NOT NULL,
    [ExecutionFlowTriggerItemID] BIGINT          NULL,
    [BatchStart]                 DATETIME        NULL,
    [IsSuspended]                BIT             NULL
);


GO
CREATE CLUSTERED INDEX [IX_SummaryQueueItem_ID]
    ON [queue].[SummaryQueueItem]([ID] ASC);

