CREATE TABLE [TrafficQuality].[ProcessedDestinationDataRecord] (
    [DDRId]         BIGINT      NULL,
    [SwitchId]      INT         NULL,
    [RecordType]    INT         NULL,
    [ExchangeId]    VARCHAR (4) NULL,
    [RecordingDate] DATETIME    NULL,
    [A1]            INT         NULL,
    [A2]            INT         NULL,
    [A3]            INT         NULL,
    [A4]            BIT         NULL,
    [TRDCode]       INT         NULL,
    [N1]            BIGINT      NULL,
    [N2]            BIGINT      NULL,
    [N3]            BIGINT      NULL,
    [B1]            BIGINT      NULL,
    [T1]            INT         NULL,
    [T2]            INT         NULL,
    [T3]            INT         NULL
);






GO
CREATE CLUSTERED INDEX [IX_TrafficQuality_ProcessedDestinationDataRecord_RecordingDate]
    ON [TrafficQuality].[ProcessedDestinationDataRecord]([RecordingDate] ASC);

