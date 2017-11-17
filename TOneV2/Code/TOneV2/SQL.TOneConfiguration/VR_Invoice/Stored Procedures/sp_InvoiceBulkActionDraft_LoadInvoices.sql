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
			SentDate
	FROM	VR_Invoice.Invoice inv with(nolock)
	JOIN    [VR_Invoice].InvoiceBulkActionDraft ibad 
	ON      inv.ID = ibad.InvoiceId 
	AND     InvoiceBulkActionIdentifier = @InvoiceBulkActionIdentifier
	where   ISNULL(inv.IsDeleted,0) = 0 AND ISNULL(inv.IsDraft, 0) = 0
END