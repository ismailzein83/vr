CREATE PROCEDURE [runtime].[sp_RuntimeManager_GetServiceURL] 
AS
BEGIN
	SELECT node.ServiceURL, node.InstanceID RuntimeNodeInstanceID FROM runtime.RuntimeManager manager with (nolock)
	JOIN [runtime].[RuntimeNodeState] node with (nolock) ON manager.InstanceID = node.InstanceID
	WHERE manager.ID = 1
END