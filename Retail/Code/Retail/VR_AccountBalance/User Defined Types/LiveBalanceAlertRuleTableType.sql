CREATE TYPE [VR_AccountBalance].[LiveBalanceAlertRuleTableType] AS TABLE (
    [AccountID]   BIGINT NOT NULL,
    [AlertRuleId] INT    NULL,
    PRIMARY KEY CLUSTERED ([AccountID] ASC));

