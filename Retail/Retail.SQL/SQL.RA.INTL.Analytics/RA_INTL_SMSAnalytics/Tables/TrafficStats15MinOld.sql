CREATE TABLE [RA_INTL_SMSAnalytics].[TrafficStats15MinOld] (
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
    [DeliveryDelayInSeconds]   DECIMAL (30, 8)  NULL,
    [OriginationMobileNetwork] INT              NULL,
    [OriginationMobileCountry] INT              NULL,
    [DestinationMobileNetwork] INT              NULL,
    [DestinationMobileCountry] INT              NULL,
    [DeliveredAttempts]        INT              NULL,
    CONSTRAINT [IX_RA_INTL_SMS_TrafficStats15Min_IDOld] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_RA_INTL_SMS_TrafficStats15Min_BatchStartOld]
    ON [RA_INTL_SMSAnalytics].[TrafficStats15MinOld]([BatchStart] ASC);

