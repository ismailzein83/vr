﻿CREATE TABLE [queue].[QueueItem] (
    [ID]                         BIGINT           NOT NULL,
    [QueueID]                    INT              NOT NULL,
    [Content]                    VARBINARY (MAX)  NOT NULL,
    [ExecutionFlowTriggerItemID] BIGINT           NOT NULL,
    [DataSourceID]               UNIQUEIDENTIFIER NULL,
    [BatchDescription]           NVARCHAR (1000)  NULL,
    [LockedByProcessID]          INT              NULL,
    [IsSuspended]                BIT              NULL,
    [ActivatorID]                UNIQUEIDENTIFIER NULL,
    [BatchStart]                 DATETIME         NULL,
    [BatchEnd]                   DATETIME         NULL
);








GO
CREATE CLUSTERED INDEX [IX_QueueItem_ID]
    ON [queue].[QueueItem]([ID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_QueueItem_QueueIDActivatorID]
    ON [queue].[QueueItem]([QueueID] ASC, [ActivatorID] ASC);

