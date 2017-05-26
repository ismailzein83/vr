

CREATE PROCEDURE [TOneWhS_BE].[sp_SaleEntityRoutingProduct_GetFilteredByOwnerAndZone]
	@CustomerOwnerType int,
	@EffectiveTime DATETIME = NULL,
	@IsFuture bit,
	@IsDefault bit,
	@ActiveCustomersInfo TOneWhS_BE.RoutingCustomerInfo READONLY,
	@ZoneIds nvarchar(max)
AS
BEGIN
	DECLARE @ZoneIDsTable TABLE (ZoneID int)
	INSERT INTO @ZoneIDsTable (ZoneID)
	select Convert(int, ParsedString) from [TOneWhS_BE].[ParseStringList](@ZoneIds)

	SET NOCOUNT ON;

	---Customer Type
		SELECT  se.[ID],se.[OwnerType],se.[OwnerID],se.[ZoneID],se.[RoutingProductID],se.[BED],se.[EED]
		FROM    [TOneWhS_BE].[SaleEntityRoutingProduct] se WITH(NOLOCK) 
		JOIN	@ActiveCustomersInfo ci on ci.CustomerId = se.OwnerId
		WHERE	((@IsFuture = 0 AND se.BED <= @EffectiveTime AND (se.EED > @EffectiveTime OR se.EED IS NULL))
				OR (@IsFuture = 1 AND (se.BED > GETDATE() OR se.EED IS NULL)))
				AND se.OwnerType = @CustomerOwnerType 
				AND (@IsDefault = 1 OR se.ZoneId IS NOT NULL)
				AND (@ZoneIds is null or se.ZoneID in (select ZoneID from @ZoneIDsTable))

	Union
	
		SELECT  se.[ID],se.[OwnerType],se.[OwnerID],se.[ZoneID],se.[RoutingProductID],se.[BED],se.[EED]
		FROM    [TOneWhS_BE].[SaleEntityRoutingProduct] se WITH(NOLOCK) 
		WHERE	((@IsFuture = 0 AND se.BED <= @EffectiveTime AND (se.EED > @EffectiveTime OR se.EED IS NULL))
				OR (@IsFuture = 1 AND (se.BED > GETDATE() OR se.EED IS NULL)))
				AND se.OwnerType <> @CustomerOwnerType
				AND (@IsDefault = 1 OR se.ZoneId IS NOT NULL)
				AND (@ZoneIds is null or se.ZoneID in (select ZoneID from @ZoneIDsTable))
END