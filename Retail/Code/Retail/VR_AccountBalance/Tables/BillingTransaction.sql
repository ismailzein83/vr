CREATE TABLE [VR_AccountBalance].[BillingTransaction] (
    [ID]                BIGINT           IDENTITY (1, 1) NOT NULL,
    [AccountTypeID]     UNIQUEIDENTIFIER NOT NULL,
    [AccountID]         VARCHAR (50)     NOT NULL,
    [TransactionTypeID] UNIQUEIDENTIFIER NOT NULL,
    [Amount]            DECIMAL (20, 6)  NOT NULL,
    [CurrencyId]        INT              NOT NULL,
    [TransactionTime]   DATETIME         NOT NULL,
    [Reference]         NVARCHAR (255)   NULL,
    [Notes]             NVARCHAR (1000)  NULL,
    [IsBalanceUpdated]  BIT              NULL,
    [ClosingPeriodId]   BIGINT           NULL,
    [CreatedTime]       DATETIME         CONSTRAINT [DF_BillingTransaction_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]         ROWVERSION       NULL,
    [IsDeleted]         BIT              NULL,
    CONSTRAINT [PK_BillingTransaction] PRIMARY KEY CLUSTERED ([ID] ASC)
);











