


CREATE PROCEDURE [dbo].[bp_RT_Full_PrepareOverrideRules]

WITH RECOMPILE
AS
BEGIN
CREATE TABLE #DistinctParentRules (RuleID INT,ParentCode varchar(20), ExcludedCodes VARCHAR(max), MainExcluded varchar(max), GeneratedExcluding varchar(max))

CREATE TABLE #TempRules(
	RuleID INT IDENTITY(1,1),
	CustomerID VARCHAR(5),
	Code VARCHAR(30),
	OurZoneID INT,
	RouteOptions  VARCHAR(max),
	IncludeSubCodes CHAR(1),
	ExcludedCodes VARCHAR(max),
	ParentID INT,
	Updated SMALLDATETIME,
	MainExcluded varchar(max),
	OriginalExcluded varchar(max),
	HasSubZone BIT,
	SubCodes VARCHAR(MAX),
	SubZoneIDs VARCHAR(MAX)
)

	DECLARE @Account_Inactive tinyint
	DECLARE @Account_Blocked tinyint
	DECLARE @Account_BlockedInbound tinyint
	DECLARE @Account_BlockedOutbound tinyint
	
	-- Set Enumerations
	SELECT @Account_Inactive = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.ActivationStatus' AND Name = 'Inactive'
	SELECT @Account_Blocked = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'Blocked'
	SELECT @Account_BlockedInbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedInbound'	
	SELECT @Account_BlockedOutbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedOutbound'		


	DECLARE @Message varchar(500) 

/**** Phase1 : Collecting and Preparing Rules ***/
-- Definitions:
-- Zone-Rule: rule for a zone, where the code is null
-- Code-Rule: rule for a specific code with/without subcodes, where the zone is null
-- CG-Rule: rule for a specific code with subcodes which the code is a Code Group (Country Code), where the zone is null
;WITH 

-- Collect the CG-Rule and translate it into Zone-Rules directly 
-- Any conflict with the Zone-Rules will be resolved later based on the BED 

OverrideRulesForCodeGroup AS 
(
SELECT 
		ro.CustomerID,
		NULL Code,
		z.ZoneID OurZoneID,
		--ro.OurZoneID ,
		ro.RouteOptions,
		ro.IncludeSubCodes,
		ro.ExcludedCodes,
		NULL ParentID					-- Added to manage the overlapped and sub rules
		,ro.Updated
		,ro.ExcludedCodes OriginalExcluded	-- Added to manage the cascading excluded codes: to save the orignal excluded codes
		, '' GeneratedExcluded			-- Added to manage the cascading excluded codes: to save the excluded codes from the sub or overlapped rules 			
		,ro.BeginEffectiveDate

FROM	RouteOverride ro  WITH(NOLOCK) 
		LEFT JOIN CarrierAccount ca  WITH(NOLOCK) ON ca.CarrierAccountID = ro.CustomerID		 
		LEFT JOIN Zone z  WITH(NOLOCK) ON z.CodeGroup = ro.Code
		
		--LEFT JOIN ZoneRates zr ON zr.ZoneID = z.ZoneID AND zr.CustomerID = ro.CustomerID -- To check if SYS sell this zone to the customer

WHERE	ro.IsEffective = 'Y' AND ro.RouteOptions IS NOT NULL 
		--AND ro.RouteOptions <> 'BLK' 
		AND ro.BlockedSuppliers IS NULL			-- Excluding The block rules in the Override table		 
		AND ca.IsDeleted = 'N' AND ca.ActivationStatus <> (@Account_Inactive)
		AND ca.RoutingStatus NOT IN (@Account_Blocked, @Account_BlockedInbound) -- Select the active Customers only
		AND ro.IncludeSubCodes = 'Y' AND ro.Code <> '*ALL*' AND ro.Code IN (SELECT cg.Code FROM CodeGroup cg)-- CG-Rules: codes with sub-codes
		
)
,

-- Collect the Zone-Rule  
OverrideRulesForZones AS 
(
SELECT 
		ro.CustomerID,
		NULL Code,
		ro.OurZoneID ,
		ro.RouteOptions,
		ro.IncludeSubCodes,
		ro.ExcludedCodes,
		NULL ParentID
		,ro.Updated
		,ro.ExcludedCodes OriginalExcluded
		, '' GeneratedExcluded
		,ro.BeginEffectiveDate

FROM	RouteOverride ro  WITH(NOLOCK) 
		LEFT JOIN CarrierAccount ca  WITH(NOLOCK) ON ca.CarrierAccountID = ro.CustomerID
		
		LEFT JOIN ZoneRates zr  WITH(NOLOCK) ON zr.ZoneID = ro.OurZoneID AND zr.CustomerID = ro.CustomerID -- To check if SYS sell this zone to the customer

WHERE	ro.IsEffective = 'Y' AND ro.RouteOptions IS NOT NULL
		--AND ro.RouteOptions <> 'BLK' 
		AND ro.BlockedSuppliers IS NULL			-- Excluding The block rules in the Override table		 
		AND ca.IsDeleted = 'N' AND ca.ActivationStatus <> (@Account_Inactive)
		AND ca.RoutingStatus NOT IN (@Account_Blocked, @Account_BlockedInbound) -- Select the active Customers only
		AND ro.Code = '*ALL*' AND ro.OurZoneID <> -1								-- Zone-Rule

),

-- Resolve the conflict among Zone-Rules and the Generated Zone-Rules (from the Code group)
-- 1- Select the zone rule that not exist in the Generated Zone-Rules
ZoneRulesNotInCodeGroupRules AS 
(
	SELECT * FROM OverrideRulesForZones srz
	WHERE NOT EXISTS (
		SELECT * FROM OverrideRulesForCodeGroup scg 
		WHERE srz.OurZoneID = scg.OurZoneID AND srz.CustomerID = scg.CustomerID )
)
,
-- 2- Select the zone rule that exist in the Generated Zone-Rules but newer (in term of BED) 
ZoneRulesInCodeGroupRules_Newer AS 
(
	SELECT * FROM OverrideRulesForZones srz
	WHERE EXISTS (
		SELECT * FROM OverrideRulesForCodeGroup scg 
		WHERE srz.BeginEffectivedate > scg.BeginEffectivedate AND srz.OurZoneID = scg.OurZoneID AND srz.CustomerID = scg.CustomerID)
)
,
-- 3- Exclude the newer zone rule from the Generated Zone-Rules 
CodeGroupRulesNotInNewerZoneRules AS 
(
	SELECT * FROM OverrideRulesForCodeGroup scg
	WHERE NOT EXISTS (
		SELECT * FROM ZoneRulesInCodeGroupRules_Newer srz 
		WHERE srz.OurZoneID = scg.OurZoneID AND srz.CustomerID = scg.CustomerID)
)
,
-- Collect the Code-Rule 
OverrideRulesForCodes AS 
(
SELECT 
		ro.CustomerID,
		ro.Code,
		rcp.ZoneID OurZoneID ,
		ro.RouteOptions,
		ro.IncludeSubCodes,
		ro.ExcludedCodes,
		NULL ParentID
		,ro.Updated
		,ro.ExcludedCodes OriginalExcluded
		, '' GeneratedExcluded
		,ro.BeginEffectiveDate

FROM	RouteOverride ro  WITH(NOLOCK) 
		LEFT JOIN CarrierAccount ca  WITH(NOLOCK) ON ca.CarrierAccountID = ro.CustomerID
		LEFT JOIN RoutePool rcp  WITH(NOLOCK) ON rcp.Code = ro.Code
		
WHERE	ro.IsEffective = 'Y' AND ro.RouteOptions IS NOT NULL
		--AND ro.RouteOptions <> 'BLK' 
		AND ro.BlockedSuppliers IS NULL			-- Excluding The block rules in the Override table		 
		AND ca.IsDeleted = 'N' AND ca.ActivationStatus <> (@Account_Inactive)
		AND ca.RoutingStatus NOT IN (@Account_Blocked, @Account_BlockedInbound) -- Select the active Customers only
		AND ro.Code <> '*ALL*' AND ro.OurZoneID = -1								-- Code-Rule: Not a Zone-Rule				
		AND ro.IncludeSubCodes = 'N' --AND  ro.Code NOT IN (SELECT cg.Code FROM CodeGroup cg) AND rcp.Code LIKE ro.Code + '%')) -- Code-Rule: Not a CG-Rule		
		AND rcp.Code IS NOT NULL AND rcp.ZoneID IS NOT NULL				-- SYS provide this code and it has a zone
),

OverrideRulesForCodesWithSub AS 
(
SELECT 
		ro.CustomerID,
		rcp.Code,
		rcp.ZoneID OurZoneID ,
		ro.RouteOptions,
		ro.IncludeSubCodes,
		ro.ExcludedCodes,
		NULL ParentID
		,ro.Updated
		,ro.ExcludedCodes OriginalExcluded
		, '' GeneratedExcluded
		,ro.BeginEffectiveDate
		,(ROW_NUMBER() OVER (PARTITION BY ro.CustomerID, rcp.Code ORDER BY ro.Code DESC)) RowNumber
FROM	RouteOverride ro  WITH(NOLOCK) 
		LEFT JOIN CarrierAccount ca  WITH(NOLOCK) ON ca.CarrierAccountID = ro.CustomerID
		LEFT JOIN RoutePool rcp  WITH(NOLOCK) ON rcp.Code IS NOT NULL-- ro.Code
		
WHERE	ro.IsEffective = 'Y' AND ro.RouteOptions IS NOT NULL
		--AND ro.RouteOptions <> 'BLK' 
		AND ro.BlockedSuppliers IS NULL			-- Excluding The block rules in the Override table		 
		AND ca.IsDeleted = 'N' AND ca.ActivationStatus <> (@Account_Inactive)
		AND ca.RoutingStatus NOT IN (@Account_Blocked, @Account_BlockedInbound) -- Select the active Customers only
		AND ro.Code <> '*ALL*' AND ro.OurZoneID = -1								-- Code-Rule: Not a Zone-Rule				
		AND ro.IncludeSubCodes = 'Y' AND ro.Code NOT IN (SELECT cg.Code FROM CodeGroup cg) -- Code-Rule: Not a CG-Rule		
		AND rcp.Code IS NOT NULL AND rcp.ZoneID IS NOT NULL	
		 AND rcp.Code LIKE ro.Code + '%'
					-- SYS provide this code and it has a zone
)
,

CodeRules AS 
(
	SELECT scg.CustomerID,scg.Code, scg.OurZoneID, scg.RouteOptions, scg.IncludeSubCodes, scg.ExcludedCodes, scg.ParentID, scg.Updated, scg.OriginalExcluded,
	scg.GeneratedExcluded
		,scg.BeginEffectiveDate
	  FROM OverrideRulesForCodesWithSub scg
	WHERE  scg.RowNumber = 1 AND  NOT EXISTS (
		SELECT * FROM OverrideRulesForCodes srz 
		WHERE srz.code = scg.code AND srz.CustomerID = scg.CustomerID)
)

,

AllOverrideRules AS (
		SELECT * FROM CodeRules
	    UNION ALL
		SELECT * FROM OverrideRulesForCodes
		UNION ALL
		SELECT * FROM CodeGroupRulesNotInNewerZoneRules
		UNION ALL 
		SELECT * FROM ZoneRulesInCodeGroupRules_Newer
		UNION ALL 
		SELECT * FROM ZoneRulesNotInCodeGroupRules
)


-- Save the collected override rules for later manipulation
INSERT INTO #TempRules With(Tablock)
SELECT 
		ro.CustomerID,
		ro.Code,
		ro.OurZoneID,
		ro.RouteOptions,
		ro.IncludeSubCodes,
		ro.ExcludedCodes,
		ro.ParentID
		,ro.Updated
		,ro.OriginalExcluded
		,ro.GeneratedExcluded
		,0 HasSubZone
		,NULL SubCodes
		,NULL SubZoneIDs				-- Added to manage the Code-rules (with sub-codes) that its sub codes belong to more than one zone

 FROM AllOverrideRules ro
 
 
;with ParentRules AS (
SELECT *  FROM #TempRules orr )
,
ChildRules AS (
SELECT      tor.RuleID, tor.CustomerID, tor.Code, tor.OurZoneID, tor.RouteOptions,
       tor.IncludeSubCodes, tor.ExcludedCodes, par.RuleID ParentID, tor.MainExcluded,
       tor.OriginalExcluded, tor.HasSubZone, tor.SubCodes, tor.SubZoneIDs 
FROM  #TempRules      tor LEFT JOIN 
            ParentRules par ON tor.CustomerID = par.CustomerID 
WHERE tor.Code IS NOT NULL AND tor.Code LIKE (par.Code + '%') AND LEN( tor.Code) > LEN( par.code)
            AND par.IncludeSubCodes= 'Y'
),

ParentRulesWithoutChild AS 
(
	SELECT tmp.RuleID, tmp.CustomerID, tmp.Code, tmp.OurZoneID, tmp.RouteOptions,
	       tmp.IncludeSubCodes, tmp.ExcludedCodes, tmp.ParentID, tmp.MainExcluded,
	       tmp.OriginalExcluded, tmp.HasSubZone, tmp.SubCodes, tmp.SubZoneIDs
	  FROM #TempRules tmp 
	  LEFT JOIN ChildRules cr ON tmp.RuleID = cr.RuleID
           
	WHERE cr.RuleID IS NULL
),

AllRules AS(
SELECT * FROM ChildRules
UNION ALL
SELECT * FROM ParentRulesWithoutChild	
)

INSERT INTO #TempOverrideRules
SELECT * FROM AllRules
	
	
   SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Override Prepared Rules Inserted to Temp', @Message
		
		
CREATE NONCLUSTERED INDEX Temp_Customer ON #TempOverrideRules(CustomerID) INCLUDE(Code,OurZoneID,RouteOptions,RuleID,IncludeSubCodes)
		
-- For Each parent Code-rule:
--		Add to excluded codes of the parent rule, if the child rule doesn’t include sub codes, the code of the child Code-Rules 
--		Add to excluded codes of the parent rule, if the child rule include sub codes, 

INSERT INTO #DistinctParentRules With(Tablock)
SELECT DISTINCT(tor.RuleID)	,tor.Code,tor.ExcludedCodes,tor.MainExcluded,tor.OriginalExcluded
  FROM #TempOverrideRules tor 
	WHERE EXISTS (SELECT * FROM #TempOverrideRules ov WHERE  tor.RuleID = ov.ParentID) --AND LEN(tor.ExcludedCodes) > 0  
	order by tor.Code DESC			-- The DESCENDANT ordering is very important to allow managing the sub code-rules from the longest code to the shortest 

-- Using the GeneratedExcluded will help at every level to add the excluded codes generated excluded codes from the sub codes from all the sub levels
-- Every Parent code-rule will add to its excluded codes the sub child coe-rule codes and its subs except its orignal excluded codes

DECLARE @RuleID INT
DECLARE @ExcludedCodes varchar(max)
DECLARE	@Result varchar(max)
DECLARE @ResultWithSubCode varchar(max)
DECLARE @ResultWithOutSubCode varchar(max)
DECLARE DistinctRulesCursor CURSOR LOCAL FOR select RuleID from #DistinctParentRules
OPEN DistinctRulesCursor
FETCH NEXT FROM DistinctRulesCursor into @RuleID

WHILE @@FETCH_STATUS = 0
BEGIN
	
	SET @Result = NULL
	SET @ResultWithOutSubCode = NULL
	SET @ResultWithSubCode = NULL
	-- Add to excluded codes of the parent rule, if the child rule doesn’t include sub codes, the code of the child Code-Rules 
	select @ResultWithOutSubCode  = COALESCE(@ResultWithOutSubCode + ',', '') + Code  FROM  #TempOverrideRules where ParentID = @RuleID and IncludeSubCodes = 'N' 
	
	-- Add to excluded codes of the parent rule, if the child rule include sub codes,
	--		the code and it’s sub codes (without considering the zones of the codes) 
	--		which not exist in the original excluded of the child-rule(because codes from sub rules of the child rule could be added to the excluded code)
	select @ResultWithSubCode =  COALESCE(@ResultWithSubCode + ',', '') + rcp.Code  
								FROM  #TempOverrideRules tor 
								left join RoutePool rcp on rcp.Code collate Database_default like (tor.Code + '%')
								where	ParentID = @RuleID and IncludeSubCodes = 'Y' and 
										rcp.Code not in (select * from ParseArray(tor.OriginalExcluded,',')) -- and len(rcp.Code) >= len(tor.Code)
	
	
	set @Result =
	case when  @ResultWithOutSubCode IS Null then '' else(@ResultWithOutSubCode + ',') end
	  + case when @ResultWithSubCode IS NULL then '' else (@ResultWithSubCode + ',') end 
	
	set @Result = CASE WHEN len(@Result)-1 <=0 THEN '' ELSE  isnull(substring(@Result,1,len(@Result)-1),'') END
	
	UPDATE #TempOverrideRules SET ExcludedCodes = isnull(OriginalExcluded, '') + @Result WHERE RuleID = @RuleID	
	
    FETCH NEXT FROM DistinctRulesCursor into @RuleID
END

CLOSE DistinctRulesCursor
DEALLOCATE DistinctRulesCursor

 ;WITH TempEmp 
    AS
    (
    SELECT RuleID,ROW_NUMBER() OVER(PARTITION by RuleID, customerid, code, OurZoneID ORDER BY RuleID)
    AS duplicateRecCount
    FROM #TempOverrideRules
    )
   
    DELETE FROM TempEmp
    WHERE duplicateRecCount > 1 
   SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Override Curser Finished', @Message


END