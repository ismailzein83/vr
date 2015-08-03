CREATE TABLE [dbo].[SaleZoneMarketPrice] (
    [SaleZoneMarketPriceID] INT            IDENTITY (1, 1) NOT NULL,
    [SaleZoneID]            INT            NOT NULL,
    [ServicesFlag]          SMALLINT       NOT NULL,
    [FromRate]              DECIMAL (9, 5) NOT NULL,
    [ToRate]                DECIMAL (9, 5) NOT NULL,
    [timestamp]             ROWVERSION     NOT NULL,
    CONSTRAINT [PK_SaleZoneMarketPrice] PRIMARY KEY CLUSTERED ([SaleZoneMarketPriceID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_SaleZoneMarketPrice_ServiceFlag]
    ON [dbo].[SaleZoneMarketPrice]([ServicesFlag] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_SaleZoneMarketPrice_SaleZoneID]
    ON [dbo].[SaleZoneMarketPrice]([SaleZoneID] ASC);

