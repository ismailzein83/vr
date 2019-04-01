﻿CREATE TABLE [TOneWhS_SMS].[BillingSMS_PartialPriced] (
    [SMSId]                   BIGINT          NOT NULL,
    [SentDateTime]            DATETIME        NULL,
    [DeliveredDateTime]       DATETIME        NULL,
    [CustomerID]              INT             NULL,
    [SupplierID]              INT             NULL,
    [OriginationMC_Id]        INT             NULL,
    [OriginationMN_Id]        INT             NULL,
    [OriginationNumberPrefix] VARCHAR (50)    NULL,
    [DestinationMCC]          VARCHAR (5)     NULL,
    [DestinationMNC]          VARCHAR (5)     NULL,
    [DestinationMC_Id]        INT             NULL,
    [DestinationMN_Id]        INT             NULL,
    [DestinationNumberPrefix] VARCHAR (50)    NULL,
    [OrigSender]              VARCHAR (40)    NULL,
    [Sender]                  VARCHAR (40)    NULL,
    [OrigReceiver]            VARCHAR (40)    NULL,
    [OrigReceiverIn]          VARCHAR (50)    NULL,
    [OrigReceiverOut]         VARCHAR (50)    NULL,
    [Receiver]                VARCHAR (40)    NULL,
    [SwitchId]                INT             NULL,
    [IDonSwitch]              BIGINT          NULL,
    [PortIN]                  VARCHAR (42)    NULL,
    [PortOUT]                 VARCHAR (42)    NULL,
    [Tag]                     VARCHAR (100)   NULL,
    [SaleRateID]              BIGINT          NULL,
    [SaleRateValue]           DECIMAL (20, 8) NULL,
    [SaleNet]                 DECIMAL (22, 6) NULL,
    [SaleCurrencyId]          INT             NULL,
    [CostRateID]              BIGINT          NULL,
    [CostRateValue]           DECIMAL (20, 8) NULL,
    [CostNet]                 DECIMAL (22, 6) NULL,
    [CostCurrencyId]          INT             NULL,
    [SaleFinancialAccountId]  INT             NULL,
    [CostFinancialAccountId]  INT             NULL,
    [CustomerDeliveryStatus]  INT             NULL,
    [SupplierDeliveryStatus]  INT             NULL,
    [Type]                    INT             NULL,
    [QueueItemId]             BIGINT          NULL
);




GO
CREATE NONCLUSTERED INDEX [IX_BillingSMS_PartialPriced_SMSId]
    ON [TOneWhS_SMS].[BillingSMS_PartialPriced]([SMSId] ASC);


GO
CREATE CLUSTERED INDEX [IX_BillingSMS_PartialPriced_SentDateTime]
    ON [TOneWhS_SMS].[BillingSMS_PartialPriced]([SentDateTime] ASC);

