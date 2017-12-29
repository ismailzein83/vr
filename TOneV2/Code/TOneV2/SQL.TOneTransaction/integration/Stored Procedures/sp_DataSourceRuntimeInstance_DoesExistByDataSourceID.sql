CREATE PROCEDURE [integration].[sp_DataSourceRuntimeInstance_DoesExistByDataSourceID]
	@DataSourceID uniqueidentifier
AS
BEGIN	
	IF EXISTS (SELECT TOP 1 NULL FROM integration.DataSourceRuntimeInstance WITH(NOLOCK) WHERE DataSourceID = @DataSourceID)
		SELECT 1
	ELSE
		SELECT 0
END