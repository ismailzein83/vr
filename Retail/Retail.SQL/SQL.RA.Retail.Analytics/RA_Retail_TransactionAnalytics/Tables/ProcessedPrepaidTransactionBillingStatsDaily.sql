CREATE TABLE [RA_Retail_TransactionAnalytics].[ProcessedPrepaidTransactionBillingStatsDaily] (
    [ID]                   BIGINT           NOT NULL,
    [OperatorID]           BIGINT           NULL,
    [MSISDN]               NVARCHAR (255)   NULL,
    [PaymentMethod]        INT              NULL,
    [SubscriberID]         BIGINT           NULL,
    [DataSourceID]         UNIQUEIDENTIFIER NULL,
    [TopUpTypeID]          INT              NULL,
    [TotalAmount]          DECIMAL (22, 8)  NULL,
    [TotalPreviousBalance] DECIMAL (22, 8)  NULL,
    [TotalCurrentBalance]  DECIMAL (22, 8)  NULL,
    [TotalIncome]          DECIMAL (22, 8)  NULL,
    [TransactionTypeID]    UNIQUEIDENTIFIER NULL,
    [BatchStart]           DATETIME         NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

