CREATE TABLE [VR_AccountBalance].[AlertRuleThresholdAction] (
    [ID]                   BIGINT          IDENTITY (1, 1) NOT NULL,
    [RuleID]               INT             NOT NULL,
    [Threshold]            DECIMAL (18, 6) NOT NULL,
    [ThresholdActionIndex] INT             NOT NULL,
    CONSTRAINT [PK_AlertRuleThresholdAction] PRIMARY KEY CLUSTERED ([ID] ASC)
);

