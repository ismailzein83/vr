CREATE TABLE [TOneWhS_BE].[SaleZoneService] (
    [ID]           BIGINT     IDENTITY (1, 1) NOT NULL,
    [OwnerType]    INT        NOT NULL,
    [OwnerID]      INT        NOT NULL,
    [ZoneID]       BIGINT     NOT NULL,
    [ServicesFlag] SMALLINT   NOT NULL,
    [BED]          DATETIME   NOT NULL,
    [EED]          DATETIME   NULL,
    [timestamp]    ROWVERSION NULL,
    CONSTRAINT [PK_SaleZoneService] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SaleZoneService_SaleZone] FOREIGN KEY ([ZoneID]) REFERENCES [TOneWhS_BE].[SaleZone] ([ID])
);






GO
CREATE NONCLUSTERED INDEX [IX_SaleZoneService_timestamp]
    ON [TOneWhS_BE].[SaleZoneService]([timestamp] DESC);

