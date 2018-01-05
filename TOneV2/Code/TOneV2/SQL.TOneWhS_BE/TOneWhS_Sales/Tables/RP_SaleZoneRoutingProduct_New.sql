CREATE TABLE [TOneWhS_Sales].[RP_SaleZoneRoutingProduct_New] (
    [ID]                BIGINT   NULL,
    [OwnerType]         TINYINT  NULL,
    [OwnerID]           INT      NULL,
    [ProcessInstanceID] BIGINT   NOT NULL,
    [RoutingProductID]  INT      NOT NULL,
    [ZoneID]            BIGINT   NOT NULL,
    [BED]               DATETIME NOT NULL,
    [EED]               DATETIME NULL
);






GO
CREATE CLUSTERED INDEX [IX_RP_SaleZoneRoutingProduct_New_ProcessInstanceID]
    ON [TOneWhS_Sales].[RP_SaleZoneRoutingProduct_New]([ProcessInstanceID] ASC);

