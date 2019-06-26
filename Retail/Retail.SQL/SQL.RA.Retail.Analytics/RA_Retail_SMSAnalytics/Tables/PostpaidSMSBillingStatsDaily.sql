CREATE TABLE [RA_Retail_SMSAnalytics].[PostpaidSMSBillingStatsDaily] (
    [ID]                         BIGINT          NOT NULL,
    [BatchStart]                 DATETIME        NULL,
    [OperatorID]                 BIGINT          NULL,
    [TotalNumberOfSMS]           INT             NULL,
    [OriginationCountryID]       INT             NULL,
    [DestinationCountryID]       INT             NULL,
    [OriginationMobileNetworkID] INT             NULL,
    [DestinationMobileNetworkID] INT             NULL,
    [TrafficDirection]           INT             NULL,
    [Rate]                       DECIMAL (22, 8) NULL,
    [RateTypeID]                 INT             NULL,
    [TotalRevenue]               DECIMAL (22, 8) NULL,
    [TotalIncome]                DECIMAL (22, 8) NULL,
    [CurrencyID]                 INT             NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

