-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE VR_Invoice.sp_InvoiceType_Insert
	@InvoiceTypeId uniqueidentifier,
	@Name nvarchar(255),
	@Settings nvarchar(MAX)
AS
BEGIN

IF NOT EXISTS(select 1 from VR_Invoice.InvoiceType where Name = @Name)
	BEGIN
	Insert into VR_Invoice.InvoiceType(ID,[Name], [Settings])
	Values(@InvoiceTypeId,@Name, @Settings)
	END
END