-- =============================================
-- Author:		Rabih
-- Create date: 11/16/2015
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleEntityRoutingProduct_GetFilteredByOwner]
	@CustomerOwnerType int,
	@EffectiveTime DATETIME = NULL,
	@IsFuture bit,
	@IsDefault bit,
	@ActiveCustomersInfo TOneWhS_BE.RoutingCustomerInfo READONLY
	
AS
BEGIN

	SET NOCOUNT ON;

	---Customer Type
		SELECT  se.[ID],se.[OwnerType],se.[OwnerID],se.[ZoneID],se.[RoutingProductID],se.[BED],se.[EED]
		FROM    [TOneWhS_BE].[SaleEntityRoutingProduct] se WITH(NOLOCK) 
		JOIN	@ActiveCustomersInfo ci on ci.CustomerId = se.OwnerId
		WHERE	((@IsFuture = 0 AND se.BED <= @EffectiveTime AND (se.EED > @EffectiveTime OR se.EED IS NULL))
				OR (@IsFuture = 1 AND (se.BED > GETDATE() OR se.EED IS NULL)))
				AND se.OwnerType = @CustomerOwnerType 
				AND (@IsDefault = 1 OR se.ZoneId IS NOT NULL)

	Union
	
		SELECT  se.[ID],se.[OwnerType],se.[OwnerID],se.[ZoneID],se.[RoutingProductID],se.[BED],se.[EED]
		FROM    [TOneWhS_BE].[SaleEntityRoutingProduct] se WITH(NOLOCK) 
		WHERE	((@IsFuture = 0 AND se.BED <= @EffectiveTime AND (se.EED > @EffectiveTime OR se.EED IS NULL))
				OR (@IsFuture = 1 AND (se.BED > GETDATE() OR se.EED IS NULL)))
				AND se.OwnerType <> @CustomerOwnerType
				AND (@IsDefault = 1 OR se.ZoneId IS NOT NULL)
END