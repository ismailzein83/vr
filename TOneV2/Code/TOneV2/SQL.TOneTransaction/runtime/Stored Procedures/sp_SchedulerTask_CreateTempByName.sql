-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [runtime].[sp_SchedulerTask_CreateTempByName]
	@TempTableName VARCHAR(200),
	@Name Nvarchar(255)
AS
BEGIN
	SET NOCOUNT ON;
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
			SELECT SC.[ID]
			  ,SC.[Name]
			  ,SC.[IsEnabled]
			  ,SC.[Status]
			  ,SC.[LastRunTime]
			  ,SC.[NextRunTime]
			  ,SC.[TriggerTypeId]
			  ,SC.[ActionTypeId]
			  ,TR.[TriggerTypeInfo]
			  ,AC.[ActionTypeInfo]
			  ,SC.[TaskSettings]
			
			INTO #RESULT
			
			from runtime.ScheduleTask SC
			JOIN runtime.SchedulerTaskTriggerType TR on SC.TriggerTypeId = TR.ID
			JOIN runtime.SchedulerTaskActionType AC on SC.ActionTypeId = AC.ID
			WHERE (@Name IS NULL OR SC.[Name] LIKE '%' + @Name + '%' ) AND SC.[TaskType] != 0
			
			DECLARE @sql VARCHAR(1000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
			EXEC(@sql)
		END                    

	SET NOCOUNT OFF
END