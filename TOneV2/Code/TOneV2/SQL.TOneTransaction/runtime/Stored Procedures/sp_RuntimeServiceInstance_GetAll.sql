CREATE PROCEDURE [runtime].[sp_RuntimeServiceInstance_GetAll] 
AS
BEGIN
	SELECT [ID]
      ,[ServiceTypeID]
      ,[ProcessID]
      ,[ServiceInstanceInfo]
  FROM [runtime].[RuntimeServiceInstance] WITH (NOLOCK)
END