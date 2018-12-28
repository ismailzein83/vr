CREATE TABLE [RA_INTL_Analytics].[TrafficStats15Min] (
    [ID]                     BIGINT           NULL,
    [BatchStart]             DATETIME         NULL,
    [OperatorID]             BIGINT           NULL,
    [NumberOfCDRs]           INT              NULL,
    [TrafficDirection]       INT              NULL,
    [SuccessfulAttempts]     INT              NULL,
    [SumOfPDDInSeconds]      DECIMAL (20, 4)  NULL,
    [DataSource]             UNIQUEIDENTIFIER NULL,
    [Probe]                  BIGINT           NULL,
    [OriginationZoneID]      BIGINT           NULL,
    [DestinationZoneID]      BIGINT           NULL,
    [OriginationCountryID]   INT              NULL,
    [DestinationCountryID]   INT              NULL,
    [TotalDurationInSeconds] DECIMAL (20, 4)  NULL
);

