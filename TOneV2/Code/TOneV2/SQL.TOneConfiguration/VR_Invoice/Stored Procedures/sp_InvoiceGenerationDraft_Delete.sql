CREATE PROCEDURE [VR_Invoice].[sp_InvoiceGenerationDraft_Delete]
	@InvoiceGenerationDraftId bigint

AS
BEGIN
	Delete FROM [VR_Invoice].[InvoiceGenerationDraft] WHERE ID = @InvoiceGenerationDraftId
END