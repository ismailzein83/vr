CREATE TYPE [ICX_SMS_Analytics].[TrafficStats15MinType] AS TABLE (
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
    [FinancialAccountID]         BIGINT           NULL,
    [BillingAccountID]           VARCHAR (50)     NULL,
    [DeliveredSMS]               INT              NULL,
    [FailedSMS]                  INT              NULL,
    [DeliveryDelayInSeconds]     BIGINT           NULL,
    [GatewayID]                  INT              NULL);



