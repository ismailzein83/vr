
CREATE PROCEDURE [LCR].[sp_RoutingRules_GetDifferentialRuleCodes] 
	@LastRun datetime
AS
BEGIN
;WITH 
		_CodeForZones_Override AS 
		(
			SELECT
				ro.CustomerID,
				c.Code
			FROM
				RouteOverride ro 
				LEFT JOIN Code c ON c.ZoneID = ro.OurZoneID 
		WHERE (ro.OurZoneID IS NOT NULL) 
		AND ro.Code = '*ALL*'

		AND 
		 c.IsEffective = 'Y'
		 AND ((ro.EndEffectiveDate > @LastRun and ro.EndEffectiveDate <= GETDATE()) OR (ro.BeginEffectiveDate > @LastRun and ro.BeginEffectiveDate <= GETDATE())) 
		)
		,
		_Codes_Override AS 
		(
		SELECT
			 DISTINCT	ro.CustomerID,
				c.Code
			FROM
				RouteOverride ro 
				LEFT JOIN Code c ON c.Code LIKE ro.Code + '%'
		WHERE (ro.Code != '*ALL*') 
		AND 
		c.IsEffective = 'Y'
		AND ((ro.EndEffectiveDate > @LastRun and ro.EndEffectiveDate <= GETDATE()) OR (ro.BeginEffectiveDate > @LastRun and ro.BeginEffectiveDate <= GETDATE())) 
		 AND  (ro.Code = c.Code AND ro.IncludeSubCodes = 'N')
		 OR
		 (c.Code LIKE ro.Code + '%' AND ro.IncludeSubCodes = 'Y')
		 
		),
		_CodeForZones_Block AS 
		(
			SELECT
			DISTINCT	rb.CustomerID,
				c.Code
			FROM
				RouteBlock rb 
				LEFT JOIN Code c ON c.ZoneID = rb.ZoneID 
		WHERE (rb.ZoneID IS NOT NULL) 
		AND 
		 c.IsEffective = 'Y'
		 AND ((rb.EndEffectiveDate > @LastRun and rb.EndEffectiveDate <= GETDATE()) OR (rb.BeginEffectiveDate > @LastRun and rb.BeginEffectiveDate <= GETDATE())) 
		)

		,
		_Codes_Block AS 
		(
		SELECT
			 DISTINCT	rb.CustomerID,
				c.Code
			FROM
				RouteBlock rb 
				LEFT JOIN Code c ON c.Code like rb.Code + '%'
		WHERE (rb.Code IS NOT NULL) 
		AND 
		c.IsEffective = 'Y'
		 AND  (rb.Code = c.Code AND rb.IncludeSubCodes = 'N')
		 OR
		 (c.Code LIKE rb.Code + '%' AND rb.IncludeSubCodes = 'Y')
		 AND ((rb.EndEffectiveDate > @LastRun and rb.EndEffectiveDate <= GETDATE()) OR (rb.BeginEffectiveDate > @LastRun and rb.BeginEffectiveDate <= GETDATE())) 
		)
		,
		_CodeForZones_SpecialRequest AS 
		(
			SELECT
			DISTINCT	sp.CustomerID,
				c.Code
			FROM
				SpecialRequest sp 
				LEFT JOIN Code c ON c.ZoneID = sp.ZoneID 
		WHERE (sp.ZoneID IS NOT NULL) 
		AND 
		 c.IsEffective = 'Y'
		 AND ((sp.EndEffectiveDate > @LastRun and sp.EndEffectiveDate <= GETDATE()) OR (sp.BeginEffectiveDate > @LastRun and sp.BeginEffectiveDate <= GETDATE())) 
		)

		,
		_Codes_SpecialRequest AS 
		(
		SELECT
			 DISTINCT	sp.CustomerID,
				c.Code
			FROM
				SpecialRequest sp 
				LEFT JOIN Code c ON c.Code like sp.Code + '%'
		WHERE (sp.Code IS NOT NULL) 
		AND 
		c.IsEffective = 'Y'
		 AND  (sp.Code = c.Code AND sp.IncludeSubCodes = 'N')
		 OR
		 (c.Code LIKE sp.Code + '%' AND sp.IncludeSubCodes = 'Y')
		 AND ((sp.EndEffectiveDate > @LastRun and sp.EndEffectiveDate <= GETDATE()) OR (sp.BeginEffectiveDate > @LastRun and sp.BeginEffectiveDate <= GETDATE()))
		),

		_AllCodes AS (
		SELECT * FROM _CodeForZones_Block
		UNION 
		SELECT * FROM _Codes_Block	
		UNION
		SELECT * FROM _CodeForZones_Override
		UNION 
		SELECT * FROM _Codes_Override	
		UNION
		SELECT * FROM _CodeForZones_SpecialRequest	
		UNION 
		SELECT * FROM _Codes_SpecialRequest	
		)

SELECT * FROM _AllCodes
ORDER BY code,customerid

END