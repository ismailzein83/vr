CREATE TABLE [ICX_SMS_Analytics].[BillingStatsDaily] (
    [ID]                         BIGINT           NOT NULL,
    [BatchStart]                 DATETIME         NULL,
    [OperatorTypeID]             UNIQUEIDENTIFIER NULL,
    [OperatorID]                 BIGINT           NULL,
    [NumberOfSMS]                INT              NULL,
    [OriginationCountryID]       INT              NULL,
    [DestinationCountryID]       INT              NULL,
    [OriginationMobileNetworkID] INT              NULL,
    [DestinationMobileNetworkID] INT              NULL,
    [TrafficDirection]           INT              NULL,
    [BillingType]                INT              NULL,
    [Rate]                       DECIMAL (20, 8)  NULL,
    [RateTypeID]                 INT              NULL,
    [CurrencyID]                 INT              NULL,
    [RecordType]                 INT              NULL,
    [Scope]                      INT              NULL,
    [TotalAmount]                DECIMAL (26, 10) NULL,
    [FinancialAccountID]         BIGINT           NULL,
    [BillingAccountID]           VARCHAR (50)     NULL,
    CONSTRAINT [IX_ICX_SMS_Analytics_BillingStatsDaily_ID] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_ICX_SMS_Analytics_BillingStatsDaily_BatchStart]
    ON [ICX_SMS_Analytics].[BillingStatsDaily]([BatchStart] ASC);

