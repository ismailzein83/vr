﻿CREATE TABLE [RA_INTL_Analytics].[BillingStatsDaily] (
    [ID]                         BIGINT          NULL,
    [BatchStart]                 DATETIME        NULL,
    [OperatorID]                 BIGINT          NULL,
    [NumberOfCDRs]               INT             NULL,
    [OriginationZoneID]          BIGINT          NULL,
    [DestinationZoneID]          BIGINT          NULL,
    [OriginationCountryID]       INT             NULL,
    [DestinationCountryID]       INT             NULL,
    [TrafficDirection]           INT             NULL,
    [Rate]                       DECIMAL (20, 8) NULL,
    [RateTypeID]                 INT             NULL,
    [TotalDurationInSeconds]     DECIMAL (20, 4) NULL,
    [TotalSaleDurationInSeconds] DECIMAL (20, 4) NULL,
    [MinimumDurationInSeconds]   DECIMAL (20, 4) NULL,
    [MaximumDurationInSeconds]   DECIMAL (20, 4) NULL,
    [TotalRevenue]               DECIMAL (22, 8) NULL,
    [TotalIncome]                DECIMAL (22, 8) NULL,
    [CurrencyID]                 INT             NULL
);


