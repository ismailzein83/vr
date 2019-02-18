﻿CREATE TYPE [RA_ICX_CDR].[BillingCDR_UnChargedType] AS TABLE (
    [CDRID]                  BIGINT           NULL,
    [OperatorID]             BIGINT           NULL,
    [InterconnectOperatorID] BIGINT           NULL,
    [TrafficDirection]       INT              NULL,
    [DataSourceID]           UNIQUEIDENTIFIER NULL,
    [ProbeID]                BIGINT           NULL,
    [IDOnSwitch]             VARCHAR (255)    NULL,
    [AttemptDateTime]        DATETIME         NULL,
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
    [TaxRuleID]              INT              NULL,
    [CurrencyID]             INT              NULL,
    [PDDInSeconds]           DECIMAL (20, 4)  NULL,
    [QueueItemId]            BIGINT           NULL);







