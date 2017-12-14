CREATE TABLE [TOneWhS_Analytics].[BillingStats30Min] (
    [Id]                       BIGINT           NULL,
    [BatchStart]               DATETIME         NULL,
    [SwitchID]                 INT              NULL,
    [CustomerId]               INT              NULL,
    [CustomerProfileId]        INT              NULL,
    [SaleZoneId]               BIGINT           NULL,
    [MasterPlanZoneID]         BIGINT           NULL,
    [SupplierId]               INT              NULL,
    [SupplierProfileId]        INT              NULL,
    [SupplierZoneId]           BIGINT           NULL,
    [NumberOfCalls]            INT              NULL,
    [FirstCallTime]            DATETIME         NULL,
    [LastCallTime]             DATETIME         NULL,
    [DurationInSeconds]        DECIMAL (20, 4)  NULL,
    [MinimumDurationInSeconds] DECIMAL (20, 4)  NULL,
    [MaximumDurationInSeconds] DECIMAL (20, 4)  NULL,
    [CountryId]                INT              NULL,
    [SaleCurrencyId]           INT              NULL,
    [SaleRateId]               BIGINT           NULL,
    [SaleRateValue]            DECIMAL (20, 8)  NULL,
    [SaleRateTypeId]           INT              NULL,
    [SaleExtraCharges]         DECIMAL (26, 10) NULL,
    [SaleDurationInSeconds]    DECIMAL (20, 4)  NULL,
    [SaleNet]                  DECIMAL (26, 10) NULL,
    [OrigSaleDealID]           INT              NULL,
    [OrigSaleDealZoneGroupNb]  INT              NULL,
    [SaleDealID]               INT              NULL,
    [SaleDealZoneGroupNb]      INT              NULL,
    [SaleDealTierNb]           INT              NULL,
    [SaleDealRateTierNb]       INT              NULL,
    [SaleDealDurInSec]         DECIMAL (20, 4)  NULL,
    [CostCurrencyId]           INT              NULL,
    [CostRateId]               BIGINT           NULL,
    [CostRateValue]            DECIMAL (20, 8)  NULL,
    [CostRateTypeId]           INT              NULL,
    [CostExtraCharges]         DECIMAL (26, 10) NULL,
    [CostDurationInSeconds]    DECIMAL (20, 4)  NULL,
    [CostNet]                  DECIMAL (26, 10) NULL,
    [OrigCostDealID]           INT              NULL,
    [OrigCostDealZoneGroupNb]  INT              NULL,
    [CostDealID]               INT              NULL,
    [CostDealZoneGroupNb]      INT              NULL,
    [CostDealTierNb]           INT              NULL,
    [CostDealRateTierNb]       INT              NULL,
    [CostDealDurInSec]         DECIMAL (20, 4)  NULL,
    [SaleFinancialAccountId]   INT              NULL,
    [CostFinancialAccountId]   INT              NULL,
    [Type]                     INT              NULL,
    CONSTRAINT [IX_BillingStats30Min_Id] UNIQUE NONCLUSTERED ([Id] ASC)
);














GO
CREATE CLUSTERED INDEX [IX_BillingStats30Min_BatchStart]
    ON [TOneWhS_Analytics].[BillingStats30Min]([BatchStart] ASC);

