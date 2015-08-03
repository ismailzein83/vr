CREATE TABLE [dbo].[Billing_Invoice] (
    [InvoiceID]          INT             IDENTITY (1, 1) NOT NULL,
    [BeginDate]          SMALLDATETIME   NOT NULL,
    [EndDate]            SMALLDATETIME   NOT NULL,
    [IssueDate]          SMALLDATETIME   NOT NULL,
    [DueDate]            SMALLDATETIME   NOT NULL,
    [SupplierID]         VARCHAR (10)    NOT NULL,
    [CustomerID]         VARCHAR (10)    NOT NULL,
    [SerialNumber]       VARCHAR (50)    NOT NULL,
    [Duration]           NUMERIC (19, 6) NULL,
    [Amount]             DECIMAL (13, 6) NOT NULL,
    [CurrencyID]         VARCHAR (3)     NULL,
    [IsLocked]           CHAR (1)        NOT NULL,
    [IsPaid]             CHAR (1)        NOT NULL,
    [PaidDate]           SMALLDATETIME   NULL,
    [UserID]             INT             NULL,
    [timestamp]          ROWVERSION      NULL,
    [InvoiceAttachement] IMAGE           NULL,
    [FileName]           VARCHAR (255)   NULL,
    [InvoicePrintedNote] VARCHAR (MAX)   NULL,
    [InvoiceNotes]       VARCHAR (MAX)   NULL,
    [PaidAmount]         DECIMAL (13, 6) NULL,
    [NumberOfCalls]      INT             NULL,
    [VatValue]           DECIMAL (13, 6) NULL,
    [SourceFileName]     NVARCHAR (100)  NULL,
    [IsSent]             CHAR (1)        NULL,
    [CustomerMask]       VARCHAR (10)    CONSTRAINT [DF_Billing_Invoice_CustomerMask] DEFAULT (NULL) NULL,
    [IsAutomatic]        CHAR (1)        NULL,
    [CreationDate]       DATETIME        CONSTRAINT [DF__Billing_I__Creat__5FC2E8D8] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_Billing_Invoice] PRIMARY KEY CLUSTERED ([InvoiceID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_Billing_Invoice_Customer]
    ON [dbo].[Billing_Invoice]([CustomerID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Billing_Invoice_Supplier]
    ON [dbo].[Billing_Invoice]([SupplierID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Billing_Invoice_IssueDate]
    ON [dbo].[Billing_Invoice]([IssueDate] DESC);


GO
CREATE NONCLUSTERED INDEX [IX_Billing_Invoice_BeginDate]
    ON [dbo].[Billing_Invoice]([BeginDate] DESC);

