CREATE TABLE [Retail_Event].[SMSEvent] (
    [ID]                     BIGINT           IDENTITY (1, 1) NOT NULL,
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
    [PackageID]              INT              NULL,
    CONSTRAINT [IX_SMSEvent_ID] UNIQUE NONCLUSTERED ([ID] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_SMSEvent_Time]
    ON [Retail_Event].[SMSEvent]([EventTime] ASC);

