
CREATE PROCEDURE [TOneWhS_BE].[sp_SalePricelistCodeChange_GetFiltered]
@PriceListID as int,
@CountryIDs varchar(max)
AS
BEGIN

DECLARE @CountryIDsTable TABLE (CountryID int)
INSERT INTO @CountryIDsTable (CountryID)
select Convert(int, ParsedString) from [TOneWhS_BE].[ParseStringList](@CountryIDs)

SELECT [PricelistID]
      ,[Code]
      ,spc.[CountryID]
      ,[RecentZoneName]
      ,[ZoneName]
      ,[Change],
	  BED,
	  EED,
	  zoneid
  FROM [TOneWhS_BE].[SalePricelistCodeChange] SP WITH(NOLOCK)
  JOIN TOneWhS_BE.SalePricelistCustomerChange spc WITH(NOLOCK) on spc.BatchID = SP.BatchID and spc.CountryID = sp.countryID
  WHERE spc.PricelistID = @PriceListID
		AND (@CountryIDs  is null or spc.CountryID in (select CountryID from @CountryIDsTable))
	
END