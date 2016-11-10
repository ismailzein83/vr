-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_Invoice].[sp_Invoice_UpdateInvoicePaid]
		@InvoiceId bigint,
		@PaidDate Datetime = null
AS
BEGIN
	UPDATE VR_Invoice.Invoice set PaidDate = @PaidDate
	where ID = @InvoiceId
END