CREATE TABLE [RA_ICX_Analytics].[TrafficStats15Min] (
    [ID]                         BIGINT           NULL,
    [BatchStart]                 DATETIME         NULL,
    [OperatorID]                 BIGINT           NULL,
    [NumberOfCDRs]               INT              NULL,
    [OriginationZoneID]          BIGINT           NULL,
    [DestinationZoneID]          BIGINT           NULL,
    [TrafficDirection]           INT              NULL,
    [SuccessfulAttempts]         BIGINT           NULL,
    [SumOfPDDInSeconds]          DECIMAL (20, 4)  NULL,
    [DurationInSeconds]          DECIMAL (20, 4)  NULL,
    [DataSource]                 UNIQUEIDENTIFIER NULL,
    [Probe]                      BIGINT           NULL,
    [InterconnectOperator]       BIGINT           NULL,
    [TotalAmount]                DECIMAL (20, 4)  NULL,
    [TotalSaleDurationInSeconds] DECIMAL (20, 4)  NULL,
    [OriginationCountryID]       INT              NULL,
    [DestinationCountryID]       INT              NULL,
    [TotalDurationInSeconds]     DECIMAL (20, 4)  NULL,
    [MaximumDurationInSeconds]   DECIMAL (20, 4)  NULL,
    [MinimumDurationInSeconds]   DECIMAL (20, 4)  NULL
);



