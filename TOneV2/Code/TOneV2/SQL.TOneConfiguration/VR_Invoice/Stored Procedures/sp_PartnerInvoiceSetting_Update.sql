-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_Invoice].[sp_PartnerInvoiceSetting_Update]
	@PartnerInvoiceSettingId uniqueidentifier,
	@Details nvarchar(MAX)
AS
BEGIN
	Update VR_Invoice.PartnerInvoiceSetting
	Set Details = @Details
	Where ID = @PartnerInvoiceSettingId
END