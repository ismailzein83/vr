CREATE PROCEDURE [bp].[sp_BPValidationMessage_GetBeforeID]
	@LessThanID BIGINT,
	@NbOfRows INT,
	@BPInstanceID INT
AS
BEGIN	            
	SELECT TOP(@NbOfRows) [ID]
						  ,[ProcessInstanceID]
						  ,[ParentProcessID]
						  ,[TargetKey]
						  ,[TargetType]
						  ,[Severity]
						  ,[Message]
	FROM [bp].[BPValidationMessage]  WITH(NOLOCK) 
	WHERE ID < @LessThanID 
	AND  ProcessInstanceID = @BPInstanceID
	ORDER BY ID DESC
END