CREATE TABLE [VR_Invoice].[InvoiceBulkActionDraft] (
    [ID]                          BIGINT           IDENTITY (1, 1) NOT NULL,
    [InvoiceBulkActionIdentifier] UNIQUEIDENTIFIER NULL,
    [InvoiceTypeId]               UNIQUEIDENTIFIER NULL,
    [InvoiceId]                   BIGINT           NULL,
    [CreatedTime]                 DATETIME         CONSTRAINT [DF_InvoiceBulkActionDraft_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_InvoiceBulkActionDraft] PRIMARY KEY CLUSTERED ([ID] ASC)
);

