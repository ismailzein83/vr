CREATE TABLE [FraudAnalysis].[StrategyExecution] (
    [ID]            BIGINT   IDENTITY (1, 1) NOT NULL,
    [ProcessID]     BIGINT   NOT NULL,
    [StrategyID]    INT      NOT NULL,
    [FromDate]      DATETIME NOT NULL,
    [ToDate]        DATETIME NOT NULL,
    [PeriodID]      INT      NOT NULL,
    [ExecutionDate] DATETIME NOT NULL,
    [IsOverriden]   BIT      CONSTRAINT [DF_StrategyExecution_IsOverriden] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_StrategyExecution] PRIMARY KEY CLUSTERED ([ID] ASC)
);






GO
CREATE NONCLUSTERED INDEX [IX_StrategyExecution_Strategy]
    ON [FraudAnalysis].[StrategyExecution]([StrategyID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_StrategyExecution_ExecutionDate]
    ON [FraudAnalysis].[StrategyExecution]([ExecutionDate] ASC);

