CREATE TABLE [queue].[QueueItemHeader] (
    [ItemID]          BIGINT          NOT NULL,
    [QueueID]         INT             NOT NULL,
    [SourceQueueID]   INT             NULL,
    [SourceItemID]    BIGINT          NULL,
    [Description]     NVARCHAR (1000) NULL,
    [Status]          INT             NOT NULL,
    [RetryCount]      INT             NULL,
    [ErrorMessage]    NVARCHAR (MAX)  NULL,
    [CreatedTime]     DATETIME        NULL,
    [LastUpdatedTime] DATETIME        NULL,
    CONSTRAINT [PK_QueueItemHeader] PRIMARY KEY CLUSTERED ([ItemID] ASC, [QueueID] ASC)
);

