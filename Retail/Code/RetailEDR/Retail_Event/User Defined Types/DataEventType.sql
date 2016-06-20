CREATE TYPE [Retail_Event].[DataEventType] AS TABLE (
    [ID]                  BIGINT           NULL,
    [EventTime]           DATETIME         NULL,
    [SubscriberAccountID] BIGINT           NULL,
    [UpVolume]            DECIMAL (20, 10) NULL,
    [DownVolume]          DECIMAL (20, 10) NULL,
    [Amount]              DECIMAL (20, 10) NULL,
    [ChargingPolicyID]    INT              NULL,
    [PackageID]           INT              NULL);

