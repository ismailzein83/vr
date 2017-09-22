
CREATE PROCEDURE  [TOneWhS_BE].[sp_SupplierRate_GetFiltered]
(
	@SupplierId INT,
	@CountriesIDs varchar(max), 
	@ZoneName varchar(max),
	@EffectiveOn DateTime
)
	AS
	BEGIN

		DECLARE @CountriesIDsTable TABLE (CountryID int)
		INSERT INTO @CountriesIDsTable (CountryID)
		SELECT Convert(int, ParsedString) from [TOneWhS_BE].[ParseStringList](@CountriesIDs)

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
				FROM	[TOneWhS_BE].[SupplierRate] SR WITH(NOLOCK) 
				INNER JOIN [TOneWhS_BE].[SupplierZone] SZ WITH(NOLOCK) ON SR.ZoneID=SZ.ID

		 WHERE	(SZ.SupplierID = @SupplierId)
				 AND (@CountriesIDs  is null or SZ.CountryID in (select CountryID from @CountriesIDsTable))
				 AND (@ZoneName  is null or LOWER(SZ.Name) like '%'+LOWER(@ZoneName)+'%')
				 AND SR.RateTypeID is null
				 AND (SR.BED < = @EffectiveOn   and (SR.EED is null or SR.EED  > @EffectiveOn) );			
		SET NOCOUNT OFF
	END