-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleEntityRoutingProduct_GetEffectiveZoneRoutingProducts]
	@OwnerType TINYINT,
	@OwnerID INT,
	@EffectiveOn DATETIME
AS
BEGIN
	SELECT OwnerType,
		OwnerID,
		ZoneID,
		RoutingProductID,
		BED,
		EED
	FROM TOneWhS_BE.SaleEntityRoutingProduct
	WHERE OwnerType = @OwnerType
		AND OwnerID = @OwnerID
		AND ZoneID IS NOT NULL
		AND BED <= @EffectiveOn AND (EED IS NULL OR EED > @EffectiveOn)
END