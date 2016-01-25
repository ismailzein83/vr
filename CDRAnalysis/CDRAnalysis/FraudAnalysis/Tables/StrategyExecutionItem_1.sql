CREATE TABLE [FraudAnalysis].[StrategyExecutionItem] (
    [ID]                       BIGINT         IDENTITY (1, 1) NOT NULL,
    [StrategyExecutionID]      INT            NOT NULL,
    [AccountNumber]            VARCHAR (50)   NOT NULL,
    [SuspicionLevelID]         INT            NOT NULL,
    [FilterValues]             VARCHAR (MAX)  NULL,
    [AggregateValues]          VARCHAR (MAX)  NULL,
    [CaseID]                   INT            NULL,
    [SuspicionOccuranceStatus] INT            NULL,
    [IMEIs]                    NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_StrategyExecutionDetails] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_StrategyExecutionDetails_CaseID]
    ON [FraudAnalysis].[StrategyExecutionItem]([CaseID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_StrategyExecutionDetails_Status]
    ON [FraudAnalysis].[StrategyExecutionItem]([SuspicionOccuranceStatus] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_StrategyExecutionDetails_SuspicionLevel]
    ON [FraudAnalysis].[StrategyExecutionItem]([SuspicionLevelID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_StrategyExecutionDetails_StrategyExecutionID]
    ON [FraudAnalysis].[StrategyExecutionItem]([StrategyExecutionID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_StrategyExecutionDetails_AccountNumber]
    ON [FraudAnalysis].[StrategyExecutionItem]([AccountNumber] ASC);

