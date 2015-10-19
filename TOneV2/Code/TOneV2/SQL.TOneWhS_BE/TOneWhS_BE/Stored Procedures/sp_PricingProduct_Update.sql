-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [TOneWhS_BE].[sp_PricingProduct_Update]
	@ID int,
	@Name nvarchar(255),
	@DefaultRoutingProductId INT,
	@SaleZonePackageID int,
	@Settings nvarchar(MAX)
AS
BEGIN

	Update TOneWhS_BE.PricingProduct
	Set Name = @Name,
		DefaultRoutingProductID=@DefaultRoutingProductId,
		SaleZonePackageID = @SaleZonePackageID,
		Settings = @Settings
	Where ID = @ID
END