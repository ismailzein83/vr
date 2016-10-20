-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [runtime].[sp_SchedulerTask_Insert] 
	@Id uniqueidentifier ,
	@Name Nvarchar(255),
	@IsEnabled bit,
	@TaskType int,
	@TriggerTypeId uniqueidentifier,
	@ActionTypeId uniqueidentifier,
	@TaskSettings varchar(MAX),
	@OwnerId int

AS
BEGIN
	Insert into runtime.ScheduleTask(Id,[Name], IsEnabled, [TaskType], TriggerTypeId, ActionTypeId, TaskSettings, OwnerId)
	values(@Id,@Name, @IsEnabled, @TaskType, @TriggerTypeId, @ActionTypeId, @TaskSettings, @OwnerId)

END