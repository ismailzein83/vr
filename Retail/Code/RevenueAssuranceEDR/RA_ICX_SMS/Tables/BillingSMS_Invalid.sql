CREATE TABLE [RA_ICX_SMS].[BillingSMS_Invalid] (
    [ID]                       BIGINT           NOT NULL,
    [OperatorID]               BIGINT           NULL,
    [DataSourceID]             UNIQUEIDENTIFIER NULL,
    [ProbeID]                  BIGINT           NULL,
    [IDOnSwitch]               VARCHAR (255)    NULL,
    [SentDateTime]             DATETIME         NULL,
    [DelivereDateTime]         DATETIME         NULL,
    [Sender]                   VARCHAR (40)     NULL,
    [Receiver]                 VARCHAR (40)     NULL,
    [TrafficDirection]         INT              NULL,
    [OutCarrier]               VARCHAR (40)     NULL,
    [InCarrier]                VARCHAR (40)     NULL,
    [OriginationZoneID]        BIGINT           NULL,
    [DestinationZoneID]        BIGINT           NULL,
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
    [CallingMNCID]             INT              NULL,
    [CallingMCCID]             INT              NULL,
    [CalledMNCID]              INT              NULL,
    [CalledMCCID]              INT              NULL,
    [QueueItemId]              BIGINT           NULL,
    [CustomerDeliveryStatus]   INT              NULL,
    [VendorDeliveryStatus]     INT              NULL,
    [DeliveryDelayInSeconds]   DECIMAL (22, 8)  NULL,
    [OriginationMobileNetwork] INT              NULL,
    [OriginationMobileCountry] INT              NULL,
    [DestinationMobileNetwork] INT              NULL,
    [DestinationMobileCountry] INT              NULL,
    [MatchedNumberPrefixID]    BIGINT           NULL,
    [InterconnectOperator]     BIGINT           NULL,
    [InDeliveryStatus]         INT              NULL,
    [OutDeliveryStatus]        INT              NULL,
    CONSTRAINT [IX_RA_ICX_SMS_BillingSMS_Invalid_ID] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_RA_ICX_SMS_BillingSMS_Invalid_SentDateTime]
    ON [RA_ICX_SMS].[BillingSMS_Invalid]([SentDateTime] ASC);

