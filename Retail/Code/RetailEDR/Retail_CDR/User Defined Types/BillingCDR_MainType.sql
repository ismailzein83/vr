﻿CREATE TYPE [Retail_CDR].[BillingCDR_MainType] AS TABLE (
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
    [ServiceTypeId]           UNIQUEIDENTIFIER NULL,
    [TrafficDirection]        INT              NULL,
    [InitiationCallType]      INT              NULL,
    [TerminationCallType]     INT              NULL,
    [Calling]                 VARCHAR (500)    NULL,
    [Called]                  VARCHAR (500)    NULL,
    [OtherPartyNumber]        VARCHAR (500)    NULL,
    [InterconnectOperatorId]  BIGINT           NULL,
    [SubscriberZoneId]        BIGINT           NULL,
    [Zone]                    BIGINT           NULL,
    [NationalCallType]        INT              NULL,
    [PackageId]               INT              NULL,
    [ChargingPolicyId]        INT              NULL,
    [SaleDurationInSeconds]   DECIMAL (20, 4)  NULL,
    [SaleRate]                DECIMAL (20, 8)  NULL,
    [SaleAmount]              DECIMAL (22, 6)  NULL,
    [SaleRateTypeId]          INT              NULL,
    [SaleCurrencyId]          INT              NULL,
    [FinancialAccountId]      BIGINT           NULL,
    [SaleRateValueRuleId]     INT              NULL,
    [SaleRateTypeRuleId]      INT              NULL,
    [SaleTariffRuleId]        INT              NULL,
    [SaleExtraChargeRuleId]   INT              NULL,
    [CDRCostId]               BIGINT           NULL,
    [SupplierName]            NVARCHAR (255)   NULL,
    [CostRate]                DECIMAL (20, 8)  NULL,
    [CostAmount]              DECIMAL (22, 6)  NULL,
    [CostCurrencyId]          INT              NULL,
    [Extension]               VARCHAR (20)     NULL,
    [QueueItemId]             BIGINT           NULL);

























