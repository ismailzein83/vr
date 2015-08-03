CREATE TABLE [dbo].[SettlementInvoiceCurrency] (
    [Id]                  INT             IDENTITY (1, 1) NOT NULL,
    [SettlementInvoiceId] INT             NULL,
    [BeginDate]           DATETIME        NULL,
    [EndDate]             DATETIME        NULL,
    [IssueDate]           DATETIME        NULL,
    [DueDate]             DATETIME        NULL,
    [SupplierId]          VARCHAR (10)    NULL,
    [CustomerId]          VARCHAR (10)    NULL,
    [Amount]              DECIMAL (13, 6) NULL,
    [CurrencyId]          VARCHAR (3)     NULL,
    [IsLocked]            CHAR (1)        NULL,
    [SourceFileName]      VARCHAR (255)   NULL,
    CONSTRAINT [PK_SettlementInvoiceCurrency] PRIMARY KEY CLUSTERED ([Id] ASC)
);

