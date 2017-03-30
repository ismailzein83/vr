CREATE TYPE [VR_AccountBalance].[BillingTransactionsByTimeTable] AS TABLE (
    [AccountID]       NVARCHAR (255) NOT NULL,
    [TransactionTime] DATETIME       NOT NULL);

