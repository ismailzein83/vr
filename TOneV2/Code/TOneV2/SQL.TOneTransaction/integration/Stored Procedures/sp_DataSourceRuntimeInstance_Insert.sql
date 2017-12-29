
CREATE PROCEDURE [integration].[sp_DataSourceRuntimeInstance_Insert]
	@InstanceID uniqueidentifier,
	@DataSourceID uniqueidentifier
AS
BEGIN
	INSERT INTO integration.DataSourceRuntimeInstance
	(ID, DataSourceID)
	VALUES
	(@InstanceID, @DataSourceID)
	
END