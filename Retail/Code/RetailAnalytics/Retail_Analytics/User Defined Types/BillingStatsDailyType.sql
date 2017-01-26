CREATE TYPE [Retail_Analytics].[BillingStatsDailyType] AS TABLE (
    [Id]                   BIGINT           NULL,
    [BatchStart]           DATETIME         NULL,
    [FinancialAccountId]   BIGINT           NULL,
    [ServiceTypeId]        UNIQUEIDENTIFIER NULL,
    [TrafficDirection]     INT              NULL,
    [InterconnectOperator] BIGINT           NULL,
    [ZoneId]               BIGINT           NULL,
    [PackageId]            INT              NULL,
    [ChargingPolicyId]     INT              NULL,
    [Rate]                 DECIMAL (20, 8)  NULL,
    [CurrencyId]           INT              NULL,
    [TotalDuration]        DECIMAL (20, 4)  NULL,
    [TotalAmount]          DECIMAL (20, 4)  NULL,
    [NumberOfCDRs]         INT              NULL);

