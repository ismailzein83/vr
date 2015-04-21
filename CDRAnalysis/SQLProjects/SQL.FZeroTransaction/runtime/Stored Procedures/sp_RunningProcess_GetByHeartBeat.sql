-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [runtime].[sp_RunningProcess_GetByHeartBeat]
	@HeartBeatReceivedWithinInSeconds INT
AS
BEGIN	
           
     SELECT [ID]
      ,[ProcessName]
      ,[MachineName]
      ,[StartedTime]
      ,[LastHeartBeatTime]
	 FROM [runtime].[RunningProcess]
	 WHERE DATEDIFF(ss, [LastHeartBeatTime], GETDATE()) < @HeartBeatReceivedWithinInSeconds
		OR @HeartBeatReceivedWithinInSeconds IS NULL
END