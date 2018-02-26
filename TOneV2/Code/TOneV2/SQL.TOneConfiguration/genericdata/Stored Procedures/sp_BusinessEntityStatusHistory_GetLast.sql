create PROCEDURE [genericdata].sp_BusinessEntityStatusHistory_GetLast
	@BusinessEntityDefinitionId uniqueidentifier,
	@BusinessEntityId varchar(50),
	@FieldName varchar(50)
AS
BEGIN
	
	SELECT TOP 1 ID ,BusinessEntityDefinitionId, BusinessEntityId,FieldName, StatusId, PreviousStatusID, StatusChangedDate, IsDeleted
	FROM  [genericdata].[BusinessEntityStatusHistory] 
	WHERE ISNULL(IsDeleted,0) = 0 
	AND   BusinessEntityDefinitionId = @BusinessEntityDefinitionId
	AND   BusinessEntityId = @BusinessEntityId
	AND   FieldName = @FieldName
	ORDER BY StatusChangedDate DESC
END