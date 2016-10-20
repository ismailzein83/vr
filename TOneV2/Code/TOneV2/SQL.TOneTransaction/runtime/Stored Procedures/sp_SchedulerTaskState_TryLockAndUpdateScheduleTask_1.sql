CREATE PROCEDURE [runtime].[sp_SchedulerTaskState_TryLockAndUpdateScheduleTask]	
	@TaskId uniqueidentifier,
	@CurrentRuntimeProcessID int,
	@RunningProcessIDs [runtime].IDIntType readonly
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	
    UPDATE [runtime].ScheduleTaskState
    SET	LockedByProcessID = @CurrentRuntimeProcessID
	WHERE TaskId = @TaskId
		  AND (LockedByProcessID IS NULL OR LockedByProcessID NOT IN (SELECT ID FROM @RunningProcessIDs))
END