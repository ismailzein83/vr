CREATE PROCEDURE  [TOneWhS_BE].[sp_SupplierRate_CreateTempByFiltered]
(
	@TempTableName varchar(200),	
	@SupplierId INT,
	@ZonesIDs varchar(max), 
	@EffectiveOn DateTime
)
	AS
	BEGIN
		SET NOCOUNT ON
		
		IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN

		DECLARE @ZonesIDsTable TABLE (ZoneID int)
		INSERT INTO @ZonesIDsTable (ZoneID)
		select Convert(int, ParsedString) from [TOneWhS_BE].[ParseStringList](@ZonesIDs)

			 SELECT     rate.[ID]
                      , rate.[PriceListID]
                      , rate.[ZoneID]
	                  , rate.[CurrencyID]
                      , rate.[NormalRate]
                      , rate.[OtherRates]
                      , rate.[BED]
                      , rate.[EED]
                      , rate.[timestamp]
             into #Result
             FROM [TOneWhS_BE].[SupplierRate] rate inner join [TOneWhS_BE].[SupplierPriceList] priceList on rate.PriceListID=priceList.ID

			 WHERE (@SupplierId =0 OR priceList.SupplierID = @SupplierId)
			 and (@ZonesIDs  is null or rate.ZoneID in (select ZoneID from @ZonesIDsTable))
			 AND   (@EffectiveOn is null or (rate.BED < = @EffectiveOn   and (rate.EED is null or rate.EED  > @EffectiveOn) ));
			
						
			declare @sql varchar(1000)
			set @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #Result';
			exec(@sql)
			
		END
		
		SET NOCOUNT OFF
	END