CREATE TABLE [VR_AccountBalance].[AlertRuleActionExecution] (
    [ID]                  BIGINT          IDENTITY (1, 1) NOT NULL,
    [AccountID]           BIGINT          NOT NULL,
    [Threshold]           DECIMAL (20, 6) NOT NULL,
    [ActionExecutionInfo] VARCHAR (MAX)   NULL,
    [ExecutionTime]       DATETIME        NOT NULL,
    CONSTRAINT [PK_AlertRuleActionExecution] PRIMARY KEY CLUSTERED ([ID] ASC)
);

