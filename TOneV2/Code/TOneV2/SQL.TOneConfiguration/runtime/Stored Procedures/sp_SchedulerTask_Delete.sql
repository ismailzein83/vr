-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [runtime].[sp_SchedulerTask_Delete]
	@Id uniqueidentifier
AS
BEGIN
	DELETE FROM [runtime].[ScheduleTask]
	WHERE ID = @Id
	
	DELETE FROM [runtime].[ScheduleTaskState]
	WHERE TaskId = @Id
END