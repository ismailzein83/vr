CREATE TABLE [RecordAnalysis].[C5Record] (
    [Id]                  BIGINT           NOT NULL,
    [MSISDN]              NVARCHAR (30)    NULL,
    [IMSI]                NVARCHAR (20)    NULL,
    [AttemptDateTime]     DATETIME         NULL,
    [AlertDateTime]       DATETIME         NULL,
    [ConnectDateTime]     DATETIME         NULL,
    [Destination]         NVARCHAR (40)    NULL,
    [DurationInSeconds]   DECIMAL (13, 4)  NULL,
    [DisconnectDateTime]  DATETIME         NULL,
    [CallClassID]         INT              NULL,
    [IsOnNet]             BIT              NULL,
    [CallTypeID]          INT              NULL,
    [SubscriberTypeID]    NVARCHAR (10)    NULL,
    [IMEI]                NVARCHAR (20)    NULL,
    [BTS]                 NVARCHAR (50)    NULL,
    [Cell]                NVARCHAR (50)    NULL,
    [SwitchId]            INT              NULL,
    [UpVolume]            DECIMAL (18, 2)  NULL,
    [DownVolume]          DECIMAL (18, 2)  NULL,
    [CellLatitude]        DECIMAL (18, 8)  NULL,
    [CellLongitude]       DECIMAL (18, 8)  NULL,
    [ServiceTypeID]       INT              NULL,
    [ServiceVASName]      NVARCHAR (50)    NULL,
    [InTrunkID]           INT              NULL,
    [OutTrunkID]          INT              NULL,
    [ReleaseCode]         NVARCHAR (50)    NULL,
    [MSISDNAreaCode]      NVARCHAR (10)    NULL,
    [DestinationAreaCode] NVARCHAR (10)    NULL,
    [Switch]              INT              NULL,
    [DataSource]          UNIQUEIDENTIFIER NULL,
    [FileName]            NVARCHAR (255)   NULL,
    [QueueItemId]         BIGINT           NULL
);


GO
CREATE CLUSTERED INDEX [IX_C5Record_AttemptDateTime]
    ON [RecordAnalysis].[C5Record]([AttemptDateTime] ASC, [MSISDN] ASC);

