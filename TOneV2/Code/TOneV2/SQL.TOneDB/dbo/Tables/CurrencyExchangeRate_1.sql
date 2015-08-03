CREATE TABLE [dbo].[CurrencyExchangeRate] (
    [CurrencyExchangeRateID] BIGINT        IDENTITY (1, 1) NOT NULL,
    [CurrencyID]             VARCHAR (3)   NULL,
    [Rate]                   FLOAT (53)    NULL,
    [ExchangeDate]           SMALLDATETIME NULL,
    [UserID]                 INT           NULL,
    [timestamp]              ROWVERSION    NULL,
    CONSTRAINT [PK_CurrencyExchangeRate] PRIMARY KEY CLUSTERED ([CurrencyExchangeRateID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_CurrencyExchangeRate_Date]
    ON [dbo].[CurrencyExchangeRate]([ExchangeDate] DESC);


GO
CREATE NONCLUSTERED INDEX [IX_CurrencyExchangeRate_Currency]
    ON [dbo].[CurrencyExchangeRate]([CurrencyID] ASC);

