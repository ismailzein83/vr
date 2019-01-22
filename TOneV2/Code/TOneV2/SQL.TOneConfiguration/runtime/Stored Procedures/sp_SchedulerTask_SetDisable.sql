CREATE PROCEDURE [runtime].[sp_SchedulerTask_SetDisable] 
	@ID uniqueidentifier
AS
BEGIN
	begin
		UPDATE	[runtime].[ScheduleTask]
		SET		IsEnabled = 0,
		LastModifiedTime=GETDATE()
		WHERE	ID = @ID
	end
END