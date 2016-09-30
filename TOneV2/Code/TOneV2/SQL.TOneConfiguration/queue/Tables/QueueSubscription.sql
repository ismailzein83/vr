CREATE TABLE [queue].[QueueSubscription] (
    [QueueID]           INT        NOT NULL,
    [SubscribedQueueID] INT        NOT NULL,
    [timestamp]         ROWVERSION NULL,
    CONSTRAINT [PK_QueueSubscription] PRIMARY KEY CLUSTERED ([QueueID] ASC, [SubscribedQueueID] ASC)
);

