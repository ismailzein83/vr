CREATE TYPE [VR_AccountBalance].[TransactionAccountUsageQueryTable] AS TABLE (
    [TransactionID]     BIGINT           NOT NULL,
    [TransactionTypeID] UNIQUEIDENTIFIER NOT NULL,
    [AccountTypeID]     UNIQUEIDENTIFIER NOT NULL,
    [AccountID]         VARCHAR (50)     NOT NULL,
    [PeriodStart]       DATETIME         NOT NULL,
    [PeriodEnd]         DATETIME         NOT NULL);

