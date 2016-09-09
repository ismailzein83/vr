CREATE PROCEDURE [runtime].[sp_RuntimeServiceInstance_Delete] 
	@ID uniqueidentifier
AS
BEGIN
	DELETE [runtime].[RuntimeServiceInstance]
  WHERE [ID] = @ID

  DECLARE @FirstID uniqueidentifier
     SET @FirstID = (SELECT TOP 1 ID FROM [runtime].[RuntimeServiceInstance])
     UPDATE [runtime].[RuntimeServiceInstance]
     SET ProcessID = ProcessID
     WHERE ID = @FirstID 
END