CREATE TYPE [dbo].[BillingStatsType] AS TABLE (
    [CallDate]      DATETIME        NOT NULL,
    [CarrierID]     VARCHAR (5)     NOT NULL,
    [SupplierID]    VARCHAR (5)     NOT NULL,
    [CostZoneID]    INT             NOT NULL,
    [SaleZoneID]    INT             NOT NULL,
    [SaleDuration]  NUMERIC (13, 4) NOT NULL,
    [CostDuration]  NUMERIC (13, 4) NOT NULL,
    [SaleAmount]    NUMERIC (13, 5) NOT NULL,
    [CostAmount]    NUMERIC (13, 5) NOT NULL,
    [NumberOfCalls] INT             NOT NULL,
    [AvgDuration]   NUMERIC (13, 5) NOT NULL,
    [Sale_Rate]     NUMERIC (13, 5) NOT NULL,
    [Cost_Rate]     NUMERIC (13, 5) NOT NULL);

