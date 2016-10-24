CREATE TABLE [TOneWhS_BE_Bkup].[SaleEntityRoutingProduct] (
    [ID]               BIGINT   NOT NULL,
    [OwnerType]        TINYINT  NOT NULL,
    [OwnerID]          INT      NOT NULL,
    [ZoneID]           BIGINT   NULL,
    [RoutingProductID] INT      NOT NULL,
    [BED]              DATETIME NOT NULL,
    [EED]              DATETIME NULL,
    [StateBackupID]    BIGINT   NOT NULL,
    CONSTRAINT [PK_SaleZoneRoutingProduct] PRIMARY KEY CLUSTERED ([ID] ASC)
);



