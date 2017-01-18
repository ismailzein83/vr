-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_Invoice].[sp_Invoice_GetAfterImportedID]
	@InvoiceTypeId uniqueidentifier,
	@LastImportedId	bigint
AS
BEGIN

	SELECT	ID,
			InvoiceTypeID,
			PartnerID,
			SerialNumber,
			FromDate,
			ToDate,
			IssueDate,
			DueDate,
			Details,
			PaidDate,
			UserId,
			CreatedTime,
			LockDate,
			Notes
	FROM	VR_Invoice.Invoice with(nolock)
	where	(InvoiceTypeId = @InvoiceTypeId) 
			AND (@LastImportedId is Null or ID > @LastImportedId)
			AND IsDeleted = 0
			order by id asc
END