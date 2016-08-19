CREATE TABLE [VRNotification].[VRAlertRuleExecution] (
    [ID]            BIGINT         IDENTITY (1, 1) NOT NULL,
    [RuleID]        BIGINT         NOT NULL,
    [EventKey]      NVARCHAR (255) NULL,
    [ExecutionData] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_VRAlertRuleExecution] PRIMARY KEY CLUSTERED ([ID] ASC)
);

