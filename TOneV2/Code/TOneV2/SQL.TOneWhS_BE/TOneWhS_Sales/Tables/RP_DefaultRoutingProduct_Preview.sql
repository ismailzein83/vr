CREATE TABLE [TOneWhS_Sales].[RP_DefaultRoutingProduct_Preview] (
    [ProcessInstanceID]                       BIGINT         NOT NULL,
    [CurrentDefaultRoutingProductName]        NVARCHAR (255) NULL,
    [IsCurrentDefaultRoutingProductInherited] BIT            NULL,
    [NewDefaultRoutingProductName]            NVARCHAR (255) NULL,
    [EffectiveOn]                             DATETIME       NOT NULL
);




GO
CREATE CLUSTERED INDEX [IX_RP_DefaultRoutingProduct_Preview_ProcessInstanceID]
    ON [TOneWhS_Sales].[RP_DefaultRoutingProduct_Preview]([ProcessInstanceID] ASC);

