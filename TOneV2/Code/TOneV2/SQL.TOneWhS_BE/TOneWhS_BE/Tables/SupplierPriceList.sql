CREATE TABLE [TOneWhS_BE].[SupplierPriceList] (
    [ID]         INT IDENTITY (1, 1) NOT NULL,
    [SupplierID] INT NOT NULL,
    [CurrencyID] INT NULL,
    CONSTRAINT [PK_SupplierPriceList] PRIMARY KEY CLUSTERED ([ID] ASC)
);



