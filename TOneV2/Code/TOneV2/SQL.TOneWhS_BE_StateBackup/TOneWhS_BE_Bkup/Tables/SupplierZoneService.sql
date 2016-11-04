CREATE TABLE [TOneWhS_BE_Bkup].[SupplierZoneService] (
    [ID]                   BIGINT        NOT NULL,
    [ZoneID]               BIGINT        NULL,
    [PriceListID]          INT           NULL,
    [SupplierID]           INT           NOT NULL,
    [ReceivedServicesFlag] VARCHAR (MAX) NOT NULL,
    [EffectiveServiceFlag] VARCHAR (MAX) NULL,
    [BED]                  DATETIME      NOT NULL,
    [EED]                  DATETIME      NULL,
    [SourceID]             VARCHAR (50)  NULL,
    [StateBackupID]        BIGINT        NOT NULL
);










GO
CREATE CLUSTERED INDEX [IX_SupplierZoneService_StateBackupID]
    ON [TOneWhS_BE_Bkup].[SupplierZoneService]([StateBackupID] ASC);

