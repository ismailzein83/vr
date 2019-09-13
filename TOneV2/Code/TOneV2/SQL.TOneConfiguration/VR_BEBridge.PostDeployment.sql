﻿



















--Make sure to use same .json file using DEVTOOLS under http://192.168.110.185:8037




























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

--[common].[ExtensionConfiguration]---------------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('DF73CDCE-01AB-4B55-9511-3A56B495A0D0','FTP Source Reader','FTP Source Reader','VR_BEBridge_SourceBeReaders'		,'{"Editor":"vr-bebridge-sourcebereaders-ftp-directive"}'),
('3BECA9AA-8F75-447B-82FE-F50DDB183730','File Source Reader','File Source Reader','VR_BEBridge_SourceBeReaders'		,'{"Editor":"vr-bebridge-sourcebereaders-file-directive"}'),
('B2AD979E-818C-46A2-8426-D60AE7F0F841','CRM Source Reader','CRM Source Reader','VR_BEBridge_SourceBeReaders'		,' {"Editor":"vr-bebridge-sourcebereaders-crm-directive"}'),
('287A031D-D476-4B03-9478-76DCB9076A95','SQL Source Reader','SQL Source Reader','VR_BEBridge_SourceBeReaders'		,'{"Editor":"vr-bebridge-sourcebereaders-sql-directive"}'),
('E761535C-9768-4047-8B61-FDE5D83A9A4A','Database Synchronizer','Database Synchronizer','VR_BEBridge_BESynchronizer','{"Editor":"vr-bebridge-synchronizer-database"}'),

('05A15B37-43CB-4D0D-B453-803645D25C03','POP3 Source Reader','POP3 Source Reader','VR_BEBridge_SourceBeReaders'							,'{"Editor":"vr-bebridge-sourcebereaders-pop3-directive"}'),
('FD7A74F9-34DD-460A-87A4-288684A6C9B1','Received Mail Message Convertor','Received Mail Message Convertor','VR_BEBridge_BEConvertor'	,'{"Editor":"vr-common-receivedmailmessageconvertor-editor"}'),
('E3BF7C73-14BA-402B-9158-A67D03635447','POP3 Connection','POP3 Connection','VRCommon_ConnectionConfig'									,'{"Editor":"vr-common-pop3connection-editor"}')
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


--[bp].[BPDefinition]----------------------5001 to 6000---------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Title],[FQTN],[Config])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('DD5BEB84-AAA3-4777-AF2F-A20BACBA5C07','Vanrise.BEBridge.BP.Arguments.SourceBESyncProcessInput','Source BE Sync','Vanrise.BEBridge.BP.BEBridgeProcess,Vanrise.BEBridge.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"ManualExecEditor":"vr-bebridge-process-manual","ScheduledExecEditor":"vr-bebridge-process-scheduled","IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":false,"ExtendedSettings":{"$type":"Vanrise.BEBridge.Business.BEReceiveDefinitionBPExtentedSettings, Vanrise.BEBridge.Business"}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[FQTN],[Config]))
merge	[bp].[BPDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[FQTN] = s.[FQTN],[Config] = s.[Config]
when not matched by target then
	insert([ID],[Name],[Title],[FQTN],[Config])
	values(s.[ID],s.[Name],s.[Title],s.[FQTN],s.[Config]);
----------------------------------------------------------------------------------------------------
END


--sec.module----------------------------------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('792AAF3B-1AD8-4C05-84C7-1C77E5A01289','BE Bridge',null,'D018C0CD-F15F-486D-80C3-F9B87C3F47B8',null,60,null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic]))
merge	[sec].[module] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Url] = s.[Url],[ParentId] = s.[ParentId],[Icon] = s.[Icon],[Rank] = s.[Rank],[AllowDynamic] = s.[AllowDynamic]
when not matched by target then
	insert([ID],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
	values(s.[ID],s.[Name],s.[Url],s.[ParentId],s.[Icon],s.[Rank],s.[AllowDynamic]);
----------------------------------------------------------------------------------------------------
end

--[sec].[View]----------------------------------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('B05D114E-8711-4621-A890-FB84E20FDA95','Receive Definition','Receive Definition','#/view/VR_BEBridge/Views/BEReceiveDefinition/BEReceiveDefinitionManagement','792AAF3B-1AD8-4C05-84C7-1C77E5A01289','VR_BEBridge/BERecieveDefinition/GetFilteredBeReceiveDefinitions',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',2)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank]
when not matched by target then
	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank]);
----------------------------------------------------------------------------------------------------
end

--[sec].[SystemAction]------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([Name],[RequiredPermissions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('VR_BEBridge/BERecieveDefinition/GetBERecieveDefinitionsInfo',null),
('VR_BEBridge/BERecieveDefinition/GetFilteredBeReceiveDefinitions','VR_SystemConfiguration: View'),
('VR_BEBridge/BERecieveDefinition/GetReceiveDefinition',null),
('VR_BEBridge/BERecieveDefinition/UpdateReceiveDefinition','VR_SystemConfiguration: Edit'),
('VR_BEBridge/BERecieveDefinition/AddReceiveDefinition','VR_SystemConfiguration: Add'),
('VR_BEBridge/BERecieveDefinition/GetSourceReaderExtensionConfigs',null),
('VR_BEBridge/BERecieveDefinition/GetTargetSynchronizerExtensionConfigs',null),
('VR_BEBridge/BERecieveDefinition/GetTargetConvertorExtensionConfigs',null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Name],[RequiredPermissions]))
merge	[sec].[SystemAction] as t
using	cte_data as s
on		1=1 and t.[Name] = s.[Name]
when matched then
	update set
	[RequiredPermissions] = s.[RequiredPermissions]
when not matched by target then
	insert([Name],[RequiredPermissions])
	values(s.[Name],s.[RequiredPermissions]);
