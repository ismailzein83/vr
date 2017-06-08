CREATE TYPE [VR_AccountBalance].[AccountUsageOverrideTable] AS TABLE (
    [AccountTypeID]            UNIQUEIDENTIFIER NOT NULL,
    [AccountID]                VARCHAR (50)     NOT NULL,
    [TransactionTypeID]        UNIQUEIDENTIFIER NOT NULL,
    [PeriodStart]              DATETIME         NOT NULL,
    [PeriodEnd]                DATETIME         NOT NULL,
    [OverridenByTransactionID] BIGINT           NOT NULL);

