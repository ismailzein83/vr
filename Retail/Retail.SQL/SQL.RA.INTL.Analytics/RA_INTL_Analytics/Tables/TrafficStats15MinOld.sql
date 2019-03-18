﻿CREATE TABLE [RA_INTL_Analytics].[TrafficStats15MinOld] (
    [ID]                       BIGINT           NOT NULL,
    [BatchStart]               DATETIME         NULL,
    [OperatorID]               BIGINT           NULL,
    [DataSource]               UNIQUEIDENTIFIER NULL,
    [Probe]                    BIGINT           NULL,
    [TrafficDirection]         INT              NULL,
    [OriginationZoneID]        BIGINT           NULL,
    [DestinationZoneID]        BIGINT           NULL,
    [OriginationCountryID]     INT              NULL,
    [DestinationCountryID]     INT              NULL,
    [NumberOfCDRs]             INT              NULL,
    [SuccessfulAttempts]       INT              NULL,
    [SumOfPDDInSeconds]        DECIMAL (20, 4)  NULL,
    [TotalDurationInSeconds]   DECIMAL (20, 4)  NULL,
    [SumOfPGADInSeconds]       DECIMAL (20, 4)  NULL,
    [MaximumDurationInSeconds] DECIMAL (20, 4)  NULL,
    [MinimumDurationInSeconds] DECIMAL (20, 4)  NULL,
    [NumberOfClearCLIs]        BIGINT           NULL,
    CONSTRAINT [IX_RA_INTL_TrafficStats15Min_IDOld] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_RA_INTL_TrafficStats15Min_BatchStartOld]
    ON [RA_INTL_Analytics].[TrafficStats15MinOld]([BatchStart] ASC);

