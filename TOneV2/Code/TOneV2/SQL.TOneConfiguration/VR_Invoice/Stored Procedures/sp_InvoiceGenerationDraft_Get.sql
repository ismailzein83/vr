create PROCEDURE [VR_Invoice].[sp_InvoiceGenerationDraft_Get]
	@InvoiceGenerationDraftId bigint

AS
BEGIN
	Select ID,InvoiceGenerationIdentifier, InvoiceTypeId, PartnerId, PartnerName, FromDate, ToDate, CustomPayload
	FROM [VR_Invoice].[InvoiceGenerationDraft] with(nolock)
	WHERE Id = @InvoiceGenerationDraftId
END