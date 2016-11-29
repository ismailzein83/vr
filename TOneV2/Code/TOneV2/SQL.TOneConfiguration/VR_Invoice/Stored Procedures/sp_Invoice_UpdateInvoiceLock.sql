-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [VR_Invoice].sp_Invoice_UpdateInvoiceLock
		@InvoiceId bigint,
		@LockDate Datetime = null
AS
BEGIN
	UPDATE VR_Invoice.Invoice set LockDate = @LockDate
	where ID = @InvoiceId
END