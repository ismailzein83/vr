﻿CREATE TABLE [RA_CDR].[BillingCDR_Failed] (
    [CDRID]                  BIGINT           NOT NULL,
    [DataSourceID]           UNIQUEIDENTIFIER NULL,
    [ProbeID]                BIGINT           NULL,
    [IDOnSwitch]             VARCHAR (255)    NULL,
    [AttemptDateTime]        DATETIME         NULL,
    [ConnectDateTime]        DATETIME         NULL,
    [DisconnectDateTime]     DATETIME         NULL,
    [DurationInSeconds]      DECIMAL (20, 4)  NULL,
    [CDPN]                   VARCHAR (40)     NULL,
    [ReleaseCode]            VARCHAR (50)     NULL,
    [CGPN]                   VARCHAR (40)     NULL,
    [OperatorID]             BIGINT           NULL,
    [InterconnectOperatorID] BIGINT           NULL,
    [TrafficDirection]       INT              NULL,
    [ZoneID]                 BIGINT           NULL,
    [SaleDurationInSeconds]  DECIMAL (20, 4)  NULL,
    [SaleRate]               DECIMAL (20, 8)  NULL,
    [SaleAmount]             DECIMAL (22, 6)  NULL,
    [SaleRateTypeID]         INT              NULL,
    [SaleCurrencyID]         INT              NULL,
    [SaleRateValueRuleID]    INT              NULL,
    [SaleTariffRuleID]       INT              NULL,
    [SaleRateTypeRuleID]     INT              NULL,
    [QueueItemId]            BIGINT           NULL,
    [MyZoneID]               BIGINT           NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_BillingCDR_Failed_Operator]
    ON [RA_CDR].[BillingCDR_Failed]([OperatorID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_BillingCDR_Failed_ID]
    ON [RA_CDR].[BillingCDR_Failed]([CDRID] ASC);


GO
CREATE CLUSTERED INDEX [IX_BillingCDR_Failed_AttemptTime]
    ON [RA_CDR].[BillingCDR_Failed]([AttemptDateTime] ASC);

