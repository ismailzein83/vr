-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_Invoice].[sp_Invoice_UpdateInvoiceApproved] 
		@InvoiceId bigint,
		@ApproveDate Datetime = null,
		@ApprovedBy int
AS
BEGIN
	UPDATE VR_Invoice.Invoice set ApprovedTime = @ApproveDate, ApprovedBy = @ApprovedBy
	where ID = @InvoiceId
END