
CREATE PROCEDURE [dbo].[bp_RT_Full_PrepareSpecialRequestRules]

WITH RECOMPILE
AS
BEGIN
CREATE TABLE #DistinctParentRules (RuleID INT,ParentCode varchar(20), ExcludedCodes VARCHAR(max), MainExcluded varchar(max), GeneratedExcluding varchar(max))
 

 CREATE TABLE #TempRules (
 	RuleID INT IDENTITY(1,1),
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

DECLARE @Message varchar(500) 

;WITH 

GroupedSpecialRequestForCodeGroup AS 
(    
SELECT sr.CustomerID
,NULL Code
,z.ZoneID
,sr.BeginEffectiveDate
,sr.EndEffectiveDate
,sr.IsEffective
,sr.ExcludedCodes
,sr.IncludeSubCodes,
STUFF((
        SELECT N'|' 
				+ [SupplierID]  + ',' 
				+ [Priority] + ',' 
				+ CAST( [NumberOfTries] AS VARCHAR(3))+ ',' 
				+ CAST( [SpecialrequestType] AS VARCHAR(1))+ ',' 
				+ CAST( [Percentage] AS VARCHAR(3))
        FROM 
        
        (
        SELECT 
				DISTINCT([SupplierID])  ,
				CASE WHEN LEN([Priority]) = 1 THEN '0' + CAST([Priority] AS VARCHAR(2)) ELSE CAST([Priority] AS VARCHAR(2)) END [Priority],
				 [NumberOfTries]  ,
				 [SpecialrequestType],
				 [Percentage] 
        FROM SpecialRequest sr2
         LEFT JOIN CarrierAccount ca ON ca.CarrierAccountID = sr2.SupplierID
				WHERE 
				CustomerID = sr.CustomerID 
				AND Code = sr.Code 
				AND sr2.IsEffective = 'Y'
				AND sr2.ZoneID IS NULL
				AND ca.IsDeleted = 'N' 
			AND ca.ActivationStatus IN (1,2)
			AND ca.RoutingStatus IN (1,3) 
			and sr2.timestamp = (SELECT MAX(timestamp) from SpecialRequest sr4 WHERE sr4.CustomerID = sr2.CustomerID AND sr2.SupplierID = sr4.SupplierID 
			AND sr4.Code = sr2.Code  )

        ) sr3     

       FOR XML PATH ('')), 1, 1, '') AS SupplierOptions
  FROM SpecialRequest sr  WITH(NOLOCK) 
    
  LEFT JOIN CarrierAccount ca  WITH(NOLOCK) ON ca.CarrierAccountID = sr.CustomerID
  
   LEFT JOIN Zone z  WITH(NOLOCK) ON z.CodeGroup = sr.Code

  INNER JOIN (
  	SELECT sr2.CustomerID,sr2.Code,sr2.ZoneID,MAX(sr2.SpecialRequestID) RuleID FROM SpecialRequest sr2  WITH(NOLOCK) 
  	WHERE sr2.IsEffective = 'Y' AND sr2.ZoneID IS  NULL 

GROUP BY sr2.CustomerID,sr2.Code,sr2.ZoneID
  ) srg ON srg.CustomerID = sr.CustomerID AND srg.Code = sr.Code AND sr.SpecialRequestID = srg.RuleID	
 

WHERE sr.ZoneID IS  NULL AND sr.IsEffective = 'Y'
 			AND ca.IsDeleted = 'N' 
			AND ca.ActivationStatus IN (1,2)
			AND ca.RoutingStatus IN (2,3) 
			AND sr.Code IN (SELECT cg.Code FROM CodeGroup cg)
			AND z.SupplierID = 'SYS'
			AND z.IsEffective = 'Y'
			AND sr.IncludeSubCodes = 'Y'
			--AND sr.CustomerID = 'C152' AND sr.Code LIKE '963%'
),

GroupedSpecialRequestForZones AS 
(  
  SELECT sr.CustomerID
,sr.Code
,sr.ZoneID
,sr.BeginEffectiveDate
,sr.EndEffectiveDate
,sr.IsEffective
,sr.ExcludedCodes
,sr.IncludeSubCodes,
STUFF((
        SELECT N'|' 
				+ [SupplierID]  + ',' 
				+ [Priority]+ ',' 
				+ CAST( [NumberOfTries] AS VARCHAR(3))+ ',' 
				+ CAST( [SpecialrequestType] AS VARCHAR(1))+ ',' 
				+ CAST( [Percentage] AS VARCHAR(3))
        FROM 
        
        (
        SELECT 
				DISTINCT([SupplierID])  ,
				CASE WHEN LEN([Priority]) = 1 THEN '0' + CAST([Priority] AS VARCHAR(2)) ELSE CAST([Priority] AS VARCHAR(2)) END [Priority],
				 [NumberOfTries]  ,
				 [SpecialrequestType],
				 [Percentage] 
        FROM SpecialRequest sr2
         LEFT JOIN CarrierAccount ca ON ca.CarrierAccountID = sr2.SupplierID
				WHERE 
				sr2.CustomerID = sr.CustomerID 
				AND sr2.ZoneID = sr.ZoneID 
				AND sr2.IsEffective = 'Y'
				AND sr2.Code IS NULL
				AND ca.IsDeleted = 'N' 
			AND ca.ActivationStatus IN (1,2)
			AND ca.RoutingStatus IN (1,3) 
			and sr2.timestamp = (SELECT MAX(timestamp) from SpecialRequest sr4 WHERE sr4.CustomerID = sr2.CustomerID AND sr2.SupplierID = sr4.SupplierID 
			AND sr4.ZoneID = sr2.ZoneID  )

        ) sr3     

       FOR XML PATH ('')), 1, 1, '') AS SupplierOptions
  FROM SpecialRequest sr  WITH(NOLOCK) 
    
  LEFT JOIN CarrierAccount ca  WITH(NOLOCK) ON ca.CarrierAccountID = sr.CustomerID
  
   INNER JOIN (
  	SELECT sr2.CustomerID,sr2.Code,sr2.ZoneID,MAX(sr2.SpecialRequestID) RuleID  FROM SpecialRequest sr2 WITH(NOLOCK) 
  	WHERE sr2.IsEffective = 'Y' AND sr2.Code IS  NULL
GROUP BY sr2.CustomerID,sr2.Code,sr2.ZoneID
  ) srg ON srg.CustomerID = sr.CustomerID AND srg.ZoneID = sr.ZoneID AND sr.SpecialRequestID = srg.RuleID	
 

WHERE sr.ZoneID IS NOT NULL AND sr.IsEffective = 'Y'
 			AND ca.IsDeleted = 'N' 
			AND ca.ActivationStatus IN (1,2)
			AND ca.RoutingStatus IN (2,3) 
			--AND sr.CustomerID = 'C152' AND sr.Code LIKE '963%'
)
,

ZoneRulesNotInCodeGroupRules AS 

(

SELECT srz.* FROM GroupedSpecialRequestForZones srz
LEFT JOIN GroupedSpecialRequestForCodeGroup scg ON srz.ZoneID = scg.ZoneID AND srz.CustomerID = scg.CustomerID
WHERE  scg.ZoneID IS NULL AND  scg.CustomerID IS NULL
)
,

ZoneRulesInCodeGroupRules_Newer AS 

(

SELECT * FROM GroupedSpecialRequestForZones srz
WHERE EXISTS (SELECT * FROM GroupedSpecialRequestForCodeGroup scg WHERE srz.BeginEffectivedate > scg.BeginEffectivedate AND srz.ZoneID = scg.ZoneID AND srz.CustomerID = scg.CustomerID)

),

CodeGroupRulesNotInNewerZoneRules AS 
(
	

SELECT scg.* FROM GroupedSpecialRequestForCodeGroup scg
LEFT JOIN ZoneRulesInCodeGroupRules_Newer srz ON srz.ZoneID = scg.ZoneID AND srz.CustomerID = scg.CustomerID
WHERE srz.ZoneID IS NULL AND srz.CustomerID IS NULL
),
GroupedSpecialRequestForCodes AS 
(

SELECT sr.CustomerID
,sr.Code
,rcp.ZoneID ZoneID
,sr.BeginEffectiveDate
,sr.EndEffectiveDate
,sr.IsEffective
,sr.ExcludedCodes
,sr.IncludeSubCodes,
STUFF((
        SELECT N'|' 
				+ [SupplierID]  + ',' 
				+ [Priority] + ',' 
				+ CAST( [NumberOfTries] AS VARCHAR(3))+ ',' 
				+ CAST( [SpecialrequestType] AS VARCHAR(1))+ ',' 
				+ CAST( [Percentage] AS VARCHAR(3))
        FROM 
        
        (
        SELECT DISTINCT([SupplierID])  ,
				CASE WHEN LEN([Priority]) = 1 THEN '0' + CAST([Priority] AS VARCHAR(2)) ELSE CAST([Priority] AS VARCHAR(2)) END [Priority],
				 [NumberOfTries]  ,
				 [SpecialrequestType],
				 [Percentage] 
        FROM SpecialRequest sr2
         LEFT JOIN CarrierAccount ca ON ca.CarrierAccountID = sr2.SupplierID
				WHERE 
				CustomerID = sr.CustomerID 
				AND Code = sr.Code 
				AND sr2.IsEffective = 'Y'
				AND sr2.ZoneID IS NULL
				AND ca.IsDeleted = 'N' 
			AND ca.ActivationStatus IN (1,2)
			AND ca.RoutingStatus IN (1,3) 
			and sr2.timestamp = (SELECT MAX(timestamp) from SpecialRequest sr4 WHERE sr4.CustomerID = sr2.CustomerID AND sr2.SupplierID = sr4.SupplierID 
				AND sr4.Code = sr2.Code  )


        ) sr3     

       FOR XML PATH ('')), 1, 1, '') AS SupplierOptions
  FROM SpecialRequest sr  WITH(NOLOCK) 
    
  LEFT JOIN CarrierAccount ca  WITH(NOLOCK) ON ca.CarrierAccountID = sr.CustomerID
  LEFT JOIN RoutePool rcp  WITH(NOLOCK) ON rcp.Code = sr.Code
  INNER JOIN (
  	SELECT sr2.CustomerID,sr2.Code,sr2.ZoneID,MAX(sr2.SpecialRequestID) RuleID
  	FROM SpecialRequest sr2  WITH(NOLOCK) 
  	WHERE sr2.IsEffective = 'Y' AND sr2.ZoneID IS  NULL 
   GROUP BY sr2.CustomerID,sr2.Code,sr2.ZoneID
  ) srg ON srg.CustomerID = sr.CustomerID AND srg.Code = sr.Code AND sr.SpecialRequestID = srg.ruleid	
 

WHERE sr.ZoneID IS  NULL AND sr.IsEffective = 'Y' AND rcp.ZoneID IS NOT NULL
 			AND ca.IsDeleted = 'N' 
			AND ca.ActivationStatus IN (1,2)
			AND ca.RoutingStatus IN (2,3) 
			AND (sr.IncludeSubCodes = 'N' OR( sr.IncludeSubCodes = 'Y' AND sr.Code NOT IN (SELECT cg.Code FROM CodeGroup cg)))
			--AND sr.CustomerID = 'C152' AND sr.Code LIKE '963%'

),

ALLGroupedRules AS (
	SELECT * FROM GroupedSpecialRequestForCodes
	UNION ALL
	SELECT * FROM CodeGroupRulesNotInNewerZoneRules
	UNION ALL 
	SELECT * FROM ZoneRulesInCodeGroupRules_Newer
	UNION ALL 
	SELECT * FROM ZoneRulesNotInCodeGroupRules
)

INSERT INTO #TempRules With(Tablock) (
		[CustomerID]
      ,[Code]
      ,[ZoneID]
      ,[SupplierOptions]
      ,[IncludeSubCodes]
      ,[ExcludedCodes]
      ,[ParentID]
      ,[MainExcluded]
      ,[OriginalExcluded]
     -- ,HasSubZone
) 

SELECT
	  cg.[CustomerID]
      ,cg.[Code]
      ,cg.[ZoneID]
      ,cg.[SupplierOptions]
      ,cg.[IncludeSubCodes]
      ,cg.[ExcludedCodes]
      ,NULL
      ,NULL
      ,NULL
      --,0
 FROM ALLGroupedRules cg
WHERE cg.SupplierOptions IS NOT NULL



;with ParentRules AS (
SELECT *  FROM #TempRules orr )
,
ChildRules AS (
SELECT      tor.RuleID, tor.CustomerID, tor.Code, tor.ZoneID, tor.SupplierOptions,
       tor.IncludeSubCodes, tor.ExcludedCodes, par.RuleID ParentID, tor.MainExcluded,
       tor.OriginalExcluded, tor.HasSubZone, tor.SubCodes, tor.SubZoneIDs 
FROM  #TempRules      tor LEFT JOIN 
            ParentRules par ON tor.CustomerID = par.CustomerID
WHERE tor.Code IS NOT NULL AND tor.Code LIKE (par.Code + '%') AND LEN( tor.Code) > LEN( par.code)
           AND par.SupplierOptions = tor.SupplierOptions 
           AND par.IncludeSubCodes= 'Y'
)
--SELECT * FROM ChildRules
,

ParentRulesWithoutChild AS 
(
	SELECT tmp.RuleID, tmp.CustomerID, tmp.Code, tmp.ZoneID, tmp.SupplierOptions,
	       tmp.IncludeSubCodes, tmp.ExcludedCodes, tmp.ParentID, tmp.MainExcluded,
	       tmp.OriginalExcluded, tmp.HasSubZone, tmp.SubCodes, tmp.SubZoneIDs
	  FROM #TempRules tmp 
	  LEFT JOIN ChildRules cr ON cr.CustomerID = tmp.CustomerID AND cr.Code = tmp.Code AND cr.ZoneID = tmp.ZoneID AND cr.SupplierOptions = tmp.SupplierOptions
           
	WHERE cr.CustomerID is null AND cr.Code IS NULL AND cr.ZoneID IS NULL AND cr.SupplierOptions IS NULL

)
--SELECT * FROM ParentRulesWithoutChild
,

AllRules AS(
SELECT * FROM ChildRules
UNION ALL
SELECT * FROM ParentRulesWithoutChild	
)

INSERT INTO #TempSpecialRequestRules
SELECT * FROM AllRules
SET @Message = CONVERT(varchar, getdate(), 121)
EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Special Request Rules Inserted', @Message


INSERT INTO #DistinctParentRules With(Tablock)
SELECT DISTINCT(tor.RuleID),tor.Code,tor.ExcludedCodes,tor.MainExcluded,tor.OriginalExcluded
  FROM #TempSpecialRequestRules tor WHERE EXISTS (SELECT * FROM #TempSpecialRequestRules ov WHERE  tor.RuleID = ov.ParentID) --AND LEN(tor.ExcludedCodes) > 0
  
DECLARE @RuleID INT
DECLARE @ExcludedCodes varchar(max)
DECLARE	@Result varchar(max)
DECLARE @ResultWithSubCode varchar(max)
DECLARE @ResultWithOutSubCode varchar(max)

DECLARE DistinctRulesCursor CURSOR LOCAL FOR select RuleID
                                             from #DistinctParentRules
OPEN DistinctRulesCursor
FETCH NEXT FROM DistinctRulesCursor into @RuleID
DECLARE @ExcludedChildCodes VARCHAR(MAX)

WHILE @@FETCH_STATUS = 0
BEGIN
	SET @Result = NULL
	SET @ResultWithOutSubCode = NULL
	SET @ResultWithSubCode = NULL
	-- Add to excluded codes of the parent rule, if the child rule doesn’t include sub codes, the code of the child Code-Rules 
	select @ResultWithOutSubCode  = COALESCE(@ResultWithOutSubCode + ',', '') + Code  FROM  #TempSpecialRequestRules where ParentID = @RuleID and IncludeSubCodes = 'N' 
	
	-- Add to excluded codes of the parent rule, if the child rule include sub codes,
	--		the code and it’s sub codes (without considering the zones of the codes) 
	--		which not exist in the original excluded of the child-rule(because codes from sub rules of the child rule could be added to the excluded code)
	select @ResultWithSubCode =  COALESCE(@ResultWithSubCode + ',', '') + rcp.Code  
								FROM  #TempSpecialRequestRules tor 
								left join RoutePool rcp on rcp.Code collate Database_default like (tor.Code + '%')
								where	tor.ParentID = @RuleID and tor.IncludeSubCodes = 'Y' and 
										rcp.Code not in (select * from ParseArray(tor.OriginalExcluded,',')) -- and len(rcp.Code) >= len(tor.Code)
	
	
	set @Result =
	case when  @ResultWithOutSubCode IS Null then '' else(@ResultWithOutSubCode + ',') end
	  + case when @ResultWithSubCode IS NULL then '' else (@ResultWithSubCode + ',') end 
	
	set @Result = CASE WHEN len(@Result)-1 <=0 THEN '' ELSE  isnull(substring(@Result,1,len(@Result)-1),'') END

	UPDATE #TempSpecialRequestRules SET ExcludedCodes = isnull(OriginalExcluded, '') + @Result WHERE RuleID = @RuleID	
		
    FETCH NEXT FROM DistinctRulesCursor into @RuleID


END

CLOSE DistinctRulesCursor
DEALLOCATE DistinctRulesCursor


	--Remove Duplicate Rules
   ;WITH TempEmp 
    AS
    (
    SELECT ROW_NUMBER() OVER(PARTITION by  RuleId,customerid, code, zoneid ORDER BY RuleID)
    AS duplicateRecCount
    FROM #TempSpecialRequestRules
    )
   
    DELETE FROM TempEmp
    WHERE duplicateRecCount > 1 

  SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Special Request Curser Finished', @Message
SELECT * FROM #TempSpecialRequestRules
-- B.S no delete for the child rules
--DELETE FROM #TempSpecialRequestRules WHERE ParentID IS NOT NULL

END