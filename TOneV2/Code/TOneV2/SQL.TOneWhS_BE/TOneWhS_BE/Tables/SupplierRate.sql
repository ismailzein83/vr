CREATE TABLE [TOneWhS_BE].[SupplierRate] (
    [ID]          BIGINT         IDENTITY (1, 1) NOT NULL,
    [PriceListID] INT            NOT NULL,
    [ZoneID]      BIGINT         NOT NULL,
    [Rate]        DECIMAL (9, 5) NOT NULL,
    [OffPeakRate] DECIMAL (9, 5) NULL,
    [WeekendRate] DECIMAL (9, 5) NULL,
    [BED]         DATETIME       NOT NULL,
    [EED]         DATETIME       NULL,
    CONSTRAINT [PK_SupplierRate] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SupplierRate_SupplierPriceList] FOREIGN KEY ([PriceListID]) REFERENCES [TOneWhS_BE].[SupplierPriceList] ([ID]),
    CONSTRAINT [FK_SupplierRate_SupplierZone] FOREIGN KEY ([ZoneID]) REFERENCES [TOneWhS_BE].[SupplierZone] ([ID])
);

