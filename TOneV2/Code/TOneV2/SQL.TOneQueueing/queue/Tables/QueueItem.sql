CREATE TABLE [queue].[QueueItem] (
    [ID]                         BIGINT           NOT NULL,
    [QueueID]                    INT              NOT NULL,
    [Content]                    VARBINARY (MAX)  NOT NULL,
    [ExecutionFlowTriggerItemID] BIGINT           NOT NULL,
    [LockedByProcessID]          INT              NULL,
    [IsSuspended]                BIT              NULL,
    [ActivatorID]                UNIQUEIDENTIFIER NULL,
    [BatchStart]                 DATETIME         NULL
);


GO
CREATE CLUSTERED INDEX [IX_QueueItem_ID]
    ON [queue].[QueueItem]([ID] ASC);

