-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_Invoice].[sp_Invoice_GetBySerialNumbers]
	@InvoiceTypeId uniqueidentifier,
	@SerialNumbers nvarchar(max)
AS
BEGIN

DECLARE @SerialNumbersTable TABLE (SerialNumber nvarchar(255))
INSERT INTO @SerialNumbersTable (SerialNumber)
select  ParsedString from [VR_Invoice].ParseStringList(@SerialNumbers)
	
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
	where	
			@InvoiceTypeId = vrIn.InvoiceTypeID and (@SerialNumbers is Null or vrIn.SerialNumber IN (SELECT SerialNumber FROM @SerialNumbersTable)) 
			and ISNULL( vrIn.IsDeleted,0) = 0 AND ISNULL(vrIn.IsDraft, 0) = 0
			
END