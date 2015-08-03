CREATE TABLE [dbo].[CurrencyLog] (
    [UpdatedDateTime] DATETIME    NULL,
    [UserID]          NCHAR (40)  NULL,
    [Host]            NCHAR (200) NULL,
    [timestamp]       ROWVERSION  NOT NULL,
    [DS_ID_auto]      INT         IDENTITY (1, 1) NOT NULL
);

