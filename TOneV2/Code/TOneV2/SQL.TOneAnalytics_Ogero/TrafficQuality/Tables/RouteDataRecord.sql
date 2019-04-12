CREATE TABLE [TrafficQuality].[RouteDataRecord] (
    [Id]                    BIGINT         NULL,
    [RecordType]            INT            NULL,
    [ExchangeIdentity]      VARCHAR (12)   NULL,
    [RecordingDate]         DATETIME       NULL,
    [RouteName]             VARCHAR (7)    NULL,
    [SwitchId]              INT            NULL,
    [A1]                    INT            NULL,
    [A2]                    INT            NULL,
    [A3]                    INT            NULL,
    [NumberOfDevices]       BIGINT         NULL,
    [NumberOfBids]          BIGINT         NULL,
    [NumberOfRejections]    BIGINT         NULL,
    [NumberOfBAnswers]      BIGINT         NULL,
    [AccTrafficLevel]       BIGINT         NULL,
    [AccNbOfBlockedDevices] BIGINT         NULL,
    [ExtraFields]           NVARCHAR (MAX) NULL,
    [FileName]              VARCHAR (255)  NULL
);




GO
CREATE CLUSTERED INDEX [IX_TrafficQuality_RouteDataRecord_RecordingDate]
    ON [TrafficQuality].[RouteDataRecord]([RecordingDate] ASC);

