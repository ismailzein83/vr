CREATE TABLE [queue].[QueueItemHeader] (
    [ItemID]                     BIGINT          NOT NULL,
    [QueueID]                    INT             NOT NULL,
    [ExecutionFlowTriggerItemID] BIGINT          NULL,
    [SourceItemID]               BIGINT          NULL,
    [Description]                NVARCHAR (1000) NULL,
    [Status]                     INT             NOT NULL,
    [RetryCount]                 INT             NULL,
    [ErrorMessage]               NVARCHAR (MAX)  NULL,
    [CreatedTime]                DATETIME        NULL,
    [LastUpdatedTime]            DATETIME        NULL,
    CONSTRAINT [PK_QueueItemHeader_1] PRIMARY KEY CLUSTERED ([ItemID] ASC),
    CONSTRAINT [FK_QueueItemHeader_QueueItemHeader] FOREIGN KEY ([ExecutionFlowTriggerItemID]) REFERENCES [queue].[QueueItemHeader] ([ItemID])
);




GO
CREATE NONCLUSTERED INDEX [IX_QueueItemHeader_Status]
    ON [queue].[QueueItemHeader]([Status] ASC);

