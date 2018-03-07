-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_Invoice].[sp_Invoice_GetBySourceId]
		@InvoiceTypeId uniqueidentifier,
		@SourceId nvarchar(100)
AS
BEGIN
	SELECT	ID,InvoiceTypeID,PartnerID,SerialNumber,FromDate,ToDate,IssueDate,DueDate,Details,PaidDate,UserId,CreatedTime,LockDate,IsAutomatic,Notes,Settings, SourceId,InvoiceSettingId,SentDate,SettlementInvoiceId
	FROM	VR_Invoice.Invoice with(nolock)
	where  InvoiceTypeID =  @InvoiceTypeId and 	SourceId = @SourceId AND ISNULL(IsDeleted,0) = 0 AND ISNULL(IsDraft, 0) = 0
END