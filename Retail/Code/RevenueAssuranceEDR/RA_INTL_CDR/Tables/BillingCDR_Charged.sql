CREATE TABLE [RA_INTL_CDR].[BillingCDR_Charged] (
    [CDRID]                 BIGINT           NOT NULL,
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
    [Rate]                  DECIMAL (22, 8)  NULL,
    [RateTypeID]            INT              NULL,
    [RateValueRuleID]       INT              NULL,
    [TariffRuleID]          INT              NULL,
    [RateTypeRuleID]        INT              NULL,
    [TaxRuleID]             BIGINT           NULL,
    [Income]                DECIMAL (22, 8)  NULL,
    [DurationInSeconds]     DECIMAL (20, 4)  NULL,
    [Revenue]               DECIMAL (22, 8)  NULL,
    [DisconnectReason]      VARCHAR (200)    NULL,
    [AlertDateTime]         DATETIME         NULL,
    [SpecialNumberGroup]    BIGINT           NULL,
    [SaleDurationInSeconds] DECIMAL (20, 4)  NULL,
    [QueueItemId]           BIGINT           NULL,
    [CurrencyID]            INT              NULL,
    [PDDInSeconds]          DECIMAL (20, 4)  NULL
);




GO
CREATE NONCLUSTERED INDEX [IX_BillingCDR_Main_Operator]
    ON [RA_INTL_CDR].[BillingCDR_Charged]([OperatorID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_BillingCDR_Main_ID]
    ON [RA_INTL_CDR].[BillingCDR_Charged]([CDRID] ASC);


GO
CREATE CLUSTERED INDEX [IX_BillingCDR_Main_AttemptTime]
    ON [RA_INTL_CDR].[BillingCDR_Charged]([AttemptDateTime] ASC);

