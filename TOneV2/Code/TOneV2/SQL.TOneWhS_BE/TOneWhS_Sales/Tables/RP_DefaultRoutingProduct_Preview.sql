CREATE TABLE [TOneWhS_Sales].[RP_DefaultRoutingProduct_Preview] (
    [ProcessInstanceID]                       BIGINT         NOT NULL,
    [CurrentDefaultRoutingProductName]        NVARCHAR (255) NULL,
    [IsCurrentDefaultRoutingProductInherited] BIT            NULL,
    [NewDefaultRoutingProductName]            NVARCHAR (255) NULL,
    [EffectiveOn]                             DATETIME       NOT NULL
);

