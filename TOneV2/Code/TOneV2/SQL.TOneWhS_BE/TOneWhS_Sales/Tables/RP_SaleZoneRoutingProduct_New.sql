CREATE TABLE [TOneWhS_Sales].[RP_SaleZoneRoutingProduct_New] (
    [ID]                BIGINT   NULL,
    [ProcessInstanceID] BIGINT   NOT NULL,
    [RoutingProductID]  INT      NOT NULL,
    [ZoneID]            BIGINT   NOT NULL,
    [BED]               DATETIME NOT NULL,
    [EED]               DATETIME NULL
);

