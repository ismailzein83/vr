CREATE TABLE [common].[CurrencyExchangeRate] (
    [ID]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [CurrencyID]       INT              NOT NULL,
    [Rate]             DECIMAL (20, 10) NOT NULL,
    [ExchangeDate]     DATETIME         NOT NULL,
    [timestamp]        ROWVERSION       NULL,
    [SourceID]         VARCHAR (50)     NULL,
    [LastModifiedTime] DATETIME         NULL,
    CONSTRAINT [PK_CurrencyExchangeRate] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_CurrencyExchangeRate_Currency] FOREIGN KEY ([CurrencyID]) REFERENCES [common].[Currency] ([ID]),
    CONSTRAINT [IX_CurrencyExchangeRate_CurrencyIDExchangeDate] UNIQUE NONCLUSTERED ([CurrencyID] ASC, [ExchangeDate] ASC)
);











