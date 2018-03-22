

create PROCEDURE  [TOneWhS_BE].[sp_SupplierRate_GetSupplierRatesForZones]
(
	@ZoneIds nvarchar(max),
	@EffectiveOn DateTime
)
	AS
	BEGIN

	IF (@ZoneIds IS NOT NULL)
	BEGIN
		DECLARE @zoneIdsTable AS TABLE (ZoneId BIGINT)
		INSERT INTO @zoneIdsTable SELECT CONVERT(BIGINT, ParsedString) FROM [TOneWhS_BE].[ParseStringList](@ZoneIds)
	END;
		 SELECT SR.[ID]
				,SR.[PriceListID]
				,SR.[ZoneID]
				,SR.[CurrencyID]
				,SR.[Rate]
				,SR.RateTypeID
				,SR.[BED]
				,SR.[EED]
				,SR.[timestamp]
				,SR.Change
				FROM	[TOneWhS_BE].[SupplierRate] SR  WITH(NOLOCK)

		 WHERE	 SR.RateTypeID is null
				 AND (SR.BED < = @EffectiveOn   and (SR.EED is null or SR.EED  > @EffectiveOn) )	
				 AND SR.[ZoneID] in (SELECT ZoneId FROM @zoneIdsTable)
		SET NOCOUNT OFF
	END