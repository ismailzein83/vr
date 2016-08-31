
CREATE PROCEDURE [runtime].[sp_LockService_GetServiceURL]	
AS
BEGIN
	
	SELECT ServiceURL FROM [runtime].LockService WITH(NOLOCK) 
	WHERE ID = 1
	
END