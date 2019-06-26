CREATE TABLE [RA_Retail_Analytics].[PostpaidBillingStatsDaily] (
    [ID]                         BIGINT          NULL,
    [BatchStart]                 DATETIME        NULL,
    [OperatorID]                 BIGINT          NULL,
    [NumberOfCDRs]               INT             NULL,
    [OriginationZoneID]          BIGINT          NULL,
    [DestinationZoneID]          BIGINT          NULL,
    [OriginationCountryID]       INT             NULL,
    [DestinationCountryID]       INT             NULL,
    [TrafficDirection]           INT             NULL,
    [TotalDurationInSeconds]     DECIMAL (20, 4) NULL,
    [TotalSaleDurationInSeconds] DECIMAL (20, 4) NULL,
    [TotalRevenue]               DECIMAL (20, 4) NULL,
    [TotalIncome]                DECIMAL (22, 8) NULL,
    [SaleRate]                   DECIMAL (22, 8) NULL,
    [SaleCurrencyID]             INT             NULL
);

