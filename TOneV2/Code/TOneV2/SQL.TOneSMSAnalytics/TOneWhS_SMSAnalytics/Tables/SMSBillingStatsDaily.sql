CREATE TABLE [TOneWhS_SMSAnalytics].[SMSBillingStatsDaily] (
    [Id]                     BIGINT           NULL,
    [BatchStart]             DATETIME         NULL,
    [SwitchID]               INT              NULL,
    [CustomerId]             INT              NULL,
    [CustomerProfileId]      INT              NULL,
    [SupplierId]             INT              NULL,
    [SupplierProfileId]      INT              NULL,
    [OriginationMN_Id]       INT              NULL,
    [OriginationMC_Id]       INT              NULL,
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
    CONSTRAINT [IX_SMSBillingStatsDaily_Id] UNIQUE NONCLUSTERED ([Id] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_SMSBillingStatsDaily_BatchStart]
    ON [TOneWhS_SMSAnalytics].[SMSBillingStatsDaily]([BatchStart] ASC);

