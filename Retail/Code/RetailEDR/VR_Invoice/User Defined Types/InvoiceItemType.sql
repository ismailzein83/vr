CREATE TYPE [VR_Invoice].[InvoiceItemType] AS TABLE (
    [InvoiceID]   BIGINT         NOT NULL,
    [ItemSetName] NVARCHAR (255) NOT NULL,
    [Name]        NVARCHAR (900) NULL,
    [Details]     NVARCHAR (MAX) NOT NULL);

