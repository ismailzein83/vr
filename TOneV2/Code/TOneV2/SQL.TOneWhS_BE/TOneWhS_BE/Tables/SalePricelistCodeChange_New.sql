CREATE TABLE [TOneWhS_BE].[SalePricelistCodeChange_New] (
    [Code]           NVARCHAR (150) NOT NULL,
    [RecentZoneName] NVARCHAR (150) NULL,
    [ZoneName]       NVARCHAR (150) NULL,
    [Change]         TINYINT        NULL,
    [BatchID]        INT            NOT NULL,
    [BED]            DATETIME       NOT NULL,
    [EED]            DATETIME       NULL,
    [CountryID]      INT            NULL
);

