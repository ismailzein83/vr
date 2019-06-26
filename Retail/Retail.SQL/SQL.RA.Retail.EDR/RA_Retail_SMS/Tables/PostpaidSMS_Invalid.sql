CREATE TABLE [RA_Retail_SMS].[PostpaidSMS_Invalid] (
    [ID]                               BIGINT           NOT NULL,
    [OperatorID]                       BIGINT           NULL,
    [DataSourceID]                     UNIQUEIDENTIFIER NULL,
    [ProbeID]                          BIGINT           NULL,
    [SentDateTime]                     DATETIME         NULL,
    [DeliveredDateTime]                DATETIME         NULL,
    [Sender]                           NVARCHAR (255)   NULL,
    [Receiver]                         NVARCHAR (255)   NULL,
    [TrafficDirection]                 INT              NULL,
    [OriginationMobileNetworkID]       INT              NULL,
    [DestinationMobileNetworkID]       INT              NULL,
    [OriginationMobileCountryID]       INT              NULL,
    [DestinationMobileCountryID]       INT              NULL,
    [InDeliveryStatus]                 INT              NULL,
    [OutDeliveryStatus]                INT              NULL,
    [DeliveryDelayInSeconds]           DECIMAL (20, 4)  NULL,
    [SubscriberID]                     BIGINT           NULL,
    [OriginationMatchedNumberPrefixID] BIGINT           NULL,
    [DestinationMatchedNumberPrefixID] BIGINT           NULL,
    [RecordType]                       INT              NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

