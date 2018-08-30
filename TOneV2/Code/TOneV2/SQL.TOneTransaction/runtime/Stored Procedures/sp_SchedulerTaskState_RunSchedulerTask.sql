-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [runtime].[sp_SchedulerTaskState_RunSchedulerTask]
	@TaskID uniqueidentifier,
	@NextRuntime datetime,
	@AllowRunIfEnabled bit
AS
BEGIN
	IF EXISTS(Select TaskId from runtime.ScheduleTaskState WITH(NOLOCK) where TaskId = @TaskId)
	BEGIN
		UPDATE runtime.ScheduleTaskState
		SET NextRuntime = @NextRuntime
		WHERE TaskID = @TaskID and LockedByProcessID is null and [Status] in (0,2,3) and (NextRuntime is not null or @AllowRunIfEnabled = 1)
	END
END