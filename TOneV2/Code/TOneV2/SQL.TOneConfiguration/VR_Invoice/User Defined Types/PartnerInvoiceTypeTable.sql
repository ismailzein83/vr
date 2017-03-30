CREATE TYPE [VR_Invoice].[PartnerInvoiceTypeTable] AS TABLE (
    [InvoiceTypeId] UNIQUEIDENTIFIER NOT NULL,
    [PartnerId]     NVARCHAR (255)   NOT NULL);

