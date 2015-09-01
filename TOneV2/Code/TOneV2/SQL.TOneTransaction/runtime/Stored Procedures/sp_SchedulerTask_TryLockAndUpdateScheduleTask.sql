
CREATE PROCEDURE [runtime].[sp_SchedulerTask_TryLockAndUpdateScheduleTask]	
	@TaskId int,
	@CurrentRuntimeProcessID int,
	@RunningProcessIDs [runtime].IDIntType readonly
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	
    UPDATE [runtime].ScheduleTask
    SET	LockedByProcessID = @CurrentRuntimeProcessID
	WHERE ID = @TaskId
		  AND (LockedByProcessID IS NULL OR LockedByProcessID NOT IN (SELECT ID FROM @RunningProcessIDs))
END