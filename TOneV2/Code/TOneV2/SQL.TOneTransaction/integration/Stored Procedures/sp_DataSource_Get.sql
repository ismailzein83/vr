-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [integration].[sp_DataSource_Get] 
@DataSourceID int
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

	FROM integration.DataSource ds
	INNER JOIN integration.AdapterType at ON at.ID = ds.AdapterID
	INNER JOIN runtime.ScheduleTask st ON st.ID = ds.TaskId
   
	WHERE DS.[ID] = @DataSourceID
END