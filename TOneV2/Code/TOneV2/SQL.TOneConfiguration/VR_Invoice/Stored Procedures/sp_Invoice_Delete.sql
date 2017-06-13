-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_Invoice].[sp_Invoice_Delete]
	@DeletedInvoiceId bigint
AS
BEGIN
	update VR_Invoice.Invoice
	set IsDeleted = 1
	where ID = @DeletedInvoiceId
END