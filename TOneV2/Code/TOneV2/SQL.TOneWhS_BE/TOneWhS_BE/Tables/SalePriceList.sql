CREATE TABLE [TOneWhS_BE].[SalePriceList] (
    [ID]         INT IDENTITY (1, 1) NOT NULL,
    [OwnerType]  INT NOT NULL,
    [OwnerID]    INT NOT NULL,
    [CurrencyID] INT NULL,
    CONSTRAINT [PK_SalePriceList] PRIMARY KEY CLUSTERED ([ID] ASC)
);

