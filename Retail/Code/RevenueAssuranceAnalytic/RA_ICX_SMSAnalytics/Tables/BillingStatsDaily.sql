CREATE TABLE [RA_ICX_SMSAnalytics].[BillingStatsDaily] (
    [ID]                       BIGINT          NOT NULL,
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
    [DestinationMobileCountry] INT             NULL,
    [InterconnectOperator]     BIGINT          NULL,
    CONSTRAINT [IX_RA_ICX_SMS_BillingStatsDaily_ID] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_RA_ICX_SMS_BillingStatsDaily_BatchStart]
    ON [RA_ICX_SMSAnalytics].[BillingStatsDaily]([BatchStart] ASC);

