-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_Widget_GetFiltered]
@WidgetName nvarchar(50),
@WidgetType int
AS
BEGIN
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
	WHERE  w.Name Like '%'+@WidgetName +'%' 
		AND
		  (w.WidgetDefinitionId = @WidgetType OR @WidgetType IS NULL)
END