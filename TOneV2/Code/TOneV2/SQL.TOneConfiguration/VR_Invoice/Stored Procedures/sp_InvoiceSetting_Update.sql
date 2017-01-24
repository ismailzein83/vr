-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_Invoice].[sp_InvoiceSetting_Update]
	@InvoiceSettingId uniqueidentifier,
	@Name nvarchar(255),
	@InvoiceTypeId uniqueidentifier,
	@IsDefault bit,
	@Details nvarchar(MAX)
AS
BEGIN
IF NOT EXISTS(select 1 from VR_Invoice.InvoiceSetting where Name = @Name and Id!=@InvoiceSettingId AND InvoiceTypeId =@InvoiceTypeId ) 
BEGIN
	Update VR_Invoice.InvoiceSetting
	Set  Name = @Name,
	    InvoiceTypeId =@InvoiceTypeId,
		IsDefault = @IsDefault,
		Details = @Details
	Where ID = @InvoiceSettingId
	END
END