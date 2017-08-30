﻿CREATE TYPE [Retail_CDR].[BillingCDR_FailedType] AS TABLE (
    [CDRID]                   BIGINT           NULL,
    [Call_Id]                 VARCHAR (200)    NULL,
    [AttemptDateTime]         DATETIME         NULL,
    [ConnectDateTime]         DATETIME         NULL,
    [DisconnectDateTime]      DATETIME         NULL,
    [DurationInSeconds]       DECIMAL (20, 4)  NULL,
    [DisconnectReason]        VARCHAR (100)    NULL,
    [CallProgressState]       VARCHAR (100)    NULL,
    [SubscriberAccountTypeId] UNIQUEIDENTIFIER NULL,
    [SubscriberAccountId]     BIGINT           NULL,
    [FinancialAccountId]      BIGINT           NULL,
    [ServiceTypeId]           UNIQUEIDENTIFIER NULL,
    [TrafficDirection]        INT              NULL,
    [InitiationCallType]      INT              NULL,
    [TerminationCallType]     INT              NULL,
    [Calling]                 VARCHAR (100)    NULL,
    [Called]                  VARCHAR (100)    NULL,
    [OtherPartyNumber]        VARCHAR (100)    NULL,
    [InterconnectOperatorId]  BIGINT           NULL,
    [SubscriberZoneId]        BIGINT           NULL,
    [Zone]                    BIGINT           NULL,
    [SaleRateValueRuleId]     INT              NULL,
    [SaleRateTypeRuleId]      INT              NULL,
    [SaleTariffRuleId]        INT              NULL,
    [SaleExtraChargeRuleId]   INT              NULL,
    [Extension]               VARCHAR (20)     NULL,
    [NationalCallType]        INT              NULL,
    [QueueItemId]             BIGINT           NULL);





















