CREATE TABLE [TOneWhS_Sales].[RP_DefaultRoutingProduct_New] (
    [ID]                BIGINT   NULL,
    [OwnerType]         TINYINT  NULL,
    [OwnerID]           INT      NULL,
    [ProcessInstanceID] BIGINT   NOT NULL,
    [RoutingProductID]  INT      NOT NULL,
    [BED]               DATETIME NOT NULL,
    [EED]               DATETIME NULL
);






GO
CREATE CLUSTERED INDEX [IX_RP_DefaultRoutingProduct_New_ProcessInstanceID]
    ON [TOneWhS_Sales].[RP_DefaultRoutingProduct_New]([ProcessInstanceID] ASC);

