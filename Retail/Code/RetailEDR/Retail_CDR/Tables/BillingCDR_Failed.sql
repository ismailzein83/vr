CREATE TABLE [Retail_CDR].[BillingCDR_Failed] (
    [CDRID]                  BIGINT           NULL,
    [IDonSwitch]             VARCHAR (100)    NULL,
    [ConnectDateTime]        DATETIME         NULL,
    [DisconnectDateTime]     DATETIME         NULL,
    [DurationInSeconds]      DECIMAL (10, 4)  NULL,
    [DisconnectReason]       VARCHAR (100)    NULL,
    [CallProgressState]      VARCHAR (100)    NULL,
    [SubscriberAccountId]    BIGINT           NULL,
    [ServiceTypeId]          UNIQUEIDENTIFIER NULL,
    [TrafficDirection]       INT              NULL,
    [Calling]                VARCHAR (100)    NULL,
    [Called]                 VARCHAR (100)    NULL,
    [InterconnectOperatorId] BIGINT           NULL,
    [SourceZoneId]           BIGINT           NULL,
    [DestinationZoneId]      BIGINT           NULL
);

