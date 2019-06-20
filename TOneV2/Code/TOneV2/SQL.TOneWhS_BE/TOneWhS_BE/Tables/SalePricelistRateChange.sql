﻿CREATE TABLE [TOneWhS_BE].[SalePricelistRateChange] (
    [PricelistId]      INT             NOT NULL,
    [Rate]             DECIMAL (20, 8) NOT NULL,
    [RateTypeId]       INT             NULL,
    [RecentRate]       DECIMAL (20, 8) NULL,
    [CountryID]        INT             NOT NULL,
    [ZoneName]         NVARCHAR (150)  NOT NULL,
    [ZoneID]           BIGINT          NULL,
    [Change]           INT             NOT NULL,
    [BED]              DATETIME        NOT NULL,
    [EED]              DATETIME        NULL,
    [RoutingProductID] INT             NULL,
    [CurrencyID]       INT             NULL,
    [Note]             NVARCHAR (MAX)  NULL
);
















GO
CREATE CLUSTERED INDEX [IX_SalePricelistRateChange_PriceListID]
    ON [TOneWhS_BE].[SalePricelistRateChange]([PricelistId] ASC);

