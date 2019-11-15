CREATE TABLE [RetailBilling].[AdvancedPayment] (
    [ID]                  BIGINT          IDENTITY (1, 1) NOT NULL,
    [BillingAccountID]    BIGINT          NULL,
    [ContractID]          BIGINT          NULL,
    [ContractServiceID]   BIGINT          NULL,
    [Amount]              DECIMAL (26, 6) NULL,
    [CurrencyID]          INT             NULL,
    [IncludedInInvoiceID] BIGINT          NULL,
    [CreatedTime]         DATETIME        NULL,
    [CreatedBy]           INT             NULL,
    [LastModifiedTime]    DATETIME        NULL,
    [LastModifiedBy]      INT             NULL,
    CONSTRAINT [PK__Advanced__3214EC272DB1C7EE] PRIMARY KEY CLUSTERED ([ID] ASC)
);

