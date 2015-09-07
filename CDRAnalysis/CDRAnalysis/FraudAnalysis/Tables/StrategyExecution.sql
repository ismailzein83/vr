CREATE TABLE [FraudAnalysis].[StrategyExecution] (
    [ID]            BIGINT   IDENTITY (1, 1) NOT NULL,
    [ProcessID]     BIGINT   NOT NULL,
    [StrategyID]    INT      NOT NULL,
    [FromDate]      DATETIME NOT NULL,
    [ToDate]        DATETIME NOT NULL,
    [PeriodID]      INT      NOT NULL,
    [ExecutionDate] DATETIME NOT NULL,
    CONSTRAINT [PK_StrategyExecution] PRIMARY KEY CLUSTERED ([ID] ASC)
);

