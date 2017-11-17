CREATE TABLE [VR_Invoice].[InvoiceBulkActionDraft] (
    [ID]                          BIGINT           IDENTITY (1, 1) NOT NULL,
    [InvoiceBulkActionIdentifier] UNIQUEIDENTIFIER NULL,
    [InvoiceTypeId]               UNIQUEIDENTIFIER NULL,
    [InvoiceId]                   BIGINT           NULL,
    [CreatedTime]                 DATETIME         CONSTRAINT [DF_InvoiceBulkActionDraft_CreatedTime] DEFAULT (getdate()) NULL
);




GO
CREATE NONCLUSTERED INDEX [IX_InvoiceBulkActionDraft_ID]
    ON [VR_Invoice].[InvoiceBulkActionDraft]([ID] ASC);


GO
CREATE CLUSTERED INDEX [IX_InvoiceBulkActionDraft_BulkActionIdentifier]
    ON [VR_Invoice].[InvoiceBulkActionDraft]([InvoiceBulkActionIdentifier] ASC);

