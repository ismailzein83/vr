CREATE TABLE [Retail_Analytics].[BillingStatsDaily] (
    [Id]                     BIGINT           NULL,
    [BatchStart]             DATETIME         NULL,
    [FinancialAccountId]     BIGINT           NULL,
    [ServiceTypeId]          UNIQUEIDENTIFIER NULL,
    [TrafficDirection]       INT              NULL,
    [InterconnectOperatorId] BIGINT           NULL,
    [ZoneId]                 BIGINT           NULL,
    [PackageId]              INT              NULL,
    [ChargingPolicyId]       INT              NULL,
    [Rate]                   DECIMAL (20, 8)  NULL,
    [CurrencyId]             INT              NULL,
    [TotalDuration]          DECIMAL (20, 4)  NULL,
    [TotalAmount]            DECIMAL (26, 10) NULL,
    [NumberOfCDRs]           INT              NULL,
    CONSTRAINT [IX_BillingStatsDaily_Id] UNIQUE NONCLUSTERED ([Id] ASC)
);






GO
CREATE CLUSTERED INDEX [IX_BillingStatsDaily_BatchStart]
    ON [Retail_Analytics].[BillingStatsDaily]([BatchStart] ASC);

