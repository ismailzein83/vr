
CREATE PROCEDURE [runtime].[sp_RunningProcess_Delete]
	@ProcessID int
AS
BEGIN
           
     DELETE [runtime].[RunningProcess]
     WHERE ID = @ProcessID
     
     DECLARE @FirstID int
     SET @FirstID = (SELECT TOP 1 ID FROM [runtime].[RunningProcess])
     UPDATE [runtime].[RunningProcess]
     SET ProcessName = ProcessName
     WHERE ID = @FirstID 
     
END