-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [runtime].[sp_LockService_UpdateLockingDetails]
	@CurrentRuntimeProcessID int,
	@LockingDetails nvarchar(max)
AS
BEGIN
	
	UPDATE runtime.LockService
    SET	LockingDetails = @LockingDetails,
		LastUpdatedTime = GETDATE()	
	WHERE ID = 1 AND LockedByProcessID = @CurrentRuntimeProcessID
	
END