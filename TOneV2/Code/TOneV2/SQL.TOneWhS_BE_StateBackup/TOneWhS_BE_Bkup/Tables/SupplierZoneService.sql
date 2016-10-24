CREATE TABLE [TOneWhS_BE_Bkup].[SupplierZoneService] (
    [ID]                   BIGINT        NOT NULL,
    [ZoneID]               BIGINT        NOT NULL,
    [PriceListID]          INT           NULL,
    [ReceivedServicesFlag] VARCHAR (MAX) NOT NULL,
    [EffectiveServiceFlag] VARCHAR (MAX) NULL,
    [BED]                  DATETIME      NOT NULL,
    [EED]                  DATETIME      NULL,
    [SourceID]             VARCHAR (50)  NULL,
    [StateBackupID]        BIGINT        NOT NULL
);





