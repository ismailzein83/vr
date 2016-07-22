
CREATE PROCEDURE [runtime].[sp_LockService_GetServiceURL]	
AS
BEGIN
	
	SELECT ServiceURL FROM runtime.LockService
	WHERE ID = 1
	
END