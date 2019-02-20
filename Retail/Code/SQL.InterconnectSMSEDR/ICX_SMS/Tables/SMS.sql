CREATE TABLE [ICX_SMS].[SMS] (
    [ID]                BIGINT           NULL,
    [DataSourceID]      UNIQUEIDENTIFIER NULL,
    [IDOnSwitch]        VARCHAR (255)    NULL,
    [Tag]               VARCHAR (255)    NULL,
    [SentDateTime]      DATETIME         NULL,
    [DeliveredDateTime] DATETIME         NULL,
    [Sender]            VARCHAR (40)     NULL,
    [Receiver]          VARCHAR (40)     NULL,
    [ExtraFields]       NVARCHAR (MAX)   NULL,
    [OriginationMNC]    VARCHAR (20)     NULL,
    [OriginationMCC]    VARCHAR (20)     NULL,
    [DestinationMNC]    VARCHAR (20)     NULL,
    [DestinationMCC]    VARCHAR (20)     NULL,
    [QueueItemId]       BIGINT           NULL,
    [InDeliveryStatus]  INT              NULL,
    [OutDeliveryStatus] INT              NULL,
    [Trunk]             NVARCHAR (MAX)   NULL,
    [GatewayID]         INT              NULL,
    CONSTRAINT [IX_SMS_ID] UNIQUE NONCLUSTERED ([ID] ASC)
);




GO
CREATE CLUSTERED INDEX [IX_SMS_SentDateTime]
    ON [ICX_SMS].[SMS]([SentDateTime] ASC);

