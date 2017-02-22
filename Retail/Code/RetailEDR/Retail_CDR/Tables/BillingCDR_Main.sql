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
    [Calling]                VARCHAR (100)    NULL,
    [Called]                 VARCHAR (100)    NULL,
    [OtherPartyNumber]       VARCHAR (100)    NULL,
    [InterconnectOperatorId] BIGINT           NULL,
    [Zone]                   BIGINT           NULL,
    [PackageId]              INT              NULL,
    [ChargingPolicyId]       INT              NULL,
    [Rate]                   DECIMAL (20, 8)  NULL,
    [Amount]                 DECIMAL (22, 6)  NULL,
    [RateTypeId]             INT              NULL,
    [CurrencyId]             INT              NULL
);










GO
CREATE NONCLUSTERED INDEX [IX_BillingCDR_Main_CDRId]
    ON [Retail_CDR].[BillingCDR_Main]([CDRID] ASC);


GO
CREATE CLUSTERED INDEX [IX_BillingCDR_Main_AttemptDateTime]
    ON [Retail_CDR].[BillingCDR_Main]([AttemptDateTime] ASC);

