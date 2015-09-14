CREATE TABLE [FraudAnalysis].[StrategyExecutionDetails] (
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



