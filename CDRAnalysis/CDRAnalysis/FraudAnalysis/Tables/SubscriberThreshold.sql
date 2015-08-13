CREATE TABLE [FraudAnalysis].[SubscriberThreshold] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [DateDay]          DATETIME       NULL,
    [SubscriberNumber] VARCHAR (50)   NULL,
    [SuspicionLevelId] INT            NULL,
    [StrategyId]       INT            NULL,
    [CriteriaValues]   NVARCHAR (MAX) NULL,
    CONSTRAINT [FK_SubscriberThreshold_Suspicion_Level] FOREIGN KEY ([SuspicionLevelId]) REFERENCES [FraudAnalysis].[Suspicion_Level] ([Id])
);


GO
CREATE CLUSTERED INDEX [IX_SubscriberThresholds]
    ON [FraudAnalysis].[SubscriberThreshold]([DateDay] ASC, [StrategyId] ASC);

