﻿CREATE TABLE [Retail_Analytics].[BillingStatsDaily] (
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
    [TotalChargedDuration]   DECIMAL (20, 4)  NULL,
    [TotalSaleAmount]        DECIMAL (26, 10) NULL,
    [SupplierName]           NVARCHAR (255)   NULL,
    [CostRate]               DECIMAL (20, 8)  NULL,
    [CostAmount]             DECIMAL (22, 6)  NULL,
    [CostCurrencyId]         INT              NULL,
    [CostAvailable]          BIT              NULL,
    [TotalProfit]            DECIMAL (26, 10) NULL,
    [ProfitStatus]           INT              NULL,
    CONSTRAINT [IX_BillingStatsDaily_Id] UNIQUE NONCLUSTERED ([Id] ASC)
);




























GO
CREATE CLUSTERED INDEX [IX_BillingStatsDaily_BatchStart]
    ON [Retail_Analytics].[BillingStatsDaily]([BatchStart] ASC);

