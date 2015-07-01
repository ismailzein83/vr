-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [runtime].sp_SchedulerTask_GetTasksbyActionType
@ActionTypeId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT [ID]
      ,[Name]
      ,[IsEnabled]
      ,[Status]
      ,[TriggerTypeId]
      ,[TaskTrigger]
      ,[ActionTypeId]
      ,[TaskAction]
      from runtime.ScheduleTask
      WHERE ActionTypeId = @ActionTypeId
END