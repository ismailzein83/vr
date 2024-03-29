﻿CREATE TABLE [TOneWhS_BE].[SalePricelistCodeChange] (
    [Code]           NVARCHAR (150) NOT NULL,
    [RecentZoneName] NVARCHAR (150) NULL,
    [ZoneName]       NVARCHAR (150) NULL,
    [ZoneID]         BIGINT         NULL,
    [Change]         INT            NULL,
    [BatchID]        INT            NULL,
    [BED]            DATETIME       NOT NULL,
    [EED]            DATETIME       NULL,
    [CountryID]      INT            NULL
);










GO
CREATE CLUSTERED INDEX [IX_SalePricelistCodeChange_BatchID]
    ON [TOneWhS_BE].[SalePricelistCodeChange]([BatchID] ASC);

