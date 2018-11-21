CREATE TABLE [RA_Analytics].[BillingStatsDaily] (
    [ID]                            BIGINT           NULL,
    [BatchStart]                    DATETIME         NULL,
    [OperatorID]                    BIGINT           NULL,
    [OriginationCountryID]          INT              NULL,
    [OriginationZoneID]             BIGINT           NULL,
    [DestinationZoneID]             BIGINT           NULL,
    [TrafficDirection]              INT              NULL,
    [Rate]                          DECIMAL (20, 8)  NULL,
    [RateTypeID]                    INT              NULL,
    [CurrencyID]                    INT              NULL,
    [TotalDurationInSeconds]        DECIMAL (20, 4)  NULL,
    [TotalBillingDurationInSeconds] DECIMAL (20, 4)  NULL,
    [TotalAmount]                   DECIMAL (26, 10) NULL,
    [MinimumDurationInSeconds]      DECIMAL (20, 4)  NULL,
    [MaximumDurationInSeconds]      DECIMAL (20, 4)  NULL,
    [NumberOfCDRs]                  INT              NULL
);

