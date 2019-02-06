CREATE TABLE [RA_ICX_SMSAnalytics].[TrafficStats15Min] (
    [ID]                       BIGINT           NOT NULL,
    [BatchStart]               DATETIME         NULL,
    [OperatorID]               BIGINT           NULL,
    [DataSource]               UNIQUEIDENTIFIER NULL,
    [Probe]                    BIGINT           NULL,
    [TrafficDirection]         INT              NULL,
    [OriginationZoneID]        BIGINT           NULL,
    [DestinationZoneID]        BIGINT           NULL,
    [OriginationCountryID]     INT              NULL,
    [DestinationCountryID]     INT              NULL,
    [NumberOfSMSs]             INT              NULL,
    [SuccessfulAttempts]       INT              NULL,
    [CallingMNCID]             INT              NULL,
    [CallingMCCID]             INT              NULL,
    [CalledMNCID]              INT              NULL,
    [CalledMCCID]              INT              NULL,
    [DeliveredAttemps]         INT              NULL,
    [DeliveryDelayInSeconds]   DECIMAL (22, 8)  NULL,
    [OriginationMobileNetwork] INT              NULL,
    [OriginationMobileCountry] INT              NULL,
    [DestinationMobileNetwork] INT              NULL,
    [DestinationMobileCountry] INT              NULL,
    [DeliveredAttempts]        INT              NULL,
    [InterconnectOperator]     BIGINT           NULL,
    CONSTRAINT [IX_RA_ICX_SMS_TrafficStats15Min_ID] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_RA_ICX_SMS_TrafficStats15Min_BatchStart]
    ON [RA_ICX_SMSAnalytics].[TrafficStats15Min]([BatchStart] ASC);

