
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [VR_Invoice].[sp_Invoice_UpdateInvoicePaidById]
		@InvoiceTypeId uniqueidentifier,
		@InvoiceId bigint,
		@PaidDate Datetime = null
AS
BEGIN
	UPDATE VR_Invoice.Invoice set PaidDate = @PaidDate
	where InvoiceTypeID = @InvoiceTypeId and ID = @InvoiceId
END