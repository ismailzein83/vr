CREATE PROCEDURE [runtime].[sp_RuntimeServiceInstance_Delete] 
	@ID uniqueidentifier
AS
BEGIN
	DELETE [runtime].[RuntimeServiceInstance]
  WHERE [ID] = @ID
END