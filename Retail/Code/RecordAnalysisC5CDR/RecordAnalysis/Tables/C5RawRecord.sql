CREATE TABLE [RecordAnalysis].[C5RawRecord] (
    [Id]                 BIGINT           NOT NULL,
    [MSISDN]             NVARCHAR (30)    NULL,
    [IMSI]               NVARCHAR (20)    NULL,
    [OtherPartyNumber]   NVARCHAR (40)    NULL,
    [AttemptDateTime]    DATETIME         NULL,
    [AlertDateTime]      DATETIME         NULL,
    [ConnectDateTime]    DATETIME         NULL,
    [DisconnectDateTime] DATETIME         NULL,
    [DurationInSeconds]  DECIMAL (13, 4)  NULL,
    [RecordType]         INT              NULL,
    [RecordDirection]    INT              NULL,
    [IMEI]               NVARCHAR (100)   NULL,
    [BTS]                NVARCHAR (50)    NULL,
    [Cell]               NVARCHAR (50)    NULL,
    [UpVolume]           DECIMAL (18, 2)  NULL,
    [DownVolume]         DECIMAL (18, 2)  NULL,
    [CellLatitude]       DECIMAL (18, 8)  NULL,
    [CellLongitude]      DECIMAL (18, 8)  NULL,
    [InTrunk]            NVARCHAR (50)    NULL,
    [OutTrunk]           NVARCHAR (50)    NULL,
    [DataSource]         UNIQUEIDENTIFIER NULL,
    [FileName]           NVARCHAR (255)   NULL,
    [QueueItemId]        BIGINT           NULL
);


GO
CREATE CLUSTERED INDEX [IX_C5RawRecord_AttemptDateTime]
    ON [RecordAnalysis].[C5RawRecord]([AttemptDateTime] ASC, [MSISDN] ASC);

