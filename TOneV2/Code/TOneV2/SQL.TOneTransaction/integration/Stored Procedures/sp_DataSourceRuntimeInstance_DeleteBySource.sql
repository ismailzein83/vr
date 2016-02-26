
CREATE PROCEDURE [integration].[sp_DataSourceRuntimeInstance_DeleteBySource]
	@DataSourceID int
AS
BEGIN
	DELETE integration.DataSourceRuntimeInstance
	WHERE DataSourceID = @DataSourceID
		
END