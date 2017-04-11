-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [TOneWhS_Invoice].[sp_CarrierInvoiceAccount_Update]
	@ID int,
	@CarrierProfileId INT,
	@CarrierAccountId INT,
	@InvoiceAccountSettings nvarchar(MAX),
	@BED datetime,
	@EED datetime = NULL
	
AS
BEGIN
	Update [TOneWhS_Invoice].CarrierInvoiceAccount
	Set CarrierProfileId = @CarrierProfileId,
		CarrierAccountId = @CarrierAccountId,
		InvoiceAccountSettings = @InvoiceAccountSettings,
		BED = @BED,
		EED = @EED
	Where ID = @ID
END