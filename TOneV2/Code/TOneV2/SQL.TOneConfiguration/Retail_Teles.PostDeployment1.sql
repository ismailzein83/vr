/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/


--[common].[ExtensionConfiguration]-------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('A3D7F334-21A0-4875-8EA2-ED54B33F292C','TelesRestAPI','Teles Rest API','VRCommon_ConnectionConfig'														,'{"Editor":"retail-teles-telesrestconnection-editor"}'),
('772C0B47-A8C0-4F15-B2F1-AEBF52B3EF08','RoutingGroupCondition','Routing Group Condition','Retail_Teles_RoutingGroupCondition'							,'{"Editor":"retail-teles-routinggroupcondition-routinggroupfilter"}'),
('09A1029D-AFC0-48E4-B1B3-FD951462E267',' MappingTelesAccount',' Mapping Teles Account','Retail_BE_AccountActionDefinitionConfig'						,'{"Editor":"retail-teles-accountactiondefinitionsettings-mappingtelesaccount"}'),
('7ACEF5E4-0392-445F-97A9-C7251A66DFFC','ChangeUsersRGs','Change Users Routing Group','Retail_BE_ProvisionerDefinition'									,'{"DefinitionEditor":"retail-teles-provisioner-definitionsettings-changeusersrgs", "RuntimeEditor":"retail-teles-provisioner-runtimesettings-changeusersrgs"}'),
('F5AEB249-3D8A-4235-8C7B-2BA5B99D0B0D','RevertUsersRGs','Revert Users Routing Group','Retail_BE_ProvisionerDefinition'									,'{"DefinitionEditor":"retail-teles-provisioner-definitionsettings-revertusersrgs", "RuntimeEditor":"retail-teles-provisioner-runtimesettings-revertusersrgs"}'),
('DFFBF1F9-EF68-43FA-BE34-AD181031CDDA','Teles Enterprise Extra Field','Teles Enterprise Extra Field','Retail_BE_AccountExtraFieldDefinitionConfig'		,'{"Editor":"retail-teles-accountextrafield-enterprise"}'),
('E02E72EA-56BD-4F86-A404-F08BE3A2E619','Teles Enterprise Business Entity','Teles Enterprise BE','VR_GenericData_BusinessEntityDefinitionSettingsConfig','{"Editor":"retail-teles-enterprisebedefinition-editor"}'),
('E440C268-19B9-4D43-820B-C407604C7EF3','InternationalCallsBlocked','International Calls Blocked','Retail_BE_AccountExtraFieldDefinitionConfig'			,'{"Editor":"retail-teles-accountextrafield-internationalcallsblocked"}')
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

END


--[genericdata].[BusinessEntityDefinition]----------------------------------------------------------
----------------------------------------------------------------------------------------------------
BEGIN

set nocount on;
;with cte_data([ID],[OldID],[Name],[Title],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('4798feef-3c93-439d-9357-3a5f78e1ae10',null,'Teles Enterprise','Teles Enterprise','{"$type":"Retail.Teles.Business.TelesEnterpriseBEDefinitionSettings, Retail.Teles.Business","ConfigId":"e02e72ea-56bd-4f86-a404-f08be3a2e619","DefinitionEditor":"retail-teles-enterprisebedefinition-editor","IdType":"System.Int32","SelectorUIControl":"retail-teles-enterprises-selector","ManagerFQTN":"Retail.Teles.Business.TelesEnterpriseManager, Retail.Teles.Business","VRConnectionId":"93035c7d-8334-4434-a0d6-a9691951668c"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[OldID],[Name],[Title],[Settings]))
merge	[genericdata].[BusinessEntityDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[OldID] = s.[OldID],[Name] = s.[Name],[Title] = s.[Title],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[OldID],[Name],[Title],[Settings])
	values(s.[ID],s.[OldID],s.[Name],s.[Title],s.[Settings]);

END

--[common].[Connection]---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
BEGIN
set nocount on;;with cte_data([ID],[Name],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('93035C7D-8334-4434-A0D6-A9691951668C','Teles API','{"$type":"Retail.Teles.Business.TelesRestConnection, Retail.Teles.Business","ConfigId":"a3d7f334-21a0-4875-8ea2-ed54b33f292c","Token":"f0106d37-e0d7-4ff4-9397-fd35e7608233","Authorization":"Basic YWRtaW5AdnIucmVzdC53cy5kZTphZG1pbkB2cg==","URL":"https://c5-iot2-prov.teles.de","ActionPrefix":"/SIPManagement/rest/v1","DefaultDomainId":52}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Settings]))merge	[common].[Connection] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Settings])	values(s.[ID],s.[Name],s.[Settings]);	
END

