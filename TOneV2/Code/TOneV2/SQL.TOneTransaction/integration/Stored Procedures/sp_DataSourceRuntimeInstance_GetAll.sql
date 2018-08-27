CREATE PROCEDURE [integration].[sp_DataSourceRuntimeInstance_GetAll]
AS
BEGIN	
	SELECT [ID]
      ,[DataSourceID]
	  ,[CreatedTime]
	FROM integration.DataSourceRuntimeInstance WITH(NOLOCK)	
END