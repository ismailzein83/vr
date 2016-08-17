CREATE TYPE [VR_AccountBalance].[AlertRuleThresholdActionsTableType] AS TABLE (
    [RuleId]               INT             NOT NULL,
    [Threshold]            DECIMAL (20, 6) NULL,
    [ThresholdActionIndex] INT             NULL);

