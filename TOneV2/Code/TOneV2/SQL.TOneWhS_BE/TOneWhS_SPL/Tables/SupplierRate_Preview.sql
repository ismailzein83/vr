CREATE TABLE [TOneWhS_SPL].[SupplierRate_Preview] (
    [PriceListId] INT            NOT NULL,
    [ZoneName]    NVARCHAR (255) NOT NULL,
    [ChangeType]  INT            NOT NULL,
    [RecentRate]  DECIMAL (9, 5) NULL,
    [NewRate]     DECIMAL (9, 5) NOT NULL,
    [BED]         DATETIME       NOT NULL,
    [EED]         DATETIME       NULL
);

