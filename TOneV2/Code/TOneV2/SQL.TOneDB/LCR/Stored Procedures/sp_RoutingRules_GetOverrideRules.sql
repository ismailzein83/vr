

CREATE PROCEDURE [LCR].[sp_RoutingRules_GetOverrideRules] 
	@ZoneIds LCR.IntIDType READONLY,
	@CodePrefix VARCHAR(5),
	@EffectiveTime datetime,
	@IsFuture bit
AS

BEGIN


	SELECT
		ro.CustomerID,
		CASE WHEN ro.Code = '*ALL*' THEN NULL ELSE ro.Code END Code,
		ro.IncludeSubCodes,
		CASE WHEN ro.Code != '*ALL*' THEN NULL ELSE ro.OurZoneID END ZoneID,
		ISNULL(ro.RouteOptions, ro.BlockedSuppliers) Options,
		CASE WHEN ro.RouteOptions IS NULL THEN 'Y' ELSE 'N' END IsBlock,
		ro.BeginEffectiveDate,
		ro.EndEffectiveDate,
		ro.IsEffective,
		ro.ExcludedCodes,
		ro.Reason
	FROM
		RouteOverride ro 
	    Left JOIN @ZoneIds z ON ro.OurZoneID = z.ID  
	    WHERE ro.RouteOptions != 'BLK' OR ro.RouteOptions IS NULL--AND ro.RouteOptions IS NOT NULL
	    AND(z.ID IS NOT NULL OR ro.Code LIKE @CodePrefix + '%' OR @CodePrefix LIKE ro.Code + '%')
		AND 
			(
				(@IsFuture = 0 AND ro.BeginEffectiveDate <= @EffectiveTime AND (ro.EndEffectiveDate IS NULL OR ro.EndEffectiveDate > @EffectiveTime))
				OR
				(@IsFuture = 1 AND (ro.BeginEffectiveDate > GETDATE() OR ro.EndEffectiveDate IS NULL))
			)

END