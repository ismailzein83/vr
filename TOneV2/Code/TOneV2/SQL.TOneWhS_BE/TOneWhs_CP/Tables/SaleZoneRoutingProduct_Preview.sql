CREATE TABLE [TOneWhs_CP].[SaleZoneRoutingProduct_Preview] (
    [ProcessInstanceID] BIGINT         NOT NULL,
    [ZoneName]          NVARCHAR (255) NOT NULL,
    [OwnerType]         TINYINT        NOT NULL,
    [OwnerID]           INT            NOT NULL,
    [RoutingProductID]  INT            NOT NULL,
    [BED]               DATETIME       NOT NULL,
    [EED]               DATETIME       NULL,
    [ChangeType]        INT            NOT NULL
);






GO
CREATE CLUSTERED INDEX [IX_SaleZoneRoutingProduct_Preview_ProcessInstanceID]
    ON [TOneWhs_CP].[SaleZoneRoutingProduct_Preview]([ProcessInstanceID] ASC);

