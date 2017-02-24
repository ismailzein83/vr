--[common].[ExtensionConfiguration]---------------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('A8085279-37BF-40C5-941B-A1E46F83DFAB','PartnerPortal_CustomerAccess_RetailAccountUser','Retail Account User','PartnerPortal_CustomerAccess_AccountStatementContextHandler','{"Editor":"partnerportal-customeraccess-retailaccountuser"}'),
('A0709FCC-0344-4B64-BC0D-50471D052D0F','PartnerPortal_CustomerAccess_AccountStatementView','Portal Account Statement','VR_Security_ViewTypeConfig','{"Editor":"/Client/Modules/Security/Views/View/GenericViewEditor.html","EnableAdd":true,"DirectiveEditor":"partnerportal-customeraccess-accountstatement-vieweditor"}'),
('B3A94A20-92ED-47BF-86D6-1034B720BE73','Retail Account Query Interceptor','Retail Account Query Interceptor','VR_GenericData_RestAPIRecordQueryInterceptorConfig','{"Editor":"partnerportal-customeraccess-retailaccountqueryinterceptor"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[ConfigType],[Settings]))
merge	[common].[ExtensionConfiguration] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[ConfigType] = s.[ConfigType],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Title],[ConfigType],[Settings])
	values(s.[ID],s.[Name],s.[Title],s.[ConfigType],s.[Settings]);
----------------------------------------------------------------------------------------------------
end