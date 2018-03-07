-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_Invoice].[sp_Invoice_GetLastByPartners]
		@PartnerInvoiceTypeTable [VR_Invoice].PartnerInvoiceTypeTable READONLY
AS
BEGIN
	SELECT	inv.ID,
			inv.InvoiceTypeID,
			inv.PartnerID,
			inv.SerialNumber,
			inv.FromDate,
			inv.ToDate,
			inv.IssueDate,
			inv.DueDate,
			inv.Details,
			inv.PaidDate,
			inv.UserId,
			inv.CreatedTime,
			inv.LockDate,Notes,
			inv.Settings,
			SourceId,
			IsAutomatic,
			InvoiceSettingId,
			SettlementInvoiceId,
			SentDate
	FROM	VR_Invoice.Invoice inv with(nolock)
	join @PartnerInvoiceTypeTable pit on  inv.InvoiceTypeID = pit.InvoiceTypeID AND inv.PartnerID = pit.PartnerId 
	where DueDate <= GETDATE() AND ISNULL(IsDeleted,0) = 0 AND ISNULL(IsDraft, 0) = 0
END