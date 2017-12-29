CREATE PROCEDURE [integration].[sp_DataSourceRuntimeInstance_DoesExistByID]
	@ID uniqueidentifier
AS
BEGIN	
	IF EXISTS (SELECT TOP 1 NULL FROM integration.DataSourceRuntimeInstance WITH(NOLOCK) WHERE ID = @ID)
		SELECT 1
	ELSE
		SELECT 0
END