/****** Object:  StoredProcedure [runtime].[sp_RuntimeServiceInstance_Delete]    Script Date: 4/16/2018 12:23:41 PM ******/
CREATE PROCEDURE [runtime].[sp_RuntimeServiceInstance_DeleteByProcess] 
	@ProcessID int
AS
BEGIN
	DELETE [runtime].[RuntimeServiceInstance]
  WHERE [ProcessID] = @ProcessID

  DECLARE @FirstID uniqueidentifier
     SET @FirstID = (SELECT TOP 1 ID FROM [runtime].[RuntimeServiceInstance])
     UPDATE [runtime].[RuntimeServiceInstance]
     SET ProcessID = ProcessID
     WHERE ID = @FirstID 
END