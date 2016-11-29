-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [VR_Invoice].sp_Invoice_UpdateInvoiceNote
		@InvoiceId bigint,
		@Notes nvarchar(MAX)
AS
BEGIN
	UPDATE VR_Invoice.Invoice set Notes = @Notes
	where ID = @InvoiceId
END