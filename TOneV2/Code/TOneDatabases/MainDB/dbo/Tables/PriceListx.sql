CREATE TABLE [dbo].[PriceListx] (
    [PriceListID]        INT            NOT NULL,
    [SupplierID]         VARCHAR (5)    NOT NULL,
    [CustomerID]         VARCHAR (5)    NOT NULL,
    [Description]        NVARCHAR (MAX) NULL,
    [CurrencyID]         VARCHAR (3)    NOT NULL,
    [BeginEffectiveDate] SMALLDATETIME  NULL,
    [EndEffectiveDate]   SMALLDATETIME  NULL,
    [IsEffective]        VARCHAR (1)    NOT NULL,
    [SourceFileName]     NVARCHAR (200) NULL,
    [UserID]             INT            NULL,
    [timestamp]          ROWVERSION     NOT NULL,
    [IsSent]             CHAR (1)       NULL
);

