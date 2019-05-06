﻿CREATE TABLE [ICX_SMS].[BillingSMS_Main] (
    [ID]                             BIGINT           NULL,
    [OperatorID]                     BIGINT           NULL,
    [DataSourceID]                   UNIQUEIDENTIFIER NULL,
    [IDOnSwitch]                     VARCHAR (255)    NULL,
    [SentDateTime]                   DATETIME         NULL,
    [DeliveredDateTime]              DATETIME         NULL,
    [Sender]                         VARCHAR (40)     NULL,
    [Receiver]                       VARCHAR (40)     NULL,
    [TrafficDirection]               INT              NULL,
    [OriginationCountry]             INT              NULL,
    [DestinationCountry]             INT              NULL,
    [Rate]                           DECIMAL (22, 8)  NULL,
    [Amount]                         DECIMAL (22, 8)  NULL,
    [RateTypeID]                     INT              NULL,
    [RateValueRuleID]                INT              NULL,
    [RateTypeRuleID]                 INT              NULL,
    [CurrencyID]                     INT              NULL,
    [OriginationMobileNetwork]       INT              NULL,
    [OriginationMobileCountry]       INT              NULL,
    [DestinationMobileNetwork]       INT              NULL,
    [DestinationMobileCountry]       INT              NULL,
    [QueueItemId]                    BIGINT           NULL,
    [BillingType]                    INT              NULL,
    [OperatorTypeID]                 UNIQUEIDENTIFIER NULL,
    [InDeliveryStatus]               INT              NULL,
    [OutDeliveryStatus]              INT              NULL,
    [OriginationMatchedNumberPrefix] BIGINT           NULL,
    [DestinationMatchedNumberPrefix] BIGINT           NULL,
    [SenderOperatorID]               BIGINT           NULL,
    [ReceiverOperatorID]             BIGINT           NULL,
    [Scope]                          INT              NULL,
    [FinancialAccountID]             BIGINT           NULL,
    [BillingAccountID]               VARCHAR (50)     NULL,
    [RecordType]                     INT              NULL,
    [GatewayID]                      INT              NULL,
    [OriginationMCC]                 VARCHAR (20)     NULL,
    [OriginationMNC]                 VARCHAR (20)     NULL,
    [DestinationMCC]                 VARCHAR (20)     NULL,
    [DestinationMNC]                 VARCHAR (20)     NULL,
    CONSTRAINT [IX_BillingSMS_Main_ID] UNIQUE NONCLUSTERED ([ID] ASC)
);






GO
CREATE CLUSTERED INDEX [IX_BillingSMS_Main_Sent_Operator]
    ON [ICX_SMS].[BillingSMS_Main]([SentDateTime] ASC, [OperatorID] ASC);

