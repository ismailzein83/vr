CREATE TABLE [RecordAnalysis].[C5InvalidRecord] (
    [RecordId]           BIGINT          NULL,
    [MSISDN]             NVARCHAR (255)  NULL,
    [IMSI]               NVARCHAR (255)  NULL,
    [OtherPartyNumber]   NVARCHAR (255)  NULL,
    [AttemptDateTime]    DATETIME        NULL,
    [AlertDateTime]      DATETIME        NULL,
    [ConnectDateTime]    DATETIME        NULL,
    [DisconnectDateTime] DATETIME        NULL,
    [DurationInSeconds]  DECIMAL (13, 4) NULL,
    [RecordType]         INT             NULL,
    [RecordDirection]    INT             NULL,
    [IMEI]               NVARCHAR (255)  NULL,
    [BTS]                NVARCHAR (255)  NULL,
    [Cell]               NVARCHAR (255)  NULL,
    [UpVolume]           DECIMAL (18, 2) NULL,
    [DownVolume]         DECIMAL (18, 2) NULL,
    [CellLatitude]       DECIMAL (18, 8) NULL,
    [CellLongitude]      DECIMAL (18, 8) NULL,
    [InTrunk]            NVARCHAR (255)  NULL,
    [OutTrunk]           NVARCHAR (255)  NULL,
    [Scope]              INT             NULL
);


GO
CREATE CLUSTERED INDEX [IX_C5InvalidRecord_AttemptDateTime]
    ON [RecordAnalysis].[C5InvalidRecord]([AttemptDateTime] ASC, [MSISDN] ASC);

