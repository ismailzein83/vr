
CREATE PROCEDURE [integration].[sp_DataSourceRuntimeInstance_InsertIfNotMaximum]
	@InstanceID uniqueidentifier,
	@DataSourceID int,
	@MaxNumberOfInstances int
AS
BEGIN
	INSERT INTO integration.DataSourceRuntimeInstance
	(ID, DataSourceID)
	SELECT @InstanceID, @DataSourceID
	WHERE (SELECT COUNT(*) FROM integration.DataSourceRuntimeInstance 
			WHERE 
			DataSourceID = @DataSourceID AND ISNULL(IsCompleted, 0) = 0) < @MaxNumberOfInstances
	
END