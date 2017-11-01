CREATE PROCEDURE [bp].[sp_BPDefinitionSummary_GetUpdated]	
	@ExecutionStatusIds varchar(max)
AS
BEGIN
	DECLARE @ExecutionStatusTable TABLE (ExecutionStatus int)
            INSERT INTO @ExecutionStatusTable (ExecutionStatus)
            select Convert(int, ParsedString) from [bp].[ParseStringList](@ExecutionStatusIds)
 

SELECT [DefinitionID],count(*) as RunningProcessNumber , max(CreatedTime) as LastProcessCreatedTime
     
    FROM [BP].[BPInstance] WITH(NOLOCK) 
    WHERE (@ExecutionStatusIds is null  or ExecutionStatus in (select ExecutionStatus from @ExecutionStatusTable))
	GROUP BY [DefinitionID]
END