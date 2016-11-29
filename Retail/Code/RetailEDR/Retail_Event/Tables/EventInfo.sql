CREATE TABLE [Retail_Event].[EventInfo] (
    [ID]                  BIGINT           IDENTITY (1, 1) NOT NULL,
    [EventTime]           DATETIME         NULL,
    [SubscriberAccountID] BIGINT           NULL,
    [ServiceTypeID]       UNIQUEIDENTIFIER NULL,
    [OldServiceTypeID]    INT              NULL,
    [Volume]              DECIMAL (20, 10) NULL,
    [VolumeUnit]          VARCHAR (50)     NULL,
    [Amount]              DECIMAL (20, 10) NULL,
    [OtherParty]          VARCHAR (50)     NULL,
    [ServiceArea]         VARCHAR (50)     NULL,
    [EventDirection]      INT              NULL,
    [ChargingPolicyID]    INT              NULL,
    [PackageID]           INT              NULL,
    [Sender]              VARCHAR (50)     NULL,
    CONSTRAINT [IX_EventInfo_ID] UNIQUE NONCLUSTERED ([ID] ASC)
);




GO
CREATE CLUSTERED INDEX [IX_EventInfo_Time]
    ON [Retail_Event].[EventInfo]([EventTime] ASC);

