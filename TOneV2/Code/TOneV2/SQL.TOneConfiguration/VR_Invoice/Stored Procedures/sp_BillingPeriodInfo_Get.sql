-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [VR_Invoice].sp_BillingPeriodInfo_Get
		@InvoiceTypeId uniqueidentifier,
		@PartnerId varchar(50)
AS
BEGIN
	SELECT	InvoiceTypeID,PartnerID,NextPeriodStart
	FROM	VR_Invoice.BillingPeriodInfo with(nolock)
	where	InvoiceTypeID = @InvoiceTypeId  AND PartnerID = @PartnerId
END