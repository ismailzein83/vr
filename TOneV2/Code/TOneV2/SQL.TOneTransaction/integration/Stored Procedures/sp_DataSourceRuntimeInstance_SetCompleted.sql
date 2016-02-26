
CREATE PROCEDURE [integration].[sp_DataSourceRuntimeInstance_SetCompleted]
	@InstanceID uniqueidentifier
AS
BEGIN
		
	UPDATE integration.DataSourceRuntimeInstance
	SET 
		IsCompleted = 1
	WHERE ID = @InstanceID
	
END