CREATE TYPE [TOneWhS_BE].[NewZoneRoutingProduct] AS TABLE (
    [ZoneID]           BIGINT   NOT NULL,
    [RoutingProductID] INT      NOT NULL,
    [BED]              DATETIME NOT NULL,
    [EED]              DATETIME NULL,
    PRIMARY KEY CLUSTERED ([ZoneID] ASC));

