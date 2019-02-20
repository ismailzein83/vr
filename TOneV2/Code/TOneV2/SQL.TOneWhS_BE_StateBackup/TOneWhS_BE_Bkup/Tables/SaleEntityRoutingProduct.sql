﻿CREATE TABLE [TOneWhS_BE_Bkup].[SaleEntityRoutingProduct] (
    [ID]               BIGINT   NOT NULL,
    [OwnerType]        INT      NOT NULL,
    [OwnerID]          INT      NOT NULL,
    [ZoneID]           BIGINT   NULL,
    [RoutingProductID] INT      NOT NULL,
    [BED]              DATETIME NOT NULL,
    [EED]              DATETIME NULL,
    [StateBackupID]    BIGINT   NOT NULL,
    [LastModifiedTime] DATETIME NULL
);












GO
CREATE CLUSTERED INDEX [IX_SaleEntityRoutingProduct_StateBackupID]
    ON [TOneWhS_BE_Bkup].[SaleEntityRoutingProduct]([StateBackupID] ASC);

