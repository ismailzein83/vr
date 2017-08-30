-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_Invoice].[sp_PartnerInvoiceSetting_InsertOrUpdate]
	@PartnerInvoiceSettingId uniqueidentifier,
	@PartnerId varchar(50),
	@InvoiceSettingId uniqueidentifier
AS
BEGIN
IF NOT EXISTS(select 1 from VR_Invoice.PartnerInvoiceSetting where ID = @PartnerInvoiceSettingId) 
BEGIN
	Insert into VR_Invoice.PartnerInvoiceSetting(ID,PartnerID,InvoiceSettingID)
	Values(@PartnerInvoiceSettingId,@PartnerId, @InvoiceSettingId)
END
ELSE
	BEGIN
	  Update VR_Invoice.PartnerInvoiceSetting
		Set PartnerId = @PartnerId,
		    InvoiceSettingId = @InvoiceSettingId
	    Where   ID = @PartnerInvoiceSettingId
	END
END