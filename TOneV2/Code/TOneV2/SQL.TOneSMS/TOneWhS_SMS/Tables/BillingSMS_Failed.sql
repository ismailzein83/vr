CREATE TABLE [TOneWhS_SMS].[BillingSMS_Failed] (
    [SMSId]                  BIGINT        NULL,
    [SentDateTime]           DATETIME      NULL,
    [DeliveredDateTime]      DATETIME      NULL,
    [CustomerId]             INT           NULL,
    [SupplierId]             INT           NULL,
    [OriginationMC_Id]       INT           NULL,
    [OriginationMN_Id]       INT           NULL,
    [DestinationMC_Id]       INT           NULL,
    [DestinationMN_Id]       INT           NULL,
    [OrigSender]             VARCHAR (40)  NULL,
    [Sender]                 VARCHAR (40)  NULL,
    [OrigReceiver]           VARCHAR (40)  NULL,
    [Receiver]               VARCHAR (40)  NULL,
    [OrigReceiverIn]         VARCHAR (50)  NULL,
    [OrigReceiverOut]        VARCHAR (50)  NULL,
    [SwitchId]               INT           NULL,
    [IDonSwitch]             BIGINT        NULL,
    [PortIN]                 VARCHAR (42)  NULL,
    [PortOUT]                VARCHAR (42)  NULL,
    [Tag]                    VARCHAR (100) NULL,
    [SaleFinancialAccountId] INT           NULL,
    [CostFinancialAccountId] INT           NULL,
    [CustomerDeliveryStatus] INT           NULL,
    [SupplierDeliveryStatus] INT           NULL,
    [Type]                   INT           NULL,
    [QueueItemId]            BIGINT        NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_BillingSMS_Failed_SMSId]
    ON [TOneWhS_SMS].[BillingSMS_Failed]([SMSId] ASC);


GO
CREATE CLUSTERED INDEX [IX_BillingSMS_Failed_SentDateTime]
    ON [TOneWhS_SMS].[BillingSMS_Failed]([SentDateTime] ASC);

