
CREATE PROCEDURE [integration].[sp_DataSourceRuntimeInstance_Insert]
	@InstanceID uniqueidentifier,
	@DataSourceID int
AS
BEGIN
	INSERT INTO integration.DataSourceRuntimeInstance
	(ID, DataSourceID, CreatedTime)
	VALUES
	(@InstanceID, @DataSourceID, GETDATE())
	
END