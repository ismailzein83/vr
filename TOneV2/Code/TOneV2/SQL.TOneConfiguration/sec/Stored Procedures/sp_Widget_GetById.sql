-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_Widget_GetById] 
	-- Add the parameters for the stored procedure here
	@Id INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SELECT	w.Id,w.WidgetDefinitionId,w.Name AS WidgetName,w.Title,w.Setting,wd.Name AS WidgetDefinitionName,wd.DirectiveName AS DirectiveName ,wd.Setting as WidgetDefinitionSetting
	FROM	[sec].Widget w   WITH(NOLOCK) 
			INNER JOIN	sec.WidgetDefinition wd  WITH(NOLOCK) ON	w.WidgetDefinitionId=wd.ID 
	where	w.Id=@Id

END