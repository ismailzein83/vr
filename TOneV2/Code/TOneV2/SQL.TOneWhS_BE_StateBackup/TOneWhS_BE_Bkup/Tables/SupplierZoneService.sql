CREATE TABLE [TOneWhS_BE_Bkup].[SupplierZoneService] (
    [ID]                   BIGINT        NOT NULL,
    [ZoneID]               BIGINT        NOT NULL,
    [PriceListID]          INT           NULL,
    [ReceivedServicesFlag] VARCHAR (MAX) NOT NULL,
    [EffectiveServiceFlag] VARCHAR (MAX) NULL,
    [BED]                  DATETIME      NOT NULL,
    [EED]                  DATETIME      NULL,
    [SourceID]             VARCHAR (50)  NULL,
    [StateBackupID]        INT           NOT NULL,
    CONSTRAINT [PK_SupplierZoneService] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SupplierZoneService_SupplierZone] FOREIGN KEY ([ZoneID]) REFERENCES [TOneWhS_BE_Bkup].[SupplierZone] ([ID])
);

