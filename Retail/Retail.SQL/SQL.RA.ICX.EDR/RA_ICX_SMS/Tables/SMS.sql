CREATE TABLE [RA_ICX_SMS].[SMS] (
    [ID]                BIGINT           NOT NULL,
    [OperatorID]        BIGINT           NULL,
    [TrafficDirection]  INT              NULL,
    [DataSourceID]      UNIQUEIDENTIFIER NULL,
    [ProbeID]           BIGINT           NULL,
    [IDOnSwitch]        VARCHAR (255)    NULL,
    [Tag]               VARCHAR (255)    NULL,
    [SentDateTime]      DATETIME         NULL,
    [DeliveredDateTime] DATETIME         NULL,
    [OutCarrier]        VARCHAR (40)     NULL,
    [InCarrier]         VARCHAR (40)     NULL,
    [Sender]            VARCHAR (40)     NULL,
    [Receiver]          VARCHAR (40)     NULL,
    [ExtraFields]       NVARCHAR (MAX)   NULL,
    [OriginationMNC]    VARCHAR (20)     NULL,
    [OriginationMCC]    VARCHAR (20)     NULL,
    [DestinationMNC]    VARCHAR (20)     NULL,
    [DestinationMCC]    VARCHAR (20)     NULL,
    [InDeliveryStatus]  INT              NULL,
    [OutDeliveryStatus] INT              NULL,
    [QueueItemId]       BIGINT           NULL,
    CONSTRAINT [IX_RA_ICX_SMS_SMS_ID] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_RA_ICX_SMS_SMS_SentDateTime]
    ON [RA_ICX_SMS].[SMS]([SentDateTime] ASC);

