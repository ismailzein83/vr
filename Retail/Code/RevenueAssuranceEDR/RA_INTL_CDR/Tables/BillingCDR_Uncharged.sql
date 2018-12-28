CREATE TABLE [RA_INTL_CDR].[BillingCDR_UnCharged] (
    [CDRID]                 BIGINT           NOT NULL,
    [DurationInSeconds]     DECIMAL (20, 4)  NULL,
    [SaleDurationInSeconds] DECIMAL (20, 4)  NULL,
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
    [QueueItemId]           BIGINT           NULL,
    [DataSourceID]          UNIQUEIDENTIFIER NULL,
    [ProbeID]               BIGINT           NULL,
    [IDOnSwitch]            VARCHAR (255)    NULL,
    [AttemptDateTime]       DATETIME         NULL,
    [ConnectDateTime]       DATETIME         NULL,
    [DisconnectDateTime]    DATETIME         NULL,
    [CDPN]                  VARCHAR (40)     NULL,
    [CGPN]                  VARCHAR (40)     NULL,
    [OperatorID]            BIGINT           NULL,
    [TrafficDirection]      INT              NULL,
    [DestinationZoneID]     BIGINT           NULL,
    [OriginationZoneID]     BIGINT           NULL,
    [CurrencyID]            INT              NULL,
    [PDDInSeconds]          DECIMAL (22, 8)  NULL
);




GO



GO



GO


