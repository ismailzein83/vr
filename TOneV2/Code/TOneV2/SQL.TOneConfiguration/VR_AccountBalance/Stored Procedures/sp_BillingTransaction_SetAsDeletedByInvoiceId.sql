-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE VR_AccountBalance.sp_BillingTransaction_SetAsDeletedByInvoiceId
	@DeletedInvoiceId bigint
AS
BEGIN
	update VR_AccountBalance.BillingTransaction
	set IsDeleted = 1
	where CreatedByInvoiceID is not null and CreatedByInvoiceID = @DeletedInvoiceId
END