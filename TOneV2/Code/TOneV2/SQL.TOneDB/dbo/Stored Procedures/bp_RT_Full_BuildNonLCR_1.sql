

CREATE PROCEDURE [dbo].[bp_RT_Full_BuildNonLCR]

WITH RECOMPILE
AS
BEGIN

DROP INDEX [IX_RouteOptionNonLCR_multikey] ON [RouteOption_NonLCR]
DROP INDEX [IX_RouteNonLCR_multikeys] ON [Route_NonLCR]
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
--	   --,GETDATE() Updated
--	   ,zr.IsTOD
--	   ,0 IsSpecialRequestAffected
--	   ,0 IsOverrideAffected
--	   ,1 IsBlockAffected
--	   ,0 IsOptionBlock
--	   ,0 RuleID
-- FROM ZoneRates zr  WITH(NOLOCK)
--LEFT JOIN RoutePool rcp ON zr.ZoneID = rcp.ZoneID
--WHERE zr.SupplierID = 'SYS' AND zr.IsBlock = 1 

--),

------------------- Case Not Exists Needs Enhancment ---------------------------
----- BLK Supplier Zone( Zone Rate) ===> SupplierZone Code(Code Supply) ===> Matched Sale Zone (Route Code Pool) 
----- ===> Customer Buy Sale Zone (Zone Rate) ===> Add Route as option Block for each customer (Route_Block)===> Add Option Block (RouteOption_Block)


----BlockedOnCostZoneRate AS 
----(
----SELECT zr.CustomerID
----	   ,zr.ProfileId
----	   ,rcp.Code
----	   ,zr.ZoneID
----	   ,zr.ActiveRate
----	   ,zr.NormalRate
----	   ,zr.OffPeakRate
----	   ,zr.WeekEndRate
----	   ,zr.ServicesFlag
----	   ,0 [State]
----	   ,GETDATE() Updated
----	   ,zr.IsTOD
----	   ,0 IsSpecialRequestAffected
----	   ,0 IsOverrideAffected
----	   ,1 IsBlockAffected
----	   ,0 IsOptionBlock
---- FROM ZoneRate zr
----LEFT JOIN RoutePool rcp ON zr.ZoneID = rcp.ZoneID
----WHERE zr.CustomerID = 'SYS' AND zr.IsBlock = 1 

----),
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
--	--GETDATE() Updated,
--	zr.IsTOD,
--	0 IsSpecialRequestAffected,
--	0 IsOverrideAffected,
--	1 IsBlockAffected,
--	0 IsOptionBlock,
--	0 RuleID


-- FROM RoutePool rcp	 WITH(NOLOCK)
--LEFT JOIN ZoneRates zr WITH(NOLOCK) ON rcp.ZoneID = zr.ZoneID
--WHERE zr.SupplierID = 'SYS' and rcp.IsBlocked = 1 

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

--SELECT * FROM Route_Block rb WITH(NOLOCK)	
--WHERE ab.ZoneID = rb.OurZoneID AND ab.Code = rb.Code AND rb.CustomerID = ab.CustomerID
--)

--)

--INSERT INTO Route_Block With(Tablock)
--SELECT * FROM AllBlockesExcludingRouteBlock

-------add block route if required---

INSERT INTO Route_NonLCR With(Tablock)
SELECT  rb.CustomerID, rb.ProfileID, rb.Code, rb.OurZoneID,
       rb.OurActiveRate,
       rb.OurServicesFlag, rb.[State], rb.IsToDAffected,
       rb.IsSpecialRequestAffected, rb.IsOverrideAffected, rb.IsBlockAffected,
       rb.IsOptionBlock,
       rb.RuleID
  FROM Route_Block rb	WITH(NOLOCK)
WHERE rb.IsBlockAffected = 1

------------ Add Override Routes --------------
INSERT INTO Route_NonLCR With(Tablock)
SELECT  ov.CustomerID, ov.ProfileID, ov.Code, ov.OurZoneID,
       ov.OurActiveRate, 
       ov.OurServicesFlag, ov.[State],  ov.IsToDAffected,
       ov.IsSpecialRequestAffected, ov.IsOverrideAffected, ov.IsBlockAffected,
       isnull(rb.IsOptionBlock,0) IsOptionBlock,
       ov.RuleID
  FROM Route_Override ov WITH(NOLOCK)
  LEFT JOIN Route_Block rb WITH(NOLOCK) ON rb.CustomerID = ov.CustomerID AND rb.Code = ov.Code AND rb.OurZoneID = ov.OurZoneID 
  WHERE rb.IsOptionBlock = 1 or rb.IsOptionBlock IS NULL  

--- L2
----Add Special Request

INSERT INTO Route_NonLCR With(Tablock)
SELECT rs.CustomerID, rs.ProfileID, rs.Code, rs.OurZoneID,
       rs.OurActiveRate, 
       rs.OurServicesFlag, rs.[State], rs.IsToDAffected,
       rs.IsSpecialRequestAffected,
       rs.IsOverrideAffected ,
       rs.IsBlockAffected,
       isnull(rb.IsOptionBlock,0) IsOptionBlock,
       rs.RuleID
   FROM Route_Special rs WITH(NOLOCK)
LEFT JOIN Route_Block rb WITH(NOLOCK) ON rb.CustomerID = rs.CustomerID AND rb.Code = rs.Code AND rb.OurZoneID = rs.OurZoneID
LEFT JOIN Route_Override rov WITH(NOLOCK)ON rs.CustomerID = rov.CustomerID AND rs.Code = rov.Code AND rs.OurZoneID = rov.OurZoneID	
WHERE (rb.IsOptionBlock = 1 or rb.IsOptionBlock IS NULL) AND (rov.CustomerID IS NULL AND rov.Code IS NULL AND rov.OurZoneID IS NULL) 


INSERT INTO Route_NonLCR With(Tablock)
SELECT rb.CustomerID, rb.ProfileID, rb.Code, rb.OurZoneID,
       rb.OurActiveRate, 
       rb.OurServicesFlag, rb.[State], rb.IsToDAffected,
       rb.IsSpecialRequestAffected,
       rb.IsOverrideAffected ,
       rb.IsBlockAffected,
       rb.IsOptionBlock,
       rb.RuleID
   FROM Route_Block rb WITH(NOLOCK)
LEFT JOIN Route_Override rov WITH(NOLOCK) ON rb.CustomerID = rov.CustomerID AND rb.Code = rov.Code AND rb.OurZoneID = rov.OurZoneID	
LEFT JOIN Route_Special ros WITH(NOLOCK) ON rb.CustomerID = ros.CustomerID AND rb.Code = ros.Code AND rb.OurZoneID = ros.OurZoneID	

WHERE (rb.IsOptionBlock = 1 )  AND (rov.CustomerID IS NULL AND rov.Code IS NULL AND rov.OurZoneID IS NULL) AND (ros.CustomerID IS NULL AND ros.Code IS NULL AND ros.OurZoneID IS NULL)
 --AND rb.CustomerID = 'c001' AND rb.Code LIKE '880%'


CREATE NONCLUSTERED INDEX [IX_RouteNonLCR_multikeys] ON [dbo].[Route_NonLCR](	[CustomerID] ASC,	[Code] ASC,	[OurZoneID] ASC)
INCLUDE ( [IsToDAffected],[IsSpecialRequestAffected],[IsOverrideAffected],[IsBlockAffected],[IsOptionBlock]) 
WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]



------------ RouteOption_NonLCR ----------------


---- ADD Block Options ------

INSERT INTO RouteOption_NonLCR With(Tablock)
SELECT 
rob.CustomerID, rob.SupplierID, rob.Code, rob.SupplierZoneID,
rob.SupplierActiveRate, rob.SupplierServicesFlag, rob.Priority, rob.NumberOfTries,
rob.[State], rob.Percentage, rob.ActionType
 FROM RouteOption_Block rob WITH(NOLOCK)


----Add Override Options ------
INSERT INTO RouteOption_NonLCR With(Tablock)
SELECT 
roo.CustomerID, roo.SupplierID, roo.Code, roo.SupplierZoneID,
roo.SupplierActiveRate,  roo.SupplierServicesFlag, roo.Priority, roo.NumberOfTries,
roo.[State], roo.Percentage,roo.ActionType
 FROM RouteOption_Override roo WITH(NOLOCK)
LEFT JOIN RouteOption_Block rob WITH(NOLOCK) ON rob.CustomerID = roo.CustomerID  AND rob.Code = roo.Code AND rob.SupplierID = roo.SupplierID --AND rob.SupplierZoneID = roo.SupplierZoneID	
WHERE rob.CustomerID IS NULL AND rob.Code IS NULL AND rob.SupplierID IS NULL --AND rob.SupplierZoneID IS NULL 


---- Add Special Request Options----
INSERT INTO RouteOption_NonLCR With(Tablock)

	SELECT ros.CustomerID, ros.SupplierID, ros.Code, ros.SupplierZoneID,
	       ros.SupplierActiveRate,  ros.SupplierServicesFlag, ros.Priority,
	       ros.NumberOfTries, 
	       ros.State ,
	       	        ros.Percentage,ros.ActionType
	  FROM RouteOption_Special ros WITH(NOLOCK)
LEFT JOIN RouteOption_Override ovo WITH(NOLOCK) ON  ovo.CustomerID = ros.CustomerID AND ovo.Code = ros.Code --AND ovo.SupplierZoneID = ros.SupplierZoneID AND ovo.SupplierID = ros.SupplierID 
LEFT JOIN RouteOption_Block rob WITH(NOLOCK) ON rob.CustomerID = ros.CustomerID AND rob.Code = ros.Code AND rob.SupplierID = ros.SupplierID--AND  rob.SupplierZoneID = ros.SupplierZoneID	
WHERE (ovo.CustomerID IS NULL AND ovo.Code IS NULL AND ovo.SupplierID IS NULL) AND (rob.CustomerID IS NULL  AND rob.Code IS NULL AND rob.SupplierID IS NULL )


CREATE NONCLUSTERED INDEX [IX_RouteOptionNonLCR_multikey] ON [dbo].[RouteOption_NonLCR] ([CustomerID] ASC,[SupplierID] ASC,[Code] ASC)
INCLUDE ( [Priority],[NumberOfTries],[State],[Percentage],[ActionType]) 
WITH (STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]



END