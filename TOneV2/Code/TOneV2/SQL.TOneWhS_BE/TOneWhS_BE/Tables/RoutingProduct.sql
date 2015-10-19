CREATE TABLE [TOneWhS_BE].[RoutingProduct] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [Name]              NVARCHAR (255) NOT NULL,
    [SaleZonePackageID] INT            NOT NULL,
    [Settings]          NVARCHAR (MAX) NOT NULL,
    [timestamp]         ROWVERSION     NULL,
    CONSTRAINT [PK_RoutingProduct] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_RoutingProduct_SaleZonePackage] FOREIGN KEY ([SaleZonePackageID]) REFERENCES [TOneWhS_BE].[SaleZonePackage] ([ID])
);

