-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_Invoice].[sp_Invoice_Get]
		@InvoiceId bigint
AS
BEGIN
	SELECT	ID,InvoiceTypeID,PartnerID,SerialNumber,FromDate,ToDate,IssueDate,DueDate,Details,PaidDate,UserId,CreatedTime
	FROM	VR_Invoice.Invoice with(nolock)
	where	ID = @InvoiceId  
END