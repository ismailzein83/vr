

CREATE PROCEDURE [LCR].[sp_RoutingRules_GetActivePriorityRules] 
	@ZoneIds LCR.IntIDType READONLY,
	@Codes LCR.CodeType READONLY,
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
        LEFT JOIN @Codes c ON c.code = p.code   
	    WHERE (z.ID IS NOT NULL) OR (p.Code = c.code) OR(c.IncludeSubCodes = 1 AND p.Code LIKE c.code + '%')
		AND 
			(
				(@IsFuture = 0 AND p.BeginEffectiveDate <= @EffectiveTime AND (p.EndEffectiveDate IS NULL OR p.EndEffectiveDate > @EffectiveTime))
				OR
				(@IsFuture = 1 AND (p.BeginEffectiveDate > GETDATE() OR p.EndEffectiveDate IS NULL))
			)
			
	ORDER BY p.CustomerID, p.Code

END