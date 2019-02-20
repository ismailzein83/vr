CREATE TABLE [TOneWhS_SMS].[SMS] (
    [Id]                     BIGINT         NOT NULL,
    [SwitchID]               INT            NULL,
    [IDonSwitch]             BIGINT         NULL,
    [Tag]                    VARCHAR (100)  NULL,
    [SentDateTime]           DATETIME       NULL,
    [DeliveredDateTime]      DATETIME       NULL,
    [IN_TRUNK]               VARCHAR (50)   NULL,
    [IN_CIRCUIT]             BIGINT         NULL,
    [IN_CARRIER]             VARCHAR (100)  NULL,
    [IN_IP]                  VARCHAR (42)   NULL,
    [OUT_TRUNK]              VARCHAR (50)   NULL,
    [OUT_CIRCUIT]            BIGINT         NULL,
    [OUT_CARRIER]            VARCHAR (100)  NULL,
    [OUT_IP]                 VARCHAR (42)   NULL,
    [Sender]                 VARCHAR (40)   NULL,
    [Receiver]               VARCHAR (40)   NULL,
    [ReceiverIn]             VARCHAR (50)   NULL,
    [ReceiverOut]            VARCHAR (50)   NULL,
    [OriginationMCC]         VARCHAR (5)    NULL,
    [OriginationMNC]         VARCHAR (5)    NULL,
    [DestinationMCC]         VARCHAR (5)    NULL,
    [DestinationMNC]         VARCHAR (5)    NULL,
    [CustomerDeliveryStatus] INT            NULL,
    [SupplierDeliveryStatus] INT            NULL,
    [ExtraFields]            NVARCHAR (MAX) NULL,
    [QueueItemId]            BIGINT         NULL,
    CONSTRAINT [IX_SMS_Id] UNIQUE NONCLUSTERED ([Id] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_SMS_SentDateTime]
    ON [TOneWhS_SMS].[SMS]([SentDateTime] ASC);

