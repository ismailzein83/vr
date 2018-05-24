CREATE TYPE [TOneWhS_Analytics].[TrafficStatsDailyType] AS TABLE (
    [Id]                             BIGINT           NULL,
    [BatchStart]                     DATETIME         NULL,
    [SwitchID]                       INT              NULL,
    [CustomerID]                     INT              NULL,
    [SellingNumberPlanID]            INT              NULL,
    [CountryID]                      INT              NULL,
    [SaleZoneID]                     BIGINT           NULL,
    [MasterPlanZoneID]               BIGINT           NULL,
    [SecondaryPlanZoneId]            BIGINT           NULL,
    [OriginatingZoneID]              BIGINT           NULL,
    [MasterPlanOriginatingZoneID]    BIGINT           NULL,
    [SecondaryPlanOriginatingZoneId] BIGINT           NULL,
    [SupplierId]                     INT              NULL,
    [SupplierZoneId]                 BIGINT           NULL,
    [FirstCDRAttempt]                DATETIME         NULL,
    [LastCDRAttempt]                 DATETIME         NULL,
    [Attempts]                       INT              NULL,
    [DeliveredAttempts]              INT              NULL,
    [SuccessfulAttempts]             INT              NULL,
    [DurationInSeconds]              DECIMAL (20, 4)  NULL,
    [SumOfPDDInSeconds]              DECIMAL (25)     NULL,
    [MaxDurationInSeconds]           DECIMAL (20, 4)  NULL,
    [NumberOfCalls]                  INT              NULL,
    [DeliveredNumberOfCalls]         INT              NULL,
    [CeiledDuration]                 DECIMAL (20, 4)  NULL,
    [SumOfPGAD]                      DECIMAL (25)     NULL,
    [UtilizationInSeconds]           DECIMAL (25, 4)  NULL,
    [PricedCalls]                    INT              NULL,
    [SaleNet]                        DECIMAL (26, 10) NULL,
    [SaleDurationInSeconds]          DECIMAL (20, 4)  NULL,
    [CostNet]                        DECIMAL (26, 10) NULL,
    [CostDurationInSeconds]          DECIMAL (20, 4)  NULL,
    [SaleCurrencyId]                 INT              NULL,
    [CostCurrencyId]                 INT              NULL,
    [PortIN]                         VARCHAR (42)     NULL,
    [PortOUT]                        VARCHAR (42)     NULL,
    [CustomerProfileId]              INT              NULL,
    [SupplierProfileId]              INT              NULL,
    [ReleaseSourceSCount]            INT              NULL,
    [SaleExtraCharges]               DECIMAL (26, 10) NULL,
    [CostExtraCharges]               DECIMAL (26, 10) NULL,
    [SaleFinancialAccountId]         INT              NULL,
    [CostFinancialAccountId]         INT              NULL,
    [Type]                           INT              NULL,
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
    [CostDealDurInSec]               DECIMAL (20, 4)  NULL);



















