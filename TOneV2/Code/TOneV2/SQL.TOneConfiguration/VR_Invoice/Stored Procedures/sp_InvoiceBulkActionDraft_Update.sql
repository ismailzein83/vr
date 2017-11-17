CREATE PROCEDURE [VR_Invoice].[sp_InvoiceBulkActionDraft_Update]
	@InvoiceBulkActionIdentifier uniqueidentifier,
	@InvoiceTypeId uniqueidentifier,
	@IsAllInvoicesSelected bit,
	@TargetInvoicesIds nvarchar(max)
AS
BEGIN

	DECLARE @TargetInvoicesIdsTable TABLE (InvoiceId varchar(50))
	INSERT INTO @TargetInvoicesIdsTable (InvoiceId)
	select ParsedString from [VR_Invoice].[ParseStringList](@TargetInvoicesIds)


	if(@IsAllInvoicesSelected = 1)
	BEGIN
		DELETE FROM [VR_Invoice].InvoiceBulkActionDraft
		WHERE (@TargetInvoicesIds IS NOT NULL and InvoiceId IN (SELECT InvoiceId FROM @TargetInvoicesIdsTable))
		 AND InvoiceBulkActionIdentifier = @InvoiceBulkActionIdentifier
	END
	ELSE
	BEGIN
		DELETE FROM [VR_Invoice].InvoiceBulkActionDraft 
		WHERE InvoiceBulkActionIdentifier = @InvoiceBulkActionIdentifier
		INSERT INTO  [VR_Invoice].InvoiceBulkActionDraft ([InvoiceBulkActionIdentifier],[InvoiceTypeId],[InvoiceId])
		SELECT @InvoiceBulkActionIdentifier,@InvoiceTypeId,InvoiceId FROM @TargetInvoicesIdsTable
	END
	
	Select Count(*) as TotalCount, min(FromDate) as MinimumFrom, max(ToDate) as MaximumTo
	FROM [VR_Invoice].InvoiceBulkActionDraft  ibad with(nolock)
	join [VR_Invoice].Invoice inv on ibad.InvoiceId = inv.ID
	WHERE InvoiceBulkActionIdentifier = @InvoiceBulkActionIdentifier

END