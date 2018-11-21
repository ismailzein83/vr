﻿CREATE TYPE [RA_INTL_CDR].[BillingCDR_UnchargedType] AS TABLE (
    [CDRID]              BIGINT           NULL,
    [DataSourceID]       UNIQUEIDENTIFIER NULL,
    [ProbeID]            BIGINT           NULL,
    [IDOnSwitch]         VARCHAR (255)    NULL,
    [AttemptDateTime]    DATETIME         NULL,
    [ConnectDateTime]    DATETIME         NULL,
    [DisconnectDateTime] DATETIME         NULL,
    [DurationInSeconds]  DECIMAL (20, 4)  NULL,
    [CDPN]               VARCHAR (40)     NULL,
    [CGPN]               VARCHAR (40)     NULL,
    [OperatorID]         BIGINT           NULL,
    [TrafficDirection]   INT              NULL,
    [DestinationZone]    BIGINT           NULL,
    [OriginationZone]    BIGINT           NULL,
    [Rate]               DECIMAL (20, 8)  NULL,
    [Amount]             DECIMAL (22, 6)  NULL,
    [RateTypeID]         INT              NULL,
    [CurrencyID]         INT              NULL,
    [RateValueRuleID]    INT              NULL,
    [TariffRuleID]       INT              NULL,
    [RateTypeRuleID]     INT              NULL,
    [QueueItemId]        BIGINT           NULL);

