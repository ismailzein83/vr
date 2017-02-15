CREATE TABLE [Retail_Analytics].[TrafficStats15Min] (
    [Id]                     BIGINT           NULL,
    [BatchStart]             DATETIME         NULL,
    [SubscriberAccountId]    BIGINT           NULL,
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
    CONSTRAINT [IX_TrafficStats15Min_Id] UNIQUE NONCLUSTERED ([Id] ASC)
);




GO
CREATE CLUSTERED INDEX [IX_TrafficStats15Min_BatchStart]
    ON [Retail_Analytics].[TrafficStats15Min]([BatchStart] ASC);

