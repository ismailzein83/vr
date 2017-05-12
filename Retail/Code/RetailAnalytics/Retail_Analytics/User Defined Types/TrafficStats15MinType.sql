CREATE TYPE [Retail_Analytics].[TrafficStats15MinType] AS TABLE (
    [Id]                     BIGINT           NULL,
    [BatchStart]             DATETIME         NULL,
    [SubscriberAccountId]    BIGINT           NULL,
    [FinancialAccountId]     BIGINT           NULL,
    [ServiceTypeId]          UNIQUEIDENTIFIER NULL,
    [TrafficDirection]       INT              NULL,
    [InitiationCallType]     INT              NULL,
    [TerminationCallType]    INT              NULL,
    [InterconnectOperatorId] BIGINT           NULL,
    [ZoneId]                 BIGINT           NULL,
    [PackageId]              INT              NULL,
    [ChargingPolicyId]       INT              NULL,
    [SaleRate]               DECIMAL (20, 8)  NULL,
    [SaleCurrencyId]         INT              NULL,
    [TotalDuration]          DECIMAL (20, 4)  NULL,
    [TotalSaleAmount]        DECIMAL (26, 10) NULL,
    [NumberOfCDRs]           INT              NULL);





