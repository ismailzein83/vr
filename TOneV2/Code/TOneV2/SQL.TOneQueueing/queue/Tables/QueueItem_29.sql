﻿CREATE TABLE [queue].[QueueItem_29] (
    [ID]                         BIGINT          NOT NULL,
    [Content]                    VARBINARY (MAX) NOT NULL,
    [ExecutionFlowTriggerItemID] BIGINT          NOT NULL,
    [LockedByProcessID]          INT             NULL,
    [IsSuspended]                BIT             NULL,
    CONSTRAINT [PK_QueueItem_29] PRIMARY KEY CLUSTERED ([ID] ASC)
);

