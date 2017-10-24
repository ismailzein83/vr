-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [VR_Invoice].sp_Invoice_UpdateInvoiceSentDate
		@InvoiceId bigint,
		@SentDate Datetime = null
AS
BEGIN
	UPDATE VR_Invoice.Invoice set SentDate = @SentDate
	where ID = @InvoiceId
END