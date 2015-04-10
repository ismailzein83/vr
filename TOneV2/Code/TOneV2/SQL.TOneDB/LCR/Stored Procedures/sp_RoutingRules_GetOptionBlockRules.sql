
CREATE PROCEDURE  [LCR].[sp_RoutingRules_GetOptionBlockRules] 
	@ZoneIds LCR.IntIDType READONLY,
	@CodePrefix VARCHAR(5),
	@EffectiveTime datetime,
	@IsFuture bit
AS

BEGIN


	SELECT
		rb.CustomerID,
		rb.Code,
		rb.IncludeSubCodes,
		rb.ZoneID,
		rb.SupplierID,
		rb.BeginEffectiveDate,
		rb.EndEffectiveDate,
		rb.IsEffective,
		rb.ExcludedCodes,
		rb.Reason
	FROM
		RouteBlock rb 
	    Left JOIN @ZoneIds z ON rb.ZoneID = z.ID  
	    WHERE (z.ID IS NOT NULL OR rb.Code LIKE @CodePrefix + '%' OR @CodePrefix LIKE rb.Code + '%')
		AND 
			(
				(@IsFuture = 0 AND rb.BeginEffectiveDate <= @EffectiveTime AND (rb.EndEffectiveDate IS NULL OR rb.EndEffectiveDate > @EffectiveTime))
				OR
				(@IsFuture = 1 AND (rb.BeginEffectiveDate > GETDATE() OR rb.EndEffectiveDate IS NULL))
			)

END