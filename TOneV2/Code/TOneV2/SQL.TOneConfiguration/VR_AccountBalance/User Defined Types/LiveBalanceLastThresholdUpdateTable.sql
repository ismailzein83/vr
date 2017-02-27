CREATE TYPE [VR_AccountBalance].[LiveBalanceLastThresholdUpdateTable] AS TABLE (
    [AccountTypeId]               UNIQUEIDENTIFIER NULL,
    [AccountID]                   VARCHAR (50)     NOT NULL,
    [LastExecutedActionThreshold] DECIMAL (20, 6)  NULL,
    [ActiveAlertsInfo]            NVARCHAR (MAX)   NULL);

