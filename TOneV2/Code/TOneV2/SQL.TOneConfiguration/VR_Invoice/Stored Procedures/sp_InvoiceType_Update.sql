-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_Invoice].[sp_InvoiceType_Update]
	@InvoiceTypeId uniqueidentifier,
	@Name nvarchar(255),
	@Settings nvarchar(MAX)
AS
BEGIN
IF NOT EXISTS(select 1 from VR_Invoice.InvoiceType where Name = @Name and Id!=@InvoiceTypeId) 
BEGIN
	Update VR_Invoice.InvoiceType
	Set Name = @Name,
		Settings = @Settings,
		LastModifiedTime = GETDATE()
	Where ID = @InvoiceTypeId
	END
END