

CREATE PROCEDURE [dbo].[bp_RT_Part_BuildNonLCR]

WITH RECOMPILE
AS
BEGIN

--;
--WITH 
--BlockedOnSaleZoneRate AS 
--(
--SELECT zr.CustomerID
--	   ,zr.ProfileId
--	   ,rcp.Code
--	   ,zr.ZoneID
--	   ,zr.ActiveRate
--	   ,zr.ServicesFlag
--	   ,0 [State]
--	   ,zr.IsTOD
--	   ,0 IsSpecialRequestAffected
--	   ,0 IsOverrideAffected
--	   ,1 IsBlockAffected
--	   ,0 IsOptionBlock
--	   ,0 RuleID
-- FROM #SaleZoneRates zr  WITH(NOLOCK)
--LEFT JOIN #RoutePool rcp ON zr.ZoneID = rcp.ZoneID
--WHERE zr.IsBlock = 1 

--),

--BlockedOnRoutePool AS 
--(

--SELECT 
--	zr.CustomerID,
--	zr.ProfileId, 
--	rcp.Code, 
--	rcp.ZoneID,
--	zr.ActiveRate,
--	zr.ServicesFlag,
--	0 [State],
--	zr.IsTOD,
--	0 IsSpecialRequestAffected,
--	0 IsOverrideAffected,
--	1 IsBlockAffected,
--	0 IsOptionBlock,
--	0 RuleID


-- FROM #RoutePool rcp	 WITH(NOLOCK)
--LEFT JOIN #SaleZoneRates zr WITH(NOLOCK) ON rcp.ZoneID = zr.ZoneID
--WHERE rcp.IsBlocked = 1 

--),
---- Duplication must be noticed
--AllBlocks AS 
--(
--SELECT * FROM 	BlockedOnSaleZoneRate
----WHERE Code IS NOT NULL
--UNION ALL 
--SELECT * FROM BlockedOnRoutePool

--)
----SELECT * FROM AllBlocks
--,
---- Duplication must be solved using Left Join
--AllBlockesExcludingRouteBlock AS (
	
--SELECT * FROM AllBlocks ab WITH(NOLOCK)
--WHERE NOT EXISTS (

--SELECT * FROM #Route_Block rb WITH(NOLOCK)	
--WHERE ab.ZoneID = rb.OurZoneID AND ab.Code = rb.Code AND rb.CustomerID = ab.CustomerID
--)

--)

--INSERT INTO #Route_Block With(Tablock)
--SELECT * FROM AllBlockesExcludingRouteBlock


-------add block route if required---

INSERT INTO #Route_NonLCR With(Tablock)
SELECT  rb.CustomerID, rb.ProfileID, rb.Code, rb.OurZoneID,
       rb.OurActiveRate,
       rb.OurServicesFlag, rb.[State], rb.IsToDAffected,
       rb.IsSpecialRequestAffected, rb.IsOverrideAffected, rb.IsBlockAffected,
       rb.IsOptionBlock,
       rb.RuleID
  FROM #Route_Block rb	WITH(NOLOCK)
WHERE rb.IsBlockAffected = 1

------------ Add Override Routes --------------
INSERT INTO #Route_NonLCR With(Tablock)
SELECT  ov.CustomerID, ov.ProfileID, ov.Code, ov.OurZoneID,
       ov.OurActiveRate, 
       ov.OurServicesFlag, ov.[State],  ov.IsToDAffected,
       ov.IsSpecialRequestAffected, ov.IsOverrideAffected, ov.IsBlockAffected,
       isnull(rb.IsOptionBlock,0) IsOptionBlock,
       ov.RuleID
  FROM #Route_Override ov WITH(NOLOCK)
  LEFT JOIN #Route_Block rb WITH(NOLOCK) ON rb.CustomerID COLLATE DATABASE_DEFAULT = ov.CustomerID AND rb.Code COLLATE DATABASE_DEFAULT = ov.Code AND rb.OurZoneID = ov.OurZoneID 
  WHERE rb.IsOptionBlock = 1 or rb.IsOptionBlock IS NULL  

--- L2
----Add Special Request

INSERT INTO #Route_NonLCR With(Tablock)
SELECT rs.CustomerID, rs.ProfileID, rs.Code, rs.OurZoneID,
       rs.OurActiveRate, 
       rs.OurServicesFlag, rs.[State], rs.IsToDAffected,
       rs.IsSpecialRequestAffected,
       rs.IsOverrideAffected ,
       rs.IsBlockAffected,
       isnull(rb.IsOptionBlock,0) IsOptionBlock,
       rs.RuleID
   FROM #Route_Special rs WITH(NOLOCK)
LEFT JOIN #Route_Block rb WITH(NOLOCK) ON rb.CustomerID COLLATE DATABASE_DEFAULT = rs.CustomerID AND rb.Code COLLATE DATABASE_DEFAULT = rs.Code AND rb.OurZoneID = rs.OurZoneID
LEFT JOIN #Route_Override rov WITH(NOLOCK)ON rs.CustomerID COLLATE DATABASE_DEFAULT = rov.CustomerID AND rs.Code COLLATE DATABASE_DEFAULT = rov.Code AND rs.OurZoneID = rov.OurZoneID	
WHERE (rb.IsOptionBlock = 1 or rb.IsOptionBlock IS NULL) AND (rov.CustomerID IS NULL AND rov.Code IS NULL AND rov.OurZoneID IS NULL) 


INSERT INTO #Route_NonLCR With(Tablock)
SELECT rb.CustomerID, rb.ProfileID, rb.Code, rb.OurZoneID,
       rb.OurActiveRate, 
       rb.OurServicesFlag, rb.[State], rb.IsToDAffected,
       rb.IsSpecialRequestAffected,
       rb.IsOverrideAffected ,
       rb.IsBlockAffected,
       rb.IsOptionBlock,
       rb.RuleID
   FROM #Route_Block rb WITH(NOLOCK)
LEFT JOIN #Route_Override rov WITH(NOLOCK) ON rb.CustomerID COLLATE DATABASE_DEFAULT = rov.CustomerID AND rb.Code COLLATE DATABASE_DEFAULT= rov.Code AND rb.OurZoneID = rov.OurZoneID	
LEFT JOIN #Route_Special ros WITH(NOLOCK) ON rb.CustomerID COLLATE DATABASE_DEFAULT = ros.CustomerID AND rb.Code COLLATE DATABASE_DEFAULT = ros.Code AND rb.OurZoneID = ros.OurZoneID	

WHERE (rb.IsOptionBlock = 1 )  AND (rov.CustomerID IS NULL AND rov.Code IS NULL AND rov.OurZoneID IS NULL) AND (ros.CustomerID IS NULL AND ros.Code IS NULL AND ros.OurZoneID IS NULL)

------------ RouteOption_NonLCR ----------------


---- ADD Block Options ------

INSERT INTO #RouteOption_NonLCR With(Tablock)
SELECT 
rob.CustomerID, rob.SupplierID, rob.Code, rob.SupplierZoneID,
rob.SupplierActiveRate, rob.SupplierServicesFlag, rob.Priority, rob.NumberOfTries,
rob.[State], rob.Percentage, rob.ActionType
 FROM #RouteOption_Block rob WITH(NOLOCK)


----Add Override Options ------
INSERT INTO #RouteOption_NonLCR With(Tablock)
SELECT 
roo.CustomerID, roo.SupplierID, roo.Code, roo.SupplierZoneID,
roo.SupplierActiveRate,  roo.SupplierServicesFlag, roo.Priority, roo.NumberOfTries,
roo.[State], roo.Percentage,roo.ActionType
 FROM #RouteOption_Override roo WITH(NOLOCK)
LEFT JOIN #RouteOption_Block rob WITH(NOLOCK) ON rob.CustomerID COLLATE DATABASE_DEFAULT = roo.CustomerID AND rob.Code COLLATE DATABASE_DEFAULT = roo.Code AND rob.SupplierID COLLATE DATABASE_DEFAULT = roo.SupplierID	
WHERE rob.CustomerID IS NULL AND rob.Code IS NULL 

---- Add Special Request Options----
INSERT INTO #RouteOption_NonLCR With(Tablock)

	SELECT ros.CustomerID, ros.SupplierID, ros.Code, ros.SupplierZoneID,
	       ros.SupplierActiveRate,  ros.SupplierServicesFlag, ros.Priority,
	       ros.NumberOfTries, 
	       ros.State ,
	       	        ros.Percentage,ros.ActionType
	  FROM #RouteOption_Special ros WITH(NOLOCK)
LEFT JOIN #RouteOption_Override ovo WITH(NOLOCK) ON  ovo.CustomerID COLLATE DATABASE_DEFAULT = ros.CustomerID AND ovo.Code = ros.Code --AND ovo.SupplierID COLLATE DATABASE_DEFAULT = ros.SupplierID
LEFT JOIN #RouteOption_Block rob WITH(NOLOCK) ON rob.CustomerID COLLATE DATABASE_DEFAULT = ros.CustomerID AND rob.Code COLLATE DATABASE_DEFAULT = ros.Code  AND rob.SupplierID = ros.SupplierID--AND  rob.SupplierZoneID = ros.SupplierZoneID	
WHERE (ovo.CustomerID IS NULL AND ovo.Code IS NULL AND ovo.SupplierID IS NULL) AND (rob.CustomerID IS NULL  AND rob.Code IS NULL AND rob.SupplierID IS NULL )


--Write Back
DELETE rb FROM Route_NonLCR rb
INNER JOIN #Route_NonLCR rbn ON rb.CustomerID COLLATE DATABASE_DEFAULT = rbn.CustomerID AND rb.Code COLLATE DATABASE_DEFAULT = rbn.Code


INSERT INTO Route_NonLCR
SELECT rnl.CustomerID, rnl.ProfileID, rnl.Code, rnl.OurZoneID, rnl.OurActiveRate,
       rnl.OurServicesFlag, rnl.[State], rnl.IsToDAffected,
       rnl.IsSpecialRequestAffected, rnl.IsOverrideAffected, rnl.IsBlockAffected,
       rnl.IsOptionBlock, rnl.RuleID
  FROM #Route_NonLCR rnl
 
 DELETE rob FROM RouteOption_NonLCR rob
INNER JOIN #RouteOption_NonLCR robn ON robn.CustomerID COLLATE DATABASE_DEFAULT = rob.CustomerID AND robn.Code COLLATE DATABASE_DEFAULT = rob.Code
 
INSERT INTO RouteOption_NonLCR
SELECT * FROM #RouteOption_NonLCR ron
  


--Testing
SELECT 'NON_LCR_Routes', * FROM #Route_NonLCR rnl
SELECT 'NON_LCR_Options',* FROM #RouteOption_NonLCR ronl
END