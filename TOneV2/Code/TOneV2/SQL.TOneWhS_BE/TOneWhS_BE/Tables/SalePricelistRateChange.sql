CREATE TABLE [TOneWhS_BE].[SalePricelistRateChange] (
    [PricelistId]      INT             NOT NULL,
    [Rate]             DECIMAL (20, 8) NOT NULL,
    [RecentRate]       DECIMAL (20, 8) NULL,
    [CountryID]        INT             NOT NULL,
    [ZoneName]         NVARCHAR (150)  NOT NULL,
    [ZoneID]           BIGINT          NULL,
    [Change]           TINYINT         NOT NULL,
    [BED]              DATETIME        NOT NULL,
    [EED]              DATETIME        NULL,
    [RoutingProductID] INT             NULL,
    [CurrencyID]       INT             NULL
);







