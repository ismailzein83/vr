CREATE TABLE [TOneWhS_BE].[CustomerBillingRecurringCharge] (
    [ID]                 BIGINT          IDENTITY (1, 1) NOT NULL,
    [FinancialAccountId] INT             NULL,
    [RecurringChargeId]  BIGINT          NULL,
    [InvoiceId]          BIGINT          NULL,
    [Amount]             DECIMAL (22, 6) NULL,
    [From]               DATETIME        NULL,
    [To]                 DATETIME        NULL,
    [CurrencyId]         INT             NULL,
    [CreatedTime]        DATETIME        CONSTRAINT [DF_CustomerBillingRecurringCharge_CreatedTime] DEFAULT (getdate()) NULL,
    [CreatedBy]          INT             NULL,
    [VAT]                DECIMAL (22, 6) NULL,
    CONSTRAINT [PK_CustomerBillingRecurringCharge] PRIMARY KEY CLUSTERED ([ID] ASC)
);

