
CREATE PROCEDURE [integration].[sp_DataSourceRuntimeInstance_TryGetAndLock]
	@CurrentRuntimeProcessID int
AS
BEGIN
	
	DECLARE @ID UNIQUEIDENTIFIER, @IsLocked bit
	SELECT TOP 1 @ID = ID FROM integration.DataSourceRuntimeInstance WITH(NOLOCK) 
	WHERE LockedByProcessID IS NULL
	ORDER BY CreatedTime
	
	IF (@ID IS NOT NULL)
	BEGIN
		UPDATE integration.DataSourceRuntimeInstance
		SET 
			LockedByProcessID = @CurrentRuntimeProcessID,
			@IsLocked = 1
		WHERE ID = @ID AND LockedByProcessID IS NULL
	END
	
	SELECT [ID]
      ,[DataSourceID]
    FROM integration.DataSourceRuntimeInstance
    WHERE ID = @ID AND @IsLocked = 1
	
END