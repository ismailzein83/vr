
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
      ,[CountryID]
      ,[RecentZoneName]
      ,[ZoneName]
      ,[Change]
  FROM [TOneWhS_BE].[SalePricelistCodeChange] SP
  WHERE SP.PricelistID = @PriceListID
		AND (@CountryIDs  is null or SP.CountryID in (select CountryID from @CountryIDsTable))
	
END