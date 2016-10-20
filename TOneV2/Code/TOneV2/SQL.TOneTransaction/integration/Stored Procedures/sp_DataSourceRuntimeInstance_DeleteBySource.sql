
CREATE PROCEDURE [integration].[sp_DataSourceRuntimeInstance_DeleteBySource]
	@DataSourceID uniqueidentifier
AS
BEGIN
	DELETE integration.DataSourceRuntimeInstance
	WHERE DataSourceID = @DataSourceID
		
END