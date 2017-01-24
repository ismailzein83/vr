-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_Invoice].[sp_InvoiceSetting_Insert]
	@InvoiceSettingId uniqueidentifier,
	@Name nvarchar(255),
	@InvoiceTypeId uniqueidentifier,
	@IsDefault bit,
	@Details nvarchar(MAX)
AS
BEGIN

IF NOT EXISTS(select 1 from VR_Invoice.InvoiceSetting where Name = @Name AND InvoiceTypeId =@InvoiceTypeId)
	BEGIN
	Insert into VR_Invoice.InvoiceSetting(ID,[Name],[InvoiceTypeId],IsDefault, Details)
	Values(@InvoiceSettingId,@Name,@InvoiceTypeId, @IsDefault,@Details)
	END
END