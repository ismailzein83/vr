-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [VR_Invoice].[sp_InvoiceAccount_TryUpdateStatus]
	-- Add the parameters for the stored procedure here
	@InvoiceTypeId uniqueidentifier,
	@PartnerId varchar(50),
	@Status int,
	@IsDeleted bit
AS
BEGIN
	IF EXISTS(select 1 from VR_Invoice.InvoiceAccount where InvoiceTypeId = @InvoiceTypeId and PartnerId = @PartnerId) 
	BEGIN
		Update VR_Invoice.InvoiceAccount
		Set [Status] = @Status,
			 IsDeleted = @IsDeleted
		Where InvoiceTypeId = @InvoiceTypeId and PartnerId = @PartnerId
	END

END