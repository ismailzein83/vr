create PROCEDURE [VR_Invoice].sp_Invoice_UpdateSettleIds
	@InvoiceId bigint,
	@InvoiceToSettleIds nvarchar(MAX)
AS
BEGIN
	    DECLARE @InvoiceToSettleIdsTable TABLE (InvoiceId bigint)
		INSERT INTO @InvoiceToSettleIdsTable (InvoiceId)
		select ParsedString from [VR_Invoice].[ParseStringList](@InvoiceToSettleIds)

	   UPDATE [VR_Invoice].[Invoice]
	   SET SettlementInvoiceId = @InvoiceId
	   WHERE ID in (select  InvoiceId FROM @InvoiceToSettleIdsTable)
END