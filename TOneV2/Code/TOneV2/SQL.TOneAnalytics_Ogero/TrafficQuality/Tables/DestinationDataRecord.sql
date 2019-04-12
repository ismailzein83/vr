CREATE TABLE [TrafficQuality].[DestinationDataRecord] (
    [Id]               BIGINT         NULL,
    [SwitchId]         INT            NULL,
    [RecordType]       INT            NULL,
    [ExchangeIdentity] VARCHAR (12)   NULL,
    [RecordingDate]    DATETIME       NULL,
    [A1]               INT            NULL,
    [A2]               INT            NULL,
    [A3]               INT            NULL,
    [A4]               BIT            NULL,
    [TRDCode]          INT            NULL,
    [N1]               BIGINT         NULL,
    [N2]               BIGINT         NULL,
    [N3]               BIGINT         NULL,
    [B1]               BIGINT         NULL,
    [T1]               INT            NULL,
    [T2]               INT            NULL,
    [T3]               INT            NULL,
    [ExtraFields]      NVARCHAR (MAX) NULL,
    [FileName]         VARCHAR (255)  NULL
);






GO
CREATE CLUSTERED INDEX [IX_TrafficQuality_DestinationDataRecord_RecordingDate]
    ON [TrafficQuality].[DestinationDataRecord]([RecordingDate] ASC);

