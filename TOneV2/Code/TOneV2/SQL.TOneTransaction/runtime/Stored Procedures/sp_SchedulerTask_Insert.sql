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
	@ActionTypeId int,
	@TaskSettings varchar(MAX),
	@Id int out
AS
BEGIN
	Insert into runtime.ScheduleTask([Name], IsEnabled, [TaskType], [Status], TriggerTypeId, ActionTypeId, TaskSettings)
	values(@Name, @IsEnabled, @TaskType, @Status, @TriggerTypeId, @ActionTypeId, @TaskSettings)
	
	SET @Id = @@IDENTITY
END