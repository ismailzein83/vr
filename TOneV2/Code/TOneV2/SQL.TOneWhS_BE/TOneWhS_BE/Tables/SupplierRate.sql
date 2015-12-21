CREATE TABLE [TOneWhS_BE].[SupplierRate] (
    [ID]          BIGINT         NOT NULL,
    [PriceListID] INT            NOT NULL,
    [ZoneID]      BIGINT         NOT NULL,
    [CurrencyID]  INT            NULL,
    [NormalRate]  DECIMAL (9, 5) NOT NULL,
    [OtherRates]  VARCHAR (MAX)  NULL,
    [BED]         DATETIME       NOT NULL,
    [EED]         DATETIME       NULL,
    [timestamp]   ROWVERSION     NULL,
    CONSTRAINT [PK_SupplierRate] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SupplierRate_SupplierPriceList] FOREIGN KEY ([PriceListID]) REFERENCES [TOneWhS_BE].[SupplierPriceList] ([ID]),
    CONSTRAINT [FK_SupplierRate_SupplierZone] FOREIGN KEY ([ZoneID]) REFERENCES [TOneWhS_BE].[SupplierZone] ([ID])
);



