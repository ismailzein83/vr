
CREATE PROCEDURE [runtime].[sp_SchedulerTask_UnlockTask]	
	@TaskId int	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	
    UPDATE [runtime].ScheduleTask
    SET	LockedByProcessID = NULL
	WHERE ID = @TaskId
END