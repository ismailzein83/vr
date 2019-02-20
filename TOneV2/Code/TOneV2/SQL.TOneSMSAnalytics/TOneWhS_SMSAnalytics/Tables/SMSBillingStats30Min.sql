CREATE TABLE [TOneWhS_SMSAnalytics].[SMSBillingStats30Min] (
    [Id]                     BIGINT           NULL,
    [BatchStart]             DATETIME         NULL,
    [SwitchID]               INT              NULL,
    [CustomerId]             INT              NULL,
    [CustomerProfileId]      INT              NULL,
    [SupplierId]             INT              NULL,
    [SupplierProfileId]      INT              NULL,
    [OriginationMC_Id]       INT              NULL,
    [OriginationMN_Id]       INT              NULL,
    [DestinationMC_Id]       INT              NULL,
    [DestinationMN_Id]       INT              NULL,
    [NumberOfSMS]            INT              NULL,
    [FirstSMSTime]           DATETIME         NULL,
    [LastSMSTime]            DATETIME         NULL,
    [SaleRateId]             BIGINT           NULL,
    [SaleRateValue]          DECIMAL (20, 8)  NULL,
    [SaleNet]                DECIMAL (26, 10) NULL,
    [SaleCurrencyId]         INT              NULL,
    [CostRateId]             BIGINT           NULL,
    [CostRateValue]          DECIMAL (20, 8)  NULL,
    [CostNet]                DECIMAL (26, 10) NULL,
    [CostCurrencyId]         INT              NULL,
    [SaleFinancialAccountId] INT              NULL,
    [CostFinancialAccountId] INT              NULL,
    [Type]                   INT              NULL,
    CONSTRAINT [IX_SMSBillingStats30Min_Id] UNIQUE NONCLUSTERED ([Id] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_SMSBillingStats30Min_BatchStart]
    ON [TOneWhS_SMSAnalytics].[SMSBillingStats30Min]([BatchStart] ASC);

