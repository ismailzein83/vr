-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [VR_Invoice].sp_PartnerInvoiceSetting_Update
	@PartnerInvoiceSettingId uniqueidentifier,
	@PartnerId varchar(50),
	@InvoiceSettingId uniqueidentifier,
	@Details nvarchar(MAX)
AS
BEGIN
IF NOT EXISTS(select 1 from VR_Invoice.PartnerInvoiceSetting where InvoiceSettingId = @InvoiceSettingId and PartnerId = @PartnerId and Id!=@PartnerInvoiceSettingId) 
BEGIN
	Update VR_Invoice.PartnerInvoiceSetting
	Set PartnerId = @PartnerId,
	    InvoiceSettingId = @InvoiceSettingId,
		Details = @Details
	Where ID = @PartnerInvoiceSettingId
	END
END