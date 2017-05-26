﻿
CREATE PROCEDURE [TOneWhS_BE].[sp_SalePricelistRateChange_GetFiltered]
@PriceListID as int,
@CountryIDs varchar(max)
AS
BEGIN
DECLARE @CountryIDsTable TABLE (CountryID int)
INSERT INTO @CountryIDsTable (CountryID)
select Convert(int, ParsedString) from [TOneWhS_BE].[ParseStringList](@CountryIDs)

SELECT [PricelistId]
      ,[Rate]
	  ,[ZoneID]
      ,[RecentRate]
      ,[CountryID]
      ,[ZoneName]
      ,[Change],
	  BED,
	  EED,
	  RoutingProductID
  FROM [TOneWhS_BE].[SalePricelistRateChange] SR
  WHERE SR.PricelistId = @PriceListID
		AND (@CountryIDs  is null or Sr.CountryID in (select CountryID from @CountryIDsTable))
	
END