CREATE TABLE [ICX_SMS].[BillingSMS_Interconnect] (
    [ID]                             BIGINT           NULL,
    [OperatorID]                     BIGINT           NULL,
    [DataSourceID]                   UNIQUEIDENTIFIER NULL,
    [IDOnSwitch]                     VARCHAR (255)    NULL,
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
    [Sender]                         VARCHAR (40)     NULL,
    [Receiver]                       VARCHAR (40)     NULL,
    [SentDateTime]                   DATETIME         NULL,
    [DeliveredDateTime]              DATETIME         NULL,
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
    [RecordType]                     INT              NULL,
    [GatewayID]                      INT              NULL,
    CONSTRAINT [IX_BillingSMS_Interconnect_ID] UNIQUE NONCLUSTERED ([ID] ASC)
);




GO
CREATE CLUSTERED INDEX [IX_BillingSMS_Interconnect_Sent_Operator]
    ON [ICX_SMS].[BillingSMS_Interconnect]([SentDateTime] ASC, [OperatorID] ASC);

