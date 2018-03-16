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
			Notes,
			SourceId,
			Settings,
			IsAutomatic,
			InvoiceSettingID,
			SentDate,
			SettlementInvoiceId,
			SplitInvoiceGroupId
	FROM	VR_Invoice.Invoice with(nolock)
	where	(InvoiceTypeId = @InvoiceTypeId) 
			AND (@LastImportedId is Null or ID > @LastImportedId)
			AND ISNULL(IsDeleted,0) = 0 AND ISNULL(IsDraft, 0) = 0
			order by id asc
END