CREATE TYPE [RA_ICX_SMSAnalytics].[BillingStatsDailyType] AS TABLE (
    [ID]                       BIGINT          NULL,
    [BatchStart]               DATETIME        NULL,
    [OperatorID]               BIGINT          NULL,
    [NumberOfSMSs]             INT             NULL,
    [OriginationCountryID]     INT             NULL,
    [DestinationCountryID]     INT             NULL,
    [OriginationMobileNetwork] INT             NULL,
    [OriginationMobileCountry] INT             NULL,
    [DestinationMobileNetwork] INT             NULL,
    [DestinationMobileCountry] INT             NULL,
    [TrafficDirection]         INT             NULL,
    [Rate]                     DECIMAL (22, 8) NULL,
    [TotalRevenue]             DECIMAL (22, 8) NULL,
    [TotalIncome]              DECIMAL (22, 8) NULL,
    [CurrencyID]               INT             NULL,
    [InterconnectOperator]     BIGINT          NULL);

