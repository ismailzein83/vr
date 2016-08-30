CREATE procedure [sec].[sp_WidgetDefinition_Get]

AS
BEGIN
	SELECT	w.ID,w.Name,w.DirectiveName,w.Setting 
	FROM	[sec].WidgetDefinition w WITH(NOLOCK) 
END