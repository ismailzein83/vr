CREATE TABLE [TOneWhS_BE].[SalePricelistCodeChange] (
    [ID]             INT            NOT NULL,
    [PricelistID]    INT            NOT NULL,
    [Code]           NVARCHAR (150) NOT NULL,
    [CountryID]      INT            NOT NULL,
    [RecentZoneName] NVARCHAR (150) NOT NULL,
    [ZoneName]       NVARCHAR (150) NOT NULL,
    [Change]         TINYINT        NULL,
    CONSTRAINT [PK_SalePricelistCodeChange] PRIMARY KEY CLUSTERED ([ID] ASC)
);

