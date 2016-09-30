CREATE TABLE [queue].[QueueItemType] (
    [ID]                   INT            IDENTITY (1, 1) NOT NULL,
    [ItemFQTN]             VARCHAR (900)  NOT NULL,
    [Title]                VARCHAR (255)  NOT NULL,
    [DefaultQueueSettings] NVARCHAR (MAX) NOT NULL,
    [CreatedTime]          DATETIME       CONSTRAINT [DF_QueueItemType_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]            ROWVERSION     NULL,
    CONSTRAINT [PK_QueueItemType] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_QueueItemType_ItemFQTN] UNIQUE NONCLUSTERED ([ItemFQTN] ASC)
);

