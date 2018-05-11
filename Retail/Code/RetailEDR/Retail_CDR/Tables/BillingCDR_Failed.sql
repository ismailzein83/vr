CREATE TABLE [Retail_CDR].[BillingCDR_Failed] (
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
    [BillingAccountId]        VARCHAR (50)     NULL,
    [ServiceTypeId]           UNIQUEIDENTIFIER NULL,
    [TrafficDirection]        INT              NULL,
    [InitiationCallType]      INT              NULL,
    [TerminationCallType]     INT              NULL,
    [OrigCalling]             VARCHAR (500)    NULL,
    [OrigCalled]              VARCHAR (500)    NULL,
    [Calling]                 VARCHAR (500)    NULL,
    [Called]                  VARCHAR (500)    NULL,
    [InterconnectOperatorId]  BIGINT           NULL,
    [SubscriberZoneId]        BIGINT           NULL,
    [Zone]                    BIGINT           NULL,
    [NationalCallType]        INT              NULL,
    [QueueItemId]             BIGINT           NULL,
    [SaleRateValueRuleId]     INT              NULL,
    [SaleRateTypeRuleId]      INT              NULL,
    [SaleTariffRuleId]        INT              NULL,
    [SaleExtraChargeRuleId]   INT              NULL,
    [Extension]               VARCHAR (20)     NULL
);
































GO
CREATE NONCLUSTERED INDEX [IX_BillingCDR_Failed_CDRId]
    ON [Retail_CDR].[BillingCDR_Failed]([CDRID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_BillingCDR_Failed_FinancialAccountId]
    ON [Retail_CDR].[BillingCDR_Failed]([FinancialAccountId] ASC);


GO
CREATE CLUSTERED INDEX [IX_BillingCDR_Failed_AttemptDateTimeAndAccountId]
    ON [Retail_CDR].[BillingCDR_Failed]([AttemptDateTime] ASC, [SubscriberAccountId] ASC);

