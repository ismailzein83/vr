-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [runtime].[sp_RunningProcess_UpdateHeartBeat]
	@ProcessId INT,
	@HeartBeatTime DATETIME OUT
AS
BEGIN	
     SET @HeartBeatTime = GETDATE()
     
     UPDATE [runtime].[RunningProcess]
	 SET [LastHeartBeatTime] = @HeartBeatTime
	 WHERE ID = @ProcessId
END