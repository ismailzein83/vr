CREATE TABLE [TOneWhS_BE].[SalePricelistRateChange] (
    [ID]          INT             NOT NULL,
    [PricelistId] INT             NOT NULL,
    [Rate]        DECIMAL (20, 8) NOT NULL,
    [RecentRate]  DECIMAL (20, 8) NULL,
    [CountryID]   INT             NOT NULL,
    [ZoneName]    NVARCHAR (150)  NOT NULL,
    [Change]      TINYINT         NULL,
    CONSTRAINT [PK_SalePricelistRateChange] PRIMARY KEY CLUSTERED ([ID] ASC)
);

