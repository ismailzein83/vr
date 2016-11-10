-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [runtime].[sp_SchedulerTaskTriggerType_GetAll] 
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT	[ID],[Name],[TriggerTypeInfo]
    FROM	[runtime].SchedulerTaskTriggerType WITH(NOLOCK) 
	ORDER BY [Name]
END