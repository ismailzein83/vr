﻿CREATE TABLE [Retail_CDR].[BillingCDR_Failed] (
    [CDRID]                  BIGINT           NULL,
    [IDonSwitch]             VARCHAR (100)    NULL,
    [AttemptDateTime]        DATETIME         NULL,
    [ConnectDateTime]        DATETIME         NULL,
    [DisconnectDateTime]     DATETIME         NULL,
    [DurationInSeconds]      DECIMAL (20, 4)  NULL,
    [DisconnectReason]       VARCHAR (100)    NULL,
    [CallProgressState]      VARCHAR (100)    NULL,
    [SubscriberAccountId]    BIGINT           NULL,
    [FinancialAccountId]     BIGINT           NULL,
    [ServiceTypeId]          UNIQUEIDENTIFIER NULL,
    [TrafficDirection]       INT              NULL,
    [InitiationCallType]     INT              NULL,
    [TerminationCallType]    INT              NULL,
    [Calling]                VARCHAR (100)    NULL,
    [Called]                 VARCHAR (100)    NULL,
    [OtherPartyNumber]       VARCHAR (100)    NULL,
    [InterconnectOperatorId] BIGINT           NULL,
    [Zone]                   BIGINT           NULL,
    [QueueItemId]            BIGINT           NULL,
    [SaleRateValueRuleId]    INT              NULL,
    [SaleRateTypeRuleId]     INT              NULL,
    [SaleTariffRuleId]       INT              NULL,
    [SaleExtraChargeRuleId]  INT              NULL,
    [Extension]              VARCHAR (20)     NULL
);
















GO
CREATE NONCLUSTERED INDEX [IX_BillingCDR_Failed_CDRId]
    ON [Retail_CDR].[BillingCDR_Failed]([CDRID] ASC);


GO
CREATE CLUSTERED INDEX [IX_BillingCDR_Failed_AttemptDateTime]
    ON [Retail_CDR].[BillingCDR_Failed]([AttemptDateTime] ASC);

