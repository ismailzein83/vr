-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleEntityRoutingProduct_UpdateZoneRoutingProducts]
	@OwnerType TINYINT,
	@OwnerID INT,
	@ZoneRoutingProductChanges TOneWhS_BE.ZoneRoutingProductChange READONLY
AS
BEGIN
	UPDATE TOneWhS_BE.SaleEntityRoutingProduct
	SET EED = rpChanges.EED
	FROM TOneWhS_BE.SaleEntityRoutingProduct rp
		INNER JOIN @ZoneRoutingProductChanges rpChanges ON rp.ZoneID = rpChanges.ZoneID
	WHERE rp.OwnerType = @OwnerType AND rp.OwnerID = @OwnerID AND rp.RoutingProductID = rpChanges.RoutingProductID
END