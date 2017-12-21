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
('E440C268-19B9-4D43-820B-C407604C7EF3','InternationalCallsBlocked','International Calls Blocked','Retail_BE_AccountExtraFieldDefinitionConfig'			,'{"Editor":"retail-teles-accountextrafield-internationalcallsblocked"}'),
('b50ff2ab-e6d2-44a4-9681-f119fec8abfc','AccountMappedToTeles','Account Mapped To Teles','Retail_BE_AccountExtraFieldDefinitionConfig'			,'{"Editor":"retail-teles-accountextrafield-accountmappedtoteles"}'),
('FD6ED9B7-F870-4C6D-A51E-36FD2219F64B'	,'Provisioning Company Screened Numbers'	,'Provisioning Company Screened Numbers',	'Retail_BE_ProvisionerDefinition',	'{"DefinitionEditor":"retail-teles-provisioner-definitionsettings-provisionaccount", "RuntimeEditor":"retail-teles-provisioner-runtimesettings-provisionaccount"}'),
('91D8FBF2-A22E-46CB-A004-4966B5C1A87C'	,'Provisioning Site Screened Numbers'	,'Provisioning Site Screened Numbers',	'Retail_BE_ProvisionerDefinition',	'{"DefinitionEditor":"retail-teles-provisioner-definitionsettings-provisionsite", "RuntimeEditor":"retail-teles-provisioner-runtimesettings-provisionsite"}'),
('638C3DCC-F05A-4FA5-83BB-5E24CB2DA9C8'	,' MappingTelesSite'	,'Mapping Teles Site',	'Retail_BE_AccountActionDefinitionConfig',	'{"Editor":"retail-teles-accountactiondefinitionsettings-mappingtelessite"}'),
('2C1CEA7E-96F1-4BB0-83DD-FE8BA4BA984C'	,' TelesAccountCondition'	,'Teles Account Condition',	'Retail_BE_AccountConditionConfig',	'{"Editor":"retail-teles-accountcondition-telesaccount"}'),
('8c4d41eb-c137-48b2-8eaa-b5428734831d','TelesTemplateEntity','Teles Template','VRCommon_GenericLKUPDefinition','{"DefinitionEditor":"retail-teles-telestemplateentitydefinitionsettings"}'),
('74384112-0d3e-4677-8df3-2c16c32a84d4'	,'MappingTelesUser'	,'Mapping Teles User',	'Retail_BE_AccountActionDefinitionConfig',	'{"Editor":"retail-teles-accountactiondefinitionsettings-mappingtelesuser"}'),
('9a63b2ed-a0b9-4364-ad6c-0977f410c1c4'	,'Provisioning User Screened Numbers'	,'Provisioning User Screened Numbers',	'Retail_BE_ProvisionerDefinition',	'{"DefinitionEditor":"retail-teles-provisioner-definitionsettings-provisionuser", "RuntimeEditor":"retail-teles-provisioner-runtimesettings-provisionuser"}'),
('f11db886-8893-441f-b5a4-3261d43e8c0f','RetailBE_AccountView_AccountTelesDIDsAndBusinessTrunks',	'Teles DIDs And Business Trunks',	'Retail_BE_AccountViewDefinitionConfig',	'{"Editor":"retail-teles-accountviewdefinitionsettings-telesdidsandbusinesstrunks"}')
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
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Title],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('4798feef-3c93-439d-9357-3a5f78e1ae10','Teles Enterprise','Teles Enterprise','{"$type":"Retail.Teles.Business.TelesEnterpriseBEDefinitionSettings, Retail.Teles.Business","ConfigId":"e02e72ea-56bd-4f86-a404-f08be3a2e619","DefinitionEditor":"retail-teles-enterprisebedefinition-editor","IdType":"System.Int32","SelectorUIControl":"retail-teles-enterprises-selector","ManagerFQTN":"Retail.Teles.Business.TelesEnterpriseManager, Retail.Teles.Business","VRConnectionId":"93035c7d-8334-4434-a0d6-a9691951668c"}'),
('FD8FAC54-DB90-4DA2-B92F-D81070EA52EC','Domain Template','Domain Template','{"$type":"Vanrise.Common.Business.GenericLKUPBEDefinitionSettings, Vanrise.Common.Business","ConfigId":"f0dec732-929c-4f75-aa35-9e19298d3092","ExtendedSettings":{"$type":"Retail.Teles.Entities.TelesTemplateEntityDefinitionSettings, Retail.Teles.Entities","ConfigId":"8c4d41eb-c137-48b2-8eaa-b5428734831d","RuntimeEditor":"retail-teles-telestemplateentitysettings"},"DefinitionEditor":"vr-common-genericlkup-be-definition","IdType":"System.Guid","ManagerFQTN":"Vanrise.Common.Business.GenericLKUPDefinitionManager, Vanrise.Common.Business","SelectorUIControl":"vr-common-genericlkupitem-selector"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Settings]))
merge	[genericdata].[BusinessEntityDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Title],[Settings])
	values(s.[ID],s.[Name],s.[Title],s.[Settings]);
----------------------------------------------------------------------------------------------------
END
--[common].[Connection]-----------------------------------------------------------------------BEGIN
set nocount on;;with cte_data([ID],[Name],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('93035C7D-8334-4434-A0D6-A9691951668C','Teles API','{"$type":"Retail.Teles.Business.TelesRestConnection, Retail.Teles.Business","ConfigId":"a3d7f334-21a0-4875-8ea2-ed54b33f292c","Token":"f0106d37-e0d7-4ff4-9397-fd35e7608233","Authorization":"Basic YWRtaW5AdnIucmVzdC53cy5kZTphZG1pbkB2cg==","URL":"https://c5-iot2-prov.teles.de","ActionPrefix":"/SIPManagement/rest/v1","DefaultDomainId":52}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Settings]))merge	[common].[Connection] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]--when matched then--	update set--	[Name] = s.[Name],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Settings])	values(s.[ID],s.[Name],s.[Settings]);----------------------------------------------------------------------------------------------------
END
--[sec].[SystemAction]----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([Name],[RequiredPermissions])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('Retail_Teles/TelesEnterprise/GetFilteredEnterpriseDIDs','Retail_BE_Subscriber: View'),('Retail_Teles/TelesEnterprise/GetFilteredEnterpriseBusinessTrunks','Retail_BE_Subscriber: View'),('Retail_Teles/TelesEnterprise/GetFilteredAccountEnterprisesDIDs','TrafficData: View'),('Retail_Teles/TelesEnterprise/SaveAccountEnterprisesDIDs','TrafficData: View')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([Name],[RequiredPermissions]))merge	[sec].[SystemAction] as tusing	cte_data as son		1=1 and t.[Name] = s.[Name]when matched then	update set	[Name] = s.[Name],[RequiredPermissions] = s.[RequiredPermissions]when not matched by target then	insert([Name],[RequiredPermissions])	values(s.[Name],s.[RequiredPermissions]);
