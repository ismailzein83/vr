﻿CREATE PROCEDURE [runtime].[sp_SchedulerTask_SetEnable]
	@ID uniqueidentifier
AS
BEGIN
	begin
		UPDATE	[runtime].[ScheduleTask]
		SET		IsEnabled = 1,
		LastModifiedTime=GETDATE()
		WHERE	ID = @ID
	end
END