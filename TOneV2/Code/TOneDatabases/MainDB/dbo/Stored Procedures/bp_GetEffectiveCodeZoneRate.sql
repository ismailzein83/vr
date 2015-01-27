CREATE PROCEDURE bp_GetEffectiveCodeZoneRate(@SupplierID varchar(10), @CDPN varchar(25), @Attempt datetime)
AS
BEGIN

	SELECT 
			c.ID CodeID, 
			C.Code, 
			c.BeginEffectiveDate AS CodeBED, 
			c.EndEffectiveDate AS CodeEED,
			z.ZoneID,
			z.CodeGroup,
			z.Name,
			z.ServicesFlag AS ZoneServicesFlag,
			z.BeginEffectiveDate AS ZoneBED, 
			z.EndEffectiveDate AS ZoneEED,
			r.RateID,
			r.PriceListID,
			r.ServicesFlag AS RateServicesFlag,
			r.Rate,
			r.BeginEffectiveDate AS RateBED, 
			r.EndEffectiveDate AS RateEED			
	  FROM Code c, Zone z, Rate r WHERE c.ZoneID = z.ZoneID AND r.ZoneID = c.ZoneID 
	AND z.SupplierID = @SupplierID
	AND @CDPN LIKE c.Code + '%'  
	AND c.BeginEffectiveDate <= @Attempt
	AND ISNULL(c.EndEffectiveDate, '2025-01-01') >= @Attempt

	AND z.BeginEffectiveDate <= @Attempt
	AND ISNULL(z.EndEffectiveDate, '2025-01-01') >= @Attempt

	AND r.BeginEffectiveDate <= @Attempt
	AND ISNULL(r.EndEffectiveDate, '2025-01-01') >= @Attempt
	
END