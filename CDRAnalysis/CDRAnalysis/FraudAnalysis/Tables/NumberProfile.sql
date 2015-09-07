CREATE TABLE [FraudAnalysis].[NumberProfile] (
    [ID]                  INT            IDENTITY (1, 1) NOT NULL,
    [AccountNumber]       VARCHAR (30)   NULL,
    [FromDate]            DATETIME       NULL,
    [ToDate]              DATETIME       NULL,
    [StrategyId]          INT            NULL,
    [AggregateValues]     NVARCHAR (MAX) NULL,
    [StrategyExecutionID] INT            NULL
);






GO
CREATE CLUSTERED INDEX [IX_NumberProfile_StrategyId]
    ON [FraudAnalysis].[NumberProfile]([StrategyId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_NumberProfile_AccountNumber]
    ON [FraudAnalysis].[NumberProfile]([AccountNumber] ASC);

