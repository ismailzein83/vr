CREATE TABLE [TOneWhS_BE].[SaleRate] (
    [ID]          BIGINT          NOT NULL,
    [PriceListID] INT             NOT NULL,
    [ZoneID]      BIGINT          NOT NULL,
    [CurrencyID]  INT             NULL,
    [Rate]        DECIMAL (20, 8) NOT NULL,
    [OtherRates]  VARCHAR (MAX)   NULL,
    [BED]         DATETIME        NOT NULL,
    [EED]         DATETIME        NULL,
    [timestamp]   ROWVERSION      NULL,
    [SourceID]    VARCHAR (50)    NULL,
    [Change]      TINYINT         NULL,
    CONSTRAINT [PK_SaleRate] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SaleRate_SalePriceList] FOREIGN KEY ([PriceListID]) REFERENCES [TOneWhS_BE].[SalePriceList] ([ID]),
    CONSTRAINT [FK_SaleRate_SaleZone] FOREIGN KEY ([ZoneID]) REFERENCES [TOneWhS_BE].[SaleZone] ([ID])
);
















GO
CREATE NONCLUSTERED INDEX [IX_SaleRate_timestamp]
    ON [TOneWhS_BE].[SaleRate]([timestamp] DESC);

