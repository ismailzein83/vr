CREATE TABLE [RA_Retail_SMS].[SMS] (
    [ID]                BIGINT           NOT NULL,
    [TrafficDirection]  INT              NULL,
    [SentDateTime]      DATETIME         NULL,
    [DeliveredDateTime] DATETIME         NULL,
    [Sender]            NVARCHAR (255)   NULL,
    [Receiver]          NVARCHAR (255)   NULL,
    [OriginationMNC]    NVARCHAR (255)   NULL,
    [OriginationMCC]    NVARCHAR (255)   NULL,
    [DestinationMNC]    NVARCHAR (255)   NULL,
    [DestinationMCC]    NVARCHAR (255)   NULL,
    [InDeliveryStatus]  INT              NULL,
    [OutDeliveryStatus] INT              NULL,
    [SubscriberType]    INT              NULL,
    [OperatorID]        BIGINT           NULL,
    [DataSourceID]      UNIQUEIDENTIFIER NULL,
    [ProbeID]           BIGINT           NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

