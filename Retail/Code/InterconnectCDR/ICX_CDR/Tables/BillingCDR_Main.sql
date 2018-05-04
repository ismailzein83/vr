﻿CREATE TABLE [ICX_CDR].[BillingCDR_Main] (
    [CDRID]                    BIGINT           NULL,
    [DataSourceID]             UNIQUEIDENTIFIER NULL,
    [SwitchID]                 INT              NULL,
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
    [QueueItemId]              BIGINT           NULL,
    [TrafficDirection]         INT              NULL,
    [BillingType]              INT              NULL,
    [Rate]                     DECIMAL (20, 8)  NULL,
    [Amount]                   DECIMAL (22, 6)  NULL,
    [RateTypeID]               INT              NULL,
    [CurrencyID]               INT              NULL,
    [RateValueRuleID]          INT              NULL,
    [TariffRuleID]             INT              NULL,
    [RateTypeRuleID]           INT              NULL,
    [OriginationZoneId]        BIGINT           NULL,
    [DestinationZoneId]        BIGINT           NULL,
    [CallType]                 INT              NULL,
    [CDRType]                  INT              NULL
);

