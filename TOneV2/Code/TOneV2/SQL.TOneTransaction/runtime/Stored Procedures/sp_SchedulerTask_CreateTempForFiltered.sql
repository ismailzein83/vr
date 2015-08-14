-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [runtime].[sp_SchedulerTask_CreateTempForFiltered]
	@TempTableName VARCHAR(200),
	@Name Nvarchar(255)
AS
BEGIN
	SET NOCOUNT ON;
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
			SELECT [Id],
			[Name],
			[IsEnabled],
			[TaskType],
			[Status],
			[LastRunTime],
			[NextRunTime],
			[TriggerTypeId],
			[TaskTrigger],
			[ActionTypeId],
			[TaskAction]
			
			INTO #RESULT
			
			FROM [runtime].[ScheduleTask]
			WHERE (@Name IS NULL OR [Name] LIKE '%' + @Name + '%' ) AND [TaskType] != 0
			
			DECLARE @sql VARCHAR(1000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
			EXEC(@sql)
		END                    

	SET NOCOUNT OFF
END