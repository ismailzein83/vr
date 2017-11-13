create PROCEDURE [VR_Invoice].sp_InvoiceBulkActionDraft_Clear
	@InvoiceBulkActionIdentifier uniqueidentifier

AS
BEGIN
	Delete FROM [VR_Invoice].InvoiceBulkActionDraft WHERE InvoiceBulkActionIdentifier = @InvoiceBulkActionIdentifier
END