CREATE TABLE [TOneWhS_BE_Bkup].[SaleEntityRoutingProduct] (
    [ID]               BIGINT   NOT NULL,
    [OwnerType]        TINYINT  NOT NULL,
    [OwnerID]          INT      NOT NULL,
    [ZoneID]           BIGINT   NULL,
    [RoutingProductID] INT      NOT NULL,
    [BED]              DATETIME NOT NULL,
    [EED]              DATETIME NULL,
    [StateBackupID]    INT      NOT NULL,
    CONSTRAINT [PK_SaleZoneRoutingProduct] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SaleEntityRoutingProduct_SaleZone] FOREIGN KEY ([ZoneID]) REFERENCES [TOneWhS_BE_Bkup].[SaleZone] ([ID])
);

