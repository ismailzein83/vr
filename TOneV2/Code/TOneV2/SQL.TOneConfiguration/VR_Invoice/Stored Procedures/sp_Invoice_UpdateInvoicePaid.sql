-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE VR_Invoice.sp_Invoice_UpdateInvoicePaid
		@InvoiceId bigint,
		@IsInvoicePaid bit
AS
BEGIN
	UPDATE VR_Invoice.Invoice set Paid = @IsInvoicePaid
	where ID = @InvoiceId
END