CREATE TABLE [queue].[QueueItemIDGen] (
    [QueueID]       INT    NOT NULL,
    [CurrentItemID] BIGINT NOT NULL,
    CONSTRAINT [PK_QueueItemIDGen] PRIMARY KEY CLUSTERED ([QueueID] ASC)
);

