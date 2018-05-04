﻿CREATE TABLE [ICX_CDR].[BillingCDR_Invalid] (
    [CDRID]                    BIGINT           NOT NULL,
    [DataSourceID]             UNIQUEIDENTIFIER NULL,
    [SwitchId]                 INT              NULL,
    [IDOnSwitch]               VARCHAR (255)    NULL,
    [AttemptDateTime]          DATETIME         NULL,
    [AlertDateTime]            DATETIME         NULL,
    [ConnectDateTime]          DATETIME         NULL,
    [DisconnectDateTime]       DATETIME         NULL,
    [DurationInSeconds]        DECIMAL (20, 4)  NULL,
    [PDDInSeconds]             DECIMAL (20, 4)  NULL,
    [BillingDurationInSeconds] DECIMAL (20, 4)  NULL,
    [CDPN]                     VARCHAR (40)     NULL,
    [ReleaseCode]              VARCHAR (50)     NULL,
    [CGPN]                     VARCHAR (40)     NULL,
    [OperatorTypeID]           UNIQUEIDENTIFIER NULL,
    [OperatorID]               BIGINT           NULL,
    [TrafficDirection]         INT              NULL,
    [BillingType]              INT              NULL,
    [Rate]                     DECIMAL (20, 8)  NULL,
    [Amount]                   DECIMAL (22, 6)  NULL,
    [RateTypeID]               INT              NULL,
    [CurrencyID]               INT              NULL,
    [RateValueRuleID]          INT              NULL,
    [TariffRuleID]             INT              NULL,
    [RateTypeRuleID]           INT              NULL,
    [QueueItemId]              BIGINT           NULL,
    [OriginationZoneId]        BIGINT           NULL,
    [DestinationZoneId]        BIGINT           NULL,
    [CallType]                 INT              NULL,
    [CDRType]                  INT              NULL,
    CONSTRAINT [IX_BillingCDR_Invalid_CDRID] UNIQUE NONCLUSTERED ([CDRID] ASC)
);




GO
CREATE CLUSTERED INDEX [IX_BillingCDR_Invalid_Attempt_Operator]
    ON [ICX_CDR].[BillingCDR_Invalid]([AttemptDateTime] ASC, [OperatorID] ASC);

