CREATE TABLE [TOneWhS_BE_Bkup].[SaleEntityService] (
    [ID]            BIGINT         NOT NULL,
    [PriceListID]   INT            NOT NULL,
    [ZoneID]        BIGINT         NULL,
    [Services]      NVARCHAR (MAX) NOT NULL,
    [BED]           DATETIME       NOT NULL,
    [EED]           DATETIME       NULL,
    [SourceID]      VARCHAR (50)   NULL,
    [StateBackupID] BIGINT         NOT NULL,
    CONSTRAINT [PK_SaleZoneService] PRIMARY KEY CLUSTERED ([ID] ASC)
);



