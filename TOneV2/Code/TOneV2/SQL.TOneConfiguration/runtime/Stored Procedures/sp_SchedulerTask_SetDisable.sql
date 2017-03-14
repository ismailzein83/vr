CREATE PROCEDURE [runtime].[sp_SchedulerTask_SetDisable] 
	@ID uniqueidentifier
AS
BEGIN
	begin
		UPDATE	[runtime].[ScheduleTask]
		SET		IsEnabled = 0
		WHERE	ID = @ID
	end
END