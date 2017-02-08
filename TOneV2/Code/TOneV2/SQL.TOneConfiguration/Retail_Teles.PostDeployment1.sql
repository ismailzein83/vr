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


--[common].[extensionconfiguration]-------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('983C3D4F-5233-4D00-805C-F60D9015FF7C','Teles API Configuration','Teles API Configuration','Retail_BE_SwitchIntegration','{"Editor":"retail-teles-switchintegrations-telesapi"}'),
('772C0B47-A8C0-4F15-B2F1-AEBF52B3EF08','RoutingGroupCondition','Routing Group Condition','Retail_Teles_RoutingGroupCondition','{"Editor":"retail-teles-routinggroupcondition-routinggroupfilter"}'),
('09A1029D-AFC0-48E4-B1B3-FD951462E267',' MappingTelesAccount',' Mapping Teles Account','Retail_BE_AccountActionDefinitionConfig','{"Editor":"retail-teles-accountactiondefinitionsettings-mappingtelesaccount"}'),
('7ACEF5E4-0392-445F-97A9-C7251A66DFFC','ChangeUsersRGs','Change Users Routing Group','Retail_BE_ProvisionerDefinition','{"DefinitionEditor":"retail-teles-provisioner-definitionsettings-changeusersrgs", "RuntimeEditor":"retail-teles-provisioner-runtimesettings-changeusersrgs"}'),
('F5AEB249-3D8A-4235-8C7B-2BA5B99D0B0D','RevertUsersRGs','Revert Users Routing Group','Retail_BE_ProvisionerDefinition','{"DefinitionEditor":"retail-teles-provisioner-definitionsettings-revertusersrgs", "RuntimeEditor":"retail-teles-provisioner-runtimesettings-revertusersrgs"}')
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

