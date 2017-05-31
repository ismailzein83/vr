-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_Invoice].[sp_InvoiceAccount_TryUpdate]
	-- Add the parameters for the stored procedure here
	@InvoiceAccountId bigint,
	@InvoiceTypeId uniqueidentifier,
	@PartnerId varchar(50),
	@BED datetime = NULL,
	@EED datetime = NULL,
	@Status int,
	@IsDeleted bit
AS
BEGIN
	IF EXISTS(select 1 from VR_Invoice.InvoiceAccount where InvoiceTypeId = @InvoiceTypeId and PartnerId = @PartnerId) 
	BEGIN
		Update VR_Invoice.InvoiceAccount
		Set  BED = @BED,
			 EED =@EED,
			 [Status] = @Status,
			 IsDeleted = @IsDeleted
		Where InvoiceTypeId = @InvoiceTypeId and PartnerId = @PartnerId
	END

END