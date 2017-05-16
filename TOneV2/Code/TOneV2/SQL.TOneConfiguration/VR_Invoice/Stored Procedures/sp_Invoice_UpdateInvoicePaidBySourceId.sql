-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [VR_Invoice].sp_Invoice_UpdateInvoicePaidBySourceId
		@InvoiceTypeId uniqueidentifier,
		@SourceId nvarchar(255),
		@PaidDate Datetime = null
AS
BEGIN
	UPDATE VR_Invoice.Invoice set PaidDate = @PaidDate
	where InvoiceTypeID = @InvoiceTypeId and SourceId = @SourceId
END