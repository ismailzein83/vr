CREATE PROCEDURE  [TOneWhS_BE].[sp_SupplierRate_GetFiltered]
(
	@SupplierId INT,
	@ZonesIDs varchar(max), 
	@EffectiveOn DateTime
)
	AS
	BEGIN
		SET NOCOUNT ON

		DECLARE @ZonesIDsTable TABLE (ZoneID int)
		INSERT INTO @ZonesIDsTable (ZoneID)
		select Convert(int, ParsedString) from [TOneWhS_BE].[ParseStringList](@ZonesIDs)

		 SELECT rate.[ID], rate.[PriceListID], rate.[ZoneID], rate.[CurrencyID], rate.[NormalRate], rate.[OtherRates],rate.RateTypeID, rate.[BED], rate.[EED], rate.[timestamp],rate.Change
         FROM	[TOneWhS_BE].[SupplierRate] rate WITH(NOLOCK) 
				inner join [TOneWhS_BE].[SupplierPriceList] priceList WITH(NOLOCK) on rate.PriceListID=priceList.ID

		 WHERE	(@SupplierId =0 OR priceList.SupplierID = @SupplierId)
				 and (@ZonesIDs  is null or rate.ZoneID in (select ZoneID from @ZonesIDsTable))
				 AND   (@EffectiveOn is null or (rate.BED < = @EffectiveOn   and (rate.EED is null or rate.EED  > @EffectiveOn) ));			
			
		
		SET NOCOUNT OFF
	END