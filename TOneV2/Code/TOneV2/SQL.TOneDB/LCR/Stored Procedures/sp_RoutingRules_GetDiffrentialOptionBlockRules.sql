
Create PROCEDURE  [LCR].[sp_RoutingRules_GetDiffrentialOptionBlockRules] 
	@LastRun datetime
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
	    WHERE ((rb.EndEffectiveDate > @LastRun and rb.EndEffectiveDate <= GETDATE()) OR (rb.BeginEffectiveDate > @LastRun and rb.BeginEffectiveDate <= GETDATE()))

END