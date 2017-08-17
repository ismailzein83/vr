CREATE TABLE [Retail_BE].[AccountPackageRecurCharge] (
    [ID]                     BIGINT           IDENTITY (1, 1) NOT NULL,
    [AccountPackageID]       INT              NOT NULL,
    [ChargeableEntityID]     UNIQUEIDENTIFIER NOT NULL,
    [BalanceAccountTypeID]   UNIQUEIDENTIFIER NULL,
    [BalanceAccountID]       VARCHAR (50)     NOT NULL,
    [ChargeDay]              DATETIME         NOT NULL,
    [ChargeAmount]           DECIMAL (24, 10) NOT NULL,
    [CurrencyID]             INT              NOT NULL,
    [TransactionTypeID]      UNIQUEIDENTIFIER NOT NULL,
    [ProcessInstanceID]      BIGINT           NOT NULL,
    [IsSentToAccountBalance] BIT              NULL,
    [CreatedTime]            DATETIME         CONSTRAINT [DF_AccountPackageRecurCharge_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_AccountPackageRecurCharge] PRIMARY KEY CLUSTERED ([ID] ASC)
);

