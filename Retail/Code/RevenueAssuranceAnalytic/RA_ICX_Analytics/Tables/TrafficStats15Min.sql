CREATE TABLE [RA_ICX_Analytics].[TrafficStats15Min] (
    [ID]                     BIGINT          NULL,
    [BatchStart]             DATETIME        NULL,
    [OperatorID]             BIGINT          NULL,
    [NumberOfCDRs]           INT             NULL,
    [OriginationZoneID]      BIGINT          NULL,
    [DestinationZoneID]      BIGINT          NULL,
    [TrafficDirection]       INT             NULL,
    [SuccessfulAttempts]     INT             NULL,
    [TotalDurationInSeconds] DECIMAL (20, 4) NULL,
    [SumOfPDDInSeconds]      DECIMAL (20, 4) NULL,
    [DurationInSeconds]      DECIMAL (20, 4) NULL,
    [ZoneID]                 BIGINT          NULL,
    [Country]                INT             NULL
);

