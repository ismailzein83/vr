CREATE TYPE [Retail_Event].[EventInfoType] AS TABLE (
    [ID]                  BIGINT           NULL,
    [EventTime]           DATETIME         NULL,
    [SubscriberAccountID] BIGINT           NULL,
    [ServiceTypeID]       UNIQUEIDENTIFIER NULL,
    [Volume]              DECIMAL (20, 10) NULL,
    [VolumeUnit]          VARCHAR (50)     NULL,
    [Amount]              DECIMAL (20, 10) NULL,
    [Sender]              VARCHAR (50)     NULL,
    [OtherParty]          VARCHAR (50)     NULL,
    [ServiceArea]         VARCHAR (50)     NULL,
    [EventDirection]      INT              NULL,
    [ChargingPolicyID]    INT              NULL,
    [PackageID]           INT              NULL);



