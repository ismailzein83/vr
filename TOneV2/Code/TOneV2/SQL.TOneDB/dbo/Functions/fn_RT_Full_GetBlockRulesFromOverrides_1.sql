

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[fn_RT_Full_GetBlockRulesFromOverrides]
(	
	-- Add the parameters for the function here
@Level VARCHAR(20),
@ZoneId INT,
@CodeGroup VARCHAR(250),
@Type VARCHAR(10)
)
RETURNS @BlockRules TABLE 
(
	CustomerID VARCHAR(5),
	SupplierID VARCHAR(5),
	Code VARCHAR(20),
	ZoneID INT,
	UpdateDate SMALLDATETIME,
	IncludeSubCodes CHAR(1),
	ExcludedCodes VARCHAR(250)
	)
AS

BEGIN

IF( @Type = 'Route')
 BEGIN
 	
	
	----- CodeOnly
	IF(@Level = 'Code')
	BEGIN
		INSERT INTO @BlockRules
		SELECT 	
				ro.CustomerID,
				ro.RouteOptions,
				ro.Code,
				ro.OurZoneID,
				ro.Updated,
				ro.IncludeSubCodes,
				ro.ExcludedCodes
				
				
		FROM	RouteOverride ro
			
		WHERE	ro.Code <> '*all*'
			    AND ro.IsEffective = 'Y'
			    AND (ro.RouteOptions = 'BLK')
			    AND ro.OurZoneID = -1
			    --AND ro.BlockedSuppliers IS NULL
			    --AND ro.Code NOT IN (SELECT cg.Code FROM CodeGroup cg)				
	END
	ELSE 
		-----Zones
		IF(@Level = 'Zones')
		BEGIN
			INSERT INTO @BlockRules
			SELECT 	
					ro.CustomerID,
					ro.RouteOptions,
					ro.Code,
					ro.OurZoneID,
					ro.Updated,
					ro.IncludeSubCodes,
					ro.ExcludedCodes								
			FROM	RouteOverride ro
			WHERE   ro.OurZoneID <> -1
				    AND ro.IsEffective = 'Y'
				    AND (ro.RouteOptions = 'BLK')-- OR ro.BlockedSuppliers IS NOT NULL)	
		END		
	ELSE 
		-----Code Groups
		IF(@Level = 'CodeGroup')
		BEGIN
			INSERT INTO @BlockRules
			SELECT 	
					ro.CustomerID,
					ro.RouteOptions,
					ro.Code,
					ro.OurZoneID,
					ro.Updated,
					ro.IncludeSubCodes,
					ro.ExcludedCodes
								
								
			FROM	RouteOverride ro
			WHERE	ro.Code <> '*all*'
					AND ro.IsEffective = 'Y'
					AND (ro.RouteOptions = 'BLK')-- OR ro.BlockedSuppliers IS NOT NULL)
					AND ro.Code IN (SELECT cg.Code FROM CodeGroup cg)	
		END	
	
END

ELSE
	
		--IF(@Level = 'Code')
				--BEGIN
					
		WITH
		OptionsFromRouteBlock AS (
		select
				rb.CustomerID,
				rb.SupplierID,
				rb.Code,
				rb.ZoneID,
				rb.UpdateDate,
				rb.IncludeSubCodes,
				rb.ExcludedCodes
				
		FROM	RouteBlock rb WITH (NOLOCK)
		WHERE	rb.IsEffective = 'Y' 
				--AND rb.Code IS NOT NULL 
				And RB.CustomerID IS NOT NULL )
		,
		OptionsFromRouteOverride AS (
    	 SELECT 
				 A.customerid,
				 Split.a.value('.', 'VARCHAR(5)') AS SupplierID, 
				 CASE WHEN A.Code = '*ALL*' THEN NULL ELSE  A.Code END Code, 
				 A.OurZoneID,
				 A.UpdateDate,
				 A.IncludeSubCodes,
				 A.ExcludedCodes
				 
		FROM  
				(SELECT
						ro.CustomerID, 
						CAST ('<M>' + REPLACE(ro.BlockedSuppliers, '|', '</M><M>') + '</M>' AS XML) AS SupplierID,
						ro.Code, 
						ro.OurZoneID,
						ro.Updated AS UpdateDate,
						ro.IncludeSubCodes,
						ro.ExcludedCodes								
				FROM	routeoverride ro
				
				WHERE	ro.IsEffective = 'Y' AND ro.BlockedSuppliers IS NOT NULL) AS A 					
						CROSS APPLY SupplierID.nodes ('/M') AS Split(a)	
					
		)
		,
		FinalOptions_Zones AS 
		(
		 SELECT DISTINCT
		        A.customerid,
				A.SupplierID, 
				rp.Code , 
				 -1 ZoneID,
				 A.UpdateDate,
				 A.IncludeSubCodes,
				 A.ExcludedCodes
		  FROM OptionsFromRouteOverride A
		 JOIN ZoneMatch zm ON zm.OurZoneID = A.OurZoneID 
		 JOIN RoutePool rp ON rp.ZoneID = A.OurZoneID
	     JOIN Zone z ON zm.SupplierZoneID = z.ZoneID AND   A.SupplierID = z.SupplierID	
	     AND z.IsEffective = 'Y' AND A.OurZoneID <> -1
		)	
	,
		FinalOptions_Codes AS 
		(
		 SELECT DISTINCT
		        A.customerid,
				A.SupplierID, 
				A.Code , 
				 -1 ZoneID,
				 A.UpdateDate,
				 A.IncludeSubCodes,
				 A.ExcludedCodes
		  FROM OptionsFromRouteOverride A
		 WHERE A.OurZoneID = -1
	    
		)
			
			INSERT INTO @BlockRules
			SELECT * FROM OptionsFromRouteBlock
			UNION ALL SELECT * FROM FinalOptions_Zones
			UNION ALL SELECT * FROM FinalOptions_Codes		
				--END
	RETURN
END