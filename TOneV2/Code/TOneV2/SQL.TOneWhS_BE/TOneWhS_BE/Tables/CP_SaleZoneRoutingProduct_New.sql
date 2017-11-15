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




GO
CREATE CLUSTERED INDEX [IX_CP_SaleZoneRoutingProduct_New_ProcessInstanceID]
    ON [TOneWhS_BE].[CP_SaleZoneRoutingProduct_New]([ProcessInstanceID] ASC);

