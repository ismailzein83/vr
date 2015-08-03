
CREATE PROCEDURE [dbo].[bp_RT_Full_BuildRouteAction_Block]
WITH RECOMPILE
AS
BEGIN

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

--DROP INDEX IX_RouteOption_Block_Code ON RouteOption_Block
--DROP INDEX IX_RouteOption_Block_Zone ON RouteOption_Block


DROP INDEX [IX_RouteOption_Block_multikey] ON [RouteOption_Block]
DROP INDEX [IX_Route_Block_multikeys] ON [Route_Block]
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
-- Adding profileID could be to the BlockedRules directly before expanding the CodeRules with sub
--									CarrierAccount should be replaced by aux_Customers
BlockedRuleswithProfile AS (
	select
			brz.*,
			ca.ProfileID		
	FROM 	BlockedRules brz
			LEFT JOIN CarrierAccount ca WITH(NOLOCK) ON ca.CarrierAccountID = brz.CustomerID
)
,

CodeRuleWithoutSubCode AS (
	SELECT	ds.*,rcp.ZoneID SupplierZoneID		
	FROM 	BlockedRuleswithProfile ds
			INNER JOIN RoutePool rcp ON rcp.code = ds.Code 
	WHERE	ds.IncludeSubCodes = 'N'
),
CodeRuleWithSubCode AS (
	SELECT	ds.CustomerID,	ds.SupplierID,rcp.Code,ds.ZoneID,ds.UpdateDate, ds.IncludeSubCodes,ds.ExcludedCodes, ds.ProfileID,rcp.ZoneID SupplierZoneID
	FROM 	BlockedRuleswithProfile ds
			INNER JOIN RoutePool rcp WITH(NOLOCK) ON rcp.code COLLATE DATABASE_DEFAULT  LIKE (ds.Code + '%') 
	WHERE	ds.IncludeSubCodes = 'Y' AND rcp.Code NOT IN (SELECT * FROM dbo.ParseArray(ds.Excludedcodes,',') pa)
) 
,
--										Duplicate routes should be deleted
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

INSERT INTO Route_Block With(Tablock)
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
--										Blocked Sale Codes are tagged in RoutePool ( if applicable)
--										Blocked Sale Zones are tagged in ZoneRates, there routes could be added here or delayed to check during LCR route build

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
		MainExcluded varchar(max),
		OriginalExcluded VARCHAR(max),
		--HasSubZone BIT,
		SubZOneIDs VARCHAR(MAX),
)


EXEC bp_RT_Full_PrepareBlockRules

------- For Code without Sub -----
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
		LEFT JOIN RouteOptionsPool cs WITH(NOLOCK) ON 			
			cs.SupplierID COLLATE DATABASE_DEFAULT = br.SupplierID  AND
			cs.Code COLLATE DATABASE_DEFAULT = br.Code 
			-- cs.SupplierZoneID  = br.ZoneID AND 
			
WHERE 	cs.Code IS NOT NULL 	AND br.Code IS not NULL		AND	br.IncludeSubCodes = 'N'
)
,


-------- For Code With Sub code

RouteOptionCodeTempWithSubCode AS(
SELECT 
		br.CustomerID,
		br.SupplierID,
		cs.Code,
		br.ZoneID SupplierZoneID,
		cs.ActiveRate,
		cs.SupplierServicesFlag

	
FROM	#TempBlockRules br WITH(NOLOCK)
		LEFT JOIN RouteOptionsPool cs  WITH(NOLOCK) ON 
			cs.SupplierID COLLATE DATABASE_DEFAULT = br.SupplierID --AND cs.SupplierZoneID  = br.SupplierZoneID
WHERE 	cs.Code IS NOT NULL AND br.Code IS NOT NULL AND  		
		br.IncludeSubCodes = 'Y' AND cs.code COLLATE DATABASE_DEFAULT LIKE (br.code + '%')			
		AND cs.Code NOT IN (SELECT * FROM dbo.ParseArray(br.Excludedcodes,',') pa)
)
,
RouteOptionCodeTemp AS (
SELECT * FROM RouteOptionCodeTempWithSubCode
UNION ALL
SELECT * FROM RouteOptionCodeTempWithoutSubCode
)
,			  
------- For Zone

RouteOptionZoneTemp AS (
SELECT 
		br.CustomerID,
		br.SupplierID,
		cs.Code,
		br.ZoneID SupplierZoneID,
		cs.ActiveRate,
		cs.SupplierServicesFlag

FROM	#TempBlockRules br WITH(NOLOCK)
		LEFT JOIN RouteOptionsPool cs WITH(NOLOCK) ON 
			cs.SupplierID COLLATE DATABASE_DEFAULT = br.SupplierID AND 
			cs.SupplierZoneID  = br.ZoneID
WHERE 
		cs.Code IS NOT NULL AND 
		br.Code IS NULL 
		AND cs.Code NOT IN (SELECT * FROM dbo.ParseArray(br.Excludedcodes,',') pa)
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

INSERT INTO RouteOption_Block With(Tablock)
SELECT		ro.*, 
			0 Proirity,0 NumberOfTries, 0 [State],	--br.UpdateDate,
			0 Percentage,	1 ActionType
FROM AllRouteOptions ro
				
CREATE NONCLUSTERED INDEX [IX_RouteOption_Block_multikey] ON [dbo].[RouteOption_Block] ([CustomerID] ASC,[SupplierID] ASC,[Code] ASC)
INCLUDE ( [Priority],[NumberOfTries],[State],[Percentage],[ActionType]) 
WITH (STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

SET @Message = CONVERT(varchar, getdate(), 121)
EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Block options Inserted', @Message


SET @Message = CONVERT(varchar, getdate(), 121)
EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Block Option Indexes Created', @Message


INSERT INTO Route_Block With(Tablock)
select
		rob.CustomerID,
		ca.ProfileID,
		rob.Code,
		rcp.ZoneID,
		0 OurActiveRate,
		0 OurServicesFlag,
		1 [State],
		--GETDATE() Updated,
		0 IsToDAffected,
		0 IsSpecialRequestAffected,
		0 IsOverrideAffected,
		0 IsBlockAffected,
		1 IsOptionBlock,
		0 RuleID
FROM 
--RouteOption_Block rob
		(SELECT CustomerID,Code FROM RouteOption_Block rob	GROUP BY CustomerID,Code) rob
		INNER JOIN RoutePool rcp  WITH(NOLOCK) ON rcp.Code = rob.Code
		LEFT JOIN CarrierAccount ca WITH(NOLOCK) ON ca.CarrierAccountID = rob.CustomerID
		LEFT JOIN Route_Block rb WITH(NOLOCK) ON rb.CustomerID = rob.CustomerID AND rb.Code = rob.Code

WHERE	rb.CustomerID IS NULL AND rb.Code IS NULL


INSERT INTO RouteOption_Block
           SELECT ca.CarrierAccountID, zr.SupplierID, cm.Code
           ,zr.ZoneID, Zr.Activerate,1
           ,0
           ,1
           ,0
           ,0
           ,1
             FROM ZoneRates zr       
           JOIN CodeMatch cm ON cm.SupplierZoneID = zr.ZoneID
           left JOIN RouteOption_Block rb ON rb.SupplierID = cm.SupplierID AND cm.Code = rb.Code 
           JOIN CarrierAccount ca ON ca.IsDeleted = 'N'
           
           WHERE zr.CustomerID = 'SYS' AND zr.isblock = 1 AND ca.CarrierAccountID <> 'SYS' AND zr.supplierid <> 'BLK' 
           AND  rb.SupplierID IS NULL AND rb.Code IS NULL
           AND 	ca.ActivationStatus <> @Account_Inactive
		And ca.RoutingStatus <> @Account_BlockedInbound 
	    AND ca.RoutingStatus <> @Account_Blocked 


CREATE NONCLUSTERED INDEX [IX_Route_Block_multikeys] ON [dbo].[Route_Block](	[CustomerID] ASC,	[Code] ASC,	[OurZoneID] ASC)
INCLUDE ( [IsToDAffected],[IsSpecialRequestAffected],[IsOverrideAffected],[IsBlockAffected],[IsOptionBlock]) 
WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]





SET @Message = CONVERT(varchar, getdate(), 121)
EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Route Block Inserted', @Message



	

END