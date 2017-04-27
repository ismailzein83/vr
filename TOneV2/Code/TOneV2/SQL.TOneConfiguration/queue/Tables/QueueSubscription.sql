CREATE TABLE [queue].[QueueSubscription] (
    [QueueID]           INT        NOT NULL,
    [SubscribedQueueID] INT        NOT NULL,
    [IsActive]          BIT        NULL,
    [CreatedTime]       DATETIME   CONSTRAINT [DF_QueueSubscription_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]         ROWVERSION NULL,
    CONSTRAINT [PK_QueueSubscription] PRIMARY KEY CLUSTERED ([QueueID] ASC, [SubscribedQueueID] ASC)
);



