CREATE TABLE [dbo].[PostpaidAmount] (
    [ID]                INT             IDENTITY (1, 1) NOT NULL,
    [CustomerProfileID] SMALLINT        NULL,
    [CustomerID]        VARCHAR (10)    NULL,
    [SupplierID]        VARCHAR (10)    NULL,
    [SupplierProfileID] SMALLINT        NULL,
    [Amount]            NUMERIC (13, 5) NULL,
    [CurrencyID]        VARCHAR (3)     NULL,
    [Date]              DATETIME        NULL,
    [Type]              SMALLINT        NULL,
    [UserID]            INT             NULL,
    [Tag]               VARCHAR (255)   NULL,
    [timestamp]         ROWVERSION      NOT NULL,
    [LastUpdate]        DATETIME        NULL,
    [ReferenceNumber]   VARCHAR (50)    NULL,
    [Note]              VARCHAR (250)   NULL,
    CONSTRAINT [PK_PostpaidAmount] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_PostpaidAmount_Type]
    ON [dbo].[PostpaidAmount]([Type] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PostpaidAmount_Date]
    ON [dbo].[PostpaidAmount]([Date] DESC);

