CREATE TABLE [RA_INTL_SMS].[SMS] (
    [ID]                       BIGINT           IDENTITY (1, 1) NOT NULL,
    [OperatorID]               BIGINT           NULL,
    [TrafficDirection]         INT              NULL,
    [DataSourceID]             UNIQUEIDENTIFIER NULL,
    [ProbeID]                  BIGINT           NULL,
    [IDOnSwitch]               VARCHAR (255)    NULL,
    [SentDateTime]             DATETIME         NULL,
    [DeliveredDateTime]        DATETIME         NULL,
    [Tag]                      VARCHAR (255)    NULL,
    [OutCarrier]               VARCHAR (40)     NULL,
    [InCarrier]                VARCHAR (40)     NULL,
    [Sender]                   VARCHAR (40)     NULL,
    [Receiver]                 VARCHAR (40)     NULL,
    [ExtraFields]              NVARCHAR (MAX)   NULL,
    [CallingMNC]               VARCHAR (10)     NULL,
    [CallingMCC]               VARCHAR (10)     NULL,
    [CalledMNC]                VARCHAR (10)     NULL,
    [CalledMCC]                VARCHAR (10)     NULL,
    [QueueItemId]              BIGINT           NULL,
    [CustomerDeliveryStatus]   VARCHAR (50)     NULL,
    [VendorDeliveryStatus]     VARCHAR (50)     NULL,
    [OriginationMobileNetwork] INT              NULL,
    [OriginationMobileCountry] INT              NULL,
    [DestinationMobileNetwork] INT              NULL,
    [DestinationMobileCountry] INT              NULL,
    [OriginationMNC]           VARCHAR (20)     NULL,
    [OriginationMCC]           VARCHAR (20)     NULL,
    [DestinationMNC]           VARCHAR (20)     NULL,
    [DestinationMCC]           VARCHAR (20)     NULL,
    CONSTRAINT [IX_RA_INTL_SMS_SMS_ID] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_RA_INTL_SMS_SMS_SentDateTime]
    ON [RA_INTL_SMS].[SMS]([SentDateTime] ASC);

