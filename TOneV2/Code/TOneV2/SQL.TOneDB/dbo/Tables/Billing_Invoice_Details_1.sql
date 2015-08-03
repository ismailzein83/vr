CREATE TABLE [dbo].[Billing_Invoice_Details] (
    [ID]            BIGINT          IDENTITY (1, 1) NOT NULL,
    [InvoiceID]     INT             NOT NULL,
    [Destination]   VARCHAR (100)   NULL,
    [FromDate]      DATETIME        NULL,
    [TillDate]      DATETIME        NULL,
    [Duration]      NUMERIC (19, 6) NULL,
    [Rate]          DECIMAL (13, 6) NULL,
    [RateType]      CHAR (1)        NULL,
    [Amount]        DECIMAL (13, 6) NULL,
    [CurrencyID]    VARCHAR (3)     NULL,
    [UserID]        INT             NULL,
    [timestamp]     ROWVERSION      NULL,
    [NumberOfCalls] INT             NULL,
    [MonthNumber]   INT             NULL,
    [YearNumber]    INT             NULL,
    CONSTRAINT [PK_Billing_Invoice_Details] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Billing_Invoice_Details_Billing_Invoice] FOREIGN KEY ([InvoiceID]) REFERENCES [dbo].[Billing_Invoice] ([InvoiceID])
);


GO
CREATE NONCLUSTERED INDEX [IX_Billing_Invoice_Details_Invoice]
    ON [dbo].[Billing_Invoice_Details]([InvoiceID] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'''N'' for Normal (Peak), ''O'' for Offpeak and ''W'' for Weekend Rate ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Billing_Invoice_Details', @level2type = N'COLUMN', @level2name = N'RateType';

