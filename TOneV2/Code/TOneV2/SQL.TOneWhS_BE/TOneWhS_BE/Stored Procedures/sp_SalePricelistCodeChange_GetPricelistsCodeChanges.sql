
CREATE PROCEDURE [TOneWhS_BE].[sp_SalePricelistCodeChange_GetPricelistsCodeChanges]
@PricelistIDs varchar(max)
AS
BEGIN

DECLARE @PricelistIDsTable TABLE (PricelistID int)
INSERT INTO @PricelistIDsTable (PricelistID)
select Convert(int, ParsedString) from [TOneWhS_BE].[ParseStringList](@PricelistIDs)

SELECT [PricelistID]
      ,[Code]
      ,spc.[CountryID]
      ,[RecentZoneName]
      ,[ZoneName]
      ,[Change],
	  BED,
	  EED,
	  zoneid
  FROM [TOneWhS_BE].[SalePricelistCodeChange] SP
  JOIN TOneWhS_BE.SalePricelistCustomerChange spc on spc.BatchID = SP.BatchID and spc.CountryID = sp.countryID
  WHERE (@PricelistIDs  is null or spc.PricelistID in (select PricelistID from @PricelistIDsTable))
	
END