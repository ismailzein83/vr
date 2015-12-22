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
	@ActionTypeId int,
	@TaskSettings varchar(MAX),
	@ExecutionInfo nvarchar(MAX)
AS
BEGIN
	UPDATE runtime.ScheduleTask
	SET Name = @Name,
		IsEnabled = @IsEnabled,
		[Status] = @Status,
		LastRunTime = @LastRunTime,
		NextRunTime = @NextRunTime,
		TriggerTypeId = @TriggerTypeId,
		ActionTypeId = @ActionTypeId,
		TaskSettings = @TaskSettings,
		ExecutionInfo = @ExecutionInfo
	WHERE ID = @ID
END