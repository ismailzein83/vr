-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_Widget_GetAll] 
	-- Add the parameters for the stored procedure here

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SELECT	w.Id,w.WidgetDefinitionId,
			w.Name AS WidgetName,
			w.Title,
			w.Setting,
			wd.Name AS WidgetDefinitionName,
			wd.DirectiveName AS DirectiveName,
			wd.Setting as WidgetDefinitionSetting
	FROM	sec.Widget w 
	JOIN	sec.WidgetDefinition wd 
	ON		w.WidgetDefinitionId=wd.ID
END