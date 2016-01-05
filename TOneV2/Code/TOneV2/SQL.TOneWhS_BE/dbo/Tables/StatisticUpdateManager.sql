CREATE TABLE [dbo].[StatisticUpdateManager] (
    [ID]                         BIGINT         IDENTITY (1, 1) NOT NULL,
    [StatisticBatchGroupKey]     VARCHAR (900)  NOT NULL,
    [StatisticBatchGroupContent] NVARCHAR (MAX) NOT NULL,
    [LockedByProcessID]          INT            NULL,
    CONSTRAINT [PK_StatisticUpdateManager] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_StatisticUpdateManager_BatchGroupKey] UNIQUE NONCLUSTERED ([StatisticBatchGroupKey] ASC)
);

