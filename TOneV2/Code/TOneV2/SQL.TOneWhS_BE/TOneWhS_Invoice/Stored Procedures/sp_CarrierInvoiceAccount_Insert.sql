-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [TOneWhS_Invoice].[sp_CarrierInvoiceAccount_Insert]
	@CarrierProfileId INT,
	@CarrierAccountId INT,
	@InvoiceAccountSettings nvarchar(MAX),
	@BED datetime,
	@EED datetime = NULL,
	@Id int out
AS
BEGIN
	Insert into [TOneWhS_Invoice].CarrierInvoiceAccount(CarrierProfileId, CarrierAccountId, InvoiceAccountSettings, BED, EED  )
	Values (@CarrierProfileId, @CarrierAccountId, @InvoiceAccountSettings, @BED, @EED )
	Set @Id = SCOPE_IDENTITY()
END