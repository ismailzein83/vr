-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [integration].[sp_DataSource_GetAll] 

AS
BEGIN
	SET NOCOUNT ON;

	SELECT ds.[ID],
		ds.[Name],
		ds.[adapterID],
		at.Name AS AdapterName,
		ds.[AdapterState],
		at.[Info],
		ds.[TaskId],
		st.IsEnabled,
		ds.[Settings]
    FROM integration.DataSource ds WITH(NOLOCK) 
	INNER JOIN integration.AdapterType at  WITH(NOLOCK) ON at.ID = ds.AdapterID
	INNER JOIN runtime.ScheduleTask st  WITH(NOLOCK) ON st.ID = ds.TaskId
    
    SET NOCOUNT OFF;
END