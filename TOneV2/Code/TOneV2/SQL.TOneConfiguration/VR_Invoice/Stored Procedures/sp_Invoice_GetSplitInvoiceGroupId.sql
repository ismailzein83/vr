create PROCEDURE [VR_Invoice].sp_Invoice_GetSplitInvoiceGroupId
	@InvoiceId bigint
AS
BEGIN
	Select SplitInvoiceGroupId FROM [VR_Invoice].[Invoice]  WHERE ID = @InvoiceId 
END