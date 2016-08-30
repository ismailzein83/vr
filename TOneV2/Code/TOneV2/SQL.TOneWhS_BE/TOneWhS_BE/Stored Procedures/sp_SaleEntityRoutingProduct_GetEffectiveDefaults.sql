-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleEntityRoutingProduct_GetEffectiveDefaults]
	@EffectiveOn DATETIME
AS
BEGIN
	SELECT	ID,OwnerType,OwnerID,RoutingProductID,BED,EED
	FROM	TOneWhS_BE.SaleEntityRoutingProduct WITH(NOLOCK) 
	WHERE	ZoneID IS NULL
			AND BED <= @EffectiveOn AND (EED IS NULL OR EED > @EffectiveOn)
END