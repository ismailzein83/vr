﻿CREATE TYPE [TOneWhS_CDR].[BillingCDR_InvalidType] AS TABLE (
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
    [ReleaseCode]                 VARCHAR (50)    NULL,
    [ReleaseSource]               VARCHAR (10)    NULL,
    [PortIN]                      VARCHAR (42)    NULL,
    [PortOUT]                     VARCHAR (42)    NULL,
    [CostRateId]                  BIGINT          NULL,
    [SaleRateId]                  BIGINT          NULL);





