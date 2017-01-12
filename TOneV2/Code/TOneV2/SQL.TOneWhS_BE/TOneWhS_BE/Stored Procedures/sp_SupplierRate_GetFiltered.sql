CREATE PROCEDURE  [TOneWhS_BE].[sp_SupplierRate_GetFiltered]
(
	@SupplierId INT,
	@ZonesIDs varchar(max), 
	@EffectiveOn DateTime
)
	AS
	BEGIN
		DECLARE @SupplierId_local INT = @SupplierId
		DECLARE @EffectiveOn_local DateTime = @EffectiveOn

		DECLARE @ZonesIDsTable TABLE (ZoneID int)
		INSERT INTO @ZonesIDsTable (ZoneID)
		select Convert(int, ParsedString) from [TOneWhS_BE].[ParseStringList](@ZonesIDs)

		 SELECT rate.[ID], rate.[PriceListID], rate.[ZoneID], rate.[CurrencyID], rate.[Rate],rate.RateTypeID, rate.[BED], rate.[EED], rate.[timestamp],rate.Change
         FROM	[TOneWhS_BE].[SupplierRate] rate WITH(NOLOCK) 
				inner join [TOneWhS_BE].[SupplierPriceList] priceList WITH(NOLOCK) on rate.PriceListID=priceList.ID

		 WHERE	(@SupplierId_local =0 OR priceList.SupplierID = @SupplierId_local)
				 and (@ZonesIDs  is null or rate.ZoneID in (select ZoneID from @ZonesIDsTable))
				 And rate.RateTypeID is null
				 AND   (rate.BED < = @EffectiveOn_local   and (rate.EED is null or rate.EED  > @EffectiveOn_local) );			
			
		
		SET NOCOUNT OFF
	END