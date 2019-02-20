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
    [FinancialAccountID]         BIGINT           NULL,
    [BillingAccountID]           VARCHAR (50)     NULL,
    [DeliveredSMS]               INT              NULL,
    [FailedSMS]                  INT              NULL,
    [DeliveryDelayInSeconds]     BIGINT           NULL,
    [GatewayID]                  INT              NULL,
    CONSTRAINT [IX_TrafficStats15Min_ID] UNIQUE NONCLUSTERED ([ID] ASC)
);




GO
CREATE CLUSTERED INDEX [IX_TrafficStats15Min_BatchStart]
    ON [ICX_SMS_Analytics].[TrafficStats15Min]([BatchStart] ASC);

