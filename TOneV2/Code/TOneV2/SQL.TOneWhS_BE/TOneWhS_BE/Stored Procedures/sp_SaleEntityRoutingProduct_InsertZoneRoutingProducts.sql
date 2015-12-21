-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleEntityRoutingProduct_InsertZoneRoutingProducts]
	@OwnerType TINYINT,
	@OwnerID INT,
	@NewZoneRoutingProducts TOneWhS_BE.NewZoneRoutingProduct READONLY
AS
BEGIN
	UPDATE TOneWhS_BE.SaleEntityRoutingProduct
	SET EED = newProducts.BED
	FROM TOneWhS_BE.SaleEntityRoutingProduct rp
		INNER JOIN @NewZoneRoutingProducts newProducts
		ON rp.ZoneID = newProducts.ZoneID
	WHERE rp.OwnerType = @OwnerType AND rp.OwnerID = @OwnerID
	
	INSERT INTO TOneWhS_BE.SaleEntityRoutingProduct (OwnerType, OwnerID, ZoneID, RoutingProductID, BED, EED)
	SELECT @OwnerType, @OwnerID, ZoneID, RoutingProductID, BED, EED FROM @NewZoneRoutingProducts
END