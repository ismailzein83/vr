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


--[common].[VRComponentType]------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[ConfigID],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('57608C11-1C43-489B-997C-FF6E8B1314C0','Profile Message Type','80791C8A-5F81-4D2E-B3D7-4240CF967FA0','{"$type":"Vanrise.Entities.SMSMessageTypeSettings, Vanrise.Entities","Objects":{"$type":"Vanrise.Entities.VRObjectVariableCollection, Vanrise.Entities","Profile":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"Profile","VRObjectTypeDefinitionId":"dfeef80e-4e65-4232-8150-15fcb08826da"},"User":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"User","VRObjectTypeDefinitionId":"e3887cc9-1fbb-44d1-b1e3-7a0922400550"},"AccountBalance":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"AccountBalance","VRObjectTypeDefinitionId":"1c93042e-939b-4022-9f13-43c3718ef644"},"Threshold":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"Threshold","VRObjectTypeDefinitionId":"1c93042e-939b-4022-9f13-43c3718ef644"}},"VRComponentTypeConfigId":"80791c8a-5f81-4d2e-b3d7-4240cf967fa0"}'),
('6937C4EC-5377-4F71-97A1-AE5EE4710282','Customer Message Type','80791C8A-5F81-4D2E-B3D7-4240CF967FA0','{"$type":"Vanrise.Entities.SMSMessageTypeSettings, Vanrise.Entities","Objects":{"$type":"Vanrise.Entities.VRObjectVariableCollection, Vanrise.Entities","Customer":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"Customer","VRObjectTypeDefinitionId":"61b75db7-aae0-4ed2-846c-d0403c26d8d7"},"AccountBalance":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"AccountBalance","VRObjectTypeDefinitionId":"1c93042e-939b-4022-9f13-43c3718ef644"},"Threshold":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"Threshold","VRObjectTypeDefinitionId":"1c93042e-939b-4022-9f13-43c3718ef644"},"User":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"User","VRObjectTypeDefinitionId":"e3887cc9-1fbb-44d1-b1e3-7a0922400550"}},"VRComponentTypeConfigId":"80791c8a-5f81-4d2e-b3d7-4240cf967fa0"}'),
('653fc3f1-33cb-4831-826c-ef9848e67a90','Send SMS','d96f17c8-29d7-4c0c-88dc-9d5fbca2178f','{"$type":"Vanrise.Notification.Entities.VRActionDefinitionSettings, Vanrise.Notification.Entities","VRComponentTypeConfigId":"d96f17c8-29d7-4c0c-88dc-9d5fbca2178f","ExtendedSettings":{"$type":"TOne.WhS.AccountBalance.MainExtensions.VRBalanceAlertActions.SendCustomerSMSActionDefinition, TOne.WhS.AccountBalance.MainExtensions","AccountSMSMessageTypeId":"6937c4ec-5377-4f71-97a1-ae5ee4710282","ProfileSMSMessageTypeId":"57608c11-1c43-489b-997c-ff6e8b1314c0","ConfigId":"114ce0ac-4848-442a-b351-be031f22e130","RuntimeEditor":"whs-accountbalance-action-send-customer-sms"}}')

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



		
--[common].[Connection]-----------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('86D4A5E8-804A-4461-B3AE-7CDE32DBE6DF','SMS Integration Connection','{"$type":"Vanrise.Common.Business.SQLConnection, Vanrise.Common.Business","ConfigId":"8224b27c-c128-4150-a4e4-5e2034bb3a36","ConnectionString":"Server=xxxx;Database=xxxx;User ID=xxxxx;Password=xxxxx"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings]))
merge	[common].[Connection] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Settings])
	values(s.[ID],s.[Name],s.[Settings]);


--[common].[Setting]---------------------------------------101 to 200----------------------------------------

;with cte_data([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('26AA64F9-67C3-4BF6-BE94-6D59030B9BE3','System SMS','VR_Common_SMS','General','{"Editor":"vr-common-smstemplate-settings-editor"}','{"$type":"Vanrise.Entities.SMSSettingData, Vanrise.Entities","SMSSendHandler":{"$type":"Vanrise.Entities.SMSSendHandler, Vanrise.Entities","Settings":{"$type":"Vanrise.Common.MainExtensions.SMSSendHandler.ExecuteDatabaseCommandSMSHandler, Vanrise.Common.MainExtensions","ConfigId":"44e97625-1b35-478a-918e-60f9c58678b4","VRConnectionId":"86d4a5e8-804a-4461-b3ae-7cde32dbe6df","CommandQuery":"insert into [dbo].[cg_subs_sms_instant] ([mobilenum],[message], [timein], [status],[type])\nvalues(''#MobileNumber#'',''#Message#'',getDate(),''N'',''R'')"}}}',0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical]))
merge	[common].[Setting] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Type] = s.[Type],[Category] = s.[Category],[Settings] = s.[Settings],[Data] = s.[Data],[IsTechnical] = s.[IsTechnical]
when not matched by target then
	insert([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
	values(s.[ID],s.[Name],s.[Type],s.[Category],s.[Settings],s.[Data],s.[IsTechnical]);



	--[common].[SMSMessageTemplate]---------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[SMSMessageTypeId],[Settings],[CreatedBy],[LastModifiedBy],[LastModifiedTime])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('41924F3E-2DDF-41AE-B75A-84052CF9993D','Account Balance SMS','6937C4EC-5377-4F71-97A1-AE5EE4710282','{"$type":"Vanrise.Entities.SMSMessageTemplateSettings, Vanrise.Entities","MobileNumber":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"@Model.GetVal(\"Customer\",\"SMS Phone Numbers\")"},"Message":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"Dear @Model.GetVal(\"Customer\",\"Customer Name\"),\nYour balance is: @Model.GetVal(\"AccountBalance\",\"Value\")"}}',95,95,'2018-05-03 11:10:53.667'),
('59B232A4-BAF1-4D49-8CAC-BFFCC6006230','Profile Balance SMS','57608C11-1C43-489B-997C-FF6E8B1314C0','{"$type":"Vanrise.Entities.SMSMessageTemplateSettings, Vanrise.Entities","MobileNumber":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"@Model.GetVal(\"Profile\",\"SMS Phone Numbers\")"},"Message":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"Dear @Model.GetVal(\"Profile\",\"Carrier Name\"),\nYour balance is: @Model.GetVal(\"AccountBalance\",\"Value\")"}}',95,95,'2018-05-03 11:12:05.407')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[SMSMessageTypeId],[Settings],[CreatedBy],[LastModifiedBy],[LastModifiedTime]))
merge	[common].[SMSMessageTemplate] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[SMSMessageTypeId] = s.[SMSMessageTypeId],[Settings] = s.[Settings],[CreatedBy] = s.[CreatedBy],[LastModifiedBy] = s.[LastModifiedBy],[LastModifiedTime] = s.[LastModifiedTime]
when not matched by target then
	insert([ID],[Name],[SMSMessageTypeId],[Settings],[CreatedBy],[LastModifiedBy],[LastModifiedTime])
	values(s.[ID],s.[Name],s.[SMSMessageTypeId],s.[Settings],s.[CreatedBy],s.[LastModifiedBy],s.[LastModifiedTime]);



