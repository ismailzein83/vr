-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [runtime].[sp_SchedulerTask_GetDueTasks]
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
      ,[LastRunTime]
      ,[NextRunTime]
      ,[TriggerTypeId]
      ,[TaskTrigger]
      ,[ActionTypeId]
      ,[TaskAction]
      from runtime.ScheduleTask
      where [NextRunTime] is Null or cast([NextRunTime] as datetime) <= cast(GETDATE() as datetime)
END