CREATE TYPE [RA_INTL_Analytics].[TrafficStats15MinType] AS TABLE (
    [ID]                     BIGINT           NULL,
    [BatchStart]             DATETIME         NULL,
    [OperatorID]             BIGINT           NULL,
    [DataSource]             UNIQUEIDENTIFIER NULL,
    [Probe]                  BIGINT           NULL,
    [TrafficDirection]       INT              NULL,
    [OriginationZoneID]      BIGINT           NULL,
    [DestinationZoneID]      BIGINT           NULL,
    [OriginationCountryID]   INT              NULL,
    [DestinationCountryID]   INT              NULL,
    [NumberOfCDRs]           INT              NULL,
    [SuccessfulAttempts]     INT              NULL,
    [SumOfPDDInSeconds]      DECIMAL (20, 4)  NULL,
    [TotalDurationInSeconds] DECIMAL (20, 4)  NULL);

