CREATE TABLE [TOneWhS_Sales].[RP_SaleZoneRoutingProduct_Preview] (
    [ZoneName]                                 NVARCHAR (255) NOT NULL,
    [ProcessInstanceID]                        BIGINT         NOT NULL,
    [CurrentSaleZoneRoutingProductName]        NVARCHAR (255) NULL,
    [CurrentSaleZoneRoutingProductId]          INT            NULL,
    [IsCurrentSaleZoneRoutingProductInherited] BIT            NULL,
    [NewSaleZoneRoutingProductName]            NVARCHAR (255) NULL,
    [NewSaleZoneRoutingProductId]              INT            NOT NULL,
    [EffectiveOn]                              DATETIME       NOT NULL,
    [ZoneId]                                   BIGINT         NULL
);






GO
CREATE NONCLUSTERED INDEX [IX_RP_SaleZoneRoutingProduct_Preview_ZoneName]
    ON [TOneWhS_Sales].[RP_SaleZoneRoutingProduct_Preview]([ZoneName] ASC);


GO
CREATE CLUSTERED INDEX [IX_RP_SaleZoneRoutingProduct_Preview_ProcessInstanceID]
    ON [TOneWhS_Sales].[RP_SaleZoneRoutingProduct_Preview]([ProcessInstanceID] ASC);

