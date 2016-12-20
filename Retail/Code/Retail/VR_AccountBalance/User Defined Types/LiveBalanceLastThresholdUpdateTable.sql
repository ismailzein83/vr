CREATE TYPE [VR_AccountBalance].[LiveBalanceLastThresholdUpdateTable] AS TABLE (
    [AccountTypeId]               UNIQUEIDENTIFIER NULL,
    [AccountID]                   BIGINT           NOT NULL,
    [LastExecutedActionThreshold] DECIMAL (20, 6)  NULL,
    [ActiveAlertsInfo]            NVARCHAR (MAX)   NULL);

