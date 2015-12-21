CREATE TABLE [TOneWhS_BE].[SalePriceList] (
    [ID]         INT        IDENTITY (1, 1) NOT NULL,
    [OwnerType]  INT        NOT NULL,
    [OwnerID]    INT        NOT NULL,
    [CurrencyID] INT        NOT NULL,
    [timestamp]  ROWVERSION NULL,
    CONSTRAINT [PK_SalePriceList] PRIMARY KEY CLUSTERED ([ID] ASC)
);



