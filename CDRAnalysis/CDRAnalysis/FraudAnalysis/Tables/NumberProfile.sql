CREATE TABLE [FraudAnalysis].[NumberProfile] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [SubscriberNumber] VARCHAR (30)   NULL,
    [FromDate]         DATETIME       NULL,
    [ToDate]           DATETIME       NULL,
    [StrategyId]       INT            NULL,
    [AggregateValues]  NVARCHAR (MAX) NULL
);




GO
CREATE CLUSTERED INDEX [IX_NumberProfile_StrategyId]
    ON [FraudAnalysis].[NumberProfile]([StrategyId] ASC);

