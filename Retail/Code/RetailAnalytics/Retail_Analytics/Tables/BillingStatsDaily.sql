CREATE TABLE [Retail_Analytics].[BillingStatsDaily] (
    [Id]                     BIGINT           NULL,
    [BatchStart]             DATETIME         NULL,
    [FinancialAccountId]     BIGINT           NULL,
    [ServiceTypeId]          UNIQUEIDENTIFIER NULL,
    [TrafficDirection]       INT              NULL,
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
    [TotalDuration]          DECIMAL (20, 4)  NULL,
    [TotalSaleDuration]      DECIMAL (20, 4)  NULL,
    [TotalSaleAmount]        DECIMAL (26, 10) NULL,
    [NumberOfCDRs]           INT              NULL,
    [SubscriberAccountId]    BIGINT           NULL,
    [SupplierName]           NVARCHAR (255)   NULL,
    [CostRate]               DECIMAL (20, 8)  NULL,
    [CostAmount]             DECIMAL (22, 6)  NULL,
    [CostCurrencyId]         INT              NULL,
    [CostAvailable]          BIT              NULL,
    CONSTRAINT [IX_BillingStatsDaily_Id] UNIQUE NONCLUSTERED ([Id] ASC)
);




















GO
CREATE CLUSTERED INDEX [IX_BillingStatsDaily_BatchStart]
    ON [Retail_Analytics].[BillingStatsDaily]([BatchStart] ASC);

