CREATE TABLE [TOneWhS_BE].[SalePricelistCodeChange] (
    [Code]           NVARCHAR (150) NOT NULL,
    [RecentZoneName] NVARCHAR (150) NULL,
    [ZoneName]       NVARCHAR (150) NULL,
    [ZoneID]         BIGINT         NULL,
    [Change]         TINYINT        NULL,
    [BatchID]        INT            NULL,
    [BED]            DATETIME       NOT NULL,
    [EED]            DATETIME       NULL,
    [CountryID]      INT            NULL
);





