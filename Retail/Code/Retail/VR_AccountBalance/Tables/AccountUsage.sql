CREATE TABLE [VR_AccountBalance].[AccountUsage] (
    [ID]                        BIGINT           IDENTITY (1, 1) NOT NULL,
    [AccountTypeID]             UNIQUEIDENTIFIER NOT NULL,
    [TransactionTypeID]         UNIQUEIDENTIFIER NOT NULL,
    [AccountID]                 BIGINT           NOT NULL,
    [CurrencyId]                INT              NOT NULL,
    [PeriodStart]               DATETIME         NOT NULL,
    [PeriodEnd]                 DATETIME         NOT NULL,
    [UsageBalance]              DECIMAL (20, 6)  NOT NULL,
    [BillingTransactionNote]    NVARCHAR (1000)  NULL,
    [BillingTransactionID]      BIGINT           NULL,
    [ShouldRecreateTransaction] BIT              NULL,
    [CreatedTime]               DATETIME         CONSTRAINT [DF_AccountUsage_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]                 ROWVERSION       NULL,
    CONSTRAINT [PK_AccountUsage] PRIMARY KEY CLUSTERED ([ID] ASC)
);




GO
CREATE NONCLUSTERED INDEX [IX_AccountUsage]
    ON [VR_AccountBalance].[AccountUsage]([AccountID] ASC, [AccountTypeID] ASC, [TransactionTypeID] ASC, [PeriodStart] ASC);

