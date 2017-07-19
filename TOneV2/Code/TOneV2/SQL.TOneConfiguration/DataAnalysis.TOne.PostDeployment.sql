﻿/*
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
--[Analytic].[DataAnalysisDefinition]---------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('164BB9EA-84F4-43DD-823D-31F632DBADD1','Profiling And Calculation Data Analysis','{"$type":"Vanrise.Analytic.Entities.DAProfCalcSettings, Vanrise.Analytic.Entities","ConfigId":"b3af681b-72ce-4dd8-9090-cc727690f7e0","DataRecordTypeId":"56e04aec-8b71-4eda-9aca-485bf9bc8cd6","ItemsConfig":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.DataAnalysisItemDefinitionConfig, Vanrise.Analytic.Entities]], mscorlib","$values":[{"$type":"Vanrise.Analytic.Entities.DataAnalysisItemDefinitionConfig, Vanrise.Analytic.Entities","TypeId":"39e04643-3c5c-4d11-9d3c-41611c34f7b3","Title":"Record Profiling","Editor":"vr-analytic-recordprofilingoutputsettings-editor","GridDirective":"vr-analytic-dataanalysisitemdefinition-grid"}]}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings]))
merge	[Analytic].[DataAnalysisDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Settings])
	values(s.[ID],s.[Name],s.[Settings]);


--[Analytic].[DataAnalysisItemDefinition]-----------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[DataAnalysisDefinitionID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('1EEC9D7E-A5E7-4E81-A4F5-3E37A7591A90','164BB9EA-84F4-43DD-823D-31F632DBADD1','Traffic Analysis Profiling','{"$type":"Vanrise.Analytic.Entities.DataAnalysis.ProfilingAndCalculation.OutputDefinitions.RecordProfilingOutputSettings, Vanrise.Analytic.Entities","ItemDefinitionTypeId":"39e04643-3c5c-4d11-9d3c-41611c34f7b3","GroupingFields":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.DAProfCalcGroupingField, Vanrise.Analytic.Entities]], mscorlib","$values":[{"$type":"Vanrise.Analytic.Entities.DAProfCalcGroupingField, Vanrise.Analytic.Entities","FieldName":"CustomerId","FieldTitle":"Customer","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","BusinessEntityDefinitionId":"ba5a57bd-1f03-440f-a469-463a48762b8f","IsNullable":true,"OrderType":1}},{"$type":"Vanrise.Analytic.Entities.DAProfCalcGroupingField, Vanrise.Analytic.Entities","FieldName":"SupplierId","FieldTitle":"Supplier","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","BusinessEntityDefinitionId":"8c286bcd-5766-487a-8b32-5d167ec342c0","IsNullable":true,"OrderType":1}},{"$type":"Vanrise.Analytic.Entities.DAProfCalcGroupingField, Vanrise.Analytic.Entities","FieldName":"SaleZoneId","FieldTitle":"Sale Zone","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","BusinessEntityDefinitionId":"900d0e5d-0fa7-428e-a83b-cd64e16f7415","IsNullable":true,"OrderType":1}},{"$type":"Vanrise.Analytic.Entities.DAProfCalcGroupingField, Vanrise.Analytic.Entities","FieldName":"SupplierZoneId","FieldTitle":"Supplier Zone","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","BusinessEntityDefinitionId":"ad86042c-0b49-4379-966a-dc0d39adba6d","IsNullable":true,"OrderType":1}}]},"AggregationFields":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.DAProfCalcAggregationField, Vanrise.Analytic.Entities]], mscorlib","$values":[{"$type":"Vanrise.Analytic.Entities.DAProfCalcAggregationField, Vanrise.Analytic.Entities","FieldName":"TotalSaleNet","FieldTitle":"Total Sale Net","RecordAggregate":{"$type":"Vanrise.Analytic.MainExtensions.DARecordAggregates.SumAggregate, Vanrise.Analytic.MainExtensions","ConfigId":"dc962a83-2fda-456f-9940-15e9be787d89","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","DataPrecision":0,"DataType":0,"IsNullable":false,"OrderType":0},"SumFieldName":"SaleNet"}},{"$type":"Vanrise.Analytic.Entities.DAProfCalcAggregationField, Vanrise.Analytic.Entities","FieldName":"TotalCostNet","FieldTitle":"Total Cost Net","RecordAggregate":{"$type":"Vanrise.Analytic.MainExtensions.DARecordAggregates.SumAggregate, Vanrise.Analytic.MainExtensions","ConfigId":"dc962a83-2fda-456f-9940-15e9be787d89","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","DataPrecision":0,"DataType":0,"IsNullable":false,"OrderType":0},"SumFieldName":"CostNet"}},{"$type":"Vanrise.Analytic.Entities.DAProfCalcAggregationField, Vanrise.Analytic.Entities","FieldName":"TotalSuccesfulAttempts","FieldTitle":"Total Succesful Attempts","RecordAggregate":{"$type":"Vanrise.Analytic.MainExtensions.DARecordAggregates.SumAggregate, Vanrise.Analytic.MainExtensions","ConfigId":"dc962a83-2fda-456f-9940-15e9be787d89","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","DataPrecision":0,"DataType":0,"IsNullable":false,"OrderType":0},"SumFieldName":"SuccessfulAttempts"}},{"$type":"Vanrise.Analytic.Entities.DAProfCalcAggregationField, Vanrise.Analytic.Entities","FieldName":"TotalAttempts","FieldTitle":"Total Attempts","RecordAggregate":{"$type":"Vanrise.Analytic.MainExtensions.DARecordAggregates.SumAggregate, Vanrise.Analytic.MainExtensions","ConfigId":"dc962a83-2fda-456f-9940-15e9be787d89","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","DataPrecision":0,"DataType":0,"IsNullable":false,"OrderType":0},"SumFieldName":"Attempts"}},{"$type":"Vanrise.Analytic.Entities.DAProfCalcAggregationField, Vanrise.Analytic.Entities","FieldName":"TotalDurationInSeconds","FieldTitle":"Total Duration In Seconds","RecordAggregate":{"$type":"Vanrise.Analytic.MainExtensions.DARecordAggregates.SumAggregate, Vanrise.Analytic.MainExtensions","ConfigId":"dc962a83-2fda-456f-9940-15e9be787d89","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","DataPrecision":0,"DataType":0,"IsNullable":false,"OrderType":0},"SumFieldName":"DurationInSeconds"}}]},"CalculationFields":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.DAProfCalcCalculationField, Vanrise.Analytic.Entities]], mscorlib","$values":[{"$type":"Vanrise.Analytic.Entities.DAProfCalcCalculationField, Vanrise.Analytic.Entities","FieldName":"ASR","FieldTitle":"ASR","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","DataPrecision":1,"DataType":0,"IsNullable":true,"OrderType":0},"Expression":"decimal? totalSuccesfulAttempts= context.GetAggregateValue(\"TotalSuccesfulAttempts\");\n\ndecimal? totalAttempts = context.GetAggregateValue(\"TotalAttempts\");\n\nif(totalSuccesfulAttempts.HasValue &amp;&amp; totalAttempts.HasValue &amp;&amp; totalAttempts.Value &gt; 0)\nreturn totalSuccesfulAttempts.Value/totalAttempts.Value * 100;\n\nreturn null;"},{"$type":"Vanrise.Analytic.Entities.DAProfCalcCalculationField, Vanrise.Analytic.Entities","FieldName":"ACD","FieldTitle":"ACD","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","DataPrecision":1,"DataType":0,"IsNullable":true,"OrderType":0},"Expression":"decimal? totalDurationInSeconds = context.GetAggregateValue(\"TotalDurationInSeconds\");\n\ndecimal? totalAttempts = context.GetAggregateValue(\"TotalAttempts\");\n\nif(totalDurationInSeconds.HasValue &amp;&amp; totalAttempts.HasValue &amp;&amp; totalAttempts.Value &gt; 0)\n{\ndecimal totalDurationInMinutes = totalDurationInSeconds.Value/60;\nreturn totalDurationInMinutes/totalAttempts.Value;\n}\nreturn null;"},{"$type":"Vanrise.Analytic.Entities.DAProfCalcCalculationField, Vanrise.Analytic.Entities","FieldName":"Profit","FieldTitle":"Profit","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","DataPrecision":1,"DataType":0,"IsNullable":true,"OrderType":0},"Expression":"decimal? totalSaleNet = context.GetAggregateValue(\"TotalSaleNet\");\ndecimal? totalCostNet = context.GetAggregateValue(\"TotalCostNet\");\n\nif(totalSaleNet.HasValue &amp;&amp; totalCostNet .HasValue)\nreturn totalSaleNet.Value - totalCostNet.Value;\nreturn null;"}]},"RecordTypeId":"3ba91471-17d2-4c16-b458-8a0959d2a4c4","Title":"Traffic Analysis Profiling"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[DataAnalysisDefinitionID],[Name],[Settings]))
merge	[Analytic].[DataAnalysisItemDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[DataAnalysisDefinitionID] = s.[DataAnalysisDefinitionID],[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[DataAnalysisDefinitionID],[Name],[Settings])
	values(s.[ID],s.[DataAnalysisDefinitionID],s.[Name],s.[Settings]);


--[genericdata].[DataRecordType]--------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[ParentID],[Fields],[ExtraFieldsEvaluator],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('3BA91471-17D2-4C16-B458-8A0959D2A4C4','Traffic Analysis Record Type',null,'{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities]], mscorlib","$values":[]}','{"$type":"Vanrise.Analytic.Business.DAProfCalcRecordTypeExtraFields, Vanrise.Analytic.Business","ConfigId":"93f44a29-235d-4c3f-900e-6d7fe780cef3","DataAnalysisDefinitionId":"164bb9ea-84f4-43dd-823d-31f632dbadd1","DataAnalysisItemDefinitionId":"1eec9d7e-a5e7-4e81-a4f5-3e37a7591a90"}',null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[ParentID],[Fields],[ExtraFieldsEvaluator],[Settings]))
merge	[genericdata].[DataRecordType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[ParentID] = s.[ParentID],[Fields] = s.[Fields],[ExtraFieldsEvaluator] = s.[ExtraFieldsEvaluator],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[ParentID],[Fields],[ExtraFieldsEvaluator],[Settings])
	values(s.[ID],s.[Name],s.[ParentID],s.[Fields],s.[ExtraFieldsEvaluator],s.[Settings]);


--[common].[VRComponentType]------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[ConfigID],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('27FCF805-8673-4928-A216-E66E64ED4DF9','Traffic Analysis Profiling Notification','FDD73530-067F-4160-AB71-7852303C785C','{"$type":"Vanrise.Notification.Entities.VRNotificationTypeSettings, Vanrise.Notification.Entities","VRComponentTypeConfigId":"fdd73530-067f-4160-ab71-7852303c785c","VRAlertLevelDefinitionId":"48fc148c-299a-4717-bd03-401bb79c082e","ExtendedSettings":{"$type":"Vanrise.GenericData.Notification.DataRecordNotificationTypeSettings, Vanrise.GenericData.Notification","ConfigId":"e64c51a2-08e0-4b7d-96f0-9ff1848a72fa","SearchRuntimeEditor":"vr-genericdata-datarecordnotificationtypesettings-searcheditor","BodyRuntimeEditor":"vr-genericdata-datarecordnotificationtypesettings-bodyeditor","DataRecordTypeId":"3ba91471-17d2-4c16-b458-8a0959d2a4c4","GridColumnDefinitions":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"CustomerId","Header":"Customer"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"SupplierId","Header":"Supplier"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"SaleZoneId","Header":"Sale Zone"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"SupplierZoneId","Header":"Supplier Zone"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"TotalSaleNet","Header":"Total Sale Net"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"TotalCostNet","Header":"Total Cost Net"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"ASR","Header":"ASR"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"ACD","Header":"ACD"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"Profit","Header":"Profit"}]}},"Security":{"$type":"Vanrise.Notification.Entities.VRNotificationTypeSecurity, Vanrise.Notification.Entities","ViewRequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"2f96aeb3-e0b2-43ba-8092-95f582c6b1ab","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View Alerts"]}}]}},"HideRequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}}}}'),
('4042CC4A-2275-428D-B3AA-9574833C0167','Send Email','D96F17C8-29D7-4C0C-88DC-9D5FBCA2178F','{"$type":"Vanrise.Notification.Entities.VRActionDefinitionSettings, Vanrise.Notification.Entities","VRComponentTypeConfigId":"d96f17c8-29d7-4c0c-88dc-9d5fbca2178f","ExtendedSettings":{"$type":"Vanrise.GenericData.Notification.DataRecordSendEmailDefinitionSettings, Vanrise.GenericData.Notification","ConfigId":"3b904e8c-2ac0-43db-a4ef-425869d40544","RuntimeEditor":"vr-genericdata-datarecord-vraction-sendemail","DataRecordTypeId":"3ba91471-17d2-4c16-b458-8a0959d2a4c4","MailMessageTypeId":"c1348cec-3c86-44d8-9c2d-e75277372f50","DataRecordObjectName":"Traffic Analysis","ObjectFieldMappings":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Notification.ObjectFieldMapping, Vanrise.GenericData.Notification]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Notification.ObjectFieldMapping, Vanrise.GenericData.Notification","ObjectName":"Customer","VRObjectTypeDefinitionId":"61b75db7-aae0-4ed2-846c-d0403c26d8d7","DataRecordFieldName":"CustomerId"}]}}}')
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


--[common].[VRObjectTypeDefinition]-----------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('4C2A01B1-75AB-4CEF-AFEA-223B591FA584','Traffic Analysis Object','{"$type":"Vanrise.Entities.VRObjectTypeDefinitionSettings, Vanrise.Entities","ObjectType":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordObjectType, Vanrise.GenericData.MainExtensions","ConfigId":"bbc57155-0412-4371-83e5-1917a8bea468","RecordTypeId":"3ba91471-17d2-4c16-b458-8a0959d2a4c4"},"Properties":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities]], mscorlib","ASR":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"ASR","Description":"ASR","PropertyEvaluator":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordFieldPropertyEvaluator, Vanrise.GenericData.MainExtensions","ConfigId":"f663bf74-99db-4746-8cbc-e74198e1786c","FieldName":"ASR","UseDescription":false}},"ACD":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"ACD","Description":"ACD","PropertyEvaluator":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordFieldPropertyEvaluator, Vanrise.GenericData.MainExtensions","ConfigId":"f663bf74-99db-4746-8cbc-e74198e1786c","FieldName":"ACD","UseDescription":false}},"Profit":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Profit","Description":"Profit","PropertyEvaluator":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordFieldPropertyEvaluator, Vanrise.GenericData.MainExtensions","ConfigId":"f663bf74-99db-4746-8cbc-e74198e1786c","FieldName":"Profit","UseDescription":false}}}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings]))
merge	[common].[VRObjectTypeDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Settings])
	values(s.[ID],s.[Name],s.[Settings]);


--[common].[MailMessageType]------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('C1348CEC-3C86-44D8-9C2D-E75277372F50','Traffic Analysis Notification','{"$type":"Vanrise.Entities.VRMailMessageTypeSettings, Vanrise.Entities","Objects":{"$type":"Vanrise.Entities.VRObjectVariableCollection, Vanrise.Entities","Traffic Analysis":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"Traffic Analysis","VRObjectTypeDefinitionId":"4c2a01b1-75ab-4cef-afea-223b591fa584"},"Customer":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"Customer","VRObjectTypeDefinitionId":"61b75db7-aae0-4ed2-846c-d0403c26d8d7"}}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings]))
merge	[common].[MailMessageType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Settings])
	values(s.[ID],s.[Name],s.[Settings]);


--[VRNotification].[VRAlertRuleType]----------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('4FC13106-BC23-4DEE-8777-453CC6799430','Traffic Statistic','{"$type":"Vanrise.Analytic.Entities.DAProfCalcAlertRuleTypeSettings, Vanrise.Analytic.Entities","CriteriaEditor":"vr-analytic-daprofcalc-alertrulecriteria","ConfigId":"57033e80-65cb-4359-95f6-22a57084d027","SettingEditor":"vr-analytic-daprofcalc-alertrulesettings","DataAnalysisDefinitionId":"164bb9ea-84f4-43dd-823d-31f632dbadd1","SourceRecordStorages":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.DAProfCalcAlertRuleRecordStorage, Vanrise.Analytic.Entities]], mscorlib","$values":[{"$type":"Vanrise.Analytic.Entities.DAProfCalcAlertRuleRecordStorage, Vanrise.Analytic.Entities","DataRecordStorageId":"ce0a0e1c-92d3-44c1-9859-ca58270d3aa4"}]},"DAProfCalcItemNotifications":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.DAProfCalcItemNotification, Vanrise.Analytic.Entities]], mscorlib","$values":[{"$type":"Vanrise.Analytic.Entities.DAProfCalcItemNotification, Vanrise.Analytic.Entities","DataAnalysisItemDefinitionId":"1eec9d7e-a5e7-4e81-a4f5-3e37a7591a90","NotificationTypeId":"27fcf805-8673-4928-a216-e66e64ed4df9"}]},"DAProfCalcSecurity":{"$type":"Vanrise.Analytic.Entities.DAProfCalcAlertRuleTypeSecurity, Vanrise.Analytic.Entities","ViewPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"2f96aeb3-e0b2-43ba-8092-95f582c6b1ab","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View Rules"]}}]}},"StartInstancePermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"2f96aeb3-e0b2-43ba-8092-95f582c6b1ab","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Add Rules"]}}]}}},"RawRecordFilterLabel":"Billing CDR Condition","Security":{"$type":"Vanrise.Notification.Entities.AlertRuleTypeSecurity, Vanrise.Notification.Entities","ViewPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"2f96aeb3-e0b2-43ba-8092-95f582c6b1ab","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View Rules"]}}]}},"AddPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"2f96aeb3-e0b2-43ba-8092-95f582c6b1ab","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Add Rules"]}}]}},"EditPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"2f96aeb3-e0b2-43ba-8092-95f582c6b1ab","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Edit Rules"]}}]}}}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings]))
merge	[VRNotification].[VRAlertRuleType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Settings])
	values(s.[ID],s.[Name],s.[Settings]);

