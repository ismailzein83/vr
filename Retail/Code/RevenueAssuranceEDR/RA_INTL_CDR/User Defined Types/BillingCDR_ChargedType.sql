﻿CREATE TYPE [RA_INTL_CDR].[BillingCDR_ChargedType] AS TABLE (
    [CDRID]                 BIGINT           NULL,
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
    [PDDInSeconds]          DECIMAL (20, 4)  NULL);





