CREATE PROCEDURE [runtime].[sp_RuntimeNodeState_GetByNodeID] 
	@RuntimeNodeID uniqueidentifier
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
	FROM runtime.RuntimeNodeState WITH (NOLOCK)
	WHERE RuntimeNodeID = @RuntimeNodeID

END