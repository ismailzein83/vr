CREATE PROCEDURE [VR_Invoice].[sp_InvoiceGenerationDraft_GetSummary]
	@InvoiceGenerationIdentifier uniqueidentifier
AS
BEGIN
	Select Count(*) as TotalCount, min(FromDate) as MinimumFrom, max(ToDate) as MaximumTo
	FROM [VR_Invoice].[InvoiceGenerationDraft] with(nolock)
	WHERE InvoiceGenerationIdentifier = @InvoiceGenerationIdentifier
END