CREATE TABLE [TOneWhS_BE].[SupplierZoneService] (
    [ID]                   BIGINT   IDENTITY (1, 1) NOT NULL,
    [ZoneID]               BIGINT   NOT NULL,
    [ReceivedServicesFlag] SMALLINT NULL,
    [EffectiveServiceFlag] SMALLINT NULL,
    [BED]                  DATETIME NOT NULL,
    [EED]                  DATETIME NULL,
    CONSTRAINT [PK_SupplierZoneService] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SupplierZoneService_SupplierZone] FOREIGN KEY ([ZoneID]) REFERENCES [TOneWhS_BE].[SupplierZone] ([ID])
);

