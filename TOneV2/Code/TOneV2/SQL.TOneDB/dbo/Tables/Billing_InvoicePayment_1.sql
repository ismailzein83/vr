CREATE TABLE [dbo].[Billing_InvoicePayment] (
    [InvoicePaymentID] INT             IDENTITY (1, 1) NOT NULL,
    [InvoiceID]        INT             NULL,
    [DueDate]          SMALLDATETIME   NULL,
    [Amount]           DECIMAL (13, 6) NULL,
    [PaidDate]         SMALLDATETIME   NULL,
    [timestamp]        ROWVERSION      NOT NULL
);

