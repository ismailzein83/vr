CREATE TABLE [TOneWhS_BE].[CurrencyExchangeRate] (
    [ID]           BIGINT     IDENTITY (1, 1) NOT NULL,
    [CurrencyID]   INT        NOT NULL,
    [Rate]         FLOAT (53) NOT NULL,
    [ExchangeDate] DATETIME   NOT NULL,
    CONSTRAINT [PK_CurrencyExchangeRate] PRIMARY KEY CLUSTERED ([ID] ASC)
);

