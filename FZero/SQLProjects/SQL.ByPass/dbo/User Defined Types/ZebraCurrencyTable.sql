CREATE TYPE [dbo].[ZebraCurrencyTable] AS TABLE (
    [CurrencyID]     VARCHAR (3)    NOT NULL,
    [Name]           NVARCHAR (100) NULL,
    [IsMainCurrency] CHAR (1)       NULL,
    [LastRate]       FLOAT (53)     NOT NULL,
    [LastUpdated]    SMALLDATETIME  NOT NULL,
    PRIMARY KEY CLUSTERED ([CurrencyID] ASC));

