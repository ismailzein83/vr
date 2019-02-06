﻿CREATE TYPE [RA_INTL_SMSAnalytics].[BillingStatsDailyType] AS TABLE (
    [ID]                       BIGINT          NULL,
    [BatchStart]               DATETIME        NULL,
    [OperatorID]               BIGINT          NULL,
    [NumberOfSMSs]             INT             NULL,
    [OriginationZoneID]        BIGINT          NULL,
    [DestinationZoneID]        BIGINT          NULL,
    [OriginationCountryID]     INT             NULL,
    [DestinationCountryID]     INT             NULL,
    [CallingMNCID]             INT             NULL,
    [CallingMCCID]             INT             NULL,
    [CalledMNCID]              INT             NULL,
    [CalledMCCID]              INT             NULL,
    [TrafficDirection]         INT             NULL,
    [Rate]                     DECIMAL (22, 8) NULL,
    [RateTypeID]               INT             NULL,
    [TotalRevenue]             DECIMAL (22, 8) NULL,
    [TotalIncome]              DECIMAL (22, 8) NULL,
    [CurrencyID]               INT             NULL,
    [OriginationMobileNetwork] INT             NULL,
    [OriginationMobileCountry] INT             NULL,
    [DestinationMobileNetwork] INT             NULL,
    [DestinationMobileCountry] INT             NULL);



