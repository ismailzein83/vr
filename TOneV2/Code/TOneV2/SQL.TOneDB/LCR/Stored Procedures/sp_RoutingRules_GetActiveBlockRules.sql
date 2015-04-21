
Create PROCEDURE [LCR].[sp_RoutingRules_GetActiveBlockRules] 
	@ZoneIds LCR.IntIDType READONLY,
	@Codes LCR.CodeType READONLY,
	@EffectiveTime datetime,
	@IsFuture BIT
	
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
	    Left JOIN @ZoneIds z ON ro.OurZoneID = z.ID
	    LEFT JOIN @Codes c ON c.code = ro.code  
	    WHERE ro.RouteOptions = 'BLK' 
	    AND (z.ID IS NOT NULL) OR (ro.Code = c.code) OR(c.IncludeSubCodes = 1 AND ro.Code LIKE c.code + '%')
		AND 
			(
				(@IsFuture = 0 AND ro.BeginEffectiveDate <= @EffectiveTime AND (ro.EndEffectiveDate IS NULL OR ro.EndEffectiveDate > @EffectiveTime))
				OR
				(@IsFuture = 1 AND (ro.BeginEffectiveDate > GETDATE() OR ro.EndEffectiveDate IS NULL))
			)

END