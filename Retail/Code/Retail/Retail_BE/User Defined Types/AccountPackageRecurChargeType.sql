CREATE TYPE [Retail_BE].[AccountPackageRecurChargeType] AS TABLE (
    [AccountPackageID]     INT              NOT NULL,
    [ChargeableEntityID]   UNIQUEIDENTIFIER NOT NULL,
    [BalanceAccountTypeID] UNIQUEIDENTIFIER NULL,
    [BalanceAccountID]     VARCHAR (50)     NOT NULL,
    [ChargeDay]            DATETIME         NOT NULL,
    [ChargeAmount]         DECIMAL (24, 10) NOT NULL,
    [CurrencyID]           INT              NOT NULL,
    [TransactionTypeID]    UNIQUEIDENTIFIER NOT NULL);

