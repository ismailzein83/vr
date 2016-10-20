-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [integration].[sp_DataSource_Get] 
@DataSourceID uniqueidentifier
AS
BEGIN
	SET NOCOUNT ON;

	SELECT ds.[ID],
		ds.[Name],
		ds.[adapterID],
		ds.[AdapterState],
		ds.[TaskId],
		st.IsEnabled,
		ds.[Settings]

	FROM integration.DataSource ds WITH(NOLOCK) 
	INNER JOIN runtime.ScheduleTask st  WITH(NOLOCK) ON st.ID = ds.TaskId
   
	WHERE DS.[ID] = @DataSourceID
END