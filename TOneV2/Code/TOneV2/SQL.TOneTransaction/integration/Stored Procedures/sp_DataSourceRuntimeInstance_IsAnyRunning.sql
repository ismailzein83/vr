
CREATE PROCEDURE [integration].[sp_DataSourceRuntimeInstance_IsAnyRunning]
	@DataSourceID uniqueidentifier,
	@RunningRuntimeProcessIDs VARCHAR(MAX) = NULL
AS
BEGIN

	DECLARE @RunningRuntimeProcessIDsTable TABLE (ID int)
	INSERT INTO @RunningRuntimeProcessIDsTable (ID)
	select Convert(int, ParsedString) from [integration].[ParseStringList](@RunningRuntimeProcessIDs)
	
	IF EXISTS (SELECT TOP 1 NULL FROM integration.DataSourceRuntimeInstance
				WHERE DataSourceID = @DataSourceID
					AND ISNULL(IsCompleted, 0) = 0--not completed
					AND (LockedByProcessID IS NULL OR LockedByProcessID IN (SELECT ID FROM @RunningRuntimeProcessIDsTable))--locked by a running process
				)
	SELECT 1
	ELSE
	SELECT 0
		
END