﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [runtime].[sp_SchedulerTask_UpdateInfo]
	@ID int,
	@Name Nvarchar(255),
	@IsEnabled bit,
	@TriggerTypeId int,
	@ActionTypeId int,
	@TaskSettings varchar(MAX)
AS
BEGIN
	UPDATE runtime.ScheduleTask
	SET Name = @Name,
		IsEnabled = @IsEnabled,
		NextRunTime = NULL,
		TriggerTypeId = @TriggerTypeId,
		ActionTypeId = @ActionTypeId,
		TaskSettings = @TaskSettings
	WHERE ID = @ID
END