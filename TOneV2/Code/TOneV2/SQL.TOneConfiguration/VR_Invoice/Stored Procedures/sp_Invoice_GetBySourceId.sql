-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [VR_Invoice].sp_Invoice_GetBySourceId
		@InvoiceTypeId uniqueidentifier,
		@SourceId nvarchar(100)
AS
BEGIN
	SELECT	ID,InvoiceTypeID,PartnerID,SerialNumber,FromDate,ToDate,IssueDate,DueDate,Details,PaidDate,UserId,CreatedTime,LockDate,Notes,TimeZoneId,TimeZoneOffset, SourceId
	FROM	VR_Invoice.Invoice with(nolock)
	where  InvoiceTypeID =  @InvoiceTypeId and 	SourceId = @SourceId
END