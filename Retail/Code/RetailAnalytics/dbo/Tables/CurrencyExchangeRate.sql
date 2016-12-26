CREATE TABLE [dbo].[CurrencyExchangeRate] (
    [CurrencyID] INT             NOT NULL,
    [Rate]       DECIMAL (18, 6) NOT NULL,
    [BED]        DATETIME        NOT NULL,
    [EED]        DATETIME        NULL
);

