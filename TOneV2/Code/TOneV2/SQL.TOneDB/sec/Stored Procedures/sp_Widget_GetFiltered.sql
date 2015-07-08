-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE sec.sp_Widget_GetFiltered
@Filter nvarchar(50)
AS
BEGIN
	SELECT	w.Id,w.WidgetDefinitionId,
			w.Name AS WidgetName,
			w.Setting,
			wd.Name AS WidgetDefinitionName,
			wd.DirectiveName AS DirectiveName,
			wd.Setting as WidgetDefinitionSetting
	FROM	sec.Widget w 
	JOIN	sec.WidgetDefinition wd 
	ON		w.WidgetDefinitionId=wd.ID
	where w.Name Like '%'+@Filter +'%' or  wd.Name Like '%'+@Filter +'%'
END