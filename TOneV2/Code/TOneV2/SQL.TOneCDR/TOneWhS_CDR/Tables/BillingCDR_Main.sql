﻿CREATE TABLE [TOneWhS_CDR].[BillingCDR_Main] (
    [CDRId]                        BIGINT          NOT NULL,
    [AttemptDateTime]              DATETIME        NOT NULL,
    [AlertDateTime]                DATETIME        NULL,
    [ConnectDateTime]              DATETIME        NULL,
    [DisconnectDateTime]           DATETIME        NULL,
    [PDDInSeconds]                 DECIMAL (20, 4) NULL,
    [DurationInSeconds]            DECIMAL (20, 4) NULL,
    [CustomerID]                   INT             NULL,
    [SaleZoneID]                   BIGINT          NULL,
    [OriginatingZoneID]            BIGINT          NULL,
    [SupplierID]                   INT             NULL,
    [SupplierZoneID]               BIGINT          NULL,
    [CGPN]                         VARCHAR (50)    NULL,
    [CDPN]                         VARCHAR (50)    NULL,
    [CDPNIn]                       VARCHAR (50)    NULL,
    [CDPNOut]                      VARCHAR (50)    NULL,
    [SaleCode]                     VARCHAR (20)    NULL,
    [SupplierCode]                 VARCHAR (20)    NULL,
    [IDonSwitch]                   BIGINT          NULL,
    [Tag]                          VARCHAR (100)   NULL,
    [SIP]                          VARCHAR (100)   NULL,
    [IsRerouted]                   BIT             NULL,
    [ReleaseCode]                  VARCHAR (50)    NULL,
    [ReleaseSource]                VARCHAR (10)    NULL,
    [IsDelivered]                  BIT             NULL,
    [SellingNumberPlanID]          INT             NULL,
    [MasterPlanZoneID]             BIGINT          NULL,
    [MasterPlanCode]               VARCHAR (20)    NULL,
    [MasterPlanOriginatingZoneId]  BIGINT          NULL,
    [PortIN]                       VARCHAR (42)    NULL,
    [PortOUT]                      VARCHAR (42)    NULL,
    [SwitchId]                     INT             NULL,
    [CountryId]                    INT             NULL,
    [SaleRateID]                   BIGINT          NULL,
    [SaleRateValue]                DECIMAL (20, 8) NULL,
    [SaleRateTypeID]               INT             NULL,
    [SaleNet]                      DECIMAL (22, 6) NULL,
    [SaleExtraChargeRateValue]     DECIMAL (20, 8) NULL,
    [SaleExtraChargeValue]         DECIMAL (22, 6) NULL,
    [SaleCurrencyId]               INT             NULL,
    [SaleDurationInSeconds]        DECIMAL (20, 4) NULL,
    [SaleRateTypeRuleId]           INT             NULL,
    [SaleExtraChargeRuleId]        INT             NULL,
    [SaleTariffRuleId]             INT             NULL,
    [OrigSaleRateID]               BIGINT          NULL,
    [OrigSaleRateValue]            DECIMAL (20, 8) NULL,
    [OrigSaleNet]                  DECIMAL (22, 6) NULL,
    [OrigSaleDealID]               INT             NULL,
    [OrigSaleDealZoneGroupNb]      INT             NULL,
    [OrigSaleExtraChargeRateValue] DECIMAL (20, 8) NULL,
    [OrigSaleExtraChargeValue]     DECIMAL (22, 6) NULL,
    [OrigSaleDurationInSeconds]    DECIMAL (20, 4) NULL,
    [OrigSaleCurrencyId]           INT             NULL,
    [SaleDealID]                   INT             NULL,
    [SaleDealZoneGroupNb]          INT             NULL,
    [SaleDealTierNb]               INT             NULL,
    [SaleDealRateTierNb]           INT             NULL,
    [SaleDealDurInSec]             DECIMAL (20, 4) NULL,
    [SecondarySaleDealTierNb]      INT             NULL,
    [SecondarySaleDealRateTierNb]  INT             NULL,
    [SecondarySaleDealDurInSec]    DECIMAL (20, 4) NULL,
    [CostRateID]                   BIGINT          NULL,
    [CostRateValue]                DECIMAL (20, 8) NULL,
    [CostRateTypeID]               INT             NULL,
    [CostNet]                      DECIMAL (22, 6) NULL,
    [CostExtraChargeRateValue]     DECIMAL (20, 8) NULL,
    [CostExtraChargeValue]         DECIMAL (22, 6) NULL,
    [CostCurrencyID]               INT             NULL,
    [CostDurationInSeconds]        DECIMAL (20, 4) NULL,
    [CostRateTypeRuleId]           INT             NULL,
    [CostExtraChargeRuleId]        INT             NULL,
    [CostTariffRuleId]             INT             NULL,
    [OrigCostRateID]               BIGINT          NULL,
    [OrigCostRateValue]            DECIMAL (20, 8) NULL,
    [OrigCostNet]                  DECIMAL (22, 6) NULL,
    [OrigCostDealID]               INT             NULL,
    [OrigCostDealZoneGroupNb]      INT             NULL,
    [OrigCostExtraChargeRateValue] DECIMAL (20, 8) NULL,
    [OrigCostExtraChargeValue]     DECIMAL (22, 6) NULL,
    [OrigCostDurationInSeconds]    DECIMAL (20, 4) NULL,
    [OrigCostCurrencyId]           INT             NULL,
    [CostDealID]                   INT             NULL,
    [CostDealZoneGroupNb]          INT             NULL,
    [CostDealTierNb]               INT             NULL,
    [CostDealRateTierNb]           INT             NULL,
    [CostDealDurInSec]             DECIMAL (20, 4) NULL,
    [SecondaryCostDealTierNb]      INT             NULL,
    [SecondaryCostDealRateTierNb]  INT             NULL,
    [SecondaryCostDealDurInSec]    DECIMAL (20, 4) NULL,
    [QueueItemId]                  BIGINT          NULL,
    [SaleFinancialAccountId]       INT             NULL,
    [CostFinancialAccountId]       INT             NULL
);


























GO
CREATE CLUSTERED INDEX [IX_BillingCDR_Main_AttemptDateTime]
    ON [TOneWhS_CDR].[BillingCDR_Main]([AttemptDateTime] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_BillingCDR_Main_CDRId]
    ON [TOneWhS_CDR].[BillingCDR_Main]([CDRId] ASC);

