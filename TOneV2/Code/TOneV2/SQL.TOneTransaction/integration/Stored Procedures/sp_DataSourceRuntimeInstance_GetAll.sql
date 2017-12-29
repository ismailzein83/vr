CREATE PROCEDURE [integration].[sp_DataSourceRuntimeInstance_GetAll]
AS
BEGIN	
	SELECT [ID]
      ,[DataSourceID]
	FROM integration.DataSourceRuntimeInstance WITH(NOLOCK)	
END