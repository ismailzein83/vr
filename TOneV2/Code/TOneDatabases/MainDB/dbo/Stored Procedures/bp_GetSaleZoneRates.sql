-- ======================================================================
-- Author:		Fadi Chamieh
-- Create date: 2009-04-24
-- Description: Get the Sale Rates for the Zones defined in our System
-- ======================================================================
CREATE PROCEDURE [dbo].[bp_GetSaleZoneRates](
	@codeFilter varchar(30) = NULL, 
	@ExcludedRate float = -1,
	@zoneNameFilter varchar(50) = NULL, 
	@ServicesFlag smallint = 0,
	@Days int = NULL
)
WITH RECOMPILE
AS
BEGIN
	SET NOCOUNT ON;

	SELECT zr.CustomerID, zr.ZoneID, zr.NormalRate, zr.OffPeakRate, zr.WeekendRate, zr.ServicesFlag
	  FROM ZoneRate zr 
		WHERE 
				zr.SupplierID = 'SYS'
			AND (@zoneNameFilter IS NULL OR ZR.ZoneID IN (SELECT ZoneID FROM Zone WHERE Name LIKE @zoneNameFilter)) 
			AND (@codeFilter IS NULL OR ZR.ZoneID IN (SELECT ZoneID FROM Code WHERE Code LIKE @codeFilter))
			AND (@ServicesFlag IS NULL OR ZR.ServicesFlag & @ServicesFlag = @ServicesFlag)
			AND zr.NormalRate <> @ExcludedRate			
	ORDER BY zr.ZoneID, zr.NormalRate
		 
END