CREATE PROCEDURE [VR_Invoice].[sp_InvoiceGenerationDraft_Update]
	@InvoiceGenerationDraftId bigint,
	@From datetime,
	@To datetime,
	@CustomPayload nvarchar(max)
AS
BEGIN

	UPDATE [VR_Invoice].[InvoiceGenerationDraft] set FromDate = @From, ToDate = @To, CustomPayload = @CustomPayload
	WHERE ID = @InvoiceGenerationDraftId

END