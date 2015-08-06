-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [runtime].[sp_SchedulerTask_Update]
	@ID int,
	@Name Nvarchar(255),
	@IsEnabled bit,
	@Status int,
	@LastRunTime datetime,
	@NextRunTime datetime,
	@TriggerTypeId int,
	@TaskTrigger varchar(1000),
	@ActionTypeId int,
	@TaskAction varchar(1000)
AS
BEGIN
	UPDATE runtime.ScheduleTask
	SET Name = @Name,
		IsEnabled = @IsEnabled,
		[Status] = @Status,
		LastRunTime = @LastRunTime,
		NextRunTime = @NextRunTime,
		TriggerTypeId = @TriggerTypeId,
		TaskTrigger = @TaskTrigger,
		ActionTypeId = @ActionTypeId,
		TaskAction = @TaskAction
	WHERE ID = @ID
END