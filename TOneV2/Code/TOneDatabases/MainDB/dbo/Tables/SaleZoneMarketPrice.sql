CREATE TABLE [dbo].[SaleZoneMarketPrice] (
    [SaleZoneMarketPriceID] INT            IDENTITY (1, 1) NOT NULL,
    [SaleZoneID]            INT            NOT NULL,
    [ServicesFlag]          SMALLINT       NOT NULL,
    [FromRate]              DECIMAL (9, 5) NOT NULL,
    [ToRate]                DECIMAL (9, 5) NOT NULL,
    CONSTRAINT [PK_SaleZoneMarketPrice] PRIMARY KEY CLUSTERED ([SaleZoneMarketPriceID] ASC)
);

