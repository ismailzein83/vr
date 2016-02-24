

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [CP_SupPriceList].[sp_CustomerSupplierMapping_Update]
	@ID int ,
	@UserID int,
	@CustomerID int,
	@MappingSettings nvarchar(max)
	AS
	BEGIN
		Update [CP_SupPriceList].[CustomerSupplierMapping]
		Set UserID = @UserID,
			CustomerID = @CustomerID,
			MappingSettings =@MappingSettings
			
		Where ID = @ID
	END