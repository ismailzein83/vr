CREATE TYPE [TOneWhS_BE].[ZoneRoutingProductChange] AS TABLE (
    [ZoneID]           BIGINT   NOT NULL,
    [RoutingProductID] INT      NOT NULL,
    [EED]              DATETIME NULL,
    PRIMARY KEY CLUSTERED ([ZoneID] ASC));

