-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [runtime].[sp_SchedulerTask_UpdateInfo]
	@ID uniqueidentifier,
	@Name Nvarchar(255),
	@IsEnabled bit,
	@TriggerTypeId uniqueidentifier,
	@ActionTypeId uniqueidentifier,
	@TaskSettings varchar(MAX)
AS
BEGIN
	UPDATE runtime.ScheduleTask
	SET Name = @Name,
		IsEnabled = @IsEnabled,
		TriggerTypeId = @TriggerTypeId,
		ActionTypeId = @ActionTypeId,
		TaskSettings = @TaskSettings
	WHERE ID = @ID
END