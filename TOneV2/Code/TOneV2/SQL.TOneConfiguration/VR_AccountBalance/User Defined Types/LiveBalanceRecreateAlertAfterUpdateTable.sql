CREATE TYPE [VR_AccountBalance].[LiveBalanceRecreateAlertAfterUpdateTable] AS TABLE (
    [AccountTypeId]      UNIQUEIDENTIFIER NULL,
    [AccountID]          VARCHAR (50)     NOT NULL,
    [RecreateAlertAfter] DATETIME         NULL);

