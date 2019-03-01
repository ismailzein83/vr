-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [runtime].[sp_RunningProcess_SetReady] 
	@ProcessID INT
AS
BEGIN
	UPDATE runtime.RunningProcess
	SET IsDraft = 0
	WHERE ID = @ProcessID
END