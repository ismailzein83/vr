CREATE PROCEDURE  [TOneWhS_BE].[sp_SupplierRate_GetZoneHistory]
(
	@ZonesIDs varchar(max)
)
	AS
	BEGIN

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
		 INNER JOIN [TOneWhS_BE].[SupplierZone] SZ WITH(NOLOCK) ON SR.ZoneID = SZ.ID

		 WHERE	 (@ZonesIDs IS NULL OR SR.ZoneID IN (SELECT ZoneID FROM @ZonesIDsTable))
				 AND SR.RateTypeID IS NULL 
				 AND (sr.eed IS NULL OR sr.bed<> sr.eed)
		order by SR.[BED] desc			
		SET NOCOUNT OFF
	END