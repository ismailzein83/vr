CREATE TYPE [VR_AccountBalance].[LiveBalanceThresholdUpdateTable] AS TABLE (
    [AccountTypeId]        UNIQUEIDENTIFIER NULL,
    [AccountID]            BIGINT           NOT NULL,
    [NextAlertThreshold]   DECIMAL (20, 6)  NULL,
    [AlertRuleId]          INT              NULL,
    [ThresholdActionIndex] INT              NULL);



