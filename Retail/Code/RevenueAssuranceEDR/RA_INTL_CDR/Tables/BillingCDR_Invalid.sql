CREATE TABLE [RA_INTL_CDR].[BillingCDR_Invalid] (
    [CDRID]              BIGINT           NOT NULL,
    [DataSourceID]       UNIQUEIDENTIFIER NULL,
    [ProbeID]            BIGINT           NULL,
    [IDOnSwitch]         VARCHAR (255)    NULL,
    [AttemptDateTime]    DATETIME         NULL,
    [ConnectDateTime]    DATETIME         NULL,
    [DisconnectDateTime] DATETIME         NULL,
    [DurationInSeconds]  DECIMAL (20, 4)  NULL,
    [CDPN]               VARCHAR (40)     NULL,
    [ReleaseCode]        VARCHAR (50)     NULL,
    [CGPN]               VARCHAR (40)     NULL,
    [OperatorID]         BIGINT           NULL,
    [TrafficDirection]   INT              NULL,
    [QueueItemId]        BIGINT           NULL,
    [DestinationZone]    BIGINT           NULL,
    [OriginationZone]    BIGINT           NULL,
    [Rate]               DECIMAL (20, 8)  NULL,
    [Amount]             DECIMAL (22, 6)  NULL,
    [RateTypeID]         INT              NULL,
    [CurrencyID]         INT              NULL,
    [RateValueRuleID]    INT              NULL,
    [TariffRuleID]       INT              NULL,
    [RateTypeRuleID]     INT              NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_BillingCDR_Invalid_Operator]
    ON [RA_INTL_CDR].[BillingCDR_Invalid]([OperatorID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_BillingCDR_Invalid_ID]
    ON [RA_INTL_CDR].[BillingCDR_Invalid]([CDRID] ASC);


GO
CREATE CLUSTERED INDEX [IX_BillingCDR_Invalid_AttemptTime]
    ON [RA_INTL_CDR].[BillingCDR_Invalid]([AttemptDateTime] ASC);

