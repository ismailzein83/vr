CREATE TABLE [TrafficQuality].[InvalidRouteDataRecord] (
    [Id]                    BIGINT      NULL,
    [RecordType]            INT         NULL,
    [SwitchName]            VARCHAR (4) NULL,
    [RecordingDate]         DATETIME    NULL,
    [RouteName]             VARCHAR (7) NULL,
    [A1]                    INT         NULL,
    [A2]                    INT         NULL,
    [A3]                    INT         NULL,
    [NumberOfDevices]       BIGINT      NULL,
    [NumberOfBids]          BIGINT      NULL,
    [NumberOfRejections]    BIGINT      NULL,
    [NumberOfBAnswers]      BIGINT      NULL,
    [AccTrafficLevel]       BIGINT      NULL,
    [AccNbOfBlockedDevices] BIGINT      NULL
);

