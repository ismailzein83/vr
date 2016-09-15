CREATE TABLE [TOneWhS_BE].[SupplierZoneService] (
    [ID]                   BIGINT        NOT NULL,
    [ZoneID]               BIGINT        NOT NULL,
    [ReceivedServicesFlag] VARCHAR (MAX) NOT NULL,
    [EffectiveServiceFlag] VARCHAR (MAX) NULL,
    [BED]                  DATETIME      NOT NULL,
    [EED]                  DATETIME      NULL,
    [timestamp]            ROWVERSION    NULL,
    CONSTRAINT [PK_SupplierZoneService] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SupplierZoneService_SupplierZone] FOREIGN KEY ([ZoneID]) REFERENCES [TOneWhS_BE].[SupplierZone] ([ID])
);






GO
CREATE NONCLUSTERED INDEX [IX_SupplierZoneService_timestamp]
    ON [TOneWhS_BE].[SupplierZoneService]([timestamp] DESC);

