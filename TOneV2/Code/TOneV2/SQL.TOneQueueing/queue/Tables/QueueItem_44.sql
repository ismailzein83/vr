﻿CREATE TABLE [queue].[QueueItem_44] (
    [ID]                         BIGINT          NOT NULL,
    [Content]                    VARBINARY (MAX) NOT NULL,
    [ExecutionFlowTriggerItemID] BIGINT          NOT NULL,
    [LockedByProcessID]          INT             NULL,
    [IsSuspended]                BIT             NULL,
    CONSTRAINT [PK_QueueItem_44] PRIMARY KEY CLUSTERED ([ID] ASC)
);

