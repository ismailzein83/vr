CREATE TABLE [RA_Retail_Transaction].[ProcessedBillingPostpaidTransaction] (
    [ID]                  BIGINT           NOT NULL,
    [TransactionDateTime] DATETIME         NULL,
    [MSISDN]              NVARCHAR (255)   NULL,
    [Amount]              DECIMAL (22, 8)  NULL,
    [OperatorID]          BIGINT           NULL,
    [DataSourceID]        UNIQUEIDENTIFIER NULL,
    [SubscriberID]        BIGINT           NULL,
    [Income]              DECIMAL (22, 8)  NULL,
    [TransactionTypeID]   UNIQUEIDENTIFIER NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

