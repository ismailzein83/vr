CREATE TABLE [StatisticManagement].[StatisticBatch] (
    [TypeID]            INT             NOT NULL,
    [BatchStart]        DATETIME        NOT NULL,
    [BatchInfo]         VARBINARY (MAX) NULL,
    [LockedByProcessID] INT             NULL,
    [CreatedTime]       DATETIME        CONSTRAINT [DF_StatisticBatch_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_StatisticBatch_1] PRIMARY KEY CLUSTERED ([TypeID] ASC, [BatchStart] ASC)
);




GO
CREATE NONCLUSTERED INDEX [IX_StatisticBatch_LockedByProcessID]
    ON [StatisticManagement].[StatisticBatch]([LockedByProcessID] ASC);

