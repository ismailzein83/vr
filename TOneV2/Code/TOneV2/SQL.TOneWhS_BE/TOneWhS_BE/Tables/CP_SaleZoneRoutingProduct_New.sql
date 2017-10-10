CREATE TABLE [TOneWhS_BE].[CP_SaleZoneRoutingProduct_New] (
    [ID]                BIGINT   NOT NULL,
    [ProcessInstanceID] BIGINT   NOT NULL,
    [ZoneID]            BIGINT   NOT NULL,
    [OwnerType]         INT      NOT NULL,
    [OwnerID]           INT      NOT NULL,
    [RoutingProductID]  INT      NOT NULL,
    [BED]               DATETIME NOT NULL,
    [EED]               DATETIME NULL
);

