CREATE TABLE [RA_INTL_Analytics].[TrafficStats15Min] (
    [ID]                       BIGINT           NOT NULL,
    [BatchStart]               DATETIME         NOT NULL,
    [OperatorID]               BIGINT           NULL,
    [DataSourceID]             UNIQUEIDENTIFIER NULL,
    [ProbeID]                  BIGINT           NULL,
    [TrafficDirection]         INT              NULL,
    [OriginationZoneID]        BIGINT           NULL,
    [DestinationZoneID]        BIGINT           NULL,
    [OriginationCountryID]     INT              NULL,
    [DestinationCountryID]     INT              NULL,
    [NumberOfCDRs]             BIGINT           NULL,
    [SuccessfulAttempts]       INT              NULL,
    [TotalDurationInSeconds]   DECIMAL (20, 4)  NULL,
    [SumOfPDDInSeconds]        DECIMAL (20, 4)  NULL,
    [MaximumDurationInSeconds] DECIMAL (20, 4)  NULL,
    [MinimumDurationInSeconds] DECIMAL (20, 4)  NULL,
    [SumOfPGADInSeconds]       DECIMAL (20, 4)  NULL,
    [NumberOfClearCLIs]        BIGINT           NULL,
    CONSTRAINT [IX_RA_INTL_TrafficStats15Min_ID] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_RA_INTL_TrafficStats15Min_BatchStart]
    ON [RA_INTL_Analytics].[TrafficStats15Min]([BatchStart] ASC);

