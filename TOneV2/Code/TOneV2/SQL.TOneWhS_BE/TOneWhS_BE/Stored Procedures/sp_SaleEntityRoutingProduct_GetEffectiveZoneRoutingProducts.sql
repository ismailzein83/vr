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
	SELECT ID,OwnerType,OwnerID,ZoneID,RoutingProductID,BED,EED
	FROM [TOneWhS_BE].SaleEntityRoutingProduct WITH(NOLOCK) 
	WHERE OwnerType = @OwnerType
		AND OwnerID = @OwnerID
		AND ZoneID IS NOT NULL
		AND BED <= @EffectiveOn AND (EED IS NULL OR EED > @EffectiveOn)
END