CREATE TABLE [dbo].[SettlementInvoice] (
    [Id]        INT      IDENTITY (1, 1) NOT NULL,
    [IssueDate] DATETIME NULL,
    [DueDate]   DATETIME NULL,
    [FromDate]  DATETIME NULL,
    [ToDate]    DATETIME NULL,
    CONSTRAINT [PK_SettlementInvoiceHeader] PRIMARY KEY CLUSTERED ([Id] ASC)
);

