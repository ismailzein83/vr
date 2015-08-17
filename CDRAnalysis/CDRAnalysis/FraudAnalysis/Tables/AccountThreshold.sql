CREATE TABLE [FraudAnalysis].[AccountThreshold] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [DateDay]          DATETIME       NULL,
    [AccountNumber]    VARCHAR (50)   NULL,
    [SuspicionLevelId] INT            NULL,
    [StrategyId]       INT            NULL,
    [CriteriaValues]   NVARCHAR (MAX) NULL
);












GO
CREATE CLUSTERED INDEX [IX_AccountThreshold_StrategyId]
    ON [FraudAnalysis].[AccountThreshold]([StrategyId] ASC);

