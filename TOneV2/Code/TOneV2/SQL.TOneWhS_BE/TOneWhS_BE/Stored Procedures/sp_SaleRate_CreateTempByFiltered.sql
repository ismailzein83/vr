CREATE PROCEDURE [TOneWhS_BE].[sp_SaleRate_CreateTempByFiltered]
	@TempTableName varchar(200),
	@EffectiveOn dateTime = null,
	@SellingNumberPlanID int ,
	@ZonesIDs varchar(max),
	@OwnerType int,
	@OwnerID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
	    DECLARE @ZonesIDsTable TABLE (ZoneID int)
		INSERT INTO @ZonesIDsTable (ZoneID)
		select Convert(int, ParsedString) from [TOneWhS_BE].[ParseStringList](@ZonesIDs)
			SELECT
				   sr.[ID]
				  ,sr.[PriceListID]
				  ,sr.[ZoneID]
				  ,sr.[CurrencyID]
				  ,sr.[Rate]
				  ,sr.[OtherRates]
				  ,sr.[BED]
				  ,sr.[EED]
			INTO #RESULT
			FROM 
			TOneWhS_BE.SaleRate sr 
			join   TOneWhS_BE.SalePriceList sp on sr.PriceListID = sp.ID
			join  TOneWhS_BE.SaleZone sz on  sr.ZoneID = sz.ID          
            WHERE 
                (@EffectiveOn is null or sr.BED < = @EffectiveOn)
            and (@EffectiveOn is null or sr.EED is null or sr.EED  > @EffectiveOn)
            and (@SellingNumberPlanID is null or @SellingNumberPlanID = sz.SellingNumberPlanID)
            and (@ZonesIDs  is null or sr.ZoneID in (select ZoneID from @ZonesIDsTable))
            and (@OwnerID = sp.OwnerID)
            and (@OwnerType = sp.OwnerType)
			DECLARE @sql VARCHAR(1000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
			EXEC(@sql)
		END
	SET NOCOUNT OFF
END