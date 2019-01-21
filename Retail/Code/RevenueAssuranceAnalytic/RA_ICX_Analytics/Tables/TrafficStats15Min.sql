CREATE TABLE [RA_ICX_Analytics].[TrafficStats15Min] (
    [ID]                         BIGINT           NOT NULL,
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
    [MinimumDurationInSeconds]   DECIMAL (20, 4)  NULL,
    CONSTRAINT [IX_RA_ICX_TrafficStats15Min_ID] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);






GO
CREATE CLUSTERED INDEX [IX_RA_ICX_TrafficStats15Min_BatchStart]
    ON [RA_ICX_Analytics].[TrafficStats15Min]([BatchStart] ASC);

