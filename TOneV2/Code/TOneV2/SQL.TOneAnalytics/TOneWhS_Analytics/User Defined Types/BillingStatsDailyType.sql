CREATE TYPE [TOneWhS_Analytics].[BillingStatsDailyType] AS TABLE (
    [Id]                             BIGINT           NULL,
    [BatchStart]                     DATETIME         NULL,
    [SwitchID]                       INT              NULL,
    [CustomerId]                     INT              NULL,
    [SupplierId]                     INT              NULL,
    [CustomerProfileId]              INT              NULL,
    [SupplierProfileId]              INT              NULL,
    [SupplierZoneId]                 BIGINT           NULL,
    [SaleZoneId]                     BIGINT           NULL,
    [MasterPlanZoneID]               BIGINT           NULL,
    [SecondaryPlanZoneId]            BIGINT           NULL,
    [OriginatingZoneId]              BIGINT           NULL,
    [MasterPlanOriginatingZoneId]    BIGINT           NULL,
    [SecondaryPlanOriginatingZoneId] BIGINT           NULL,
    [CostCurrencyId]                 INT              NULL,
    [SaleCurrencyId]                 INT              NULL,
    [NumberOfCalls]                  INT              NULL,
    [FirstCallTime]                  DATETIME         NULL,
    [LastCallTime]                   DATETIME         NULL,
    [MinimumDurationInSeconds]       DECIMAL (20, 4)  NULL,
    [MaximumDurationInSeconds]       DECIMAL (20, 4)  NULL,
    [CostNet]                        DECIMAL (26, 10) NULL,
    [SaleNet]                        DECIMAL (26, 10) NULL,
    [SaleDurationInSeconds]          DECIMAL (20, 4)  NULL,
    [CostDurationInSeconds]          DECIMAL (20, 4)  NULL,
    [SaleRateId]                     BIGINT           NULL,
    [CostRateId]                     BIGINT           NULL,
    [DurationInSeconds]              DECIMAL (20, 4)  NULL,
    [CountryId]                      INT              NULL,
    [CostExtraCharges]               DECIMAL (26, 10) NULL,
    [SaleExtraCharges]               DECIMAL (26, 10) NULL,
    [SaleRateTypeId]                 INT              NULL,
    [CostRateTypeId]                 INT              NULL,
    [SaleRateValue]                  DECIMAL (20, 8)  NULL,
    [CostRateValue]                  DECIMAL (20, 8)  NULL,
    [OrigSaleDealID]                 INT              NULL,
    [OrigSaleDealZoneGroupNb]        INT              NULL,
    [SaleDealID]                     INT              NULL,
    [SaleDealZoneGroupNb]            INT              NULL,
    [SaleDealTierNb]                 INT              NULL,
    [SaleDealRateTierNb]             INT              NULL,
    [SaleDealDurInSec]               DECIMAL (20, 4)  NULL,
    [OrigCostDealID]                 INT              NULL,
    [OrigCostDealZoneGroupNb]        INT              NULL,
    [CostDealID]                     INT              NULL,
    [CostDealZoneGroupNb]            INT              NULL,
    [CostDealTierNb]                 INT              NULL,
    [CostDealRateTierNb]             INT              NULL,
    [CostDealDurInSec]               DECIMAL (20, 4)  NULL,
    [SaleFinancialAccountId]         INT              NULL,
    [CostFinancialAccountId]         INT              NULL,
    [Type]                           INT              NULL);

















