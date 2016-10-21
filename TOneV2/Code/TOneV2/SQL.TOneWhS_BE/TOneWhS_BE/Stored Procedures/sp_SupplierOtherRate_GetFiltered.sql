CREATE PROCEDURE  [TOneWhS_BE].[sp_SupplierOtherRate_GetFiltered]
(
	@ZoneID bigint, 
	@EffectiveOn DateTime
)
	AS
	BEGIN
		SET NOCOUNT ON

		 SELECT rate.[ID], rate.[PriceListID], rate.[ZoneID], rate.[CurrencyID], rate.[Rate],rate.RateTypeID, rate.[BED], rate.[EED], rate.[timestamp],rate.Change
         FROM	[TOneWhS_BE].[SupplierRate] rate WITH(NOLOCK) 
				inner join [TOneWhS_BE].[SupplierPriceList] priceList WITH(NOLOCK) on rate.PriceListID=priceList.ID

		 WHERE	rate.ZoneID = @ZoneID and rate.RateTypeID is not null
				 AND   (rate.BED < = @EffectiveOn   and (rate.EED is null or rate.EED  > @EffectiveOn) );			
			
		
		SET NOCOUNT OFF
	END