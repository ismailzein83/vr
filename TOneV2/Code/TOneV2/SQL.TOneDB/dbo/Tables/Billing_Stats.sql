CREATE TABLE [dbo].[Billing_Stats] (
    [CallDate]          SMALLDATETIME   NOT NULL,
    [CustomerID]        VARCHAR (5)     NOT NULL,
    [SupplierID]        VARCHAR (5)     NOT NULL,
    [CostZoneID]        INT             NOT NULL,
    [SaleZoneID]        INT             NOT NULL,
    [Cost_Currency]     VARCHAR (3)     NULL,
    [Sale_Currency]     VARCHAR (3)     NULL,
    [NumberOfCalls]     INT             NULL,
    [FirstCallTime]     CHAR (6)        NULL,
    [LastCallTime]      CHAR (6)        NULL,
    [MinDuration]       NUMERIC (13, 4) NULL,
    [MaxDuration]       NUMERIC (13, 4) NULL,
    [AvgDuration]       NUMERIC (13, 4) NULL,
    [Cost_Nets]         FLOAT (53)      NULL,
    [Cost_Discounts]    FLOAT (53)      NULL,
    [Cost_Commissions]  FLOAT (53)      NULL,
    [Cost_ExtraCharges] FLOAT (53)      NULL,
    [Sale_Nets]         FLOAT (53)      NULL,
    [Sale_Discounts]    FLOAT (53)      NULL,
    [Sale_Commissions]  FLOAT (53)      NULL,
    [Sale_ExtraCharges] FLOAT (53)      NULL,
    [Sale_Rate]         FLOAT (53)      NULL,
    [Cost_Rate]         FLOAT (53)      NULL,
    [Sale_RateType]     TINYINT         CONSTRAINT [DF_Billing_Stats_Sale_RateType] DEFAULT ((0)) NULL,
    [Cost_RateType]     TINYINT         CONSTRAINT [DF_Billing_Stats_Cost_RateType] DEFAULT ((0)) NULL,
    [SaleDuration]      NUMERIC (13, 4) NULL,
    [CostDuration]      NUMERIC (13, 4) NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_Billing_Stats_Supplier]
    ON [dbo].[Billing_Stats]([SupplierID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Billing_Stats_Customer]
    ON [dbo].[Billing_Stats]([CustomerID] ASC);


GO
CREATE CLUSTERED INDEX [IX_Billing_Stats_Date]
    ON [dbo].[Billing_Stats]([CallDate] DESC);

