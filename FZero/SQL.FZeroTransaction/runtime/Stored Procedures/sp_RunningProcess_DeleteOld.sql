-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE runtime.sp_RunningProcess_DeleteOld
	@heartBeatReceivedBeforeInSeconds INT
AS
BEGIN	
           
     DELETE
	 FROM [runtime].[RunningProcess]
	 WHERE DATEDIFF(ss, [LastHeartBeatTime], GETDATE()) > @heartBeatReceivedBeforeInSeconds
END