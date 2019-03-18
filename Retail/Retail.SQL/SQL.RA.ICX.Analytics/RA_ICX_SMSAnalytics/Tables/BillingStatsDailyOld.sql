CREATE TABLE [RA_ICX_SMSAnalytics].[BillingStatsDailyOld] (
    [ID]                       BIGINT          NOT NULL,
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
    [InterconnectOperator]     BIGINT          NULL,
    CONSTRAINT [IX_RA_ICX_SMS_BillingStatsDaily_IDOld] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_RA_ICX_SMS_BillingStatsDaily_BatchStartOld]
    ON [RA_ICX_SMSAnalytics].[BillingStatsDailyOld]([BatchStart] ASC);

