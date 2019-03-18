CREATE TABLE [RA_INTL_CDR].[BillingCDR_Charged] (
    [CDRID]                 BIGINT           NOT NULL,
    [DataSourceID]          UNIQUEIDENTIFIER NULL,
    [ProbeID]               BIGINT           NULL,
    [IDOnSwitch]            VARCHAR (255)    NULL,
    [AttemptDateTime]       DATETIME         NULL,
    [ConnectDateTime]       DATETIME         NULL,
    [DisconnectDateTime]    DATETIME         NULL,
    [DurationInSeconds]     DECIMAL (20, 4)  NULL,
    [CGPN]                  VARCHAR (40)     NULL,
    [CDPN]                  VARCHAR (40)     NULL,
    [TrafficDirection]      INT              NULL,
    [DestinationZoneID]     BIGINT           NULL,
    [OriginationZoneID]     BIGINT           NULL,
    [Rate]                  DECIMAL (22, 8)  NULL,
    [Revenue]               DECIMAL (22, 8)  NULL,
    [RateTypeID]            INT              NULL,
    [RateValueRuleID]       INT              NULL,
    [TariffRuleID]          INT              NULL,
    [RateTypeRuleID]        INT              NULL,
    [TaxRuleId]             INT              NULL,
    [Income]                DECIMAL (22, 8)  NULL,
    [DisconnectReason]      VARCHAR (200)    NULL,
    [AlertDateTime]         DATETIME         NULL,
    [SpecialNumberGroup]    BIGINT           NULL,
    [CurrencyID]            INT              NULL,
    [SaleDurationInSeconds] DECIMAL (20, 4)  NULL,
    [PDDInSeconds]          DECIMAL (20, 4)  NULL,
    [QueueItemId]           BIGINT           NULL,
    [OperatorID]            BIGINT           NULL,
    CONSTRAINT [IX_RA_INTL_BillingCDR_Main_ID] PRIMARY KEY NONCLUSTERED ([CDRID] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_RA_INTL_BillingCDR_Main_AttemptTime]
    ON [RA_INTL_CDR].[BillingCDR_Charged]([AttemptDateTime] ASC);

