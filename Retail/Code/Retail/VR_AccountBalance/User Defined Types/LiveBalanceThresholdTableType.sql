CREATE TYPE [VR_AccountBalance].[LiveBalanceThresholdTableType] AS TABLE (
    [AccountID]            BIGINT          NOT NULL,
    [Threshold]            DECIMAL (20, 6) NULL,
    [ThresholdActionIndex] INT             NULL,
    [AlertRuleId]          INT             NULL,
    PRIMARY KEY CLUSTERED ([AccountID] ASC));

