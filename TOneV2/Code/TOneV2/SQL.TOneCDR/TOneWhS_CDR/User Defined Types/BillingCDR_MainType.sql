﻿CREATE TYPE [TOneWhS_CDR].[BillingCDR_MainType] AS TABLE (
    [CDRId]                       BIGINT          NULL,
    [SwitchId]                    INT             NULL,
    [AttemptDateTime]             DATETIME        NULL,
    [AlertDateTime]               DATETIME        NULL,
    [ConnectDateTime]             DATETIME        NULL,
    [DisconnectDateTime]          DATETIME        NULL,
    [DurationInSeconds]           DECIMAL (20, 4) NULL,
    [CustomerID]                  INT             NULL,
    [SellingNumberPlanID]         INT             NULL,
    [SaleZoneID]                  BIGINT          NULL,
    [SaleCode]                    VARCHAR (20)    NULL,
    [MasterPlanZoneID]            BIGINT          NULL,
    [MasterPlanCode]              VARCHAR (20)    NULL,
    [OriginatingZoneID]           BIGINT          NULL,
    [MasterPlanOriginatingZoneId] BIGINT          NULL,
    [SupplierID]                  INT             NULL,
    [SupplierZoneID]              BIGINT          NULL,
    [CDPN]                        VARCHAR (50)    NULL,
    [CGPN]                        VARCHAR (50)    NULL,
    [SupplierCode]                VARCHAR (20)    NULL,
    [CDPNOut]                     VARCHAR (50)    NULL,
    [IDonSwitch]                  BIGINT          NULL,
    [Tag]                         VARCHAR (100)   NULL,
    [SIP]                         VARCHAR (100)   NULL,
    [IsRerouted]                  BIT             NULL,
    [SaleRateID]                  BIGINT          NULL,
    [SaleRateValue]               DECIMAL (20, 8) NULL,
    [SaleRateTypeID]              INT             NULL,
    [SaleNet]                     DECIMAL (22, 6) NULL,
    [SaleCurrencyId]              INT             NULL,
    [SaleDurationInSeconds]       DECIMAL (20, 4) NULL,
    [CostRateID]                  BIGINT          NULL,
    [CostRateValue]               DECIMAL (20, 8) NULL,
    [CostRateTypeID]              INT             NULL,
    [CostNet]                     DECIMAL (22, 6) NULL,
    [CostCurrencyID]              INT             NULL,
    [CostDurationInSeconds]       DECIMAL (20, 4) NULL,
    [ReleaseCode]                 VARCHAR (50)    NULL,
    [ReleaseSource]               VARCHAR (10)    NULL,
    [PortIN]                      VARCHAR (42)    NULL,
    [PortOUT]                     VARCHAR (42)    NULL,
    [CostRateTypeRuleId]          INT             NULL,
    [SaleRateTypeRuleId]          INT             NULL,
    [CostTariffRuleId]            INT             NULL,
    [SaleTariffRuleId]            INT             NULL,
    [CostExtraChargeRuleId]       INT             NULL,
    [SaleExtraChargeRuleId]       INT             NULL,
    [CostExtraChargeValue]        DECIMAL (22, 6) NULL,
    [SaleExtraChargeValue]        DECIMAL (22, 6) NULL,
    [CDPNIn]                      VARCHAR (50)    NULL,
    [CountryId]                   INT             NULL,
    [SaleExtraChargeRateValue]    DECIMAL (20, 8) NULL,
    [CostExtraChargeRateValue]    DECIMAL (20, 8) NULL);













