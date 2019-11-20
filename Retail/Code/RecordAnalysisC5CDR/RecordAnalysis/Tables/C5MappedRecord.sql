CREATE TABLE [RecordAnalysis].[C5MappedRecord] (
    [RecordId]           BIGINT          NULL,
    [MSISDN]             NVARCHAR (30)   NULL,
    [IMSI]               NVARCHAR (20)   NULL,
    [OtherPartyNumber]   NVARCHAR (40)   NULL,
    [AttemptDateTime]    DATETIME        NULL,
    [AlertDateTime]      DATETIME        NULL,
    [ConnectDateTime]    DATETIME        NULL,
    [DurationInSeconds]  DECIMAL (13, 4) NULL,
    [DisconnectDateTime] DATETIME        NULL,
    [RecordType]         INT             NULL,
    [RecordDirection]    INT             NULL,
    [Scope]              INT             NULL,
    [IMEI]               NVARCHAR (100)  NULL,
    [BTS]                NVARCHAR (50)   NULL,
    [Cell]               NVARCHAR (50)   NULL,
    [UpVolume]           DECIMAL (18, 2) NULL,
    [DownVolume]         DECIMAL (18, 2) NULL,
    [CellLatitude]       DECIMAL (18, 8) NULL,
    [CellLongitude]      DECIMAL (18, 8) NULL,
    [InTrunk]            VARCHAR (50)    NULL,
    [OutTrunk]           VARCHAR (50)    NULL
);


GO
CREATE CLUSTERED INDEX [IX_C5MappedRecord_AttemptDateTime]
    ON [RecordAnalysis].[C5MappedRecord]([AttemptDateTime] ASC, [MSISDN] ASC);

