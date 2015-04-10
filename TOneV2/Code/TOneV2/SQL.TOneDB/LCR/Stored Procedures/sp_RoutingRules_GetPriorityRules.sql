

CREATE PROCEDURE [LCR].[sp_RoutingRules_GetPriorityRules] 
	@ZoneIds LCR.IntIDType READONLY,
	@CodePrefix VARCHAR(5),
	@EffectiveTime datetime,
	@IsFuture bit
AS

BEGIN
	SELECT
		p.CustomerID,
		p.Code,
		p.IncludeSubCodes,
		p.ZoneID,
		p.SupplierID,
		p.SpecialRequestType [TYPE],
		p.Priority,
		p.Percentage,
		p.BeginEffectiveDate,
		p.EndEffectiveDate,
		p.IsEffective,
		p.ExcludedCodes,
		p.Reason
	FROM
		SpecialRequest p 
	    Left JOIN @ZoneIds z ON p.ZoneID = z.ID  
	    WHERE (z.ID IS NOT NULL OR p.Code LIKE @CodePrefix + '%' OR @CodePrefix LIKE p.Code + '%')
		AND 
			(
				(@IsFuture = 0 AND p.BeginEffectiveDate <= @EffectiveTime AND (p.EndEffectiveDate IS NULL OR p.EndEffectiveDate > @EffectiveTime))
				OR
				(@IsFuture = 1 AND (p.BeginEffectiveDate > GETDATE() OR p.EndEffectiveDate IS NULL))
			)
			
	ORDER BY p.CustomerID, p.Code

END