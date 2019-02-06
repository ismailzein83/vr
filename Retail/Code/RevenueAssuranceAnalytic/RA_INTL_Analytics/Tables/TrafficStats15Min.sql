CREATE TABLE [RA_INTL_Analytics].[TrafficStats15Min] (
    [ID]                       BIGINT           NOT NULL,
    [BatchStart]               DATETIME         NULL,
    [OperatorID]               BIGINT           NULL,
    [NumberOfCDRs]             INT              NULL,
    [TrafficDirection]         INT              NULL,
    [SuccessfulAttempts]       INT              NULL,
    [SumOfPDDInSeconds]        DECIMAL (20, 4)  NULL,
    [DataSource]               UNIQUEIDENTIFIER NULL,
    [Probe]                    BIGINT           NULL,
    [OriginationZoneID]        BIGINT           NULL,
    [DestinationZoneID]        BIGINT           NULL,
    [OriginationCountryID]     INT              NULL,
    [DestinationCountryID]     INT              NULL,
    [TotalDurationInSeconds]   DECIMAL (20, 4)  NULL,
    [SumOfPGADInSeconds]       DECIMAL (20, 4)  NULL,
    [MaximumDurationInSeconds] DECIMAL (20, 4)  NULL,
    [MinimumDurationInSeconds] DECIMAL (20, 4)  NULL,
    [NumberOfClearCLIs]        BIGINT           NULL,
    CONSTRAINT [IX_RA_INTL_TrafficStats15Min_ID] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);






GO
CREATE CLUSTERED INDEX [IX_RA_INTL_TrafficStats15Min_BatchStart]
    ON [RA_INTL_Analytics].[TrafficStats15Min]([BatchStart] ASC);

