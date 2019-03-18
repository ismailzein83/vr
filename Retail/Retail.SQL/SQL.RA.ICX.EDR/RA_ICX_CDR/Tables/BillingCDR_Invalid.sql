CREATE TABLE [RA_ICX_CDR].[BillingCDR_Invalid] (
    [CDRID]                  BIGINT           NOT NULL,
    [OperatorID]             BIGINT           NULL,
    [InterconnectOperatorID] BIGINT           NULL,
    [TrafficDirection]       INT              NULL,
    [DataSourceID]           UNIQUEIDENTIFIER NULL,
    [ProbeID]                BIGINT           NULL,
    [IDOnSwitch]             VARCHAR (MAX)    NULL,
    [AttemptDateTime]        DATETIME         NOT NULL,
    [ConnectDateTime]        DATETIME         NULL,
    [DisconnectDateTime]     DATETIME         NULL,
    [DurationInSeconds]      DECIMAL (20, 4)  NULL,
    [SaleDurationInSeconds]  DECIMAL (20, 4)  NULL,
    [DisconnectReason]       VARCHAR (200)    NULL,
    [AlertDateTime]          DATETIME         NULL,
    [CGPN]                   VARCHAR (40)     NULL,
    [CDPN]                   VARCHAR (40)     NULL,
    [OriginationZoneID]      BIGINT           NULL,
    [DestinationZoneID]      BIGINT           NULL,
    [Rate]                   DECIMAL (22, 8)  NULL,
    [SpecialNumberGroup]     BIGINT           NULL,
    [RateTypeID]             INT              NULL,
    [RateValueRuleID]        INT              NULL,
    [RateTypeRuleID]         INT              NULL,
    [TariffRuleID]           INT              NULL,
    [Revenue]                DECIMAL (22, 8)  NULL,
    [Income]                 DECIMAL (22, 8)  NULL,
    [TaxRuleId]              INT              NULL,
    [CurrencyID]             INT              NULL,
    [PDDInSeconds]           DECIMAL (20, 4)  NULL,
    [QueueItemId]            BIGINT           NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_RA_ICX_BillingCDR_Invalid_AttemptDateTime]
    ON [RA_ICX_CDR].[BillingCDR_Invalid]([AttemptDateTime] ASC);

