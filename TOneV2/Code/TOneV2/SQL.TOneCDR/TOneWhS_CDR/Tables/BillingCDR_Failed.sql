CREATE TABLE [TOneWhS_CDR].[BillingCDR_Failed] (
    [CDRId]                       BIGINT          NOT NULL,
    [AttemptDateTime]             DATETIME        NULL,
    [AlertDateTime]               DATETIME        NULL,
    [ConnectDateTime]             DATETIME        NULL,
    [DisconnectDateTime]          DATETIME        NULL,
    [DurationInSeconds]           DECIMAL (20, 4) NULL,
    [CustomerID]                  INT             NULL,
    [SaleZoneID]                  BIGINT          NULL,
    [OriginatingZoneID]           BIGINT          NULL,
    [SupplierID]                  INT             NULL,
    [CGPN]                        VARCHAR (50)    NULL,
    [CDPN]                        VARCHAR (50)    NULL,
    [CDPNIn]                      VARCHAR (50)    NULL,
    [CDPNOut]                     VARCHAR (50)    NULL,
    [SaleCode]                    VARCHAR (20)    NULL,
    [SupplierCode]                VARCHAR (20)    NULL,
    [IDonSwitch]                  BIGINT          NULL,
    [Tag]                         VARCHAR (100)   NULL,
    [SIP]                         VARCHAR (100)   NULL,
    [IsRerouted]                  BIT             NULL,
    [SupplierZoneID]              BIGINT          NULL,
    [ReleaseCode]                 VARCHAR (50)    NULL,
    [ReleaseSource]               VARCHAR (10)    NULL,
    [SellingNumberPlanID]         INT             NULL,
    [MasterPlanZoneID]            BIGINT          NULL,
    [MasterPlanCode]              VARCHAR (20)    NULL,
    [MasterPlanOriginatingZoneId] BIGINT          NULL,
    [PortIN]                      VARCHAR (42)    NULL,
    [PortOUT]                     VARCHAR (42)    NULL,
    [SwitchId]                    INT             NULL,
    [CountryId]                   INT             NULL
);










GO
CREATE CLUSTERED INDEX [IX_BillingCDR_Failed_AttemptDateTime]
    ON [TOneWhS_CDR].[BillingCDR_Failed]([AttemptDateTime] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_BillingCDR_Failed_CDRId]
    ON [TOneWhS_CDR].[BillingCDR_Failed]([CDRId] ASC);

