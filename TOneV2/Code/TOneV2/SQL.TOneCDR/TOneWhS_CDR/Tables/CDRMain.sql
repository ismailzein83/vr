CREATE TABLE [TOneWhS_CDR].[CDRMain] (
    [ID]                BIGINT          NOT NULL,
    [CustomerID]        INT             NULL,
    [SupplierID]        INT             NULL,
    [Attempt]           DATETIME        NULL,
    [DurationInSeconds] INT             NULL,
    [Alert]             DATETIME        NULL,
    [Connect]           DATETIME        NULL,
    [Disconnect]        DATETIME        NULL,
    [PortOut]           NVARCHAR (50)   NULL,
    [PortIn]            NVARCHAR (50)   NULL,
    [SaleCode]          NVARCHAR (50)   NULL,
    [SaleZoneID]        BIGINT          NULL,
    [SupplierCode]      NVARCHAR (50)   NULL,
    [SupplierZoneID]    BIGINT          NULL,
    [CDPN]              NVARCHAR (50)   NULL,
    [CGPN]              NVARCHAR (50)   NULL,
    [CostRateValue]     DECIMAL (13, 4) NULL,
    [CostTotalNet]      DECIMAL (13, 4) NULL,
    [CostCurrencyID]    INT             NULL,
    [SaleRateValue]     DECIMAL (13, 4) NULL,
    [SaleTotalNet]      DECIMAL (13, 4) NULL,
    [SaleCurrencyID]    INT             NULL,
    [ReleaseCode]       NVARCHAR (50)   NULL,
    [ReleaseSource]     NVARCHAR (50)   NULL,
    [SwitchID]          INT             NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_CDRMain_Customer]
    ON [TOneWhS_CDR].[CDRMain]([CustomerID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CDRMain_SupplierID]
    ON [TOneWhS_CDR].[CDRMain]([SupplierID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CDRMain_SaleZoneID]
    ON [TOneWhS_CDR].[CDRMain]([SaleZoneID] ASC);


GO
CREATE CLUSTERED INDEX [IX_CDRMain_Attempt]
    ON [TOneWhS_CDR].[CDRMain]([Attempt] DESC);

