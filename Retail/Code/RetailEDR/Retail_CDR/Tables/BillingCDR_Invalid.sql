CREATE TABLE [Retail_CDR].[BillingCDR_Invalid] (
    [CDRID]                  BIGINT           NULL,
    [IDonSwitch]             VARCHAR (100)    NULL,
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
    [AttemptDateTime]        DATETIME         NULL
);








GO
CREATE NONCLUSTERED INDEX [IX_BillingCDR_Invalid_CDRId]
    ON [Retail_CDR].[BillingCDR_Invalid]([CDRID] ASC);


GO
CREATE CLUSTERED INDEX [IX_BillingCDR_Invalid_AttemptDateTime]
    ON [Retail_CDR].[BillingCDR_Invalid]([AttemptDateTime] ASC);

