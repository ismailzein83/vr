CREATE TABLE [TOneWhS_BE].[SaleZoneService] (
    [ID]           BIGINT   IDENTITY (1, 1) NOT NULL,
    [ZoneID]       BIGINT   NOT NULL,
    [ServicesFlag] SMALLINT NULL,
    [BED]          DATETIME NOT NULL,
    [EED]          DATETIME NULL,
    CONSTRAINT [PK_SaleZoneService] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SaleZoneService_SaleZone] FOREIGN KEY ([ZoneID]) REFERENCES [TOneWhS_BE].[SaleZone] ([ID])
);

