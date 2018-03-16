-- =============================================
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
	@Status int = null,
	@IsSelectAll bit = null,
	@InvoiceBulkActionIdentifier uniqueidentifier = null,
	@IsSent bit = null,
	@IsPaid bit = null
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
			vrIn.SettlementInvoiceId,
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
			vrIn.SplitInvoiceGroupId
			into #InvoicesResult
	FROM	VR_Invoice.Invoice vrIn with(nolock)  
	Inner Join VR_Invoice.InvoiceAccount vrInAcc 
	on vrIn.InvoiceTypeID = vrInAcc.InvoiceTypeId and 
	   vrIn.PartnerID = vrInAcc.PartnerID and 
	   ISNULL(vrInAcc.IsDeleted, 0) = 0 and
	   (@Status IS NULL OR vrInAcc.[Status] = @Status) AND
	   (@EffectiveDate IS NULL OR ((vrInAcc.BED <= @EffectiveDate OR vrInAcc.BED IS NULL) AND (vrInAcc.EED > @EffectiveDate OR vrInAcc.EED IS NULL))) AND
	   (@IsEffectiveInFuture IS NUll OR (@IsEffectiveInFuture = 1 and (vrInAcc.EED IS NULL or vrInAcc.EED >=  GETDATE()))  OR  (@IsEffectiveInFuture = 0 and  vrInAcc.EED <=  GETDATE()))
	where	(vrIn.InvoiceTypeId = @InvoiceTypeId) AND  
			(@PartnerIds is Null or vrIn.PartnerID IN (SELECT PartnerId FROM @PartnerIdsTable)) AND 
			(@PartnerPrefix is null Or vrIn.PartnerId like  (@PartnerPrefix +'%')) AND 
			vrIn.FromDate >= @FromDate AND 
			(@ToDate is null OR vrIn.ToDate <= @ToDate)   AND 
			(@IssueDate is null OR vrIn.IssueDate =@IssueDate ) And 
			 ISNULL( vrIn.IsDeleted,0) = 0 AND ISNULL(vrIn.IsDraft, 0) = 0
			 AND (@IsPaid IS NULL OR (vrIn.PaidDate IS NULL AND  @IsPaid = 0) OR (vrIn.PaidDate IS NOT NULL AND  @IsPaid = 1))
			 AND (@IsSent IS NULL OR (vrIn.SentDate IS NULL AND  @IsSent = 0) OR (vrIn.SentDate IS NOT NULL AND  @IsSent = 1))

	IF(@IsSelectAll IS NOT NULL AND @IsSelectAll = 1 AND @InvoiceBulkActionIdentifier IS NOT NULL)
	BEGIN

		DELETE FROM [VR_Invoice].InvoiceBulkActionDraft
		WHERE InvoiceBulkActionIdentifier = @InvoiceBulkActionIdentifier

		INSERT INTO [VR_Invoice].InvoiceBulkActionDraft([InvoiceBulkActionIdentifier],[InvoiceTypeId],[InvoiceId])
		SELECT @InvoiceBulkActionIdentifier,InvoiceTypeId, ID FROM  #InvoicesResult
		
	END
	ELSE IF(@IsSelectAll IS NOT NULL AND @IsSelectAll =0 AND @InvoiceBulkActionIdentifier IS NOT NULL)
	BEGIN
		DELETE FROM [VR_Invoice].InvoiceBulkActionDraft
		WHERE InvoiceBulkActionIdentifier = @InvoiceBulkActionIdentifier
	END
	SELECT * from #InvoicesResult
END