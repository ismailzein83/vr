-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_Invoice].[sp_Invoice_Get]
		@InvoiceIds NVARCHAR(MAX)
AS
BEGIN

	DECLARE @InvoiceIdsTable TABLE (InvoiceId BIGINT)
	INSERT INTO @InvoiceIdsTable (InvoiceId)
	select Convert(nvarchar(50), ParsedString) from [VR_Invoice].ParseStringList(@InvoiceIds)

	SELECT	ID,InvoiceTypeID,PartnerID,SerialNumber,FromDate,ToDate,SplitInvoiceGroupId,IssueDate,DueDate,Details,PaidDate,UserId,CreatedTime,LockDate,Notes,IsAutomatic, SourceId,Settings,InvoiceSettingID,SentDate,SettlementInvoiceId
	FROM	VR_Invoice.Invoice with(nolock)
	where	ID IN (SELECT InvoiceId FROM  @InvoiceIdsTable)
END