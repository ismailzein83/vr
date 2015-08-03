
CREATE PROCEDURE [dbo].[bp_RT_Part_BuildRouteAction_Override]

AS
BEGIN

	SET NOCOUNT ON;

	 
	
CREATE TABLE #RouteCodeTemp(
	[CustomerID] VARCHAR(5) NOT NULL,
	[ProfileID] [int] NULL,
	[Code] [VARCHAR] (20) NULL,
	[OurZoneID] [int] NULL,
	[OurActiveRate] [real] NULL,
	[OurServicesFlag] [smallint] NULL,
	[State] BIT NOT NULL,				
	[IsToDAffected] BIT NOT NULL,
	[IsSpecialRequestAffected] BIT NOT NULL,
	[IsOverrideAffected] BIT NOT NULL,
	[IsBlockAffected] BIT NOT NULL,
	[IsOptionBlock] BIT NULL,
	[RuleID] INT NOT NULL
	--[Updated] [datetime] NULL,
)

CREATE TABLE #RouteZoneTemp(
	[CustomerID] VARCHAR(5) NOT NULL,
	[ProfileID] [int] NULL,
	[Code] [VARCHAR] (20) NULL,
	[OurZoneID] [int] NULL,
	[OurActiveRate] [real] NULL,
	[OurServicesFlag] [smallint] NULL,
	[State] BIT NOT NULL,				
	[IsToDAffected] BIT NOT NULL,
	[IsSpecialRequestAffected] BIT NOT NULL,
	[IsOverrideAffected] BIT NOT NULL,
	[IsBlockAffected] BIT NOT NULL,
	[IsOptionBlock] BIT NULL,
	[RuleID] INT NOT NULL

)


CREATE TABLE #TempOverrideRules(
	RuleID INT,
	CustomerID VARCHAR(5),
	Code VARCHAR(30),
	OurZoneID INT,
	RouteOptions  VARCHAR(max),
	IncludeSubCodes CHAR(1),
	ExcludedCodes VARCHAR(max),
	ParentID INT,
	MainExcluded varchar(max),
	OriginalExcluded varchar(max),
	HasSubZone BIT,
	SubCodes VARCHAR(MAX),
	SubZoneIDs VARCHAR(MAX)
)
		
CREATE TABLE #TempOverrideRulesWithRates (
	RuleID INT IDENTITY(1,1),
	CustomerID VARCHAR(5),
	Code VARCHAR(30),
	OurZoneID INT,
	RouteOptions  VARCHAR(max),
	IncludeSubCodes CHAR(1),
	ExcludedCodes VARCHAR(max),
	ProfileId int ,
	OurActiveRate real ,
	OurServicesFlag smallint ,
	IsToDAffected tinyint
)
--DROP INDEX [IX_RouteOverride_mutikeys] ON [Route_Override]

DECLARE @Message varchar(500)
	DECLARE @MaxSuppliersPerRoute INT 
	SET @MaxSuppliersPerRoute = 8
exec  bp_RT_Part_PrepareOverrideRules

INSERT INTO #TempOverrideRulesWithRates With(Tablock)
SELECT 
		tor.CustomerID,tor.Code,tor.OurZoneID,tor.RouteOptions,tor.IncludeSubCodes,tor.ExcludedCodes
      ,zr.[ProfileID]
      ,zr.ActiveRate [OurActiveRate]
      ,zr.ServicesFlag [OurServicesFlag]
      ,zr.IsTOD [IsToDAffected]
      
      
 FROM #TempOverrideRules tor WITH(NOLOCK)
 
 INNER JOIN #SaleZoneRates zr WITH(NOLOCK) ON tor.OurZoneID = zr.ZoneID AND zr.CustomerID COLLATE DATABASE_DEFAULT = tor.CustomerID COLLATE DATABASE_DEFAULT
-- WHERE zr.IsBlock = 0	

   SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Override With Rates Created', @Message

;WITH RouteOverride_CodeWithoutSub AS 
(

	SELECT 

			tmp.[CustomerID]
           ,tmp.[ProfileID]
           ,rcp.[Code]
           , rcp.ZoneID [OurZoneID]
           ,tmp.[OurActiveRate]
           ,tmp.[OurServicesFlag]
           ,1 [State]
           ,tmp.[IsToDAffected]
            ,0 [IsSpecialRequestAffected]
			,CASE WHEN RouteOptions = 'BLK' THEN 0 ELSE 1 END [IsOverrideAffected]
            ,CASE WHEN RouteOptions = 'BLK' THEN 1 ELSE 0 END [IsBlockAffected]
           ,0 [IsOptionBlock]
			,tmp.RuleID 

 FROM #TempOverrideRulesWithRates tmp WITH(NOLOCK),#RoutePool rcp WITH(NOLOCK)

WHERE tmp.code IS NOT NULL 
AND 
	(tmp.IncludeSubCodes = 'N' AND tmp.code COLLATE DATABASE_DEFAULT = rcp.code) 
)
,
RouteOverride_CodeWithSub AS 
(
	SELECT 
			tmp.[CustomerID]
           ,tmp.[ProfileID]
           ,rcp.[Code]
           , rcp.ZoneID [OurZoneID]
           ,tmp.[OurActiveRate]
           ,tmp.[OurServicesFlag]
           ,1 [State]
           ,tmp.[IsToDAffected]
            ,0 [IsSpecialRequestAffected]
            ,CASE WHEN RouteOptions = 'BLK' THEN 0 ELSE 1 END [IsOverrideAffected]
           ,CASE WHEN RouteOptions = 'BLK' THEN 1 ELSE 0 END [IsBlockAffected]
           ,0 [IsOptionBlock]
			,tmp.RuleID 

 FROM #TempOverrideRulesWithRates tmp WITH(NOLOCK),#RoutePool rcp WITH(NOLOCK)

WHERE tmp.code IS NOT NULL 
AND 
	(tmp.IncludeSubCodes = 'Y' AND rcp.code COLLATE DATABASE_DEFAULT LIKE (tmp.code + '%')
	
	AND 1= ( CASE WHEN  PATINDEX('%,%',tmp.ExcludedCodes) > 0  AND 
		                   rcp.Code COLLATE DATABASE_DEFAULT NOT IN 
		                   (SELECT * FROM dbo.ParseArray(tmp.ExcludedCodes,','))
		                   THEN 1
		              WHEN  PATINDEX('%,%',tmp.ExcludedCodes) = 0 AND  
     			         rcp.Code COLLATE DATABASE_Default  NOT LIKE tmp.ExcludedCodes THEN 1
	
		                   ELSE 0 END 
		       )
	
	 )
)

,
AllCodeRules AS 
(
SELECT * FROM RouteOverride_CodeWithoutSub
UNION ALL 
SELECT * FROM RouteOverride_CodeWithSub
)
,
 RouteOverride_Zone AS 
(

	SELECT 
	tmp.[CustomerID]
           ,tmp.[ProfileID]
           ,rcp.[Code]
           , rcp.ZoneID [OurZoneID]
           ,tmp.[OurActiveRate]
           ,tmp.[OurServicesFlag]
           ,1 [State]
           ,tmp.[IsToDAffected]
            ,0 [IsSpecialRequestAffected]
       	    ,CASE WHEN RouteOptions = 'BLK' THEN 0 ELSE 1 END [IsOverrideAffected]
           ,CASE WHEN RouteOptions = 'BLK' THEN 1 ELSE 0 END [IsBlockAffected]
           ,0 [IsOptionBlock]
			,tmp.RuleID 

 FROM #TempOverrideRulesWithRates tmp WITH(NOLOCK)
 INNER JOIN #RoutePool rcp WITH(NOLOCK) ON rcp.ZoneID = tmp.OurZoneID
WHERE tmp.code IS NULL AND rcp.Code COLLATE DATABASE_DEFAULT NOT IN (SELECT * FROM dbo.ParseArray(tmp.Excludedcodes,',') pa)

)


,
ZoneRulesNotInCodRules AS 

(
	SELECT tmp.* FROM RouteOverride_Zone tmp
	 LEFT JOIN AllCodeRules rct WITH(NOLOCK) ON rct.CustomerID = tmp.CustomerID COLLATE DATABASE_DEFAULT AND rct.Code COLLATE DATABASE_DEFAULT = tmp.Code AND rct.OurZoneID = tmp.OurZoneID
	WHERE  rct.CustomerID IS NULL AND rct.Code IS NULL  AND rct.OurZoneID IS NULL


)

,
AllOverrideRules AS 
(
SELECT * FROM AllCodeRules
UNION ALL
SELECT * FROM ZoneRulesNotInCodRules
)

INSERT INTO #Route_Override With(Tablock)
SELECT rz.CustomerID, rz.ProfileID, rz.Code, rz.OurZoneID, rz.OurActiveRate,
       rz.OurServicesFlag, rz.[State], rz.IsToDAffected,
       rz.IsSpecialRequestAffected, rz.IsOverrideAffected, rz.IsBlockAffected,
       rz.IsOptionBlock, rz.RuleID
FROM AllOverrideRules rz

--CREATE NONCLUSTERED INDEX [IX_RouteOverride_mutikeys] ON [Route_Override] ([RuleID],[Code]) INCLUDE ([CustomerID])

SET @Message = CONVERT(varchar, getdate(), 121)
EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Override Zone Rules  Inserted to Route_Override', @Message

	
----------------- Add Override Options --------------------------

; WITH 

RouteOverrideWithSuppliers AS (

	SELECT 
					ro.CustomerID, ro.Code, ro.OurZoneID,
					(SELECT o.IsLoss FROM dbo.fn_RT_Full_ParseOverrideOptions(ro.RouteOptions, '|') o WHERE o.Position = 1) IsLoss,
					(SELECT o.Percentage FROM dbo.fn_RT_Full_ParseOverrideOptions(ro.RouteOptions, '|') o WHERE o.Position = 1) Percentage,
					(SELECT o.[value] FROM dbo.fn_RT_Full_ParseOverrideOptions(ro.RouteOptions, '|') o WHERE o.Position = 1) SupplierID,
					1 as Position,ro.RuleID,ro.IncludeSubCodes,ro.ExcludedCodes
					
				FROM #TempOverrideRulesWithRates ro WITH(NOLOCK) WHERE (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 1) IS NOT NULL
				
					UNION
				SELECT 
					ro.CustomerID, ro.Code, ro.OurZoneID, 
					(SELECT o.IsLoss FROM dbo.fn_RT_Full_ParseOverrideOptions(ro.RouteOptions, '|') o WHERE o.Position = 2) IsLoss,
					(SELECT o.Percentage FROM dbo.fn_RT_Full_ParseOverrideOptions(ro.RouteOptions, '|') o WHERE o.Position = 2) Percentage,
					(SELECT o.[value] FROM dbo.fn_RT_Full_ParseOverrideOptions(ro.RouteOptions, '|') o WHERE o.Position = 2) SupplierID,
					2 as Position ,ro.RuleID,ro.IncludeSubCodes,ro.ExcludedCodes
					
				FROM #TempOverrideRulesWithRates ro WITH(NOLOCK) WHERE  (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 2) IS NOT NULL

			UNION	
				SELECT 
					ro.CustomerID, ro.Code, ro.OurZoneID, 
					(SELECT o.IsLoss FROM dbo.fn_RT_Full_ParseOverrideOptions(ro.RouteOptions, '|') o WHERE o.Position = 3) IsLoss,
					(SELECT o.Percentage FROM dbo.fn_RT_Full_ParseOverrideOptions(ro.RouteOptions, '|') o WHERE o.Position = 3) Percentage,
					(SELECT o.[value] FROM dbo.fn_RT_Full_ParseOverrideOptions(ro.RouteOptions, '|') o WHERE o.Position = 3) SupplierID,
					3 as Position ,ro.RuleID,ro.IncludeSubCodes,ro.ExcludedCodes
					
				FROM #TempOverrideRulesWithRates ro WITH(NOLOCK) WHERE  (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 3) IS NOT NULL
			UNION
			SELECT 
					ro.CustomerID, ro.Code, ro.OurZoneID, 
					(SELECT o.IsLoss FROM dbo.fn_RT_Full_ParseOverrideOptions(ro.RouteOptions, '|') o WHERE o.Position = 4) IsLoss,
				(SELECT o.Percentage FROM dbo.fn_RT_Full_ParseOverrideOptions(ro.RouteOptions, '|') o WHERE o.Position = 4) Percentage,
					(SELECT o.[value] FROM dbo.fn_RT_Full_ParseOverrideOptions(ro.RouteOptions, '|') o WHERE o.Position = 4) SupplierID,
					4 as Position ,ro.RuleID,ro.IncludeSubCodes,ro.ExcludedCodes
					
				FROM #TempOverrideRulesWithRates ro WITH(NOLOCK) WHERE  (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 4) IS NOT NULL
			UNION
				SELECT 
					ro.CustomerID, ro.Code, ro.OurZoneID, 
					(SELECT o.IsLoss FROM dbo.fn_RT_Full_ParseOverrideOptions(ro.RouteOptions, '|') o WHERE o.Position = 5) IsLoss,
					(SELECT o.Percentage FROM dbo.fn_RT_Full_ParseOverrideOptions(ro.RouteOptions, '|') o WHERE o.Position = 5) Percentage,
					(SELECT o.[value] FROM dbo.fn_RT_Full_ParseOverrideOptions(ro.RouteOptions, '|') o WHERE o.Position = 5) SupplierID,
					5 as Position ,ro.RuleID,ro.IncludeSubCodes,ro.ExcludedCodes
					
				FROM #TempOverrideRulesWithRates ro WITH(NOLOCK) WHERE (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 5) IS NOT NULL
			UNION	
				SELECT 
					ro.CustomerID, ro.Code, ro.OurZoneID, 
					(SELECT o.IsLoss FROM dbo.fn_RT_Full_ParseOverrideOptions(ro.RouteOptions, '|') o WHERE o.Position = 6) IsLoss,
					(SELECT o.Percentage FROM dbo.fn_RT_Full_ParseOverrideOptions(ro.RouteOptions, '|') o WHERE o.Position = 6) Percentage,
					(SELECT o.[value] FROM dbo.fn_RT_Full_ParseOverrideOptions(ro.RouteOptions, '|') o WHERE o.Position = 6) SupplierID,
					6 as Position ,ro.RuleID,ro.IncludeSubCodes,ro.ExcludedCodes
							FROM #TempOverrideRulesWithRates ro WITH(NOLOCK) WHERE (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 6) IS NOT NULL
			UNION	
				SELECT 
					ro.CustomerID, ro.Code, ro.OurZoneID, 
					(SELECT o.IsLoss FROM dbo.fn_RT_Full_ParseOverrideOptions(ro.RouteOptions, '|') o WHERE o.Position = 7) IsLoss,
					(SELECT o.Percentage FROM dbo.fn_RT_Full_ParseOverrideOptions(ro.RouteOptions, '|') o WHERE o.Position = 7) Percentage,
					(SELECT o.[value] FROM dbo.fn_RT_Full_ParseOverrideOptions(ro.RouteOptions, '|') o WHERE o.Position = 7) SupplierID,
					7 as Position ,ro.RuleID,ro.IncludeSubCodes,ro.ExcludedCodes
							FROM #TempOverrideRulesWithRates ro WITH(NOLOCK) WHERE (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 7) IS NOT NULL
			UNION	
				SELECT 
					ro.CustomerID, ro.Code, ro.OurZoneID, 
					(SELECT o.IsLoss FROM dbo.fn_RT_Full_ParseOverrideOptions(ro.RouteOptions, '|') o WHERE o.Position = 8) IsLoss,
					(SELECT o.Percentage FROM dbo.fn_RT_Full_ParseOverrideOptions(ro.RouteOptions, '|') o WHERE o.Position = 8) Percentage,
					(SELECT o.[value] FROM dbo.fn_RT_Full_ParseOverrideOptions(ro.RouteOptions, '|') o WHERE o.Position = 8) SupplierID,
					8 as Position ,ro.RuleID,ro.IncludeSubCodes,ro.ExcludedCodes
							FROM #TempOverrideRulesWithRates ro WITH(NOLOCK) WHERE (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 8) IS NOT NULL



)

SELECT *  INTO #RouteOverrideWithSuppliers 
 FROM RouteOverrideWithSuppliers rov 

INSERT INTO #RouteOption_Override With(Tablock)
SELECT 

            ro.[CustomerID]
           ,rov.[SupplierID]
           ,cs.[Code]
           ,cs.[SupplierZoneID]
           ,cs.ActiveRate [SupplierActiveRate]
           ,cs.[SupplierServicesFlag]
           ,@MaxSuppliersPerRoute - rov.Position + 1 AS [Priority]
           ,1 [NumberOfTries]
           ,1 [State]
           ,rov.[Percentage]
           ,CASE WHEN rov.IsLoss = 0 THEN 2 ELSE 5 END ActionType

 FROM (#RouteOverrideWithSuppliers rov WITH(NOLOCK)
INNER JOIN #Route_Override ro WITH(NOLOCK) ON ro.RuleID = rov.RuleID)  
INNER JOIN #RouteOptionsPool cs WITH(NOLOCK) ON cs.SupplierID COLLATE DATABASE_DEFAULT = rov.SupplierID AND ro.Code COLLATE DATABASE_DEFAULT = cs.Code
 	WHERE rov.SupplierID <> 'BLK'
 
 
INSERT INTO #RouteOption_Override With(Tablock)
SELECT 
DISTINCT
            ro.[CustomerID]
           ,rov.[SupplierID]
           ,cs.[Code]
           ,-1
           ,0 [SupplierActiveRate]
           ,1
           ,@MaxSuppliersPerRoute - rov.Position + 1 AS [Priority]
           ,1 [NumberOfTries]
           ,1 [State]
           ,rov.[Percentage]
           ,1 ActionType

 FROM (#RouteOverrideWithSuppliers rov WITH(NOLOCK)
INNER JOIN #Route_Override ro WITH(NOLOCK) ON ro.RuleID = rov.RuleID)  
INNER JOIN #RouteOptionsPool cs WITH(NOLOCK) ON ro.Code = cs.Code COLLATE DATABASE_DEFAULT
	WHERE rov.SupplierID = 'BLK'
 
 
   SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Overrid Optionse Inserted', @Message

CREATE NONCLUSTERED INDEX [IX_#RouteOverride_mutikeys] ON [#Route_Override] ([RuleID],[Code]) INCLUDE ([CustomerID])

---Write Back
DELETE rb FROM Route_Override rb
INNER JOIN #Route_Override rbn ON rb.CustomerID COLLATE DATABASE_DEFAULT = rbn.CustomerID AND rb.Code COLLATE DATABASE_DEFAULT = rbn.Code

INSERT INTO Route_Override
SELECT rb.CustomerID, 
	   rb.ProfileID, 
	   rb.Code, 
	   rb.OurZoneID, 
	   rb.OurActiveRate,
       rb.OurServicesFlag, 
       rb.[State], 
       rb.IsToDAffected,
       rb.IsSpecialRequestAffected, 
       rb.IsOverrideAffected, 
       rb.IsBlockAffected,
       rb.IsOptionBlock, 
       rb.RuleID
  FROM #Route_Override rb
  
  DELETE rob FROM RouteOption_Override rob
  INNER JOIN #RouteOption_Override robn ON robn.CustomerID COLLATE DATABASE_DEFAULT = rob.CustomerID AND robn.Code COLLATE DATABASE_DEFAULT = rob.Code
  
  INSERT INTO RouteOption_Override
  SELECT * FROM #RouteOption_Override rob
  
--Testing
SELECT * FROM #Route_Override rb
SELECT * FROM #RouteOption_Override rob

--Testing
SELECT 'OverrideRoutes',* FROM #Route_Override ro
SELECT 'OverrideOptions',* FROM #RouteOption_Override roo

END