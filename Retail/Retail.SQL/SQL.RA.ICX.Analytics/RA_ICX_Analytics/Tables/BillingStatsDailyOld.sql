CREATE TABLE [RA_ICX_Analytics].[BillingStatsDailyOld] (
    [ID]                         BIGINT          NOT NULL,
    [BatchStart]                 DATETIME        NULL,
    [OperatorID]                 BIGINT          NULL,
    [InterconnectOperatorID]     BIGINT          NULL,
    [TrafficDirection]           INT             NULL,
    [NumberOfCDRs]               INT             NULL,
    [OriginationZoneID]          BIGINT          NULL,
    [DestinationZoneID]          BIGINT          NULL,
    [OriginationCountryID]       INT             NULL,
    [DestinationCountryID]       INT             NULL,
    [Rate]                       DECIMAL (20, 8) NULL,
    [RateTypeID]                 INT             NULL,
    [TotalDurationInSeconds]     DECIMAL (20, 4) NULL,
    [TotalSaleDurationInSeconds] DECIMAL (20, 4) NULL,
    [MinimumDurationInSeconds]   DECIMAL (20, 4) NULL,
    [MaximumDurationInSeconds]   DECIMAL (20, 4) NULL,
    [TotalRevenue]               DECIMAL (26, 8) NULL,
    [TotalIncome]                DECIMAL (26, 8) NULL,
    [CurrencyID]                 INT             NULL,
    CONSTRAINT [IX_RA_ICX_BillingStatsDaily_IDOld] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_RA_ICX_BillingStatsDaily_BatchStartOld]
    ON [RA_ICX_Analytics].[BillingStatsDailyOld]([BatchStart] ASC);

