CREATE TABLE [Retail_Analytics].[TrafficStats15Min] (
    [Id]                      BIGINT           NULL,
    [BatchStart]              DATETIME         NULL,
    [SubscriberAccountTypeId] UNIQUEIDENTIFIER NULL,
    [SubscriberAccountId]     BIGINT           NULL,
    [FinancialAccountId]      BIGINT           NULL,
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
    [SuccessfulCalls]         INT              NULL,
    [TotalDuration]           DECIMAL (20, 4)  NULL,
    [TotalSaleDuration]       DECIMAL (20, 4)  NULL,
    [TotalSaleAmount]         DECIMAL (26, 10) NULL,
    CONSTRAINT [IX_TrafficStats15Min_Id] UNIQUE NONCLUSTERED ([Id] ASC)
);


























GO
CREATE NONCLUSTERED INDEX [IX_TrafficStats15Min_FinancialAccountId]
    ON [Retail_Analytics].[TrafficStats15Min]([FinancialAccountId] ASC);


GO
CREATE CLUSTERED INDEX [IX_TrafficStats15Min_BatchStartAndAccountId]
    ON [Retail_Analytics].[TrafficStats15Min]([BatchStart] ASC, [SubscriberAccountId] ASC);

