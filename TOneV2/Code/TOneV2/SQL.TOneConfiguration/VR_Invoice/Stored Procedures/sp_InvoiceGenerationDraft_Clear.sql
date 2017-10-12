CREATE PROCEDURE [VR_Invoice].[sp_InvoiceGenerationDraft_Clear]
	@InvoiceGenerationIdentifier uniqueidentifier

AS
BEGIN
	Delete FROM [VR_Invoice].[InvoiceGenerationDraft] WHERE InvoiceGenerationIdentifier = @InvoiceGenerationIdentifier
END