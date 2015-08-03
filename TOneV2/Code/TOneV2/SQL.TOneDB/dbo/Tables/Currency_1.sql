CREATE TABLE [dbo].[Currency] (
    [CurrencyID]     VARCHAR (3)    NOT NULL,
    [Name]           NVARCHAR (100) NOT NULL,
    [IsMainCurrency] CHAR (1)       CONSTRAINT [DF_Currency_IsMainCurrency] DEFAULT ('N') NOT NULL,
    [IsVisible]      CHAR (1)       CONSTRAINT [DF_Currency_IsVisible] DEFAULT ('Y') NOT NULL,
    [LastRate]       FLOAT (53)     NULL,
    [LastUpdated]    SMALLDATETIME  NULL,
    [UserID]         INT            NULL,
    [timestamp]      ROWVERSION     NULL,
    [DS_ID_auto]     INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_Currency] PRIMARY KEY CLUSTERED ([CurrencyID] ASC)
);

