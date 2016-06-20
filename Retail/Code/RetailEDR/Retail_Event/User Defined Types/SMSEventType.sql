CREATE TYPE [Retail_Event].[SMSEventType] AS TABLE (
    [ID]                     BIGINT           NULL,
    [EventTime]              DATETIME         NULL,
    [SubscriberAccountId]    BIGINT           NULL,
    [TrafficDirection]       INT              NULL,
    [Sender]                 VARCHAR (20)     NULL,
    [Receiver]               VARCHAR (20)     NULL,
    [InterconnectOperatorID] INT              NULL,
    [SourceAreaID]           INT              NULL,
    [DestinationAreaID]      INT              NULL,
    [SourceZoneID]           BIGINT           NULL,
    [DestinationZoneID]      BIGINT           NULL,
    [Amount]                 DECIMAL (20, 10) NULL,
    [ChargingPolicyID]       INT              NULL,
    [PackageID]              INT              NULL);

