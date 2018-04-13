

create PROCEDURE  [TOneWhS_BE].[sp_SupplierRate_GetSupplierRates]
(
	@ZoneIds nvarchar(max),
	@BeginDate DateTime,
	@EndDate Datetime
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
				 and  (@BeginDate is null or (SR.BED <=@EndDate and (SR.EED is null or SR.EED > @EndDate  )) )
				 AND SR.[ZoneID] in (SELECT ZoneId FROM @zoneIdsTable)
		SET NOCOUNT OFF
	END