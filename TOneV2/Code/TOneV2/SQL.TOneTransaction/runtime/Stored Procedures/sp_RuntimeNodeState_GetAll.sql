/****** Object:  StoredProcedure [runtime].[sp_RuntimeNodeState_GetTimedOutNodes]    Script Date: 3/28/2018 1:28:13 PM ******/
CREATE PROCEDURE [runtime].[sp_RuntimeNodeState_GetAll] 
AS
BEGIN
	
	SELECT [RuntimeNodeID]
      ,[InstanceID]
	  ,MachineName
	  ,OSProcessID
	  ,OSProcessName
      ,[ServiceURL]
      ,[StartedTime]
      ,[LastHeartBeatTime]
	  ,DATEDIFF(ss, [LastHeartBeatTime], GETDATE()) NbOfSecondsHeartBeatReceived
	FROM runtime.RuntimeNodeState WITH(NOLOCK)

END