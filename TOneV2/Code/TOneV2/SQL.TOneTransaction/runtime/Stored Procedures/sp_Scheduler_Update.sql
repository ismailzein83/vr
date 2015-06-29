-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [runtime].[sp_Scheduler_Update]
	@ID int,
	@Name Nvarchar(255),
	@IsEnabled bit,
	@TriggerTypeId int,
	@TaskTrigger varchar(1000),
	@ActionTypeId int,
	@TaskAction varchar(1000)
AS
BEGIN
	UPDATE runtime.ScheduleTask
	SET Name = @Name,
		IsEnabled = @IsEnabled,
		TriggerTypeId = @TriggerTypeId,
		TaskTrigger = @TaskTrigger,
		ActionTypeId = @ActionTypeId,
		TaskAction = @TaskAction
	WHERE ID = @ID
END