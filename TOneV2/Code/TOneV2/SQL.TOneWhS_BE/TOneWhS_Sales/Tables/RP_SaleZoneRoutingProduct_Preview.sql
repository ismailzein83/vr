CREATE TABLE [TOneWhS_Sales].[RP_SaleZoneRoutingProduct_Preview] (
    [ZoneName]                                 NVARCHAR (255) NOT NULL,
    [ProcessInstanceID]                        BIGINT         NOT NULL,
    [CurrentSaleZoneRoutingProductName]        NVARCHAR (255) NULL,
    [IsCurrentSaleZoneRoutingProductInherited] BIT            NULL,
    [NewSaleZoneRoutingProductName]            NVARCHAR (255) NULL,
    [EffectiveOn]                              DATETIME       NOT NULL
);

