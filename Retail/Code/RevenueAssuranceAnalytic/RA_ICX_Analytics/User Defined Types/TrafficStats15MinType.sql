CREATE TYPE [RA_ICX_Analytics].[TrafficStats15MinType] AS TABLE (
    [ID]                       BIGINT           NULL,
    [BatchStart]               DATETIME         NULL,
    [OperatorID]               BIGINT           NULL,
    [InterconnectOperator]     BIGINT           NULL,
    [DataSource]               UNIQUEIDENTIFIER NULL,
    [TrafficDirection]         INT              NULL,
    [Probe]                    BIGINT           NULL,
    [OriginationZoneID]        BIGINT           NULL,
    [DestinationZoneID]        BIGINT           NULL,
    [OriginationCountryID]     INT              NULL,
    [DestinationCountryID]     INT              NULL,
    [NumberOfCDRs]             INT              NULL,
    [SuccessfulAttempts]       BIGINT           NULL,
    [TotalDurationInSeconds]   DECIMAL (20, 4)  NULL,
    [SumOfPDDInSeconds]        DECIMAL (20, 4)  NULL,
    [MaximumDurationInSeconds] DECIMAL (20, 4)  NULL,
    [MinimumDurationInSeconds] DECIMAL (20, 4)  NULL,
    [SumOfPGADInSeconds]       DECIMAL (20, 4)  NULL,
    [NumberOfClearCLIs]        BIGINT           NULL);





