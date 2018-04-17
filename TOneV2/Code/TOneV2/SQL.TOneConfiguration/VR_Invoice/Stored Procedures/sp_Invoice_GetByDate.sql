-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_Invoice].[sp_Invoice_GetByDate]
	@InvoiceTypeId uniqueidentifier,
	@PartnerIds nvarchar(MAX),
	@FromDate datetime,
	@ToDate datetime
AS
BEGIN
	    DECLARE @PartnerIdsTable TABLE (PartnerId varchar(50))
		INSERT INTO @PartnerIdsTable (PartnerId)
		select ParsedString from [VR_Invoice].[ParseStringList](@PartnerIds)


	SELECT	vrIn.ID,
			vrIn.InvoiceTypeID,
			vrIn.PartnerID,SerialNumber,
			vrIn.FromDate,
			vrIn.ToDate,
			vrIn.IssueDate,
			vrIn.DueDate,
			vrIn.Details,
			vrIn.PaidDate,
			vrIn.UserId,
			vrIn.CreatedTime,
			vrIn.LockDate,
			vrIn.Notes,
			vrIn.SourceId,
			vrIn.Settings,
			vrIn.IsAutomatic,
			vrIn.InvoiceSettingId,
			vrIn.SentDate,
			vrIn.SettlementInvoiceId,
			vrIn.SplitInvoiceGroupId,
			vrIn.ApprovedBy,
			vrIn.ApprovedTime,
			vrIn.NeedApproval
	FROM	VR_Invoice.Invoice vrIn with(nolock)  
	where	vrIn.InvoiceTypeId = @InvoiceTypeId AND  
			vrIn.PartnerID  IN (select PartnerId from @PartnerIdsTable) AND 
			vrIn.FromDate < @ToDate  AND 
			vrIn.ToDate > @FromDate AND
			ISNULL(vrIn.IsDeleted,0) = 0 AND 
			ISNULL(vrIn.IsDraft, 0) = 0
END