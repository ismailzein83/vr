
CREATE PROCEDURE [TOneWhS_BE].[sp_SalePricelistRateChange_GetPricelistsRateChanges]
@PricelistIDs varchar(max)

AS
BEGIN
DECLARE @PricelistIDsTable TABLE (PricelistID int)
INSERT INTO @PricelistIDsTable (PricelistID)
select Convert(int, ParsedString) from [TOneWhS_BE].[ParseStringList](@PricelistIDs)

SELECT [PricelistId]
      ,[Rate]
	  ,[ZoneID]
      ,[RecentRate]
      ,[CountryID]
      ,[ZoneName]
      ,[Change],
	  BED,
	  EED,
	  RoutingProductID,
	  CurrencyID
  FROM [TOneWhS_BE].[SalePricelistRateChange] SR
  WHERE (@PricelistIDs  is null or Sr.PricelistId in (select PricelistID from @PricelistIDsTable))
	
END