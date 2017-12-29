
CREATE PROCEDURE [integration].[sp_DataSourceRuntimeInstance_InsertIfNotMaximum]
	@InstanceID uniqueidentifier,
	@DataSourceID uniqueidentifier,
	@MaxNumberOfInstances int
AS
BEGIN
	INSERT INTO integration.DataSourceRuntimeInstance
	(ID, DataSourceID)
	SELECT @InstanceID, @DataSourceID
	WHERE (SELECT COUNT(*) FROM integration.DataSourceRuntimeInstance 
			WHERE 
			DataSourceID = @DataSourceID) < @MaxNumberOfInstances
	
END