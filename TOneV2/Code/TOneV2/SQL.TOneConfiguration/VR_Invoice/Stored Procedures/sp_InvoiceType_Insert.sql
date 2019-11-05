-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_Invoice].[sp_InvoiceType_Insert]
	@InvoiceTypeId uniqueidentifier,
	@Name nvarchar(255),
	@DevProjectID uniqueidentifier,
	@Settings nvarchar(MAX)
AS
BEGIN

IF NOT EXISTS(select 1 from VR_Invoice.InvoiceType where Name = @Name)
	BEGIN
	Insert into VR_Invoice.InvoiceType(ID,[Name],[DevProjectID],[Settings])
	Values(@InvoiceTypeId,@Name,@DevProjectID, @Settings)
	END
END