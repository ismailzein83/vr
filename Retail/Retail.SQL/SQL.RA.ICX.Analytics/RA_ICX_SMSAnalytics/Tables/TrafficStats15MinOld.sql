CREATE TABLE [RA_ICX_SMSAnalytics].[TrafficStats15MinOld] (
    [ID]                       BIGINT           NOT NULL,
    [BatchStart]               DATETIME         NULL,
    [OperatorID]               BIGINT           NULL,
    [DataSource]               UNIQUEIDENTIFIER NULL,
    [Probe]                    BIGINT           NULL,
    [TrafficDirection]         INT              NULL,
    [OriginationCountryID]     INT              NULL,
    [DestinationCountryID]     INT              NULL,
    [NumberOfSMSs]             INT              NULL,
    [SuccessfulAttempts]       INT              NULL,
    [DeliveredAttempts]        INT              NULL,
    [DeliveryDelayInSeconds]   DECIMAL (22, 8)  NULL,
    [OriginationMobileNetwork] INT              NULL,
    [OriginationMobileCountry] INT              NULL,
    [DestinationMobileNetwork] INT              NULL,
    [DestinationMobileCountry] INT              NULL,
    [InterconnectOperator]     BIGINT           NULL,
    CONSTRAINT [IX_RA_ICX_SMS_TrafficStats15Min_IDOld] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_RA_ICX_SMS_TrafficStats15Min_BatchStartOld]
    ON [RA_ICX_SMSAnalytics].[TrafficStats15MinOld]([BatchStart] ASC);

