CREATE TABLE [TOneWhS_SPL].[SupplierCode_Preview] (
    [PriceListId]    INT            NOT NULL,
    [Code]           VARCHAR (20)   NOT NULL,
    [ChangeType]     INT            NOT NULL,
    [RecentZoneName] NVARCHAR (255) NULL,
    [ZoneName]       NVARCHAR (255) NOT NULL,
    [BED]            DATETIME       NOT NULL,
    [EED]            DATETIME       NULL
);

