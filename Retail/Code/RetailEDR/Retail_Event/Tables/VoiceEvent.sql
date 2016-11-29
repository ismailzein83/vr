CREATE TABLE [Retail_Event].[VoiceEvent] (
    [ID]                     BIGINT           IDENTITY (1, 1) NOT NULL,
    [EventTime]              DATETIME         NULL,
    [SubscriberAccountID]    BIGINT           NULL,
    [ServiceTypeID]          UNIQUEIDENTIFIER NULL,
    [OldServiceTypeID]       INT              NULL,
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
    [PackageId]              INT              NULL,
    CONSTRAINT [IX_VoiceEvent_ID] UNIQUE NONCLUSTERED ([ID] ASC)
);




GO
CREATE CLUSTERED INDEX [IX_VoiceEvent_Time]
    ON [Retail_Event].[VoiceEvent]([EventTime] ASC);

