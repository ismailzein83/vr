-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [runtime].[sp_ServiceInstance_Delete] 
	@ServiceInstanceID uniqueidentifier
AS
BEGIN
	DELETE [runtime].[ServiceInstance]
  WHERE [ServiceInstanceID] = @ServiceInstanceID
END