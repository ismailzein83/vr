CREATE PROCEDURE [genericdata].[sp_BusinessEntityHistoryStack_GetLast]
	@BusinessEntityDefinitionId uniqueidentifier,
	@BusinessEntityId varchar(50),
	@FieldName varchar(50)
AS
BEGIN
	
	SELECT TOP 1 ID ,BusinessEntityDefinitionId, BusinessEntityId,FieldName, StatusId, PreviousStatusID, StatusChangedDate, IsDeleted,MoreInfo,PreviousMoreInfo
	FROM  [genericdata].[BusinessEntityHistoryStack] 
	WHERE ISNULL(IsDeleted,0) = 0 
	AND   BusinessEntityDefinitionId = @BusinessEntityDefinitionId
	AND   BusinessEntityId = @BusinessEntityId
	AND   FieldName = @FieldName
	ORDER BY StatusChangedDate DESC
END