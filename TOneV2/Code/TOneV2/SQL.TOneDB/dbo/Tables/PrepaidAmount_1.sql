CREATE TABLE [dbo].[PrepaidAmount] (
    [ID]                INT             IDENTITY (1, 1) NOT NULL,
    [SupplierID]        VARCHAR (10)    NULL,
    [CustomerID]        VARCHAR (10)    NULL,
    [Amount]            NUMERIC (13, 5) NULL,
    [CurrencyID]        VARCHAR (3)     NULL,
    [Date]              DATETIME        NULL,
    [Type]              SMALLINT        NULL,
    [UserID]            INT             NULL,
    [Tag]               VARCHAR (255)   NULL,
    [timestamp]         ROWVERSION      NOT NULL,
    [LastUpdate]        DATETIME        NULL,
    [CustomerProfileID] SMALLINT        NULL,
    [SupplierProfileID] SMALLINT        NULL,
    [ReferenceNumber]   VARCHAR (250)   NULL,
    [Note]              VARCHAR (250)   NULL,
    CONSTRAINT [PK_PrepaidAmount] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_PrepaidAmount_Type]
    ON [dbo].[PrepaidAmount]([Type] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PrepaidAmount_Date]
    ON [dbo].[PrepaidAmount]([Date] ASC);

