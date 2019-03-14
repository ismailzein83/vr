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

--[common].[ExtensionConfiguration]-----------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('8BC7B9F4-872B-4189-AA6D-3C252E6C4019','BlockPortOutDefinitionSettings','Block Port Out','VR_Notification_VRActionDefinition','{"Editor":"whs-routing-actiondefinition-blockportout"}'),
('0AF9DF74-7844-4608-B6ED-66F093524BC7','BlockPortInDefinitionSettings','Block Port In','VR_Notification_VRActionDefinition','{"Editor":"whs-routing-actiondefinition-blockportin"}'),
('0FFBCC52-9261-4B13-8514-9D20B4F6B029','BlockOriginationNumberDefinitionSettings','Block Origination Number','VR_Notification_VRActionDefinition','{"Editor":"whs-routing-actiondefinition-blockoriginationnumber"}'),
('E5240E24-512F-43E3-9E4D-A15F75D9F5BE','BlockDestinationNumberDefinitionSettings','Block Destination Number','VR_Notification_VRActionDefinition','{"Editor":"whs-routing-actiondefinition-blockdestinationnumber"}')
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


	--[common].[VRComponentType]------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[ConfigID],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('203112B5-5964-4EE6-8473-03094E179F69','Block Port In','D96F17C8-29D7-4C0C-88DC-9D5FBCA2178F','{"$type":"Vanrise.Notification.Entities.VRActionDefinitionSettings, Vanrise.Notification.Entities","VRComponentTypeConfigId":"d96f17c8-29d7-4c0c-88dc-9d5fbca2178f","ExtendedSettings":{"$type":"TOne.WhS.Routing.MainExtensions.VRActions.BlockPortInDefinitionSettings, TOne.WhS.Routing.MainExtensions","ConfigId":"0af9df74-7844-4608-b6ed-66f093524bc7","RuntimeEditor":"whs-routing-action-blockportin","DataRecordTypeId":"3ba91471-17d2-4c16-b458-8a0959d2a4c4","SwitchFieldName":"SwitchId","PortInFieldName":"PortIN"}}'),
('FAAFA941-DEF5-4208-815E-70BCA4AB7170','Block Port Out','D96F17C8-29D7-4C0C-88DC-9D5FBCA2178F','{"$type":"Vanrise.Notification.Entities.VRActionDefinitionSettings, Vanrise.Notification.Entities","VRComponentTypeConfigId":"d96f17c8-29d7-4c0c-88dc-9d5fbca2178f","ExtendedSettings":{"$type":"TOne.WhS.Routing.MainExtensions.VRActions.BlockPortOutDefinitionSettings, TOne.WhS.Routing.MainExtensions","ConfigId":"8bc7b9f4-872b-4189-aa6d-3c252e6c4019","RuntimeEditor":"whs-routing-action-blockportout","DataRecordTypeId":"3ba91471-17d2-4c16-b458-8a0959d2a4c4","SwitchFieldName":"SwitchId","PortOutFieldName":"PortOUT"}}'),
('9F16476D-5763-45B8-A952-FAC0BFC6E9F5','Block Origination Number','D96F17C8-29D7-4C0C-88DC-9D5FBCA2178F','{"$type":"Vanrise.Notification.Entities.VRActionDefinitionSettings, Vanrise.Notification.Entities","VRComponentTypeConfigId":"d96f17c8-29d7-4c0c-88dc-9d5fbca2178f","ExtendedSettings":{"$type":"TOne.WhS.Routing.MainExtensions.VRActions.BlockOriginationNumberDefinitionSettings, TOne.WhS.Routing.MainExtensions","ConfigId":"0ffbcc52-9261-4b13-8514-9d20b4f6b029","RuntimeEditor":"whs-routing-action-blockoriginationnumber","DataRecordTypeId":"3ba91471-17d2-4c16-b458-8a0959d2a4c4","OriginationNumberFieldName":"CGPN"}}'),
('7EB05969-63D0-4ABE-8ED5-EA3EFA8857CA','Block Destination Number','D96F17C8-29D7-4C0C-88DC-9D5FBCA2178F','{"$type":"Vanrise.Notification.Entities.VRActionDefinitionSettings, Vanrise.Notification.Entities","VRComponentTypeConfigId":"d96f17c8-29d7-4c0c-88dc-9d5fbca2178f","ExtendedSettings":{"$type":"TOne.WhS.Routing.MainExtensions.VRActions.BlockDestinationNumberDefinitionSettings, TOne.WhS.Routing.MainExtensions","ConfigId":"e5240e24-512f-43e3-9e4d-a15f75d9f5be","RuntimeEditor":"whs-routing-action-blockdestinationnumber","DataRecordTypeId":"3ba91471-17d2-4c16-b458-8a0959d2a4c4","DestinationNumberFieldName":"CDPN"}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[ConfigID],[Settings]))
merge	[common].[VRComponentType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[ConfigID] = s.[ConfigID],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[ConfigID],[Settings])
	values(s.[ID],s.[Name],s.[ConfigID],s.[Settings]);
