CREATE TABLE [TOneWhS_BE].[SalePricelistCodeChange_New] (
    [Code]           NVARCHAR (150) NOT NULL,
    [RecentZoneName] NVARCHAR (150) NULL,
    [ZoneName]       NVARCHAR (150) NULL,
    [ZoneID]         BIGINT         NULL,
    [Change]         TINYINT        NULL,
    [BatchID]        INT            NOT NULL,
    [BED]            DATETIME       NOT NULL,
    [EED]            DATETIME       NULL,
    [CountryID]      INT            NULL
);






GO
CREATE CLUSTERED INDEX [IX_SalePricelistCodeChange_New_BatchID]
    ON [TOneWhS_BE].[SalePricelistCodeChange_New]([BatchID] ASC);

