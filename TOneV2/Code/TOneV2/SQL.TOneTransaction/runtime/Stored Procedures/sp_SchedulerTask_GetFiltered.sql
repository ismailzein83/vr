-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [runtime].[sp_SchedulerTask_GetFiltered] 
(
	@FromRow int ,
	@ToRow int,
	@Name Nvarchar(255)
)
AS
BEGIN
	SET NOCOUNT ON

;WITH Tasks_CTE (Id, Name, IsEnabled, [Status], [TriggerTypeId], TaskTrigger, [ActionTypeId], [TaskAction], RowNumber) AS 
	(
		SELECT runtime.[ScheduleTask].[ID], runtime.[ScheduleTask].[Name],runtime.[ScheduleTask].[IsEnabled], runtime.[ScheduleTask].[Status], 
		runtime.ScheduleTask.[TriggerTypeId], runtime.ScheduleTask.TaskTrigger,
		runtime.ScheduleTask.[ActionTypeId], runtime.ScheduleTask.[TaskAction],  ROW_NUMBER()  
		OVER ( ORDER BY  runtime.[ScheduleTask].[ID] ASC) AS RowNumber 
			FROM runtime.[ScheduleTask] 

				WHERE (@Name IS NULL OR runtime.[ScheduleTask].Name  LIKE '%' + @Name + '%' )
	)
	SELECT Id, Name, IsEnabled, [Status], [TriggerTypeId], TaskTrigger, [ActionTypeId], [TaskAction], RowNumber 
	FROM Tasks_CTE WHERE RowNumber between @FromRow AND @ToRow                           

SET NOCOUNT OFF
END