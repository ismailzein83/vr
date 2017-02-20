CREATE PROCEDURE  [TOneWhS_BE].[sp_SupplierRate_GetPending]
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
		SELECT Convert(int, ParsedString) FROM [TOneWhS_BE].[ParseStringList](@ZonesIDs)

		 SELECT  SR.[ID]
				,SR.[PriceListID]
				,SR.[ZoneID]
				,SR.[CurrencyID]
				,SR.[Rate]
				,SR.RateTypeID
				,SR.[BED]
				,SR.[EED]
				,SR.[timestamp]
				,SR.Change
         FROM	[TOneWhS_BE].[SupplierRate] SR WITH(NOLOCK) 
				INNER JOIN [TOneWhS_BE].[SupplierPriceList] SP WITH(NOLOCK) ON SR.PriceListID=SP.ID

		 WHERE	 SP.SupplierID = @SupplierId_local
				 AND (@ZonesIDs IS NULL OR SR.ZoneID IN (SELECT ZoneID FROM @ZonesIDsTable))
				 AND SR.RateTypeID IS NULL
				 AND SR.BED > @EffectiveOn_local
					
		SET NOCOUNT OFF
	END