
CREATE PROCEDURE [dbo].[bp_RT_Full_BuildRouteAction_SpecialRequest]

WITH RECOMPILE
AS
BEGIN

SET NOCOUNT ON

DECLARE @HighestPriority tinyint
	SET @HighestPriority = 0

	DECLARE @ForcedRoute tinyint
	SET @ForcedRoute = 1

	DECLARE @Message varchar(500) 

DROP INDEX [IX_RouteSpecial_mutikeys] ON [Route_Special]
--DROP INDEX [IX_RouteOption_Special_multikey] ON [RouteOption_Special]

				CREATE TABLE [dbo].[#RouteOptionTEMP](
					[RuleId] INT NULL,
					[CustomerID] VARCHAR(5) NOT NULL,
					[SupplierID] VARCHAR(5) NOT NULL,
					[Code] [VARCHAR] (20) NOT NULL,
					[SupplierZoneID] [int] NULL,
					[SupplierActiveRate] [real] NULL,
					[SupplierServicesFlag] [smallint] NULL,
					[Priority] [tinyint] NOT NULL,
					[NumberOfTries] [tinyint] NULL,
					[State] BIT NOT NULL,					
					[Percentage] [tinyint] NULL,
					[ActionType] [tinyint] NULL
				)

CREATE TABLE #RouteCodeTemp(
	[CustomerID] VARCHAR(5) NOT NULL,
	[ProfileID] [int] NULL,
	[Code] [VARCHAR] (20) NULL,
	[OurZoneID] [int] NULL,
	[OurActiveRate] [real] NULL,
	[OurServicesFlag] [smallint] NULL,
	[IsToDAffected] BIT NOT NULL,
	[RuleID] INT NOT NULL
)

CREATE TABLE #RouteZoneTemp(
	[CustomerID] VARCHAR(5) NOT NULL,
	[ProfileID] [int] NULL,
	[Code] [VARCHAR] (20) NULL,
	[OurZoneID] [int] NULL,
	[OurActiveRate] [real] NULL,
	[OurServicesFlag] [smallint] NULL,
	[IsToDAffected] BIT NOT NULL,
	[RuleID] INT NOT NULL
)

 
 CREATE TABLE #TempSpecialRequestRules (
 	RuleID INT,
	CustomerID varchar(5) ,
	Code varchar(30) ,
	ZoneID int ,
	SupplierOptions varchar(max) ,
	IncludeSubCodes char(1) ,
	ExcludedCodes varchar(max) ,
	ParentID INT,
	MainExcluded varchar(max),
	OriginalExcluded varchar(max),
	HasSubZone BIT,
	SubCodes VARCHAR(MAX),
	SubZoneIDs VARCHAR(MAX)
	 )
 
 CREATE TABLE #TempSpecialRequestRulesWithRates (
	RuleID INT IDENTITY(1,1),
	CustomerID VARCHAR(5),
	Code VARCHAR(30),
	ZoneID INT,
	SupplierOptions varchar(max) ,
	IncludeSubCodes char(1) ,
	ExcludedCodes varchar(max) ,
	ProfileID INT,
	OurActiveRate real ,
	OurServicesFlag smallint ,
	IsToDAffected TINYINT
)

EXEC bp_RT_Full_PrepareSpecialRequestRules

INSERT INTO #TempSpecialRequestRulesWithRates With(Tablock)
SELECT sr.CustomerID, sr.Code, sr.ZoneID,sr.SupplierOptions,sr.IncludeSubCodes,
       sr.ExcludedCodes
	  ,zr.[ProfileID]
      ,zr.ActiveRate [OurActiveRate]
      ,zr.ServicesFlag [OurServicesFlag]
      ,zr.IsTOD [IsToDAffected]	
 FROM #TempSpecialRequestRules sr WITH(NOLOCK)
	INNER JOIN ZoneRates zr WITH(NOLOCK) ON sr.ZoneID = zr.ZoneID AND zr.CustomerID = sr.CustomerID COLLATE DATABASE_DEFAULT
 WHERE zr.SupplierID = 'SYS' AND zr.IsBlock = 0	
 
   SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Special Request With Rates Created', @Message

TRUNCATE TABLE Route_Special
TRUNCATE TABLE RouteOption_Special 

;WITH 
------- For Code without Sub -----
--INSERT INTO #RouteCodeTemp With(Tablock)
CodeSpecialRules_WithoutSub AS 
(
SELECT 
  srt.CustomerID,
  srt.ProfileId,
  rcp.[Code],
  rcp.ZoneID OurZoneID,
  srt.OurActiveRate, 
  srt.OurServicesFlag,
  srt.IsToDAffected,
  srt.RuleID
  FROM #TempSpecialRequestRulesWithRates srt  WITH(NOLOCK) --,RoutePool rcp
  LEFT JOIN RoutePool rcp  WITH(NOLOCK) ON rcp.Code = srt.Code COLLATE DATABASE_DEFAULT AND rcp.ZoneID = srt.ZoneID 
 WHERE srt.code IS NOT NULL 
AND 
	(srt.IncludeSubCodes = 'N' AND srt.code COLLATE DATABASE_DEFAULT = rcp.code) 
),
CodeSpecialRules_WithSub AS 
(
SELECT 
  srt.CustomerID,
  srt.ProfileId,
  rcp.[Code],
  rcp.ZoneID OurZoneID,
  srt.OurActiveRate, 
  srt.OurServicesFlag,
  srt.IsToDAffected,
  srt.RuleID
  FROM #TempSpecialRequestRulesWithRates srt  WITH(NOLOCK) ,RoutePool rcp  WITH(NOLOCK)  --LEFT JOIN excludedList exl ON rcp.code = exl.exCode
  
 WHERE srt.code IS NOT NULL 
AND 

	srt.IncludeSubCodes = 'Y' AND rcp.code COLLATE DATABASE_DEFAULT LIKE (srt.code + '%')
	AND 1= ( CASE WHEN  PATINDEX('%,%',srt.ExcludedCodes) > 0  AND 
		                   rcp.Code NOT IN 
		                   (SELECT * FROM dbo.ParseArray(srt.ExcludedCodes,','))
		                   THEN 1
		              WHEN  PATINDEX('%,%',srt.ExcludedCodes) = 0 AND  
     			         rcp.Code COLLATE DATABASE_Default  NOT LIKE srt.ExcludedCodes THEN 1
	                  ELSE 0 END 
	)
)
,	
AllCodeRules AS 
(
SELECT * FROM CodeSpecialRules_WithoutSub
UNION ALL
SELECT * FROM CodeSpecialRules_WithSub	
),
------- For Zone
ZoneSpecialRules AS 
(
	SELECT 
  srt.CustomerID,
  srt.ProfileId,
  rcp.[Code],
  rcp.ZoneID OurZoneID,
  srt.OurActiveRate, 
  srt.OurServicesFlag,
  srt.IsToDAffected,
  srt.RuleID
  FROM #TempSpecialRequestRulesWithRates srt WITH(NOLOCK) 
   INNER JOIN RoutePool rcp WITH(NOLOCK)  ON rcp.ZoneID = srt.ZoneID
WHERE srt.code IS NULL 
AND rcp.Code NOT IN (SELECT * FROM dbo.ParseArray(srt.Excludedcodes,',') pa)
),

ZoneRulesWithoutCode AS 
(
SELECT srt.* FROM ZoneSpecialRules srt	
--LEFT JOIN AllCodeRules rct  WITH(NOLOCK) ON rct.CustomerID = srt.CustomerID AND rct.Code = srt.Code AND rct.OurZoneID = srt.OurZoneID
--WHERE rct.CustomerID IS NULL AND rct.Code IS NULL AND rct.OurZoneID IS NULL
),

AllSpecialrules AS 
(
SELECT * FROM AllCodeRules
UNION ALL
SELECT * FROM ZoneRulesWithoutCode	
)

INSERT INTO Route_Special With(Tablock)
SELECT 
  srt.CustomerID,
  srt.ProfileId,
  srt.[Code],
  srt.OurZoneID,
  srt.OurActiveRate, 
  srt.OurServicesFlag,
  0 [STATE],
  srt.IsToDAffected,
  1 IsSpecialRequestAffected,
  0 IsOverrideAffected,
  0 IsBlockAffected,
  0 IsOptionBlock,
  srt.RuleID
 FROM AllSpecialrules srt WITH(NOLOCK) 
ORDER BY srt.ruleid desc
--TRUNCATE TABLE RouteOption_Special

CREATE NONCLUSTERED INDEX [IX_RouteSpecial_mutikeys] ON [Route_Special] ([RuleID],[Code]) INCLUDE ([CustomerID])
  SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Special Request Routes Inserted', @Message

; WITH 

SpecialRequestWithSuppliers AS (

	SELECT 
					ro.CustomerID, ro.Code, ro.ZoneID,
					(SELECT op.SupplierID FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 1), '|') op) SupplierID,
					(SELECT op.Priority FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 1), '|') op) Priority,
					(SELECT op.NumberOfTries FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 1), '|') op) NumberOfTries,
					(SELECT op.SpecialRequestType FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 1), '|') op) SpecialRequestType,
					(SELECT op.Percentage FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 1), '|') op) Percentage,
					1 as Position,ro.RuleID,ro.IncludeSubCodes,ro.ExcludedCodes
					
			     	FROM #TempSpecialRequestRulesWithRates ro  WITH(NOLOCK) WHERE (SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 1) IS NOT NULL
				
					UNION
	SELECT 
					ro.CustomerID, ro.Code, ro.ZoneID,
					(SELECT op.SupplierID FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 2), '|') op) SupplierID,
					(SELECT op.Priority FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 2), '|') op) Priority,
					(SELECT op.NumberOfTries FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 2), '|') op) NumberOfTries,
					(SELECT op.SpecialRequestType FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 2), '|') op) SpecialRequestType,
					(SELECT op.Percentage FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 2), '|') op) Percentage,
					
					2 as Position,ro.RuleID,ro.IncludeSubCodes,ro.ExcludedCodes
					
				FROM #TempSpecialRequestRulesWithRates ro  WITH(NOLOCK) WHERE (SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 2) IS NOT NULL
	
			UNION	
	SELECT 
					ro.CustomerID, ro.Code, ro.ZoneID,
					(SELECT op.SupplierID FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 3), '|') op) SupplierID,
					(SELECT op.Priority FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 3), '|') op) Priority,
					(SELECT op.NumberOfTries FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 3), '|') op) NumberOfTries,
					(SELECT op.SpecialRequestType FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 3), '|') op) SpecialRequestType,
					(SELECT op.Percentage FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 3), '|') op) Percentage,
					
					3 as Position,ro.RuleID,ro.IncludeSubCodes,ro.ExcludedCodes
					
				FROM #TempSpecialRequestRulesWithRates ro  WITH(NOLOCK) WHERE (SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 3) IS NOT NULL
			UNION
	SELECT 
					ro.CustomerID, ro.Code, ro.ZoneID,
					(SELECT op.SupplierID FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 4), '|') op) SupplierID,
					(SELECT op.Priority FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 4), '|') op) Priority,
					(SELECT op.NumberOfTries FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 4), '|') op) NumberOfTries,
					(SELECT op.SpecialRequestType FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 4), '|') op) SpecialRequestType,
					(SELECT op.Percentage FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 4), '|') op) Percentage,
				
					4 as Position,ro.RuleID,ro.IncludeSubCodes,ro.ExcludedCodes
					
				FROM #TempSpecialRequestRulesWithRates ro  WITH(NOLOCK) WHERE (SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 4) IS NOT NULL
			UNION
	SELECT 
					ro.CustomerID, ro.Code, ro.ZoneID,
					(SELECT op.SupplierID FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 5), '|') op) SupplierID,
					(SELECT op.Priority FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 5), '|') op) Priority,
					(SELECT op.NumberOfTries FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 5), '|') op) NumberOfTries,
					(SELECT op.SpecialRequestType FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 5), '|') op) SpecialRequestType,
					(SELECT op.Percentage FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 5), '|') op) Percentage,
					5 as Position,ro.RuleID,ro.IncludeSubCodes,ro.ExcludedCodes
					
				FROM #TempSpecialRequestRulesWithRates ro  WITH(NOLOCK) WHERE (SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 5) IS NOT NULL
			UNION	
	SELECT 
					ro.CustomerID, ro.Code, ro.ZoneID,
					(SELECT op.SupplierID FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 6), '|') op) SupplierID,
					(SELECT op.Priority FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 6), '|') op) Priority,
					(SELECT op.NumberOfTries FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 6), '|') op) NumberOfTries,
					(SELECT op.SpecialRequestType FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 6), '|') op) SpecialRequestType,
					(SELECT op.Percentage FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 6), '|') op) Percentage,
					6 as Position,ro.RuleID,ro.IncludeSubCodes,ro.ExcludedCodes
					
				FROM #TempSpecialRequestRulesWithRates ro  WITH(NOLOCK) WHERE (SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 6) IS NOT NULL
			UNION	
	SELECT 
					ro.CustomerID, ro.Code, ro.ZoneID,
					(SELECT op.SupplierID FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 7), '|') op) SupplierID,
					(SELECT op.Priority FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 7), '|') op) Priority,
					(SELECT op.NumberOfTries FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 7), '|') op) NumberOfTries,
					(SELECT op.SpecialRequestType FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 7), '|') op) SpecialRequestType,
					(SELECT op.Percentage FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 7), '|') op) Percentage,
					7 as Position,ro.RuleID,ro.IncludeSubCodes,ro.ExcludedCodes
					
				FROM #TempSpecialRequestRulesWithRates ro  WITH(NOLOCK) WHERE (SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 7) IS NOT NULL
			UNION	
	SELECT 
					ro.CustomerID, ro.Code, ro.ZoneID,
					(SELECT op.SupplierID FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 8), '|') op) SupplierID,
					(SELECT op.Priority FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 8), '|') op) Priority,
					(SELECT op.NumberOfTries FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 8), '|') op) NumberOfTries,
					(SELECT op.SpecialRequestType FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 8), '|') op) SpecialRequestType,
					(SELECT op.Percentage FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 8), '|') op) Percentage,
					8 as Position,ro.RuleID,ro.IncludeSubCodes,ro.ExcludedCodes
					
				FROM #TempSpecialRequestRulesWithRates ro  WITH(NOLOCK) WHERE (SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 8) IS NOT NULL
			UNION	
	SELECT 
					ro.CustomerID, ro.Code, ro.ZoneID,
					(SELECT op.SupplierID FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 9), '|') op) SupplierID,
					(SELECT op.Priority FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 9), '|') op) Priority,
					(SELECT op.NumberOfTries FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 9), '|') op) NumberOfTries,
					(SELECT op.SpecialRequestType FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 9), '|') op) SpecialRequestType,
					(SELECT op.Percentage FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 9), '|') op) Percentage,
					9 as Position,ro.RuleID,ro.IncludeSubCodes,ro.ExcludedCodes
					
				FROM #TempSpecialRequestRulesWithRates ro  WITH(NOLOCK) WHERE (SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 9) IS NOT NULL

			UNION	
	SELECT 
					ro.CustomerID, ro.Code, ro.ZoneID,
					(SELECT op.SupplierID FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 10), '|') op) SupplierID,
					(SELECT op.Priority FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 10), '|') op) Priority,
					(SELECT op.NumberOfTries FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 10), '|') op) NumberOfTries,
					(SELECT op.SpecialRequestType FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 10), '|') op) SpecialRequestType,
					(SELECT op.Percentage FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 10), '|') op) Percentage,
					10 as Position,ro.RuleID,ro.IncludeSubCodes,ro.ExcludedCodes
					
				FROM #TempSpecialRequestRulesWithRates ro  WITH(NOLOCK) WHERE (SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 10) IS NOT NULL

			UNION	
	SELECT 
					ro.CustomerID, ro.Code, ro.ZoneID,
					(SELECT op.SupplierID FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 11), '|') op) SupplierID,
					(SELECT op.Priority FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 11), '|') op) Priority,
					(SELECT op.NumberOfTries FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 11), '|') op) NumberOfTries,
					(SELECT op.SpecialRequestType FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 11), '|') op) SpecialRequestType,
					(SELECT op.Percentage FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 11), '|') op) Percentage,
					11 as Position,ro.RuleID,ro.IncludeSubCodes,ro.ExcludedCodes
					
				FROM #TempSpecialRequestRulesWithRates ro  WITH(NOLOCK) WHERE (SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 11) IS NOT NULL

			UNION	
	SELECT 
					ro.CustomerID, ro.Code, ro.ZoneID,
					(SELECT op.SupplierID FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 12), '|') op) SupplierID,
					(SELECT op.Priority FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 12), '|') op) Priority,
					(SELECT op.NumberOfTries FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 12), '|') op) NumberOfTries,
					(SELECT op.SpecialRequestType FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 12), '|') op) SpecialRequestType,
					(SELECT op.Percentage FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 12), '|') op) Percentage,
					12 as Position,ro.RuleID,ro.IncludeSubCodes,ro.ExcludedCodes
					
				FROM #TempSpecialRequestRulesWithRates ro  WITH(NOLOCK) WHERE (SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 12) IS NOT NULL

			UNION	
	SELECT 
					ro.CustomerID, ro.Code, ro.ZoneID,
					(SELECT op.SupplierID FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 13), '|') op) SupplierID,
					(SELECT op.Priority FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 13), '|') op) Priority,
					(SELECT op.NumberOfTries FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 13), '|') op) NumberOfTries,
					(SELECT op.SpecialRequestType FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 13), '|') op) SpecialRequestType,
					(SELECT op.Percentage FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 13), '|') op) Percentage,
					13 as Position,ro.RuleID,ro.IncludeSubCodes,ro.ExcludedCodes
					
				FROM #TempSpecialRequestRulesWithRates ro  WITH(NOLOCK) WHERE (SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 13) IS NOT NULL

			UNION	
	SELECT 
					ro.CustomerID, ro.Code, ro.ZoneID,
					(SELECT op.SupplierID FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 14), '|') op) SupplierID,
					(SELECT op.Priority FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 14), '|') op) Priority,
					(SELECT op.NumberOfTries FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 14), '|') op) NumberOfTries,
					(SELECT op.SpecialRequestType FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 14), '|') op) SpecialRequestType,
					(SELECT op.Percentage FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 14), '|') op) Percentage,
					14 as Position,ro.RuleID,ro.IncludeSubCodes,ro.ExcludedCodes
					
				FROM #TempSpecialRequestRulesWithRates ro  WITH(NOLOCK) WHERE (SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 14) IS NOT NULL

			UNION	
	SELECT 
					ro.CustomerID, ro.Code, ro.ZoneID,
					(SELECT op.SupplierID FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 15), '|') op) SupplierID,
					(SELECT op.Priority FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 15), '|') op) Priority,
					(SELECT op.NumberOfTries FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 15), '|') op) NumberOfTries,
					(SELECT op.SpecialRequestType FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 15), '|') op) SpecialRequestType,
					(SELECT op.Percentage FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 15), '|') op) Percentage,
					15 as Position,ro.RuleID,ro.IncludeSubCodes,ro.ExcludedCodes
					
				FROM #TempSpecialRequestRulesWithRates ro  WITH(NOLOCK) WHERE (SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 15) IS NOT NULL

			UNION	
	SELECT 
					ro.CustomerID, ro.Code, ro.ZoneID,
					(SELECT op.SupplierID FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 16), '|') op) SupplierID,
					(SELECT op.Priority FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 16), '|') op) Priority,
					(SELECT op.NumberOfTries FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 16), '|') op) NumberOfTries,
					(SELECT op.SpecialRequestType FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 16), '|') op) SpecialRequestType,
					(SELECT op.Percentage FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 16), '|') op) Percentage,
					16 as Position,ro.RuleID,ro.IncludeSubCodes,ro.ExcludedCodes
					
				FROM #TempSpecialRequestRulesWithRates ro  WITH(NOLOCK) WHERE (SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 16) IS NOT NULL

			UNION	
	SELECT 
					ro.CustomerID, ro.Code, ro.ZoneID,
					(SELECT op.SupplierID FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 17), '|') op) SupplierID,
					(SELECT op.Priority FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 17), '|') op) Priority,
					(SELECT op.NumberOfTries FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 17), '|') op) NumberOfTries,
					(SELECT op.SpecialRequestType FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 17), '|') op) SpecialRequestType,
					(SELECT op.Percentage FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 17), '|') op) Percentage,
					17 as Position,ro.RuleID,ro.IncludeSubCodes,ro.ExcludedCodes
					
				FROM #TempSpecialRequestRulesWithRates ro  WITH(NOLOCK) WHERE (SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 17) IS NOT NULL

			UNION	
	SELECT 
					ro.CustomerID, ro.Code, ro.ZoneID,
					(SELECT op.SupplierID FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 18), '|') op) SupplierID,
					(SELECT op.Priority FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 18), '|') op) Priority,
					(SELECT op.NumberOfTries FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 18), '|') op) NumberOfTries,
					(SELECT op.SpecialRequestType FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 18), '|') op) SpecialRequestType,
					(SELECT op.Percentage FROM dbo.fn_RT_Full_ParseSpecialRequestOption((SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 18), '|') op) Percentage,
					18 as Position,ro.RuleID,ro.IncludeSubCodes,ro.ExcludedCodes
					
				FROM #TempSpecialRequestRulesWithRates ro  WITH(NOLOCK) WHERE (SELECT o.[value] FROM dbo.fn_RT_Full_ParseArraySpecialRequest(ro.SupplierOptions, '|') o WHERE o.Position = 18) IS NOT NULL

)

INSERT INTO #RouteOptionTemp With(Tablock)
SELECT 
			srt.ruleid
            ,rs.[CustomerID]
           ,srt.[SupplierID]
           ,cs.[Code]
           ,cs.[SupplierZoneID]
           ,cs.ActiveRate [SupplierActiveRate]
           ,cs.[SupplierServicesFlag]
           ,srt.Priority [Priority]
           ,srt.[NumberOfTries]
           ,1 [State]-- temp change for test
           ,srt.[Percentage]
           , CASE WHEN srt.SpecialRequestType = 0 THEN 3 ELSE 4 END ActionType

 FROM (SpecialRequestWithSuppliers srt INNER JOIN Route_Special rs  WITH(NOLOCK) ON  rs.RuleID = srt.RuleId)
INNER JOIN RouteOptionsPool cs  WITH(NOLOCK) ON cs.SupplierID = srt.SupplierID COLLATE DATABASE_DEFAULT AND  cs.Code =  rs.Code
--ORDER BY rs.RuleID  desc

;WITH TempSpecialRoutes AS (
    SELECT ROW_NUMBER() OVER(PARTITION by  customerid, code ORDER BY customerid)
    AS duplicateRecCount
    FROM Route_Special
    )
   
    DELETE FROM TempSpecialRoutes
    WHERE duplicateRecCount > 1 



;WITH TempEmp 
    AS
    (
    SELECT  ROW_NUMBER() OVER(PARTITION by  customerid, code, SupplierID ORDER BY RuleId desc)
    AS duplicateRecCount
    FROM #RouteOptionTemp
    )
    
    --SELECT * FROM #RouteOptionTemp
   
    DELETE FROM TempEmp
    WHERE duplicateRecCount > 1 

--SELECT * FROM RouteOption_Special ros

INSERT INTO RouteOption_Special With(Tablock)
SELECT 
			[CustomerID]
           ,[SupplierID]
           ,[Code]
           ,[SupplierZoneID]
           ,[SupplierActiveRate]
           ,[SupplierServicesFlag]
           ,[Priority]
           ,[NumberOfTries]
           ,[State]
           ,[Percentage]
           ,  ActionType

 FROM #RouteOptionTemp srt


  SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Special Request Route Options Inserted', @Message
END