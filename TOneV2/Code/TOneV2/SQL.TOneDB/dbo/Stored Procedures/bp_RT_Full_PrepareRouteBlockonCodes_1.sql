CREATE PROCEDURE [dbo].[bp_RT_Full_PrepareRouteBlockonCodes]
AS
BEGIN
	SET NOCOUNT ON;  
	
	CREATE TABLE #DistinctParentRules (RuleID INT,ParentCode varchar(20), ExcludedCodes VARCHAR(max), MainExcluded varchar(max), GeneratedExcluding varchar(max))
	
		
  INSERT INTO RouteBlockConcatinated
SELECT 	
								rb.RouteBlockID,
								rb.CustomerID,
								rb.SupplierID,
								rb.Code,
								rb.ZoneID,
								rb.UpdateDate,
								rb.IncludeSubCodes,
								rb.ExcludedCodes,
								NULL parentID,
								rb.ExcludedCodes		
											
															
																
							FROM   RouteBlock rb WITH (NOLOCK) 
							WHERE 
								rb.IsEffective = 'Y' 
								AND rb.CustomerID IS NULL 
								AND rb.SupplierID IS NOT NULL 
								AND rb.ZoneID IS NULL 
								AND rb.Code IS NOT NULL 

  
 UPDATE RouteBlockConcatinated SET parentID = (SELECT TOP 1 RouteBlockID FROM RouteBlockConcatinated cbr2 
                                               WHERE RouteBlockConcatinated.Code LIKE (cbr2.code + '%') AND LEN( RouteBlockConcatinated.Code) != LEN( cbr2.code) 
                                               AND RouteBlockConcatinated.SupplierID = cbr2.SupplierID ORDER BY code ASC) 
-- B.S
WHERE RouteBlockConcatinated.IncludeSubCodes= 'Y'
 
 
 INSERT INTO #DistinctParentRules With(Tablock)
SELECT DISTINCT(tor.RouteBlockID)	,tor.Code,tor.ExcludedCodes,'',tor.OriginalExcluded
  FROM RouteBlockConcatinated tor 
	WHERE EXISTS (SELECT * FROM RouteBlockConcatinated ov WHERE  tor.RouteBlockID = ov.ParentID) --AND LEN(tor.ExcludedCodes) > 0  
	order by tor.Code DESC	
 
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
	select @ResultWithOutSubCode  = COALESCE(@ResultWithOutSubCode + ',', '') + Code  FROM  RouteBlockConcatinated where ParentID = @RuleID and IncludeSubCodes = 'N' 
	
	-- Add to excluded codes of the parent rule, if the child rule include sub codes,
	--		the code and it’s sub codes (without considering the zones of the codes) 
	--		which not exist in the original excluded of the child-rule(because codes from sub rules of the child rule could be added to the excluded code)
	select @ResultWithSubCode =  COALESCE(@ResultWithSubCode + ',', '') + tor.Code  
								FROM  RouteBlockConcatinated tor 
								where	routeblockid = @RuleID	OR ParentID = @RuleID and IncludeSubCodes = 'Y' 

	
	set @Result =
	case when  @ResultWithOutSubCode IS Null then '' else(@ResultWithOutSubCode + ',') end
	  + case when @ResultWithSubCode IS NULL then '' else (@ResultWithSubCode + ',') end 
	
	set @Result = CASE WHEN len(@Result)-1 <=0 THEN ',' ELSE  isnull(substring(','+@Result,1,len(','+@Result)-1),'') END
	
	UPDATE RouteBlockConcatinated SET ExcludedCodes = isnull(OriginalExcluded, '') + @Result WHERE routeblockid = @RuleID	--OR ParentID = @RuleID
	
    FETCH NEXT FROM DistinctRulesCursor into @RuleID
END

CLOSE DistinctRulesCursor
DEALLOCATE DistinctRulesCursor
 
 
--DELETE FROM RouteBlockConcatinated WHERE ParentID IS NOT NULL
END