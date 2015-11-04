CREATE TABLE [TOneWhS_BE].[SaleZoneRoutingProduct] (
    [ID]               BIGINT   IDENTITY (1, 1) NOT NULL,
    [OwnerType]        INT      NOT NULL,
    [OwnerID]          INT      NOT NULL,
    [ZoneID]           BIGINT   NOT NULL,
    [RoutingProductID] INT      NOT NULL,
    [BED]              DATETIME NULL,
    [EED]              DATETIME NOT NULL,
    CONSTRAINT [PK_SaleZoneRoutingProduct] PRIMARY KEY CLUSTERED ([ID] ASC)
);

