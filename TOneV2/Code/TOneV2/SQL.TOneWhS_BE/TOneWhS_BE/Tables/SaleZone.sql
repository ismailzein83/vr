CREATE TABLE [TOneWhS_BE].[SaleZone] (
    [ID]        BIGINT         IDENTITY (1, 1) NOT NULL,
    [PackageID] INT            NOT NULL,
    [CountryID] INT            NULL,
    [Name]      NVARCHAR (255) NOT NULL,
    [BED]       DATETIME       NOT NULL,
    [EED]       DATETIME       NULL,
    [timestamp] ROWVERSION     NULL,
    CONSTRAINT [PK_SaleZone] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SaleZone_Country] FOREIGN KEY ([CountryID]) REFERENCES [TOneWhS_BE].[Country] ([ID]),
    CONSTRAINT [FK_SaleZone_SaleZonePackage] FOREIGN KEY ([PackageID]) REFERENCES [TOneWhS_BE].[SaleZonePackage] ([ID])
);

