CREATE PROCEDURE [runtime].[sp_RuntimeManager_GetServiceURL] 
AS
BEGIN
	SELECT ServiceURL FROM runtime.RuntimeManager with (nolock) WHERE ID = 1
END