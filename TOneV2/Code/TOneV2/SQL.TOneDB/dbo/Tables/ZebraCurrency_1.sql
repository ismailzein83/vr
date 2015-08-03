CREATE TABLE [dbo].[ZebraCurrency] (
    [CurrencyID]     VARCHAR (3)    NOT NULL,
    [Name]           NVARCHAR (100) NOT NULL,
    [IsMainCurrency] CHAR (1)       CONSTRAINT [DF_ZebraCurrency_IsMainCurrency] DEFAULT ('N') NOT NULL,
    [LastRate]       FLOAT (53)     NULL,
    [LastUpdated]    SMALLDATETIME  NULL,
    [UserID]         INT            NULL,
    [timestamp]      ROWVERSION     NULL,
    CONSTRAINT [PK_ZebraCurrency] PRIMARY KEY CLUSTERED ([CurrencyID] ASC)
);

