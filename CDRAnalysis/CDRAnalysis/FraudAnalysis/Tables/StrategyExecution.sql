CREATE TABLE [FraudAnalysis].[StrategyExecution] (
    [ID]                  BIGINT     IDENTITY (1, 1) NOT NULL,
    [ProcessID]           BIGINT     NOT NULL,
    [StrategyID]          INT        NOT NULL,
    [FromDate]            DATETIME   NOT NULL,
    [ToDate]              DATETIME   NOT NULL,
    [PeriodID]            INT        NOT NULL,
    [ExecutionDate]       DATETIME   NOT NULL,
    [CancellationDate]    DATETIME   NULL,
    [ExecutedBy]          INT        NOT NULL,
    [CancelledBy]         INT        NULL,
    [NumberofSubscribers] BIGINT     NULL,
    [NumberofCDRs]        BIGINT     NULL,
    [NumberofSuspicions]  BIGINT     NULL,
    [ExecutionDuration]   FLOAT (53) NULL,
    [Status]              INT        NOT NULL,
    CONSTRAINT [PK_StrategyExecution] PRIMARY KEY CLUSTERED ([ID] ASC)
);








GO
CREATE NONCLUSTERED INDEX [IX_StrategyExecution_Strategy]
    ON [FraudAnalysis].[StrategyExecution]([StrategyID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_StrategyExecution_ExecutionDate]
    ON [FraudAnalysis].[StrategyExecution]([ExecutionDate] ASC);

