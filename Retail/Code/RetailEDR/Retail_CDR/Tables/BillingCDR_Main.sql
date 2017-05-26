CREATE TABLE [Retail_CDR].[BillingCDR_Main] (
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
    [PackageId]              INT              NULL,
    [ChargingPolicyId]       INT              NULL,
    [SaleDurationInSeconds]  DECIMAL (20, 4)  NULL,
    [SaleRate]               DECIMAL (20, 8)  NULL,
    [SaleAmount]             DECIMAL (22, 6)  NULL,
    [SaleRateTypeId]         INT              NULL,
    [SaleCurrencyId]         INT              NULL,
    [SaleRateValueRuleId]    INT              NULL,
    [SaleRateTypeRuleId]     INT              NULL,
    [SaleTariffRuleId]       INT              NULL,
    [SaleExtraChargeRuleId]  INT              NULL,
    [QueueItemId]            BIGINT           NULL,
    [CostRate]               DECIMAL (20, 8)  NULL,
    [CostAmount]             DECIMAL (22, 6)  NULL,
    [Extension]              VARCHAR (20)     NULL
);
















GO
CREATE NONCLUSTERED INDEX [IX_BillingCDR_Main_CDRId]
    ON [Retail_CDR].[BillingCDR_Main]([CDRID] ASC);


GO
CREATE CLUSTERED INDEX [IX_BillingCDR_Main_AttemptDateTime]
    ON [Retail_CDR].[BillingCDR_Main]([AttemptDateTime] ASC);

