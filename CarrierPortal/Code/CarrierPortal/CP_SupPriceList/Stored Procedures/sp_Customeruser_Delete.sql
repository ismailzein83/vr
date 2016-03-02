
CREATE PROCEDURE [CP_SupPriceList].[sp_Customeruser_Delete]
	@UserID int
	
AS
BEGIN

	Delete [CP_SupPriceList].[CustomerUser]
	Where UserID = @UserID
END