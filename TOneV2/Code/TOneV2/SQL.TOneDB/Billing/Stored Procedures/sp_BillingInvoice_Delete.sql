-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Billing].[sp_BillingInvoice_Delete]
	@InvoiceID INT
AS
BEGIN
	DELETE FROM Billing_Invoice_Details WHERE InvoiceID = @InvoiceID
	--How come the delete operation succeeds without this statement? Billing_Invoice_Costs is linked to the Billing_Inovice
	--DELETE FROM Billing_Invoice_Costs WHERE InvoiceID = @InvoiceID
	DELETE FROM Billing_Invoice WHERE InvoiceID = @InvoiceID
END