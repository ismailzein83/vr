﻿CREATE TABLE [dbo].[CurrencyExchangeRate] (
    [ID]           BIGINT          IDENTITY (1, 1) NOT NULL,
    [CurrencyID]   INT             NOT NULL,
    [Rate]         DECIMAL (18, 5) NOT NULL,
    [ExchangeDate] DATETIME        NOT NULL,
    [timestamp]    ROWVERSION      NULL,
    CONSTRAINT [PK_CurrencyExchangeRate] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_CurrencyExchangeRate_Currency] FOREIGN KEY ([CurrencyID]) REFERENCES [dbo].[Currency] ([ID])
);

