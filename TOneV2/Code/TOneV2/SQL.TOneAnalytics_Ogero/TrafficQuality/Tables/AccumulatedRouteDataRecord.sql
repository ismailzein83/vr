﻿CREATE TABLE [TrafficQuality].[AccumulatedRouteDataRecord] (
    [Id]                       BIGINT      NULL,
    [SwitchId]                 INT         NULL,
    [CarrierAccountId]         INT         NULL,
    [SwitchName]               VARCHAR (4) NULL,
    [RecordingDate]            DATETIME    NULL,
    [RouteName]                VARCHAR (7) NULL,
    [A1In]                     INT         NULL,
    [A2In]                     INT         NULL,
    [A3In]                     INT         NULL,
    [NumberOfRecordsIn]        INT         NULL,
    [NumberOfDevicesIn]        BIGINT      NULL,
    [NumberOfBidsIn]           BIGINT      NULL,
    [NumberOfRejectionsIn]     BIGINT      NULL,
    [NumberOfBAnswersIn]       BIGINT      NULL,
    [AccTrafficLevelIn]        BIGINT      NULL,
    [AccNbOfBlockedDevicesIn]  BIGINT      NULL,
    [A1Out]                    INT         NULL,
    [A2Out]                    INT         NULL,
    [A3Out]                    INT         NULL,
    [NumberOfRecordsOut]       INT         NULL,
    [NumberOfDevicesOut]       BIGINT      NULL,
    [NumberOfBidsOut]          BIGINT      NULL,
    [NumberOfRejectionsOut]    BIGINT      NULL,
    [NumberOfBAnswersOut]      BIGINT      NULL,
    [AccTrafficLevelOut]       BIGINT      NULL,
    [AccNbOfBlockedDevicesOut] BIGINT      NULL
);




GO
CREATE CLUSTERED INDEX [IX_TrafficQuality_AccumulatedRouteDataRecord_RecordingDate]
    ON [TrafficQuality].[AccumulatedRouteDataRecord]([RecordingDate] ASC);

