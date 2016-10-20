
CREATE PROCEDURE [integration].[sp_DataSourceRuntimeInstance_InsertIfNotMaximum]
	@InstanceID uniqueidentifier,
	@DataSourceID uniqueidentifier,
	@MaxNumberOfInstances int
AS
BEGIN
	INSERT INTO integration.DataSourceRuntimeInstance
	(ID, DataSourceID, CreatedTime)
	SELECT @InstanceID, @DataSourceID, GETDATE()
	WHERE (SELECT COUNT(*) FROM integration.DataSourceRuntimeInstance 
			WHERE 
			DataSourceID = @DataSourceID AND ISNULL(IsCompleted, 0) = 0) < @MaxNumberOfInstances
	
END