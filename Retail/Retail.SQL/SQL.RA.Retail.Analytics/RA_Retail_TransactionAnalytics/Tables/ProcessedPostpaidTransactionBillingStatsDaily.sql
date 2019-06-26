CREATE TABLE [RA_Retail_TransactionAnalytics].[ProcessedPostpaidTransactionBillingStatsDaily] (
    [ID]                BIGINT           NOT NULL,
    [MSISDN]            NVARCHAR (255)   NULL,
    [OperatorID]        BIGINT           NULL,
    [SubscriberID]      BIGINT           NULL,
    [DataSourceID]      UNIQUEIDENTIFIER NULL,
    [TotalAmount]       DECIMAL (22, 8)  NULL,
    [TotalIncome]       DECIMAL (22, 8)  NULL,
    [TransactionTypeID] UNIQUEIDENTIFIER NULL,
    [BatchStart]        DATETIME         NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

