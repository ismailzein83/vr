CREATE TABLE [queue].[QueueItem_31] (
    [ID]                         BIGINT          NOT NULL,
    [Content]                    VARBINARY (MAX) NOT NULL,
    [ExecutionFlowTriggerItemID] BIGINT          NOT NULL,
    [LockedByProcessID]          INT             NULL,
    [IsSuspended]                BIT             NULL,
    CONSTRAINT [PK_QueueItem_31] PRIMARY KEY CLUSTERED ([ID] ASC)
);

