﻿CREATE TYPE [RA_ICX_CDR].[BillingCDR_InvalidType] AS TABLE (
    [CDRID]                  BIGINT           NULL,
    [DataSourceID]           UNIQUEIDENTIFIER NULL,
    [ProbeID]                BIGINT           NULL,
    [IDOnSwitch]             VARCHAR (255)    NULL,
    [AttemptDateTime]        DATETIME         NULL,
    [ConnectDateTime]        DATETIME         NULL,
    [DisconnectDateTime]     DATETIME         NULL,
    [DurationInSeconds]      DECIMAL (20, 4)  NULL,
    [CDPN]                   VARCHAR (40)     NULL,
    [CGPN]                   VARCHAR (40)     NULL,
    [OperatorID]             BIGINT           NULL,
    [TrafficDirection]       INT              NULL,
    [DestinationZoneID]      BIGINT           NULL,
    [OriginationZoneID]      BIGINT           NULL,
    [SpecialNumberGroup]     BIGINT           NULL,
    [InterconnectOperatorID] BIGINT           NULL,
    [SaleDurationInSeconds]  DECIMAL (20, 4)  NULL,
    [DisconnectReason]       VARCHAR (200)    NULL,
    [AlertDateTime]          DATETIME         NULL,
    [Rate]                   DECIMAL (20, 8)  NULL,
    [RateTypeID]             INT              NULL,
    [RateValueRuleID]        INT              NULL,
    [RateTypeRuleID]         INT              NULL,
    [TariffRuleID]           INT              NULL,
    [Revenue]                DECIMAL (20, 8)  NULL,
    [Income]                 DECIMAL (20, 8)  NULL,
    [TaxRuleID]              INT              NULL,
    [QueueItemId]            BIGINT           NULL,
    [CurrencyID]             INT              NULL,
    [PDDInSeconds]           DECIMAL (20, 4)  NULL);





