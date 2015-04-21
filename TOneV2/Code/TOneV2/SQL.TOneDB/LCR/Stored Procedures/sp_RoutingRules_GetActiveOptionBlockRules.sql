
CREATE PROCEDURE  [LCR].[sp_RoutingRules_GetActiveOptionBlockRules] 
	@ZoneIds LCR.IntIDType READONLY,
	@Codes LCR.CodeType READONLY,
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
	    LEFT JOIN @Codes c ON c.code = rb.code  
	    WHERE z.ID IS NOT NULL OR (rb.Code = c.code) OR(c.IncludeSubCodes = 1 AND rb.Code LIKE c.code + '%')
		AND 
			(
				(@IsFuture = 0 AND rb.BeginEffectiveDate <= @EffectiveTime AND (rb.EndEffectiveDate IS NULL OR rb.EndEffectiveDate > @EffectiveTime))
				OR
				(@IsFuture = 1 AND (rb.BeginEffectiveDate > GETDATE() OR rb.EndEffectiveDate IS NULL))
			)

END