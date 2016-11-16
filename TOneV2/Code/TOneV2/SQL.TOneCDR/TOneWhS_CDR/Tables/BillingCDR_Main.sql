CREATE TABLE [TOneWhS_CDR].[BillingCDR_Main] (
    [CDRId]                       BIGINT          NOT NULL,
    [AttemptDateTime]             DATETIME        NOT NULL,
    [AlertDateTime]               DATETIME        NULL,
    [ConnectDateTime]             DATETIME        NULL,
    [DisconnectDateTime]          DATETIME        NULL,
    [DurationInSeconds]           DECIMAL (20, 4) NULL,
    [CustomerID]                  INT             NULL,
    [SaleZoneID]                  BIGINT          NULL,
    [OriginatingZoneID]           BIGINT          NULL,
    [SupplierID]                  INT             NULL,
    [SupplierZoneID]              BIGINT          NULL,
    [CDPN]                        VARCHAR (50)    NULL,
    [CGPN]                        VARCHAR (50)    NULL,
    [SaleCode]                    VARCHAR (20)    NULL,
    [SupplierCode]                VARCHAR (20)    NULL,
    [CDPNOut]                     VARCHAR (50)    NULL,
    [CDPNIn]                      VARCHAR (50)    NULL,
    [IDonSwitch]                  BIGINT          NULL,
    [Tag]                         VARCHAR (100)   NULL,
    [SIP]                         VARCHAR (100)   NULL,
    [IsRerouted]                  BIT             NULL,
    [SaleRateID]                  BIGINT          NULL,
    [SaleRateValue]               DECIMAL (20, 8) NULL,
    [SaleRateTypeID]              INT             NULL,
    [SaleNet]                     DECIMAL (20, 4) NULL,
    [SaleCurrencyId]              INT             NULL,
    [SaleDurationInSeconds]       DECIMAL (20, 4) NULL,
    [CostRateID]                  BIGINT          NULL,
    [CostRateValue]               DECIMAL (20, 8) NULL,
    [CostRateTypeID]              INT             NULL,
    [CostNet]                     DECIMAL (20, 4) NULL,
    [CostCurrencyID]              INT             NULL,
    [CostDurationInSeconds]       DECIMAL (20, 4) NULL,
    [ReleaseCode]                 VARCHAR (50)    NULL,
    [ReleaseSource]               VARCHAR (10)    NULL,
    [SellingNumberPlanID]         INT             NULL,
    [MasterPlanZoneID]            BIGINT          NULL,
    [MasterPlanCode]              VARCHAR (20)    NULL,
    [MasterPlanOriginatingZoneId] BIGINT          NULL,
    [PortIN]                      VARCHAR (42)    NULL,
    [PortOUT]                     VARCHAR (42)    NULL,
    [SwitchId]                    INT             NULL,
    [CostRateTypeRuleId]          INT             NULL,
    [SaleRateTypeRuleId]          INT             NULL,
    [CostTariffRuleId]            INT             NULL,
    [SaleTariffRuleId]            INT             NULL,
    [CostExtraChargeRuleId]       INT             NULL,
    [SaleExtraChargeRuleId]       INT             NULL,
    [CostExtraChargeValue]        DECIMAL (20, 4) NULL,
    [SaleExtraChargeValue]        DECIMAL (20, 4) NULL,
    [CountryId]                   INT             NULL
);












GO
CREATE CLUSTERED INDEX [IX_BillingCDR_Main_AttemptDateTime]
    ON [TOneWhS_CDR].[BillingCDR_Main]([AttemptDateTime] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_BillingCDR_Main_CDRId]
    ON [TOneWhS_CDR].[BillingCDR_Main]([CDRId] ASC);

