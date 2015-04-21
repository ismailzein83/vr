

Create PROCEDURE [LCR].[sp_RoutingRules_GetDifferentialPriorityRules] 
	@LastRun datetime
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
	    WHERE ((p.EndEffectiveDate > @LastRun and p.EndEffectiveDate <= GETDATE()) OR (p.BeginEffectiveDate > @LastRun and p.BeginEffectiveDate <= GETDATE())) 

			
	ORDER BY p.CustomerID, p.Code

END