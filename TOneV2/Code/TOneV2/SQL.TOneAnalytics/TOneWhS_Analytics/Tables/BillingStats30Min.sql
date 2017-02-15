CREATE TABLE [TOneWhS_Analytics].[BillingStats30Min] (
    [Id]                       BIGINT           NULL,
    [BatchStart]               DATETIME         NULL,
    [CustomerId]               INT              NULL,
    [SupplierId]               INT              NULL,
    [CustomerProfileId]        INT              NULL,
    [SupplierProfileId]        INT              NULL,
    [SupplierZoneId]           BIGINT           NULL,
    [SaleZoneId]               BIGINT           NULL,
    [CostCurrencyId]           INT              NULL,
    [SaleCurrencyId]           INT              NULL,
    [NumberOfCalls]            INT              NULL,
    [FirstCallTime]            DATETIME         NULL,
    [LastCallTime]             DATETIME         NULL,
    [MinimumDurationInSeconds] DECIMAL (20, 4)  NULL,
    [MaximumDurationInSeconds] DECIMAL (20, 4)  NULL,
    [CostNet]                  DECIMAL (26, 10) NULL,
    [SaleNet]                  DECIMAL (26, 10) NULL,
    [SaleDurationInSeconds]    DECIMAL (20, 4)  NULL,
    [CostDurationInSeconds]    DECIMAL (20, 4)  NULL,
    [SaleRateId]               BIGINT           NULL,
    [CostRateId]               BIGINT           NULL,
    [DurationInSeconds]        DECIMAL (20, 4)  NULL,
    [CountryId]                INT              NULL,
    [CostExtraCharges]         DECIMAL (26, 10) NULL,
    [SaleExtraCharges]         DECIMAL (26, 10) NULL,
    [SaleRateTypeId]           INT              NULL,
    [CostRateTypeId]           INT              NULL,
    [SaleRateValue]            DECIMAL (20, 8)  NULL,
    [CostRateValue]            DECIMAL (20, 8)  NULL,
    CONSTRAINT [IX_BillingStats30Min_Id] UNIQUE NONCLUSTERED ([Id] ASC)
);




GO
CREATE CLUSTERED INDEX [IX_BillingStats30Min_BatchStart]
    ON [TOneWhS_Analytics].[BillingStats30Min]([BatchStart] ASC);

