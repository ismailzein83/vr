
CREATE PROCEDURE [TOneWhS_BE].[sp_SalePricelistRPChange_GetPricelistsRPChanges]
@PricelistIDs varchar(max)
AS
BEGIN

DECLARE @PricelistIDsTable TABLE (PricelistID int)
INSERT INTO @PricelistIDsTable (PricelistID)
select Convert(int, ParsedString) from [TOneWhS_BE].[ParseStringList](@PricelistIDs)

SELECT SpRP.[ZoneName]
	  ,SpRP.[ZoneID]
      ,SpRP.[RoutingProductId]
      ,SpRP.[RecentRoutingProductId]
      ,SpRP.[BED]
      ,SpRP.[EED]
      ,SpRP.[PriceListId]
      ,SpRP.[CountryId]
  FROM [TOneWhS_BE].[SalePricelistRPChange] SpRP
WHERE (@PricelistIDs  is null or SpRP.PricelistId in (select PricelistID from @PricelistIDsTable))
	
END