CREATE TABLE [queue].[QueueActivation] (
    [ID]          BIGINT   IDENTITY (1, 1) NOT NULL,
    [QueueID]     INT      NOT NULL,
    [CreatedTime] DATETIME CONSTRAINT [DF_QueueActivation_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_QueueActivation] PRIMARY KEY CLUSTERED ([ID] ASC)
);

