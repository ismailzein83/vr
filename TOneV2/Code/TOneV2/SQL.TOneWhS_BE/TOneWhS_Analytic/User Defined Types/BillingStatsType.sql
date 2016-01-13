CREATE TYPE [TOneWhS_Analytic].[BillingStatsType] AS TABLE (
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
    [MinDuration]      NUMERIC (20, 6) NULL,
    [MaxDuration]      NUMERIC (20, 6) NULL,
    [AvgDuration]      NUMERIC (20, 6) NULL,
    [CostNets]         NUMERIC (20, 6) NULL,
    [CostExtraCharges] NUMERIC (20, 6) NULL,
    [SaleNets]         NUMERIC (20, 6) NULL,
    [SaleExtraCharges] NUMERIC (20, 6) NULL,
    [SaleRate]         NUMERIC (20, 6) NULL,
    [CostRate]         NUMERIC (20, 6) NULL,
    [SaleDuration]     INT             NULL,
    [CostDuration]     INT             NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC));



