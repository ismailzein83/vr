CREATE TYPE [RA_INTL_SMSAnalytics].[TrafficStats15MinType] AS TABLE (
    [ID]                       BIGINT           NULL,
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
    [DeliveredAttempts]        INT              NULL);

