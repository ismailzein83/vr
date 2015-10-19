CREATE TABLE [TOneWhS_BE].[SaleRate] (
    [ID]               BIGINT         IDENTITY (1, 1) NOT NULL,
    [PriceListID]      INT            NOT NULL,
    [ZoneID]           BIGINT         NOT NULL,
    [RoutingProductID] INT            NULL,
    [Rate]             DECIMAL (9, 5) NOT NULL,
    [OffPeakRate]      DECIMAL (9, 5) NULL,
    [WeekendRate]      DECIMAL (9, 5) NULL,
    [BED]              DATETIME       NOT NULL,
    [EED]              DATETIME       NULL,
    CONSTRAINT [PK_SaleRate] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SaleRate_RoutingProduct] FOREIGN KEY ([RoutingProductID]) REFERENCES [TOneWhS_BE].[RoutingProduct] ([ID]),
    CONSTRAINT [FK_SaleRate_SalePriceList] FOREIGN KEY ([PriceListID]) REFERENCES [TOneWhS_BE].[SalePriceList] ([ID]),
    CONSTRAINT [FK_SaleRate_SaleZone] FOREIGN KEY ([ZoneID]) REFERENCES [TOneWhS_BE].[SaleZone] ([ID])
);

