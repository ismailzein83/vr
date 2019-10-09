
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_Invoice].[sp_Invoice_UpdateInvoiceStatus]
		@InvoiceId bigint,
		@StatusID uniqueidentifier
AS
BEGIN
	UPDATE VR_Invoice.Invoice set StatusID = @StatusID
	where ID = @InvoiceId
END