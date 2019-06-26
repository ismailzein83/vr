CREATE TABLE [RA_Retail_Transaction].[ProcessedRawBillingPostpaidTransaction] (
    [ID]                  BIGINT           NOT NULL,
    [TransactionDateTime] DATETIME         NULL,
    [OperatorID]          BIGINT           NULL,
    [MSISDN]              NVARCHAR (255)   NULL,
    [Amount]              DECIMAL (22, 8)  NULL,
    [DataSourceID]        UNIQUEIDENTIFIER NULL,
    [TransactionTypeID]   UNIQUEIDENTIFIER NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

