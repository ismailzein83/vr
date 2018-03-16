-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_Invoice].[sp_Invoice_GetLast]
		@InvoiceTypeId uniqueidentifier,
		@PartnerId varchar(50)
AS
BEGIN
	SELECT top(1)	ID,InvoiceTypeID,SettlementInvoiceId,SplitInvoiceGroupId,PartnerID,SerialNumber,FromDate,ToDate,IssueDate,DueDate,Details,PaidDate,UserId,IsAutomatic,CreatedTime,Settings,LockDate,Notes, SourceId,InvoiceSettingId,SentDate
	FROM	VR_Invoice.Invoice with(nolock)
	where	InvoiceTypeID = @InvoiceTypeId  AND  PartnerID = @PartnerId  AND ISNULL(IsDeleted,0) = 0 AND ISNULL(IsDraft, 0) = 0
	Order by CreatedTime desc
END