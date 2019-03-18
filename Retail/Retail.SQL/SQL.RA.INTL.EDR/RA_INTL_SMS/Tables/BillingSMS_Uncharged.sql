CREATE TABLE [RA_INTL_SMS].[BillingSMS_Uncharged] (
    [ID]                       BIGINT           NOT NULL,
    [OperatorID]               BIGINT           NULL,
    [DataSourceID]             UNIQUEIDENTIFIER NULL,
    [ProbeID]                  BIGINT           NULL,
    [IDOnSwitch]               VARCHAR (255)    NULL,
    [SentDateTime]             DATETIME         NULL,
    [DeliveredDateTime]        DATETIME         NULL,
    [Sender]                   VARCHAR (40)     NULL,
    [Receiver]                 VARCHAR (40)     NULL,
    [TrafficDirection]         INT              NULL,
    [OutCarrier]               VARCHAR (40)     NULL,
    [InCarrier]                VARCHAR (40)     NULL,
    [OriginationCountryID]     INT              NULL,
    [DestinationCountryID]     INT              NULL,
    [Rate]                     DECIMAL (22, 8)  NULL,
    [Revenue]                  DECIMAL (22, 8)  NULL,
    [RateTypeID]               INT              NULL,
    [RateValueRuleID]          INT              NULL,
    [RateTypeRuleID]           INT              NULL,
    [TaxRuleID]                INT              NULL,
    [Income]                   DECIMAL (22, 8)  NULL,
    [CurrencyID]               INT              NULL,
    [OriginationMobileNetwork] INT              NULL,
    [OriginationMobileCountry] INT              NULL,
    [DestinationMobileNetwork] INT              NULL,
    [DestinationMobileCountry] INT              NULL,
    [InDeliveryStatus]         INT              NULL,
    [OutDeliveryStatus]        INT              NULL,
    [DeliveryDelayInSeconds]   DECIMAL (22, 8)  NULL,
    [MatchedNumberPrefix]      BIGINT           NULL,
    [QueueItemId]              BIGINT           NULL
);


GO
CREATE CLUSTERED INDEX [IX_RA_INTL_SMS_BillingSMS_UnCharged_SentDateTime]
    ON [RA_INTL_SMS].[BillingSMS_Uncharged]([SentDateTime] ASC);

