﻿CREATE TABLE [ICX_Analytics].[BillingStatsDaily] (
    [ID]                            BIGINT           NULL,
    [BatchStart]                    DATETIME         NULL,
    [SwitchID]                      INT              NULL,
    [OperatorTypeID]                UNIQUEIDENTIFIER NULL,
    [OperatorID]                    BIGINT           NULL,
    [NumberOfCDRs]                  INT              NULL,
    [OriginationZoneID]             BIGINT           NULL,
    [DestinationZoneID]             BIGINT           NULL,
    [TrafficDirection]              INT              NULL,
    [BillingType]                   INT              NULL,
    [Rate]                          DECIMAL (20, 8)  NULL,
    [RateTypeID]                    INT              NULL,
    [CurrencyID]                    INT              NULL,
    [CDRType]                       INT              NULL,
    [CallType]                      INT              NULL,
    [TotalDurationInSeconds]        DECIMAL (20, 4)  NULL,
    [TotalBillingDurationInSeconds] DECIMAL (20, 4)  NULL,
    [TotalAmount]                   DECIMAL (26, 10) NULL,
    [MinimumDurationInSeconds]      DECIMAL (20, 4)  NULL,
    [MaximumDurationInSeconds]      DECIMAL (20, 4)  NULL
);

