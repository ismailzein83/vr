
CREATE PROCEDURE [dbo].[bp_RT_Part_BuildRouteAction_Block]

AS
BEGIN

	SET NOCOUNT ON;
DECLARE @Message varchar(500) 
DECLARE @UpdateStamp datetime
SET @UpdateStamp = getdate()

	DECLARE @Account_Inactive tinyint
	DECLARE @Account_Blocked tinyint
	DECLARE @Account_BlockedInbound tinyint
	DECLARE @Account_BlockedOutbound tinyint
	
	-- Set Enumerations
	SELECT @Account_Inactive = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.ActivationStatus' AND Name = 'Inactive'
	SELECT @Account_Blocked = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'Blocked'
	SELECT @Account_BlockedInbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedInbound'	
	SELECT @Account_BlockedOutbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedOutbound'	

/***************************	Blocked Routes  		***********************************/
CREATE TABLE #RouteOptionCodeTemp(
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
				--[Updated] [datetime] NULL,
			) ON [PRIMARY]


CREATE TABLE #RouteOptionZoneTemp(
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
				--[Updated] [datetime] NULL,
			) ON [PRIMARY]

 
;WITH 
BlockedRules AS (
   SELECT * FROM dbo.fn_RT_Full_GetBlockRulesFromOverrides('Code',-2,'','Route') gbrfo
),

BlockedRuleswithProfile AS (
	select
			brz.*,
			0 ProfileID--ca.ProfileID		
	FROM 	BlockedRules brz
			LEFT JOIN #CustomerFilter ca WITH(NOLOCK) ON ca.CustomerID COLLATE DATABASE_DEFAULT = brz.CustomerID
),

CodeRuleWithoutSubCode AS (
	SELECT	ds.*,rcp.ZoneID SupplierZoneID		
	FROM 	BlockedRuleswithProfile ds
			INNER JOIN #RoutePool rcp ON rcp.code COLLATE DATABASE_DEFAULT = ds.Code 
	WHERE	ds.IncludeSubCodes = 'N'
),
CodeRuleWithSubCode AS (
	SELECT	ds.CustomerID,	ds.SupplierID,rcp.Code,ds.ZoneID,ds.UpdateDate, ds.IncludeSubCodes,ds.ExcludedCodes, ds.ProfileID,rcp.ZoneID SupplierZoneID
	FROM 	BlockedRuleswithProfile ds
			INNER JOIN RoutePool rcp WITH(NOLOCK) ON rcp.code COLLATE DATABASE_DEFAULT  LIKE (ds.Code + '%') 
	WHERE	ds.IncludeSubCodes = 'Y'  AND rcp.Code  COLLATE DATABASE_DEFAULT  NOT IN (SELECT * FROM dbo.ParseArray(ds.Excludedcodes,',') pa)

) 
,
--Duplicate routes should be deleted
CodeRuleWithoutSubCodeNoDuplicatio AS (
SELECT		wtsc.* 
FROM		CodeRuleWithoutSubCode wtsc LEFT JOIN 
			CodeRuleWithSubCode wsc ON
			wsc.CustomerID = wtsc.CustomerID AND --COLLATE DATABASE_DEFAULT  = wtsc.CustomerID AND 		
			wsc.code = wtsc.code AND --COLLATE DATABASE_DEFAULT = wtsc.code AND 
			wsc.SupplierZoneID =  wtsc.SupplierZoneID  --COLLATE DATABASE_DEFAULT = wtsc.SupplierZoneID
WHERE		wsc.CustomerID IS NULL AND wsc.code IS NULL AND	wsc.SupplierZoneID IS NULL
)
,
BlockedCodeRulesWithZones AS (
	SELECT * FROM CodeRuleWithSubCode
	UNION ALL 
	SELECT * FROM CodeRuleWithoutSubCodeNoDuplicatio
)

INSERT INTO #Route_Block With(Tablock)
SELECT 
		brz.CustomerID,
		brz.ProfileID,
		brz.code,
		brz.SupplierZoneID,
		0 ActiveRate,
		0 ServicesFlag,
		0 [State],
		0 IsTOD,
		0 [IsSpecialRequestAffected],
		0 [IsOverrideAffected],
		1 [IsBlockAffected],
		0 [IsOptionBlock],		
		0 RuleID 
FROM	BlockedCodeRulesWithZones brz
--Blocked Sale Codes are tagged in #RoutePool ( if applicable)
--Blocked Sale Zones are tagged in ZoneRates, there routes could be added here or delayed to check during LCR route build

/***************************	Blocked Route Options 		***********************************/

CREATE TABLE #TempBlockRules ( 
		RuleID INT,
		CustomerID VARCHAR(5),
		SupplierID VARCHAR(5),
		Code VARCHAR(20),
		ZoneID INT,
		UpdateDate DATETIME,
		IncludeSubCodes CHAR(1),
		ExcludedCodes VARCHAR(max),
		ParentID INT,
		MainExcluded varchar(250),
		OriginalExcluded varchar(250),
		--HasSubZone BIT,
		SubZOneIDs VARCHAR(MAX),
)


EXEC bp_RT_Part_PrepareBlockRules

------- For Code without Sub -----
--INSERT INTO #RouteOptionCodeTemp With(Tablock)
;WITH 
RouteOptionCodeTempWithoutSubCode AS (
SELECT 
		br.CustomerID,
		br.SupplierID,
		cs.Code,
		br.ZoneID,
		cs.ActiveRate,
		cs.SupplierServicesFlag
	
FROM	#TempBlockRules br WITH(NOLOCK)
		LEFT JOIN #RouteOptionsPool cs WITH(NOLOCK) ON 			
			cs.SupplierID COLLATE DATABASE_DEFAULT = br.SupplierID  AND
			cs.Code COLLATE DATABASE_DEFAULT = br.Code 
			-- cs.SupplierZoneID  = br.ZoneID AND 
			
WHERE 	cs.Code IS NOT NULL 	AND br.Code IS not NULL		AND	br.IncludeSubCodes = 'N'
)
,

RouteOptionCodeTempWithSubCode AS(
SELECT 
		br.CustomerID,
		br.SupplierID,
		cs.Code,
		br.ZoneID SupplierZoneID,
		cs.ActiveRate,
		cs.SupplierServicesFlag
	
FROM	#TempBlockRules br WITH(NOLOCK)
		LEFT JOIN #RouteOptionsPool cs  WITH(NOLOCK) ON 
			cs.SupplierID COLLATE DATABASE_DEFAULT = br.SupplierID --AND cs.SupplierZoneID  = br.SupplierZoneID
WHERE 	cs.Code IS NOT NULL AND br.Code IS NOT NULL AND  		
		br.IncludeSubCodes = 'Y' AND cs.code LIKE (br.code + '%')			
		AND cs.Code COLLATE DATABASE_Default NOT IN (SELECT * FROM dbo.ParseArray(br.Excludedcodes,',') pa)
)
,
RouteOptionCodeTemp AS (
SELECT * FROM RouteOptionCodeTempWithSubCode
UNION ALL
SELECT * FROM RouteOptionCodeTempWithoutSubCode
)
,			  

RouteOptionZoneTemp AS (
SELECT 
		br.CustomerID,
		br.SupplierID,
		cs.Code,
		br.ZoneID SupplierZoneID,
		cs.ActiveRate,
		cs.SupplierServicesFlag
FROM	#TempBlockRules br WITH(NOLOCK)
		LEFT JOIN #RouteOptionsPool cs WITH(NOLOCK) ON 
			cs.SupplierID COLLATE DATABASE_DEFAULT = br.SupplierID AND 
			cs.SupplierZoneID  = br.ZoneID
WHERE 
		cs.Code IS NOT NULL AND 
		br.Code IS NULL 
		AND cs.Code COLLATE DATABASE_Default NOT IN (SELECT * FROM dbo.ParseArray(br.Excludedcodes,',') pa)
)
,

RouteOptionZoneNotInCodeRuleOptions AS (
SELECT	rozt.* 
FROM	RouteOptionZoneTemp rozt WITH(NOLOCK)
		LEFT JOIN RouteOptionCodeTemp roct ON 
			roct.CustomerID = rozt.CustomerID AND roct.Code = rozt.Code AND 
			roct.SupplierID = rozt.SupplierID AND roct.SupplierZoneID = rozt.SupplierZoneID
WHERE	roct.CustomerID IS NULL AND roct.Code IS NULL AND 
		roct.SupplierID IS NULL AND roct.SupplierZoneID IS NULL
)
,
AllRouteOptions AS (
SELECT * FROM RouteOptionCodeTemp
UNION ALL
SELECT * FROM RouteOptionZoneNotInCodeRuleOptions
)

INSERT INTO #RouteOption_Block With(Tablock)
SELECT		ro.*, 
			0 Proirity,0 NumberOfTries, 0 [State],	--br.UpdateDate,
			0 Percentage,	1 ActionType
FROM AllRouteOptions ro
				

SET @Message = CONVERT(varchar, getdate(), 121)
EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Block options Inserted', @Message

SET @Message = CONVERT(varchar, getdate(), 121)
EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Block Option Indexes Created', @Message


INSERT INTO #Route_Block With(Tablock)
select
		rob.CustomerID,
		0 ProfileID,--ca.ProfileID,
		rob.Code,
		rcp.ZoneID,
		0 OurActiveRate,
		0 OurServicesFlag,
		1 [State],
		0 IsToDAffected,
		0 IsSpecialRequestAffected,
		0 IsOverrideAffected,
		0 IsBlockAffected,
		1 IsOptionBlock,
		0 RuleID
FROM 
		(SELECT CustomerID,Code FROM #RouteOption_Block rob	GROUP BY CustomerID,Code) rob
		INNER JOIN #RoutePool rcp  WITH(NOLOCK) ON rcp.Code COLLATE DATABASE_DEFAULT = rob.Code
		LEFT JOIN #CustomerFilter ca WITH(NOLOCK) ON ca.CustomerID COLLATE DATABASE_DEFAULT = rob.CustomerID
		LEFT JOIN #Route_Block rb WITH(NOLOCK) ON rb.CustomerID COLLATE DATABASE_DEFAULT  = rob.CustomerID AND rb.Code COLLATE DATABASE_DEFAULT  = rob.Code
		

WHERE	rb.CustomerID IS NULL AND rb.Code IS NULL

SET @Message = CONVERT(varchar, getdate(), 121)
EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Route Block Inserted', @Message

  INSERT INTO #RouteOption_Block
           
           SELECT ca.CustomerID, zr.SupplierID, cm.Code
           ,zr.ZoneID, Zr.Activerate,1
           ,0
           ,1
           ,0
           ,0
           ,1
             FROM #CostZoneRates zr 
           JOIN CodeMatch cm ON cm.SupplierZoneID = zr.ZoneID
           left JOIN #RouteOption_Block rb ON rb.SupplierID COLLATE DATABASE_DEFAULT = cm.SupplierID AND cm.Code COLLATE DATABASE_DEFAULT = rb.Code 
           JOIN #CustomerFilter ca ON ca.[IsActive] = 1
           WHERE zr.CustomerID = 'SYS' AND zr.isblock = 1 AND ca.CustomerID <> 'SYS' AND zr.supplierid <> 'BLK'
           AND  rb.SupplierID IS NULL AND rb.Code IS NULL



CREATE NONCLUSTERED INDEX IX_#RouteOption_Block_Code ON #RouteOption_Block(Code ASC)
CREATE NONCLUSTERED INDEX IX_#RouteOption_Block_Zone ON #RouteOption_Block(SupplierZoneID ASC)

--
--
--
---Write Back

INSERT INTO Route_Block
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
  FROM #Route_Block rb
  


DELETE rb FROM Route_Block rb
INNER JOIN #Route_Block rbn ON rb.CustomerID COLLATE DATABASE_DEFAULT = rbn.CustomerID AND rb.Code COLLATE DATABASE_DEFAULT = rbn.Code


  DELETE rob FROM RouteOption_Block rob
  INNER JOIN #RouteOption_Block robn ON robn.CustomerID COLLATE DATABASE_DEFAULT = rob.CustomerID AND robn.Code COLLATE DATABASE_DEFAULT = rob.Code
  
  
  
  
  INSERT INTO RouteOption_Block
  SELECT * FROM #RouteOption_Block rob
  
--Testing
SELECT 'BlockRoute',* FROM #Route_Block rb
SELECT 'BlockOption',* FROM #RouteOption_Block rob

END