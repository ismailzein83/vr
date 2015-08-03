CREATE TABLE [dbo].[SettlementInvoiceActualAmount] (
    [ID]                        INT             IDENTITY (1, 1) NOT NULL,
    [SettlementInvoiceDetailId] INT             NULL,
    [InvoiceId]                 INT             NULL,
    [BeginDate]                 DATETIME        NULL,
    [EndDate]                   DATETIME        NULL,
    [IssueDate]                 DATETIME        NULL,
    [DueDate]                   DATETIME        NULL,
    [CarrierAccountId]          VARCHAR (10)    NULL,
    [Duration]                  NUMERIC (19, 6) NULL,
    [Calls]                     INT             NULL,
    [Amount]                    DECIMAL (13, 6) NULL,
    [CurrencyId]                VARCHAR (3)     NULL,
    [IsLocked]                  CHAR (1)        NULL,
    [SourceFileName]            NVARCHAR (100)  NULL,
    [InvoiceAttachement]        VARBINARY (MAX) NULL,
    CONSTRAINT [PK_SettlementInvoiceActualAmount] PRIMARY KEY CLUSTERED ([ID] ASC)
);

