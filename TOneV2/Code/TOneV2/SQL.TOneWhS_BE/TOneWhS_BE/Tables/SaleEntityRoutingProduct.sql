CREATE TABLE [TOneWhS_BE].[SaleEntityRoutingProduct] (
    [ID]               BIGINT     IDENTITY (1, 1) NOT NULL,
    [OwnerType]        TINYINT    NOT NULL,
    [OwnerID]          INT        NOT NULL,
    [ZoneID]           BIGINT     NULL,
    [RoutingProductID] INT        NOT NULL,
    [BED]              DATETIME   NOT NULL,
    [EED]              DATETIME   NULL,
    [timestamp]        ROWVERSION NULL,
    CONSTRAINT [PK_SaleZoneRoutingProduct] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SaleEntityRoutingProduct_RoutingProduct] FOREIGN KEY ([RoutingProductID]) REFERENCES [TOneWhS_BE].[RoutingProduct] ([ID]),
    CONSTRAINT [FK_SaleEntityRoutingProduct_SaleZone] FOREIGN KEY ([ZoneID]) REFERENCES [TOneWhS_BE].[SaleZone] ([ID])
);

