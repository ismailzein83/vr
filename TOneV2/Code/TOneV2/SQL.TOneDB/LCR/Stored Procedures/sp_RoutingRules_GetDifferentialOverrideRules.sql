

Create PROCEDURE [LCR].[sp_RoutingRules_GetDifferentialOverrideRules] 
	@LastRun datetime
AS
BEGIN
	SELECT
		ro.CustomerID,
		CASE WHEN ro.Code = '*ALL*' THEN NULL ELSE ro.Code END Code,
		ro.IncludeSubCodes,
		CASE WHEN ro.Code != '*ALL*' THEN NULL ELSE ro.OurZoneID END ZoneID,
		ro.RouteOptions,
		ro.BeginEffectiveDate,
		ro.EndEffectiveDate,
		ro.IsEffective,
		ro.ExcludedCodes,
		ro.Reason
	FROM
		RouteOverride ro 
	    WHERE ro.RouteOptions != 'BLK' AND ro.RouteOptions IS NOT NULL
	    AND ((ro.EndEffectiveDate > @LastRun and ro.EndEffectiveDate <= GETDATE()) OR (ro.BeginEffectiveDate > @LastRun and ro.BeginEffectiveDate <= GETDATE())) 

END