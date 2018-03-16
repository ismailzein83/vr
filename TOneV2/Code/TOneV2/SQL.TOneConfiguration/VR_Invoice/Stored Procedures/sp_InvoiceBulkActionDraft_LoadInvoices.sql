CREATE PROCEDURE [VR_Invoice].[sp_InvoiceBulkActionDraft_LoadInvoices]
	@InvoiceBulkActionIdentifier uniqueidentifier
AS
BEGIN
	SELECT	inv.ID,
			inv.InvoiceTypeID,
			PartnerID,
			SerialNumber,
			FromDate,
			ToDate,
			IssueDate,
			DueDate,Details,
			PaidDate,
			UserId,
			inv.CreatedTime,
			LockDate,
			Notes,
			IsAutomatic,
			SourceId,
			Settings,
			InvoiceSettingID,
			SettlementInvoiceId,
			SplitInvoiceGroupId,
			SentDate
	FROM	[VR_Invoice].InvoiceBulkActionDraft ibad  with(nolock)
	JOIN  	VR_Invoice.Invoice inv with(nolock)
	ON      inv.ID = ibad.InvoiceId   
			AND ISNULL(inv.IsDeleted,0) = 0 
			AND ISNULL(inv.IsDraft, 0) = 0
	where   InvoiceBulkActionIdentifier = @InvoiceBulkActionIdentifier
END