-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [VR_Invoice].sp_Invoice_GetByDate
	@InvoiceTypeId uniqueidentifier,
	@PartnerId varchar(50),
	@FromDate datetime,
	@ToDate datetime
AS
BEGIN
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
			vrIn.SentDate
	FROM	VR_Invoice.Invoice vrIn with(nolock)  
	where	vrIn.InvoiceTypeId = @InvoiceTypeId AND  
			vrIn.PartnerID = @PartnerId AND 
			vrIn.FromDate < @ToDate  AND 
			vrIn.ToDate > @FromDate AND
			ISNULL(vrIn.IsDeleted,0) = 0 AND 
			ISNULL(vrIn.IsDraft, 0) = 0
END