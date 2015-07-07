-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [runtime].[sp_SchedulerTask_Insert] 
	@Name Nvarchar(255),
	@IsEnabled bit,
	@TaskType int,
	@Status bit,
	@TriggerTypeId int,
	@TaskTrigger varchar(1000),
	@ActionTypeId int,
	@TaskAction varchar(1000),
	@Id int out
AS
BEGIN
	Insert into runtime.ScheduleTask([Name], IsEnabled, [TaskType], [Status], TriggerTypeId, TaskTrigger, ActionTypeId, TaskAction)
	values(@Name, @IsEnabled, @TaskType, @Status, @TriggerTypeId, @TaskTrigger, @ActionTypeId, @TaskAction)
	
	SET @Id = @@IDENTITY
END