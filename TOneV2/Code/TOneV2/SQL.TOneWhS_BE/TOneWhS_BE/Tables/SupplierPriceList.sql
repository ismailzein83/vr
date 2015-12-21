CREATE TABLE [TOneWhS_BE].[SupplierPriceList] (
    [ID]         INT        NOT NULL,
    [SupplierID] INT        NOT NULL,
    [CurrencyID] INT        NOT NULL,
    [timestamp]  ROWVERSION NULL,
    CONSTRAINT [PK_SupplierPriceList] PRIMARY KEY CLUSTERED ([ID] ASC)
);





