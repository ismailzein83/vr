﻿CREATE TYPE [Retail_Analytics].[TrafficStats15MinType] AS TABLE (
    [Id]                     BIGINT           NULL,
    [BatchStart]             DATETIME         NULL,
    [SubscriberAccountId]    BIGINT           NULL,
    [FinancialAccountId]     BIGINT           NULL,
    [ServiceTypeId]          UNIQUEIDENTIFIER NULL,
    [TrafficDirection]       INT              NULL,
    [CallProgressState]      VARCHAR (100)    NULL,
    [InitiationCallType]     INT              NULL,
    [TerminationCallType]    INT              NULL,
    [InterconnectOperatorId] BIGINT           NULL,
    [SubscriberZoneId]       BIGINT           NULL,
    [ZoneId]                 BIGINT           NULL,
    [NationalCallType]       INT              NULL,
    [PackageId]              INT              NULL,
    [ChargingPolicyId]       INT              NULL,
    [SaleRate]               DECIMAL (20, 8)  NULL,
    [SaleCurrencyId]         INT              NULL,
    [NumberOfCDRs]           INT              NULL,
    [TotalDuration]          DECIMAL (20, 4)  NULL,
    [TotalSaleDuration]      DECIMAL (20, 4)  NULL,
    [TotalSaleAmount]        DECIMAL (26, 10) NULL);















