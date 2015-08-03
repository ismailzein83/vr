CREATE VIEW [dbo].[vw_CodeRoutes]
As
SELECT
	RT.CustomerID,
	RT.Code,	
	OZ.ZoneID AS OurZoneID, OZ.Name AS OurZone, 
	RT.OurServicesFlag,	RT.OurNormalRate, RT.OurOffPeakRate, RT.OurWeekendRate,		
	RT.State AS RouteState,	
	RO.SupplierID,
	RO.SupplierZoneID, SZ.Name AS SupplierZone,
	RO.SupplierServicesFlag, RO.SupplierNormalRate AS SupplierNormalRate, RO.SupplierOffPeakRate, RO.SupplierWeekendRate,
	RO.State AS RouteOptionState
	
FROM
	Zone OZ
	, [Route] RT
	, RouteOption RO, 
	Zone SZ
WHERE 
		OZ.ZoneID=RT.OurZoneID
	AND RT.RouteID=RO.RouteID
	AND RO.SupplierZoneID = SZ.ZoneID