-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_Invoice].[sp_Invoice_GetLastInvoices]
		@InvoiceTypeId uniqueidentifier,
		@PartnerId varchar(50),
		@BeforeDate datetime = NULL,
		@LastInvoices int
AS
BEGIN
	SELECT top(@LastInvoices)	ID,InvoiceTypeID,PartnerID,SplitInvoiceGroupId,SettlementInvoiceId,SerialNumber,FromDate,ToDate,IssueDate,DueDate,Details,PaidDate,IsAutomatic,UserId,Settings,CreatedTime,LockDate,Notes, SourceId,InvoiceSettingId,SentDate
	FROM	VR_Invoice.Invoice with(nolock)
	where	InvoiceTypeID = @InvoiceTypeId  AND  PartnerID = @PartnerId  AND ISNULL( IsDraft,0) = 0 AND (@BeforeDate IS NULL OR CreatedTime <@BeforeDate) AND ISNULL( IsDeleted,0) = 0
	Order by CreatedTime desc
END