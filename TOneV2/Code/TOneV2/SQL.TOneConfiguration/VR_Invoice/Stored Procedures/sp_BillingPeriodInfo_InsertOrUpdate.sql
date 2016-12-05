-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [VR_Invoice].sp_BillingPeriodInfo_InsertOrUpdate
		@InvoiceTypeId uniqueidentifier,
		@PartnerId varchar(50),
		@NextPeriodStart Datetime
AS
BEGIN
	If EXISTS(SELECT 1 from VR_Invoice.BillingPeriodInfo where  InvoiceTypeID = @InvoiceTypeId and PartnerID = @PartnerId)
	BEGIN
	 UPDATE VR_Invoice.BillingPeriodInfo SET NextPeriodStart = @NextPeriodStart WHERE InvoiceTypeID = @InvoiceTypeId and PartnerID = @PartnerId
	END
	ELSE
	BEGIN
		INSERT INTO VR_Invoice.BillingPeriodInfo (InvoiceTypeID,PartnerID,NextPeriodStart) VALUES (@InvoiceTypeId,@PartnerId,@NextPeriodStart)
	END
END