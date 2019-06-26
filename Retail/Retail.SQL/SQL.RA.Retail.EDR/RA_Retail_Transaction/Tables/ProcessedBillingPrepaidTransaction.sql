CREATE TABLE [RA_Retail_Transaction].[ProcessedBillingPrepaidTransaction] (
    [ID]                  BIGINT           NOT NULL,
    [TransactionDateTime] DATETIME         NULL,
    [OperatorID]          BIGINT           NULL,
    [MSISDN]              NVARCHAR (255)   NULL,
    [Amount]              DECIMAL (22, 8)  NULL,
    [PreviousBalance]     DECIMAL (22, 8)  NULL,
    [CurrentBalance]      DECIMAL (22, 8)  NULL,
    [PaymentMethod]       INT              NULL,
    [SubscriberID]        BIGINT           NULL,
    [DataSourceID]        UNIQUEIDENTIFIER NULL,
    [Income]              DECIMAL (22, 8)  NULL,
    [TopUpTypeID]         INT              NULL,
    [TransactionTypeID]   UNIQUEIDENTIFIER NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

