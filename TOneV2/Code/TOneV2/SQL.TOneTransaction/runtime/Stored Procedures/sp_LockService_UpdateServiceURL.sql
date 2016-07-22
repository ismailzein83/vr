-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [runtime].[sp_LockService_UpdateServiceURL]
	@ServiceURL varchar(255)
AS
BEGIN
	
	UPDATE runtime.LockService
    SET	ServiceURL = @ServiceURL,
		LastUpdatedTime = GETDATE()	
	WHERE ID = 1
	
END