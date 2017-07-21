-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [VR_Invoice].[sp_InvoiceAccount_TryUpdateEffectiveDate]
	-- Add the parameters for the stored procedure here
	@InvoiceTypeId uniqueidentifier,
	@PartnerId varchar(50),
	@BED datetime = NULL,
	@EED datetime = NULL
AS
BEGIN
	IF EXISTS(select 1 from VR_Invoice.InvoiceAccount where InvoiceTypeId = @InvoiceTypeId and PartnerId = @PartnerId) 
	BEGIN
		Update VR_Invoice.InvoiceAccount
		Set  BED = @BED,
			 EED =@EED
		Where InvoiceTypeId = @InvoiceTypeId and PartnerId = @PartnerId
	END

END