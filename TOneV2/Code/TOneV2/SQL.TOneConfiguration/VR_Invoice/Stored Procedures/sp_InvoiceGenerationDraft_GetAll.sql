CREATE PROCEDURE [VR_Invoice].[sp_InvoiceGenerationDraft_GetAll]
	@InvoiceGenerationIdentifier uniqueidentifier

AS
BEGIN
	Select ID,InvoiceGenerationIdentifier, InvoiceTypeId, PartnerId, PartnerName, FromDate, ToDate, CustomPayload
	FROM [VR_Invoice].[InvoiceGenerationDraft]
	WHERE InvoiceGenerationIdentifier = @InvoiceGenerationIdentifier
END