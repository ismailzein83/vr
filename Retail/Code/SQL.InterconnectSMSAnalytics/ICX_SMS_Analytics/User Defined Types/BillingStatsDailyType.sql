CREATE TYPE [ICX_SMS_Analytics].[BillingStatsDailyType] AS TABLE (
    [ID]                         BIGINT           NULL,
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
    [GatewayID]                  INT              NULL);



