CREATE procedure [sec].[sp_WidgetDefinition_GetWidgets]

as
begin
select w.ID,w.Name,w.DirectiveName,w.Setting from sec.WidgetDefinition w
end