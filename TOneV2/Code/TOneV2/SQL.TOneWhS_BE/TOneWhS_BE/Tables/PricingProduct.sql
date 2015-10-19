CREATE TABLE [TOneWhS_BE].[PricingProduct] (
    [ID]                      INT            IDENTITY (1, 1) NOT NULL,
    [Name]                    NVARCHAR (255) NOT NULL,
    [DefaultRoutingProductID] INT            NULL,
    [SaleZonePackageID]       INT            NOT NULL,
    [Settings]                NVARCHAR (MAX) NULL,
    [timestamp]               ROWVERSION     NULL,
    CONSTRAINT [PK_PricingProduct] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_PricingProduct_RoutingProduct] FOREIGN KEY ([DefaultRoutingProductID]) REFERENCES [TOneWhS_BE].[RoutingProduct] ([ID]),
    CONSTRAINT [FK_PricingProduct_SaleZonePackage] FOREIGN KEY ([SaleZonePackageID]) REFERENCES [TOneWhS_BE].[SaleZonePackage] ([ID])
);

