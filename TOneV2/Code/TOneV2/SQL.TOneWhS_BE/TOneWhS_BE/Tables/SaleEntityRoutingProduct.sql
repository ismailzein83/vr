CREATE TABLE [TOneWhS_BE].[SaleEntityRoutingProduct] (
    [ID]               BIGINT     NOT NULL,
    [OwnerType]        TINYINT    NOT NULL,
    [OwnerID]          INT        NOT NULL,
    [ZoneID]           BIGINT     NULL,
    [RoutingProductID] INT        NOT NULL,
    [BED]              DATETIME   NOT NULL,
    [EED]              DATETIME   NULL,
    [timestamp]        ROWVERSION NULL,
    [LastModifiedTime] DATETIME   CONSTRAINT [DF_SaleEntityRoutingProduct_LastModifiedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_SaleZoneRoutingProduct] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SaleEntityRoutingProduct_RoutingProduct] FOREIGN KEY ([RoutingProductID]) REFERENCES [TOneWhS_BE].[RoutingProduct] ([ID]),
    CONSTRAINT [FK_SaleEntityRoutingProduct_SaleZone] FOREIGN KEY ([ZoneID]) REFERENCES [TOneWhS_BE].[SaleZone] ([ID])
);








GO
CREATE NONCLUSTERED INDEX [IX_SaleEntityRoutingProduct_timestamp]
    ON [TOneWhS_BE].[SaleEntityRoutingProduct]([timestamp] DESC);

