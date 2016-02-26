
CREATE PROCEDURE [integration].[sp_DataSourceRuntimeInstance_TryGetAndLock]
	@CurrentRuntimeProcessID int
AS
BEGIN
	
	DECLARE @ID UNIQUEIDENTIFIER
	IF EXISTS (SELECT TOP 1 ID FROM integration.DataSourceRuntimeInstance WHERE LockedByProcessID IS NULL)
	BEGIN
		UPDATE integration.DataSourceRuntimeInstance
		SET 
			@ID = ID,
			LockedByProcessID = @CurrentRuntimeProcessID
		WHERE ID = (SELECT TOP 1 ID FROM integration.DataSourceRuntimeInstance WHERE LockedByProcessID IS NULL)
	END
	
	SELECT [ID]
      ,[DataSourceID]
    FROM integration.DataSourceRuntimeInstance
    WHERE ID = @ID
	
END