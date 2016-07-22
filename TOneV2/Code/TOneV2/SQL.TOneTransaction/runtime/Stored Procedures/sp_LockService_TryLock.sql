-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [runtime].[sp_LockService_TryLock]
	@CurrentRuntimeProcessID int,
	@RunningProcessIDs varchar(max)
AS
BEGIN

	IF NOT EXISTS (SELECT TOP 1 NULL FROM runtime.LockService with (nolock) WHERE ID = 1)
	BEGIN
		INSERT INTO runtime.LockService
		(ID)
		SELECT 1
		WHERE NOT EXISTS (SELECT TOP 1 NULL FROM runtime.LockService WHERE ID = 1)
	END
	
	DECLARE @RunningProcessIDsTable TABLE (ID int)
	INSERT INTO @RunningProcessIDsTable (ID)
	SELECT Convert(int, ParsedString) FROM runtime.[ParseStringList](@RunningProcessIDs)
	
	UPDATE runtime.LockService
    SET	LockedByProcessID = @CurrentRuntimeProcessID,
		ServiceURL = NULL,
		LastLockedTime = GETDATE(),
		LastUpdatedTime = GETDATE()
	WHERE ID = 1
		  AND (LockedByProcessID IS NULL OR LockedByProcessID NOT IN (SELECT ID FROM @RunningProcessIDsTable))
		  
	SELECT LockingDetails FROM runtime.LockService
	WHERE ID = 1 AND LockedByProcessID = @CurrentRuntimeProcessID
	
END