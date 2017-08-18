﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_Invoice].[sp_Invoice_GetFiltered]
	@InvoiceTypeId uniqueidentifier,
	@PartnerIds nvarchar(MAX),
	@PartnerPrefix nvarchar(50),
	@FromDate datetime,
	@ToDate datetime,
	@IssueDate datetime,
	@EffectiveDate  datetime = null,
	@IsEffectiveInFuture bit,
	@Status int = null
AS
BEGIN
DECLARE @PartnerIdsTable TABLE (PartnerId nvarchar(50))
INSERT INTO @PartnerIdsTable (PartnerId)
select Convert(nvarchar(50), ParsedString) from [VR_Invoice].ParseStringList(@PartnerIds)
	
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
			vrIn.TimeZoneId,
			vrIn.TimeZoneOffset, 
			vrIn.SourceId,
			vrIn.IsAutomatic
	FROM	VR_Invoice.Invoice vrIn with(nolock)  
	Inner Join VR_Invoice.InvoiceAccount vrInAcc 
	on vrIn.InvoiceTypeID = vrInAcc.InvoiceTypeId and 
	   vrIn.PartnerID = vrInAcc.PartnerID and 
	   ISNULL(vrInAcc.IsDeleted, 0) = 0 and
	   (@Status IS NULL OR vrInAcc.[Status] = @Status) AND
	   (@EffectiveDate IS NULL OR (vrInAcc.BED <= @EffectiveDate AND (vrInAcc.EED > @EffectiveDate OR vrInAcc.EED IS NULL))) AND
	   (@IsEffectiveInFuture IS NUll OR (@IsEffectiveInFuture = 1 and (vrInAcc.EED IS NULL or vrInAcc.EED >=  GETDATE()))  OR  (@IsEffectiveInFuture = 0 and  vrInAcc.EED <=  GETDATE()))
	where	(vrIn.InvoiceTypeId = @InvoiceTypeId) AND  
			(@PartnerIds is Null or vrIn.PartnerID IN (SELECT PartnerId FROM @PartnerIdsTable)) AND 
			(@PartnerPrefix is null Or vrIn.PartnerId like  (@PartnerPrefix +'%')) AND 
			vrIn.FromDate >= @FromDate AND 
			(@ToDate is null OR vrIn.ToDate <= @ToDate)   AND 
			(@IssueDate is null OR vrIn.IssueDate =@IssueDate ) And 
			 ISNULL( vrIn.IsDeleted,0) = 0 AND ISNULL(vrIn.IsDraft, 0) = 0
			
END