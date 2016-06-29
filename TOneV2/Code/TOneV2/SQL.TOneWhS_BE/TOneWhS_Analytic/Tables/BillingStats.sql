CREATE TABLE [TOneWhS_Analytic].[BillingStats] (
    [ID]               BIGINT          NOT NULL,
    [CallDate]         DATETIME        NOT NULL,
    [CustomerID]       INT             NOT NULL,
    [SupplierID]       INT             NOT NULL,
    [SupplierZoneID]   BIGINT          NOT NULL,
    [SaleZoneID]       BIGINT          NOT NULL,
    [CostCurrency]     INT             NULL,
    [SaleCurrency]     INT             NULL,
    [NumberOfCalls]    INT             NULL,
    [FirstCallTime]    VARCHAR (8)     NULL,
    [LastCallTime]     VARCHAR (8)     NULL,
    [MinDuration]      INT             NULL,
    [MaxDuration]      INT             NULL,
    [AvgDuration]      NUMERIC (20, 6) NULL,
    [CostNets]         NUMERIC (20, 6) NULL,
    [CostExtraCharges] NUMERIC (20, 6) NULL,
    [SaleNets]         NUMERIC (20, 6) NULL,
    [SaleExtraCharges] NUMERIC (20, 6) NULL,
    [SaleRate]         NUMERIC (20, 6) NULL,
    [CostRate]         NUMERIC (20, 6) NULL,
    [SaleDuration]     NUMERIC (20, 6) NULL,
    [CostDuration]     NUMERIC (20, 6) NULL,
    [SaleRateType]     INT             NULL,
    [CostRateType]     INT             NULL,
    [CostCommissions]  NUMERIC (20, 6) NULL,
    [SaleCommissions]  NUMERIC (20, 6) NULL,
    [CostDiscounts]    NUMERIC (20, 6) NULL,
    [SaleDiscounts]    NUMERIC (20, 6) NULL,
    [SaleRateId]       BIGINT          NULL,
    [CostRateId]       BIGINT          NULL,
    CONSTRAINT [IX_BillingStats_ID] UNIQUE NONCLUSTERED ([ID] ASC)
);










GO
CREATE CLUSTERED INDEX [IX_BillingStats_DateTimeFirst]
    ON [TOneWhS_Analytic].[BillingStats]([CallDate] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Billing_Stats_Supplier]
    ON [TOneWhS_Analytic].[BillingStats]([SupplierID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Billing_Stats_Customer]
    ON [TOneWhS_Analytic].[BillingStats]([CustomerID] ASC);

