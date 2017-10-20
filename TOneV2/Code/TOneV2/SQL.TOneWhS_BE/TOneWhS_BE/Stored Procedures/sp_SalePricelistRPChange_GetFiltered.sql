
CREATE PROCEDURE [TOneWhS_BE].[sp_SalePricelistRPChange_GetFiltered]
@PriceListID as int,
@CountryIDs varchar(max)
AS
BEGIN
DECLARE @CountryIDsTable TABLE (CountryID int)
INSERT INTO @CountryIDsTable (CountryID)
select Convert(int, ParsedString) from [TOneWhS_BE].[ParseStringList](@CountryIDs)

SELECT SpRP.[ZoneName]
	  ,SpRP.[ZoneID]
      ,SpRP.[RoutingProductId]
      ,SpRP.[RecentRoutingProductId]
      ,SpRP.[BED]
      ,SpRP.[EED]
      ,SpRP.[PriceListId]
      ,SpRP.[CountryId]
	  ,SpRP.[CustomerId]
  FROM [TOneWhS_BE].[SalePricelistRPChange] SpRP WITH(NOLOCK)
  WHERE SpRP.PricelistId = @PriceListID
		AND (@CountryIDs  is null or SpRP.CountryID in (select CountryID from @CountryIDsTable))
	
END