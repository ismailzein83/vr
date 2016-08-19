-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [runtime].[sp_ServiceInstance_GetByType] 
	@ServiceType uniqueidentifier
AS
BEGIN
	SELECT [ServiceInstanceID]
      ,[ServiceType]
      ,[ProcessID]
      ,[ServiceInstanceInfo]
  FROM [runtime].[ServiceInstance]
  WHERE ServiceType = @ServiceType
END