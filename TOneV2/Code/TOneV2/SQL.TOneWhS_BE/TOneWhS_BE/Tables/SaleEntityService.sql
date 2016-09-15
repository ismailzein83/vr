CREATE TABLE [TOneWhS_BE].[SaleEntityService] (
    [ID]          BIGINT         NOT NULL,
    [PriceListID] INT            NOT NULL,
    [ZoneID]      BIGINT         NULL,
    [Services]    NVARCHAR (MAX) NOT NULL,
    [BED]         DATETIME       NOT NULL,
    [EED]         DATETIME       NULL,
    [timestamp]   ROWVERSION     NULL,
    CONSTRAINT [PK_SaleZoneService] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SaleZoneService_SaleZone] FOREIGN KEY ([ZoneID]) REFERENCES [TOneWhS_BE].[SaleZone] ([ID])
);




GO
CREATE NONCLUSTERED INDEX [IX_SaleZoneService_timestamp]
    ON [TOneWhS_BE].[SaleEntityService]([timestamp] DESC);

