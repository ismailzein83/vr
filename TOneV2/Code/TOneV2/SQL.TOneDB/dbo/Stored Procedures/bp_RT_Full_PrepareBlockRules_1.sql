

CREATE PROCEDURE [dbo].[bp_RT_Full_PrepareBlockRules]

WITH RECOMPILE
AS
BEGIN
	
CREATE TABLE #DistinctParentRules (RuleID INT, ExcludedCodes VARCHAR(max))
CREATE TABLE #TempRules ( 
		RuleID INT IDENTITY(1,1),
		CustomerID VARCHAR(5),
		SupplierID VARCHAR(5),
		Code VARCHAR(20),
		ZoneID INT,
		UpdateDate DATETIME,
		IncludeSubCodes CHAR(1),
		ExcludedCodes VARCHAR(max),
		ParentID INT,
		MainExcluded varchar(max),
		OriginalExcluded varchar(max),
		--HasSubZone BIT,
		SubZOneIDs VARCHAR(MAX),
)
DECLARE @Message varchar(500) 

;WITH 
BlockedRules AS (
   SELECT * FROM dbo.fn_RT_Full_GetBlockRulesFromOverrides('Code',-2,'','RouteOption') gbrfo --WHERE gbrfo.CustomerID = 'c071' AND gbrfo.Code LIKE '963'
)
--SELECT * FROM BlockedRules
,

BlockCodeRulesWithoutDuplication AS(
SELECT	br.* 
FROM	BlockedRules br LEFT JOIN 
		(SELECT		br2.CustomerID,br2.SupplierID,br2.Code,MAX(br2.UpdateDate) MaxDate 
		 FROM BlockedRules br2
		where		br2.Code IS NOT NULL
		GROUP BY	br2.CustomerID,br2.SupplierID,br2.Code
		) br2 ON	br2.CustomerID=br.CustomerID AND  br2.SupplierID = br.SupplierID and br2.Code = br.Code and  br2.MaxDate = br.UpdateDate 	
)
--SELECT * FROM BlockCodeRulesWithoutDuplication
,
BlockZoneRulesForCodeGroup AS (
SELECT 
		ro.CustomerID,
		NULL Code,
		z.ZoneID,
		--ro.ZoneID ,
		ro.SupplierID,
		ro.IncludeSubCodes,
		ro.ExcludedCodes,
		NULL ParentID
		--,ro.Updated
		,ro.ExcludedCodes MainExcluded
		, '' OriginalExcluded
		,ro.UpdateDate
		--,ro.BeginEffectiveDate
FROM	BlockCodeRulesWithoutDuplication ro 
		LEFT JOIN RouteOptionsPool cs  WITH(NOLOCK) ON cs.SupplierID = ro.SupplierID AND cs.Code = ro.Code
		LEFT JOIN CodeGroup cg  WITH(NOLOCK) ON cg.Code = ro.code
		LEFT JOIN Zone z  WITH(NOLOCK) ON z.CodeGroup = ro.Code --AND z.SupplierID = ro.SupplierID

WHERE	ro.IncludeSubCodes = 'Y' 
		--AND cs.SupplierZoneID IS NOT NULL
		AND z.SupplierID = ro.SupplierID
		AND z.IsEffective = 'Y' AND ro.code IS NOT NULL AND cg.Code IS NOT NULL
)
--SELECT * FROM BlockZoneRulesForCodeGroup
,

ZoneRules AS (
SELECT	br2.CustomerID,br2.Code,br2.ZoneID,br2.SupplierID,br2.IncludeSubCodes,br2.ExcludedCodes,NULL ParentID
		--,ro.Updated
		,br2.ExcludedCodes MainExcluded, '' OriginalExcluded,br2.UpdateDate
FROM	BlockedRules br2
WHERE	br2.ZoneID IS NOT NULL AND br2.zoneid <> - 1	
),

BlockZoneRuleForCodeGroupNotInZoneRule AS (
SELECT	cg.* 
FROM	BlockZoneRulesForCodeGroup cg left JOIN 
		ZoneRules zr ON zr.CustomerID = cg.CustomerID AND zr.ZoneID= cg.ZoneID AND zr.SupplierID = cg.SupplierID
WHERE	zr.CustomerID IS NULL AND zr.ZoneID IS NULL AND zr.SupplierID IS NULL	
)
,
ALLZoneRules AS (
SELECT * FROM ZoneRules
UNION ALL
SELECT * FROM  BlockZoneRuleForCodeGroupNotInZoneRule	
)
,

BlockRulesForCodes AS (
SELECT 
		ro.CustomerID,
		ro.Code,
		cs.SupplierZoneID ZoneID ,
		ro.SupplierID,
		ro.IncludeSubCodes,
		ro.ExcludedCodes,
		NULL ParentID
		--,ro.Updated
		,ro.ExcludedCodes MainExcluded
		, '' OriginalExcluded
		,ro.UpdateDate
		--,ro.BeginEffectiveDate
FROM	BlockCodeRulesWithoutDuplication ro 
		LEFT JOIN RouteOptionsPool cs  WITH(NOLOCK) ON cs.Code = ro.Code AND cs.SupplierID = ro.SupplierID
		LEFT JOIN CodeGroup cg  WITH(NOLOCK) ON cg.Code = ro.Code
WHERE	ro.Code IS NOT NULL
		AND (ro.ZoneID IS NULL OR ro.zoneid = -1) 
		AND (ro.IncludeSubCodes = 'N' OR( ro.IncludeSubCodes = 'Y' AND cg.Code IS NULL))
		--AND ro.Code NOT IN (SELECT cg.Code FROM CodeGroup cg)
),

AllBlockRules AS (
	SELECT * FROM BlockRulesForCodes
	UNION ALL
	SELECT * FROM ALLZoneRules
)

--1.	Get the rule list ( Code-rules with/without Zone-Rules) Not Code Group
--4.	Set the zone of the Code-rules if not exist (either cost or sale zone)


INSERT INTO #TempRules With(Tablock)
SELECT 
		ro.CustomerID,
		ro.SupplierID,
		ro.Code,
		ro.ZoneID,
		GETDATE() UpdateDate,
		ro.IncludeSubCodes,
		ro.ExcludedCodes,
		ro.ParentID
		--,ro.Updated
		,ro.MainExcluded
		,ro.OriginalExcluded
		,'' SubZoneIDs
FROM	AllBlockRules ro

;with ParentRules AS (
SELECT *  FROM #TempRules orr )
,
ChildRules AS (
SELECT      tor.RuleID, tor.CustomerID, tor.SupplierID, tor.Code, tor.ZoneID,
       tor.UpdateDate, tor.IncludeSubCodes, tor.ExcludedCodes, Par.RuleID ParentID,
       tor.MainExcluded, tor.OriginalExcluded, tor.SubZOneIDs
FROM  #TempRules      tor LEFT JOIN 
            ParentRules par ON tor.CustomerID = par.CustomerID aND par.SupplierID = tor.SupplierID
WHERE tor.Code IS NOT NULL AND tor.Code LIKE (par.Code + '%') AND LEN( tor.Code) > LEN( par.code)
            AND par.IncludeSubCodes= 'Y' 
),

ParentRulesWithoutChild AS 
(
	SELECT tmp.RuleID, tmp.CustomerID, tmp.SupplierID, tmp.Code, tmp.ZoneID,
	       tmp.UpdateDate, tmp.IncludeSubCodes, tmp.ExcludedCodes, tmp.ParentID,
	       tmp.MainExcluded, tmp.OriginalExcluded, tmp.SubZOneIDs
	  FROM #TempRules tmp 
	  LEFT JOIN ChildRules cr ON tmp.RuleID = cr.RuleID
           
	WHERE cr.RuleID IS NULL
),

AllRules AS(
SELECT * FROM ChildRules
UNION ALL
SELECT * FROM ParentRulesWithoutChild	
)



INSERT INTO #TempBlockRules
SELECT * FROM AllRules

-- CREATE NONCLUSTERED INDEX Temp_Code ON #TempBlockRules(CustomerID,SupplierID, RuleID) INCLUDE(Code) 


SET @Message = CONVERT(varchar, getdate(), 121)
EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Block Prepared Rules Inserted to Temp', @Message

INSERT INTO #DistinctParentRules With(Tablock)
SELECT DISTINCT(tor.RuleID),tor.ExcludedCodes
		  FROM #TempBlockRules tor WHERE EXISTS (SELECT * FROM #TempBlockRules ov WHERE  tor.RuleID = ov.ParentID)-- AND LEN(tor.ExcludedCodes) > 0
  

DECLARE @RuleID INT
DECLARE @ExcludedCodes varchar(max)
DECLARE	@Result varchar(max)
DECLARE @ResultWithSubCode varchar(max)
DECLARE @ResultWithOutSubCode varchar(max)
DECLARE @ExcludedChildCodes VARCHAR(MAX)

  
set @RuleID = NULL
SET  @ExcludedCodes = NULL 

DECLARE DistinctRulesCursor CURSOR LOCAL FOR select RuleID,ExcludedCodes from #DistinctParentRules
OPEN DistinctRulesCursor
FETCH NEXT FROM DistinctRulesCursor into @RuleID,@ExcludedCodes

WHILE @@FETCH_STATUS = 0
BEGIN
		
	SET @Result = NULL
	SET @ResultWithOutSubCode = NULL
	SET @ResultWithSubCode = NULL
	select @ResultWithOutSubCode  = COALESCE(@ResultWithOutSubCode + ',', '') + Code  FROM  #TempBlockRules where ParentID = @RuleID and IncludeSubCodes = 'N' 
	
	-- Add to excluded codes of the parent rule, if the child rule include sub codes,
	--		the code and it’s sub codes (without considering the zones of the codes) 
	--		which not exist in the original excluded of the child-rule(because codes from sub rules of the child rule could be added to the excluded code)
	select @ResultWithSubCode =  COALESCE(@ResultWithSubCode + ',', '') + rcp.Code  
								FROM  #TempBlockRules tor 
								left join RoutePool rcp on rcp.Code collate Database_default like (tor.Code + '%')
								where	ParentID = @RuleID and IncludeSubCodes = 'Y' and 
										rcp.Code not in (select * from ParseArray(tor.OriginalExcluded,',')) -- and len(rcp.Code) >= len(tor.Code)
	

	set @Result =
	case when  @ResultWithOutSubCode IS Null then '' else(@ResultWithOutSubCode + ',') end
	  + case when @ResultWithSubCode IS NULL then '' else (@ResultWithSubCode + ',') end 
	
	set @Result = CASE WHEN len(@Result)-1 <=0 THEN '' ELSE  isnull(substring(@Result,1,len(@Result)-1),'') END
	
	UPDATE #TempBlockRules SET ExcludedCodes = isnull(OriginalExcluded, '') + @Result WHERE RuleID = @RuleID	
		
    FETCH NEXT FROM DistinctRulesCursor into @RuleID,@ExcludedCodes
END

CLOSE DistinctRulesCursor
DEALLOCATE DistinctRulesCursor

   SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Block Curser Finished', @Message

DELETE FROM #TempBlockRules WHERE ParentID IS NOT NULL
	--Remove Duplicate Rules
   ;WITH TempEmp 
    AS
    (
    SELECT RuleID,ROW_NUMBER() OVER(PARTITION by RuleID, customerid, code, zoneid ORDER BY RuleID)
    AS duplicateRecCount
    FROM #TempBlockRules
    )
   
    DELETE FROM TempEmp
    WHERE duplicateRecCount > 1 

END