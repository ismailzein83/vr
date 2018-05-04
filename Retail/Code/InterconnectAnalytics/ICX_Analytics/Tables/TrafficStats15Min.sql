﻿CREATE TABLE [ICX_Analytics].[TrafficStats15Min] (
    [ID]                     BIGINT           NULL,
    [BatchStart]             DATETIME         NULL,
    [SwitchID]               INT              NULL,
    [OperatorTypeID]         UNIQUEIDENTIFIER NULL,
    [OperatorID]             BIGINT           NULL,
    [NumberOfCDRs]           INT              NULL,
    [OriginationZoneID]      BIGINT           NULL,
    [DestinationZoneID]      BIGINT           NULL,
    [TrafficDirection]       INT              NULL,
    [CDRType]                INT              NULL,
    [CallType]               INT              NULL,
    [SuccessfulAttempts]     INT              NULL,
    [DurationInSeconds]      DECIMAL (20, 4)  NULL,
    [TotalDurationInSeconds] DECIMAL (20, 4)  NULL,
    [SumOfPDDInSeconds]      DECIMAL (20, 4)  NULL,
    [SumOfPGAD]              DECIMAL (25)     NULL,
    [FirstCDRAttempt]        DATETIME         NULL,
    [LastCDRAttempt]         DATETIME         NULL
);

