-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [VR_Invoice].sp_InvoiceSetting_Delete
	@InvoiceSettingId uniqueidentifier
AS
BEGIN
	update VR_Invoice.InvoiceSetting
	set IsDeleted = 1
	where ID = @InvoiceSettingId
END