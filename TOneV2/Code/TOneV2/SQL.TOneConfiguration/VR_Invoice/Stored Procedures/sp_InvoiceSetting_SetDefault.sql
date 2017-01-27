-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_Invoice].[sp_InvoiceSetting_SetDefault]
	@InvoiceSettingId uniqueidentifier
AS
BEGIN

	DECLARE @InvoiceTypeID UniqueIdentifier = (SELECT [InvoiceTypeId] FROM [VR_Invoice].[InvoiceSetting] WHERE [ID] = @InvoiceSettingId)
	
	UPDATE VR_Invoice.InvoiceSetting
	SET IsDefault = CASE WHEN ID = @InvoiceSettingId THEN 1 ELSE 0 END
	WHERE [InvoiceTypeId] = @InvoiceTypeID AND (IsDefault = 1 OR ID = @InvoiceSettingId)
END