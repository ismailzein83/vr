﻿CREATE TYPE [TOneWhS_CDR].[BillingCDR_InvalidType] AS TABLE (
    [CDRId]                       BIGINT          NULL,
    [SwitchId]                    INT             NULL,
    [AttemptDateTime]             DATETIME        NULL,
    [AlertDateTime]               DATETIME        NULL,
    [ConnectDateTime]             DATETIME        NULL,
    [DisconnectDateTime]          DATETIME        NULL,
    [DurationInSeconds]           DECIMAL (20, 4) NULL,
    [PDDInSeconds]                DECIMAL (20, 4) NULL,
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
    [CGPN]                        VARCHAR (50)    NULL,
    [CDPN]                        VARCHAR (50)    NULL,
    [CDPNIn]                      VARCHAR (50)    NULL,
    [CDPNOut]                     VARCHAR (50)    NULL,
    [SupplierCode]                VARCHAR (20)    NULL,
    [IDonSwitch]                  BIGINT          NULL,
    [Tag]                         VARCHAR (100)   NULL,
    [SIP]                         VARCHAR (100)   NULL,
    [IsRerouted]                  BIT             NULL,
    [SaleRateId]                  BIGINT          NULL,
    [CostRateId]                  BIGINT          NULL,
    [ReleaseCode]                 VARCHAR (50)    NULL,
    [ReleaseSource]               VARCHAR (10)    NULL,
    [IsDelivered]                 BIT             NULL,
    [PortIN]                      VARCHAR (42)    NULL,
    [PortOUT]                     VARCHAR (42)    NULL,
    [CountryId]                   INT             NULL,
    [SaleTariffRuleId]            INT             NULL,
    [CostTariffRuleId]            INT             NULL,
    [QueueItemId]                 BIGINT          NULL);















