-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPTask_UpdateTaskExecution]
	@TaskID bigint,
	@ExecutedBy int,
	@Status int,
	@TaskExecutionInformation nvarchar(max),
	@Notes nvarchar(max),
	@Decision nvarchar(255)
AS
BEGIN
Update [bp].[BPTask] set [ExecutedBy] = @ExecutedBy,
						 [Status] = @Status,
						 [TaskExecutionInformation] = @TaskExecutionInformation,
						 [Notes] = @Notes,
						 [Decision] = @Decision ,
						 [LastUpdatedTime] = GETDATE()
where ID = @TaskID

END