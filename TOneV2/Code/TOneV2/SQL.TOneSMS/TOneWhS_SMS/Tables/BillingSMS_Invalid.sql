CREATE TABLE [TOneWhS_SMS].[BillingSMS_Invalid] (
    [SMSId]                  BIGINT        NOT NULL,
    [SentDateTime]           DATETIME      NULL,
    [DeliveredDateTime]      DATETIME      NULL,
    [CustomerID]             INT           NULL,
    [SupplierID]             INT           NULL,
    [OriginationMC_Id]       INT           NULL,
    [OriginationMN_Id]       INT           NULL,
    [DestinationMC_Id]       INT           NULL,
    [DestinationMN_Id]       INT           NULL,
    [OrigSender]             VARCHAR (40)  NULL,
    [Sender]                 VARCHAR (40)  NULL,
    [OrigReceiver]           VARCHAR (40)  NULL,
    [Receiver]               VARCHAR (40)  NULL,
    [OrigReceiverIn]         VARCHAR (50)  NOT NULL,
    [OrigReceiverOut]        VARCHAR (50)  NULL,
    [SwitchId]               INT           NULL,
    [IDonSwitch]             BIGINT        NULL,
    [PortIN]                 VARCHAR (42)  NULL,
    [PortOUT]                VARCHAR (42)  NULL,
    [Tag]                    VARCHAR (100) NULL,
    [SaleRateId]             BIGINT        NULL,
    [CostRateId]             BIGINT        NULL,
    [SaleFinancialAccountId] INT           NULL,
    [CostFinancialAccountId] INT           NULL,
    [CustomerDeliveryStatus] INT           NULL,
    [SupplierDeliveryStatus] INT           NULL,
    [Type]                   INT           NULL,
    [QueueItemId]            BIGINT        NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_BillingSMS_Invalid_SMSId]
    ON [TOneWhS_SMS].[BillingSMS_Invalid]([SMSId] ASC);


GO
CREATE CLUSTERED INDEX [IX_BillingSMS_Invalid_SentDateTime]
    ON [TOneWhS_SMS].[BillingSMS_Invalid]([SentDateTime] ASC);

