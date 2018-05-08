﻿CREATE TYPE [ICX_Analytics].[BillingStatsDailyType] AS TABLE (
    [ID]                            BIGINT           NULL,
    [BatchStart]                    DATETIME         NULL,
    [SwitchID]                      INT              NULL,
    [OperatorTypeID]                UNIQUEIDENTIFIER NULL,
    [OperatorID]                    BIGINT           NULL,
    [NumberOfCDRs]                  INT              NULL,
    [OriginationCountryID]          INT              NULL,
    [DestinationCountryID]          INT              NULL,
    [OriginationZoneID]             BIGINT           NULL,
    [DestinationZoneID]             BIGINT           NULL,
    [TrafficDirection]              INT              NULL,
    [BillingType]                   INT              NULL,
    [RateTypeID]                    INT              NULL,
    [Rate]                          DECIMAL (20, 8)  NULL,
    [CurrencyID]                    INT              NULL,
    [CDRType]                       INT              NULL,
    [CallType]                      INT              NULL,
    [TotalDurationInSeconds]        DECIMAL (20, 4)  NULL,
    [TotalBillingDurationInSeconds] DECIMAL (20, 4)  NULL,
    [TotalAmount]                   DECIMAL (26, 10) NULL,
    [MinimumDurationInSeconds]      DECIMAL (20, 4)  NULL,
    [MaximumDurationInSeconds]      DECIMAL (20, 4)  NULL);





