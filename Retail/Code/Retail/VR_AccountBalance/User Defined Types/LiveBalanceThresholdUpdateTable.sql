CREATE TYPE [VR_AccountBalance].[LiveBalanceThresholdUpdateTable] AS TABLE (
    [AccountTypeId]      UNIQUEIDENTIFIER NULL,
    [AccountID]          VARCHAR (50)     NOT NULL,
    [NextAlertThreshold] DECIMAL (20, 6)  NULL,
    [AlertRuleId]        INT              NULL);





