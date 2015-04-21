

Create PROCEDURE [LCR].[sp_RoutingRules_GetDifferentialBlockSupplierRouteRules] 
	@LastRun datetime
AS

BEGIN
	SELECT
		ro.CustomerID,
		CASE WHEN ro.Code = '*ALL*' THEN NULL ELSE ro.Code END Code,
		ro.IncludeSubCodes,
		CASE WHEN ro.Code != '*ALL*' THEN NULL ELSE ro.OurZoneID END ZoneID,
		ro.BlockedSuppliers,
		ro.BeginEffectiveDate,
		ro.EndEffectiveDate,
		ro.IsEffective,
		ro.ExcludedCodes,
		ro.Reason
	FROM
		RouteOverride ro 
	    WHERE ro.BlockedSuppliers IS NOT NULL
	    AND ((ro.EndEffectiveDate > @LastRun and ro.EndEffectiveDate <= GETDATE()) OR (ro.BeginEffectiveDate > @LastRun and ro.BeginEffectiveDate <= GETDATE())) 
END