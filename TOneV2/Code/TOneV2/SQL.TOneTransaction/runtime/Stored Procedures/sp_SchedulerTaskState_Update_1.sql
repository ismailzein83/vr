-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [runtime].[sp_SchedulerTaskState_Update]
	@TaskID uniqueidentifier,
	@Status int,
	@NextRuntime datetime,
	@LastRuntime datetime,
	@ExecutionInfo nvarchar(max)
AS
BEGIN
	UPDATE runtime.ScheduleTaskState
	SET [Status] = @Status,
		NextRuntime = @NextRuntime,
		LastRuntime = @LastRuntime,
		ExecutionInfo = @ExecutionInfo
	WHERE TaskID = @TaskID
END