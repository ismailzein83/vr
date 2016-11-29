CREATE TYPE [Retail_Event].[VoiceEventType] AS TABLE (
    [ID]                     BIGINT           NULL,
    [EventTime]              DATETIME         NULL,
    [SubscriberAccountID]    BIGINT           NULL,
    [ServiceTypeID]          UNIQUEIDENTIFIER NULL,
    [TrafficDirection]       INT              NULL,
    [Calling]                VARCHAR (20)     NULL,
    [Called]                 VARCHAR (20)     NULL,
    [Duration]               DECIMAL (20, 10) NULL,
    [InterconnectOperatorId] INT              NULL,
    [SourceAreaId]           INT              NULL,
    [DestinationAreaId]      INT              NULL,
    [SourceZoneId]           BIGINT           NULL,
    [DestinationZoneId]      BIGINT           NULL,
    [Amount]                 DECIMAL (20, 10) NULL,
    [ChargingPolicyId]       INT              NULL,
    [PackageId]              INT              NULL);



