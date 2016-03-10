

CREATE PROCEDURE [CP_SupPriceList].[sp_CustomerSupplierMaping_Delete]
	@ID int
	
AS
BEGIN

	Delete [CP_SupPriceList].[CustomerSupplierMapping]
	Where ID = @ID
END