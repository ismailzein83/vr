CREATE TYPE [TOneWhS_BE].[NewSaleEntityRoutingProduct] AS TABLE (
    [OwnerType]        INT      NOT NULL,
    [OwnerID]          INT      NOT NULL,
    [ZoneID]           BIGINT   NULL,
    [RoutingProductID] INT      NOT NULL,
    [BED]              DATETIME NOT NULL,
    [EED]              DATETIME NULL);

