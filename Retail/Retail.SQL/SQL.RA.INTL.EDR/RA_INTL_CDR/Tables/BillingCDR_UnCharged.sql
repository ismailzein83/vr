CREATE TABLE [RA_INTL_CDR].[BillingCDR_UnCharged] (
    [CDRID]                 BIGINT           NOT NULL,
    [OperatorID]            BIGINT           NULL,
    [DataSourceID]          UNIQUEIDENTIFIER NULL,
    [ProbeID]               BIGINT           NULL,
    [IDOnSwitch]            VARCHAR (255)    NULL,
    [AttemptDateTime]       DATETIME         NULL,
    [ConnectDateTime]       DATETIME         NULL,
    [DisconnectDateTime]    DATETIME         NULL,
    [DurationInSeconds]     DECIMAL (20, 4)  NULL,
    [SaleDurationInSeconds] DECIMAL (20, 4)  NULL,
    [CDPN]                  VARCHAR (40)     NULL,
    [CGPN]                  VARCHAR (40)     NULL,
    [TrafficDirection]      INT              NULL,
    [DestinationZoneID]     BIGINT           NULL,
    [OriginationZoneID]     BIGINT           NULL,
    [Rate]                  DECIMAL (22, 8)  NULL,
    [Revenue]               DECIMAL (22, 8)  NULL,
    [RateTypeID]            INT              NULL,
    [RateValueRuleID]       INT              NULL,
    [TariffRuleID]          INT              NULL,
    [RateTypeRuleID]        INT              NULL,
    [TaxRuleID]             INT              NULL,
    [Income]                DECIMAL (22, 8)  NULL,
    [DisconnectReason]      VARCHAR (200)    NULL,
    [AlertDateTime]         DATETIME         NULL,
    [SpecialNumberGroup]    BIGINT           NULL,
    [CurrencyID]            INT              NULL,
    [PDDInSeconds]          DECIMAL (22, 8)  NULL,
    [QueueItemId]           BIGINT           NULL,
    CONSTRAINT [IX_RA_INTL_BillingCDR_UnCharged_CDRID] PRIMARY KEY NONCLUSTERED ([CDRID] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_RA_INTL_BillingCDR_UnCharged_AttemptDateTime]
    ON [RA_INTL_CDR].[BillingCDR_UnCharged]([AttemptDateTime] ASC);

