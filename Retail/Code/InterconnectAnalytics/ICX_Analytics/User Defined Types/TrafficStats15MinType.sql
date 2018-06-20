CREATE TYPE [ICX_Analytics].[TrafficStats15MinType] AS TABLE (
    [ID]                       BIGINT           NULL,
    [BatchStart]               DATETIME         NULL,
    [SwitchID]                 INT              NULL,
    [OperatorTypeID]           UNIQUEIDENTIFIER NULL,
    [OperatorID]               BIGINT           NULL,
    [NumberOfCDRs]             INT              NULL,
    [OriginationCountryID]     INT              NULL,
    [DestinationCountryID]     INT              NULL,
    [OriginationZoneID]        BIGINT           NULL,
    [DestinationZoneID]        BIGINT           NULL,
    [TrafficDirection]         INT              NULL,
    [CDRType]                  INT              NULL,
    [CallType]                 INT              NULL,
    [SuccessfulAttempts]       INT              NULL,
    [TotalDurationInSeconds]   DECIMAL (20, 4)  NULL,
    [SumOfPDDInSeconds]        DECIMAL (20, 4)  NULL,
    [SumOfPGAD]                DECIMAL (25)     NULL,
    [FirstCDRAttempt]          DATETIME         NULL,
    [LastCDRAttempt]           DATETIME         NULL,
    [MinimumDurationInSeconds] DECIMAL (20, 4)  NULL,
    [MaximumDurationInSeconds] DECIMAL (20, 4)  NULL,
    [FinancialAccountId]       BIGINT           NULL,
    [BillingAccountId]         VARCHAR (50)     NULL);





