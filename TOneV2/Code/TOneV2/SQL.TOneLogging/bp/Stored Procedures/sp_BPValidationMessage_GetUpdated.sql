CREATE PROCEDURE [bp].[sp_BPValidationMessage_GetUpdated]
	@NbOfRows INT,
	@GreaterThanID BIGINT,
	@BPInstanceID INT
AS
BEGIN
	
IF (@GreaterThanID IS NULL)
	BEGIN
	SELECT TOP(@NbOfRows) [ID]
						  ,[ProcessInstanceID]
						  ,[ParentProcessID]
						  ,[TargetKey]
						  ,[TargetType]
						  ,[Severity]
						  ,[Message]
            INTO #temp_table
			FROM [bp].[BPValidationMessage] WITH(NOLOCK) 
            WHERE ProcessInstanceID  = @BPInstanceID
            ORDER BY ID DESC
            
            SELECT * FROM #temp_table
	END
	
	ELSE
	BEGIN
	SELECT TOP(@NbOfRows) [ID]
						  ,[ProcessInstanceID]
						  ,[ParentProcessID]
						  ,[TargetKey]
						  ,[TargetType]
						  ,[Severity]
						  ,[Message]
            INTO #temp2_table
            FROM [bp].[BPValidationMessage] WITH(NOLOCK) 
			WHERE ProcessInstanceID  = @BPInstanceID 
			AND ID >@GreaterThanID
            ORDER BY ID
            
            SELECT * FROM #temp2_table
	END
END