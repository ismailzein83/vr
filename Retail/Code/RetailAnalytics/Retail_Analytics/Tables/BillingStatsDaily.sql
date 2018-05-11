CREATE TABLE [Retail_Analytics].[BillingStatsDaily] (
    [Id]                      BIGINT           NULL,
    [BatchStart]              DATETIME         NULL,
    [SubscriberAccountTypeId] UNIQUEIDENTIFIER NULL,
    [SubscriberAccountId]     BIGINT           NULL,
    [FinancialAccountId]      BIGINT           NULL,
    [BillingAccountId]        VARCHAR (50)     NULL,
    [ServiceTypeId]           UNIQUEIDENTIFIER NULL,
    [TrafficDirection]        INT              NULL,
    [CallProgressState]       VARCHAR (100)    NULL,
    [InitiationCallType]      INT              NULL,
    [TerminationCallType]     INT              NULL,
    [InterconnectOperatorId]  BIGINT           NULL,
    [SubscriberZoneId]        BIGINT           NULL,
    [ZoneId]                  BIGINT           NULL,
    [NationalCallType]        INT              NULL,
    [PackageId]               INT              NULL,
    [ChargingPolicyId]        INT              NULL,
    [SaleRate]                DECIMAL (20, 8)  NULL,
    [SaleCurrencyId]          INT              NULL,
    [IsCharged]               BIT              NULL,
    [NumberOfCDRs]            INT              NULL,
    [TotalDuration]           DECIMAL (20, 4)  NULL,
    [TotalSaleDuration]       DECIMAL (20, 4)  NULL,
    [TotalChargedDuration]    DECIMAL (20, 4)  NULL,
    [TotalSaleAmount]         DECIMAL (26, 10) NULL,
    [SupplierName]            NVARCHAR (255)   NULL,
    [CostRate]                DECIMAL (20, 8)  NULL,
    [CostAmount]              DECIMAL (22, 6)  NULL,
    [CostCurrencyId]          INT              NULL,
    [CostAvailable]           BIT              NULL,
    [TotalProfit]             DECIMAL (26, 10) NULL,
    [ProfitStatus]            INT              NULL,
    CONSTRAINT [IX_BillingStatsDaily_Id] UNIQUE NONCLUSTERED ([Id] ASC)
);


































GO
CREATE NONCLUSTERED INDEX [IX_BillingStatsDaily_FinancialAccountId]
    ON [Retail_Analytics].[BillingStatsDaily]([FinancialAccountId] ASC);


GO
CREATE CLUSTERED INDEX [IX_BillingStatsDaily_BatchStartAndAccountId]
    ON [Retail_Analytics].[BillingStatsDaily]([BatchStart] ASC, [SubscriberAccountId] ASC);

