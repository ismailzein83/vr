CREATE TABLE [FraudAnalysis].[AccountThreshold] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [DateDay]          DATETIME       NULL,
    [AccountNumber]    VARCHAR (50)   NULL,
    [SuspicionLevelId] INT            NULL,
    [StrategyId]       INT            NULL,
    [CriteriaValues]   NVARCHAR (MAX) NULL,
    CONSTRAINT [FK_SubscriberThreshold_Suspicion_Level] FOREIGN KEY ([SuspicionLevelId]) REFERENCES [FraudAnalysis].[SuspicionLevel] ([Id])
);








GO
CREATE CLUSTERED INDEX [IX_SubscriberThreshold_StrategyId]
    ON [FraudAnalysis].[AccountThreshold]([StrategyId] ASC);

