CREATE TABLE [TOneWhS_Analytics].[TrafficStats15Min] (
    [Id]                          BIGINT          NULL,
    [BatchStart]                  DATETIME        NULL,
    [CustomerID]                  INT             NULL,
    [SaleZoneID]                  BIGINT          NULL,
    [CountryID]                   INT             NULL,
    [OriginatingZoneID]           BIGINT          NULL,
    [SupplierId]                  INT             NULL,
    [SupplierZoneId]              BIGINT          NULL,
    [FirstCDRAttempt]             DATETIME        NULL,
    [LastCDRAttempt]              DATETIME        NULL,
    [Attempts]                    INT             NULL,
    [DeliveredAttempts]           INT             NULL,
    [SuccessfulAttempts]          INT             NULL,
    [DurationInSeconds]           DECIMAL (20, 4) NULL,
    [SumOfPDDInSeconds]           DECIMAL (25)    NULL,
    [MaxDurationInSeconds]        DECIMAL (20, 4) NULL,
    [NumberOfCalls]               INT             NULL,
    [DeliveredNumberOfCalls]      INT             NULL,
    [CeiledDuration]              DECIMAL (20, 4) NULL,
    [SumOfPGAD]                   DECIMAL (25)    NULL,
    [UtilizationInSeconds]        DECIMAL (25, 4) NULL,
    [PricedCalls]                 INT             NULL,
    [SaleNet]                     DECIMAL (20, 4) NULL,
    [SaleDurationInSeconds]       DECIMAL (20, 4) NULL,
    [CostNet]                     DECIMAL (20, 4) NULL,
    [CostDurationInSeconds]       DECIMAL (20, 4) NULL,
    [SaleCurrencyId]              INT             NULL,
    [CostCurrencyId]              INT             NULL,
    [SellingNumberPlanID]         INT             NULL,
    [MasterPlanZoneID]            BIGINT          NULL,
    [MasterPlanOriginatingZoneID] BIGINT          NULL,
    [PortIN]                      VARCHAR (42)    NULL,
    [PortOUT]                     VARCHAR (42)    NULL,
    [SwitchID]                    INT             NULL,
    [CustomerProfileId]           INT             NULL,
    [SupplierProfileId]           INT             NULL,
    [ReleaseSourceSCount]         INT             NULL,
    CONSTRAINT [IX_TrafficStats15Min_Id] UNIQUE NONCLUSTERED ([Id] ASC)
);








GO
CREATE CLUSTERED INDEX [IX_TrafficStats15Min_BatchStart]
    ON [TOneWhS_Analytics].[TrafficStats15Min]([BatchStart] ASC);

