CREATE TABLE [Retail_Event].[DataEvent] (
    [ID]                  BIGINT           IDENTITY (1, 1) NOT NULL,
    [EventTime]           DATETIME         NULL,
    [SubscriberAccountID] BIGINT           NULL,
    [UpVolume]            DECIMAL (20, 10) NULL,
    [DownVolume]          DECIMAL (20, 10) NULL,
    [Amount]              DECIMAL (20, 10) NULL,
    [ChargingPolicyID]    INT              NULL,
    [PackageID]           INT              NULL,
    CONSTRAINT [IX_DataEvent_ID] UNIQUE NONCLUSTERED ([ID] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_DataEvent_Time]
    ON [Retail_Event].[DataEvent]([EventTime] ASC);

