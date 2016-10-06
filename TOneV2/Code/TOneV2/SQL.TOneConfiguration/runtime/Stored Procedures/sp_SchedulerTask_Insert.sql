-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [runtime].[sp_SchedulerTask_Insert] 
	@Name Nvarchar(255),
	@IsEnabled bit,
	@TaskType int,
	@TriggerTypeId uniqueidentifier,
	@ActionTypeId uniqueidentifier,
	@TaskSettings varchar(MAX),
	@OwnerId int,
	@Id int out
AS
BEGIN
	Insert into runtime.ScheduleTask([Name], IsEnabled, [TaskType], TriggerTypeId, ActionTypeId, TaskSettings, OwnerId)
	values(@Name, @IsEnabled, @TaskType, @TriggerTypeId, @ActionTypeId, @TaskSettings, @OwnerId)
	
	SET @Id = SCOPE_IDENTITY()
END