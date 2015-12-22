-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [runtime].[sp_SchedulerTask_GetByActionTypeId]
@ActionTypeId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
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
      ,SC.[OwnerId]
      ,SC.[ExecutionInfo]
      from runtime.ScheduleTask SC
      JOIN runtime.SchedulerTaskTriggerType TR on SC.TriggerTypeId = TR.ID
      JOIN runtime.SchedulerTaskActionType AC on SC.ActionTypeId = AC.ID
      WHERE SC.ActionTypeId = @ActionTypeId
END