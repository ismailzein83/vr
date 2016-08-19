
CREATE PROCEDURE [runtime].[sp_RunningProcess_IsExists]
	@ProcessID int
AS
BEGIN           
     SELECT ID FROM runtime.RunningProcess WHERE ID = @ProcessID
END