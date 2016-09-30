-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [runtime].[sp_SchedulerTask_GetAll]

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT SC.[ID]
      ,SC.[Name]
      ,SC.[IsEnabled]
      ,SC.[TriggerTypeId]
      ,SC.[ActionTypeId]
      ,TR.[TriggerTypeInfo]
      ,AC.[ActionTypeInfo]
      ,SC.[TaskSettings]
      ,SC.[OwnerId]
      from runtime.ScheduleTask SC WITH(NOLOCK) 
      JOIN runtime.SchedulerTaskTriggerType TR  WITH(NOLOCK) on SC.TriggerTypeId = TR.ID
      JOIN runtime.SchedulerTaskActionType AC  WITH(NOLOCK) on SC.ActionTypeId = AC.ID

END