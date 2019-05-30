﻿CREATE TABLE [TOneWhS_CDR].[BillingCDR_Interconnect] (
    [CDRId]                          BIGINT          NOT NULL,
    [SwitchId]                       INT             NULL,
    [AttemptDateTime]                DATETIME        NULL,
    [AlertDateTime]                  DATETIME        NULL,
    [ConnectDateTime]                DATETIME        NULL,
    [DisconnectDateTime]             DATETIME        NULL,
    [PDDInSeconds]                   DECIMAL (20, 4) NULL,
    [DurationInSeconds]              DECIMAL (20, 4) NULL,
    [CustomerID]                     INT             NULL,
    [CustomerAccountManagerId]       INT             NULL,
    [SellingNumberPlanID]            INT             NULL,
    [SaleZoneID]                     BIGINT          NULL,
    [SaleCode]                       VARCHAR (20)    NULL,
    [MasterPlanZoneID]               BIGINT          NULL,
    [MasterPlanCode]                 VARCHAR (20)    NULL,
    [SecondaryPlanZoneId]            BIGINT          NULL,
    [SecondaryPlanCode]              VARCHAR (20)    NULL,
    [OriginatingZoneID]              BIGINT          NULL,
    [MasterPlanOriginatingZoneId]    BIGINT          NULL,
    [SecondaryPlanOriginatingZoneId] BIGINT          NULL,
    [SupplierID]                     INT             NULL,
    [SupplierAccountManagerId]       INT             NULL,
    [SupplierZoneID]                 BIGINT          NULL,
    [CGPN]                           VARCHAR (50)    NULL,
    [OrigCGPN]                       VARCHAR (50)    NULL,
    [CDPN]                           VARCHAR (50)    NULL,
    [OrigCDPN]                       VARCHAR (50)    NULL,
    [OrigCDPNIn]                     VARCHAR (50)    NULL,
    [OrigCDPNOut]                    VARCHAR (50)    NULL,
    [SupplierCode]                   VARCHAR (20)    NULL,
    [IDonSwitch]                     BIGINT          NULL,
    [Tag]                            VARCHAR (100)   NULL,
    [SIP]                            VARCHAR (100)   NULL,
    [IsRerouted]                     BIT             NULL,
    [ReleaseCode]                    VARCHAR (50)    NULL,
    [ReleaseSource]                  VARCHAR (10)    NULL,
    [IsDelivered]                    BIT             NULL,
    [PortIN]                         VARCHAR (42)    NULL,
    [PortOUT]                        VARCHAR (42)    NULL,
    [CountryId]                      INT             NULL,
    [SaleFinancialAccountId]         INT             NULL,
    [CostFinancialAccountId]         INT             NULL,
    [Type]                           INT             NULL,
    [QueueItemId]                    BIGINT          NULL,
    [ExtraFields]                    NVARCHAR (MAX)  NULL
);














GO
CREATE NONCLUSTERED INDEX [IX_BillingCDR_Interconnect_CDRId]
    ON [TOneWhS_CDR].[BillingCDR_Interconnect]([CDRId] ASC);


GO
CREATE CLUSTERED INDEX [IX_BillingCDR_Interconnect_AttemptDateTime]
    ON [TOneWhS_CDR].[BillingCDR_Interconnect]([AttemptDateTime] ASC);

