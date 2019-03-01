
CREATE PROCEDURE [runtime].[sp_RuntimeManager_GetNonTimedOutRuntimeManagerInstanceID]
	@TimeoutInSeconds float
AS
BEGIN
	SELECT manager.InstanceID 
	FROM [runtime].[RuntimeManager] manager with(nolock)
	JOIN [runtime].[RuntimeNodeState] node with(nolock) ON manager.InstanceID = node.InstanceID
	WHERE manager.ID = 1
		AND DATEDIFF(ss, node.[LastHeartBeatTime], GETDATE()) < @TimeoutInSeconds

END