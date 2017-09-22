

CREATE PROCEDURE  [TOneWhS_BE].[sp_SupplierRate_GetSupplierRatesForZone]
(
	@ZoneId INT,
	@EffectiveOn DateTime
)
	AS
	BEGIN


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

		 WHERE	(SR.ZoneID = @ZoneId)
				 AND SR.RateTypeID is null
				 AND (SR.BED < = @EffectiveOn   and (SR.EED is null or SR.EED  > @EffectiveOn) )	
		SET NOCOUNT OFF
	END