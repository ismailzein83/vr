CREATE TABLE [RA_ICX_Analytics].[TrafficStats15Min] (
    [ID]                       BIGINT           NOT NULL,
    [BatchStart]               DATETIME         NULL,
    [OperatorID]               BIGINT           NULL,
    [InterconnectOperatorID]   BIGINT           NULL,
    [DataSourceID]             UNIQUEIDENTIFIER NULL,
    [TrafficDirection]         INT              NULL,
    [ProbeID]                  BIGINT           NULL,
    [OriginationZoneID]        BIGINT           NULL,
    [DestinationZoneID]        BIGINT           NULL,
    [OriginationCountryID]     INT              NULL,
    [DestinationCountryID]     INT              NULL,
    [NumberOfCDRs]             INT              NULL,
    [SuccessfulAttempts]       BIGINT           NULL,
    [TotalDurationInSeconds]   DECIMAL (30, 4)  NULL,
    [SumOfPDDInSeconds]        DECIMAL (20, 4)  NULL,
    [MaximumDurationInSeconds] DECIMAL (20, 4)  NULL,
    [MinimumDurationInSeconds] DECIMAL (20, 4)  NULL,
    [SumOfPGADInSeconds]       DECIMAL (20, 4)  NULL,
    [NumberOfClearCLIs]        BIGINT           NULL,
    CONSTRAINT [IX_RA_ICX_TrafficStats15Min_ID] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_RA_ICX_TrafficStats15Min_BatchStart]
    ON [RA_ICX_Analytics].[TrafficStats15Min]([BatchStart] ASC);

