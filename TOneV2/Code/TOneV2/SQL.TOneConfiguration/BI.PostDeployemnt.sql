--sec.WidgetDefinition--------
set nocount on;
set identity_insert [sec].[WidgetDefinition] on;
;with cte_data([ID],[Name],[DirectiveName],[Setting])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Report                                            ','vr-bi-datagrid                                    ','{"DirectiveTemplateURL":"/Client/Modules/BI/Directives/Templates/vr-bi-datagrid-directive-template.html","Sections":[1]}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                '),
(2,'Chart                                             ','vr-bi-chart                                       ','{"DirectiveTemplateURL":"/Client/Modules/BI/Directives/Templates/vr-bi-chart-directive-template.html","Sections":[1]}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   '),
(3,'Summary                                           ','vr-bi-summary                                     ','{"DirectiveTemplateURL":"/Client/Modules/BI/Directives/Templates/vr-bi-summary-directive-template.html","Sections":[0]}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 ')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[DirectiveName],[Setting]))
merge	[sec].[WidgetDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[DirectiveName] = s.[DirectiveName],[Setting] = s.[Setting]
when not matched by target then
	insert([ID],[Name],[DirectiveName],[Setting])
	values(s.[ID],s.[Name],s.[DirectiveName],s.[Setting])
when not matched by source then
	delete;
set identity_insert [sec].[WidgetDefinition] off; 