CREATE TABLE [VR_Invoice].[InvoiceItem] (
    [ID]          BIGINT         IDENTITY (1, 1) NOT NULL,
    [InvoiceID]   BIGINT         NOT NULL,
    [ItemSetName] NVARCHAR (255) NULL,
    [Name]        NVARCHAR (900) NOT NULL,
    [Details]     NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_InvoiceDetails] PRIMARY KEY CLUSTERED ([ID] ASC)
);



