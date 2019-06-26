CREATE TABLE [RA_Retail_Transaction].[ProcessedRawBillingPrepaidTransaction] (
    [ID]                  BIGINT           NOT NULL,
    [TransactionDateTime] DATETIME         NULL,
    [OperatorID]          BIGINT           NULL,
    [MSISDN]              NVARCHAR (255)   NULL,
    [Amount]              DECIMAL (22, 8)  NULL,
    [PreviousBalance]     DECIMAL (22, 8)  NULL,
    [CurrentBalance]      DECIMAL (22, 8)  NULL,
    [PaymentMethod]       INT              NULL,
    [DataSourceID]        UNIQUEIDENTIFIER NULL,
    [SourceID]            NVARCHAR (255)   NULL,
    [TransactionTypeID]   UNIQUEIDENTIFIER NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

