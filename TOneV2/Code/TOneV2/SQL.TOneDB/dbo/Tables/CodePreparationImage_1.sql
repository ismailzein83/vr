CREATE TABLE [dbo].[CodePreparationImage] (
    [IsLastCodePreparationImage] CHAR (1)       NOT NULL,
    [CustomerID]                 VARCHAR (5)    NOT NULL,
    [PricelistName]              NVARCHAR (200) NOT NULL,
    [RateSheet]                  IMAGE          NOT NULL,
    [CodeSheet]                  IMAGE          NULL,
    [CurrencyID]                 VARCHAR (3)    NULL
);

