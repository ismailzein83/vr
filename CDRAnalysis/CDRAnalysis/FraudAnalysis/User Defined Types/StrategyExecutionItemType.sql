CREATE TYPE [FraudAnalysis].[StrategyExecutionItemType] AS TABLE (
    [ID]                       BIGINT       NULL,
    [SuspicionOccuranceStatus] INT          NULL,
    [CaseID]                   INT          NULL,
    [AccountNumber]            VARCHAR (50) NULL);



