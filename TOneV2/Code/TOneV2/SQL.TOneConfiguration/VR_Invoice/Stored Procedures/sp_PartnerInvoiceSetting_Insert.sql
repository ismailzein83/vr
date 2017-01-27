-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [VR_Invoice].sp_PartnerInvoiceSetting_Insert
	@PartnerInvoiceSettingId uniqueidentifier,
	@PartnerId varchar(50),
	@InvoiceSettingId uniqueidentifier,
	@Details nvarchar(MAX)
AS
BEGIN

IF NOT EXISTS(select 1 from VR_Invoice.PartnerInvoiceSetting where InvoiceSettingId = @InvoiceSettingId and PartnerId = @PartnerId)
	BEGIN
	Insert into VR_Invoice.PartnerInvoiceSetting(ID,PartnerID,InvoiceSettingID,Details)
	Values(@PartnerInvoiceSettingId,@PartnerId, @InvoiceSettingId,@Details)
	END
END