CREATE TABLE [ICX_SMS_Analytics].[TrafficStats15Min] (
    [ID]                         BIGINT           NULL,
    [BatchStart]                 DATETIME         NULL,
    [OperatorTypeID]             UNIQUEIDENTIFIER NULL,
    [OperatorID]                 BIGINT           NULL,
    [NumberOfSMS]                INT              NULL,
    [OriginationCountry]         INT              NULL,
    [DestinationCountry]         INT              NULL,
    [OriginationMobileNetworkID] INT              NULL,
    [DestinationMobileNetworkID] INT              NULL,
    [TrafficDirection]           INT              NULL,
    [RecordType]                 INT              NULL,
    [Scope]                      INT              NULL,
    [SuccessfulAttempts]         INT              NULL,
    [FirstSMSAttempt]            DATETIME         NULL,
    [LastSMSAttempt]             DATETIME         NULL,
    [FinancialAccountID]         BIGINT           NULL,
    [BillingAccountID]           VARCHAR (50)     NULL,
    [DataSource]                 UNIQUEIDENTIFIER NULL,
    [DeliveredAttempts]          INT              NULL,
    [DeliveryDelayInSeconds]     DECIMAL (22, 8)  NULL
);

