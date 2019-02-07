﻿CREATE TABLE [ICX_CDR].[BillingCDR_Failed] (
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
    [FinancialAccountId]       BIGINT           NULL,
    [BillingAccountId]         VARCHAR (50)     NULL,
    [CallingOperatorId]        BIGINT           NULL,
    [CalledOperatorId]         BIGINT           NULL,
    CONSTRAINT [IX_BillingCDR_Failed_CDRID] UNIQUE NONCLUSTERED ([CDRID] ASC)
);








GO
CREATE CLUSTERED INDEX [IX_BillingCDR_Failed_Attempt_Operator]
    ON [ICX_CDR].[BillingCDR_Failed]([AttemptDateTime] ASC, [OperatorID] ASC);

