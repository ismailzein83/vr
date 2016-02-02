CREATE TABLE [queue].[QueueItem_17] (
    [ID]                         BIGINT          NOT NULL,
    [Content]                    VARBINARY (MAX) NOT NULL,
    [ExecutionFlowTriggerItemID] BIGINT          NOT NULL,
    [LockedByProcessID]          INT             NULL,
    [IsSuspended]                BIT             NULL,
    CONSTRAINT [PK_QueueItem_17] PRIMARY KEY CLUSTERED ([ID] ASC)
);

