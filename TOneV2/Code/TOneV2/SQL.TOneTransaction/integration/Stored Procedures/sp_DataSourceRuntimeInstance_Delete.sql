CREATE PROCEDURE [integration].[sp_DataSourceRuntimeInstance_Delete]
	@ID uniqueidentifier
AS
BEGIN	
	DELETE
	integration.DataSourceRuntimeInstance WHERE ID = @ID
END