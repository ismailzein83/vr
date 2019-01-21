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
--[Analytic].[DataAnalysisDefinition]---------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('A8520A8F-B914-44EF-96E4-4E1AFF5358E0','Profiling And Calculation Data Analysis','{"$type":"Vanrise.Analytic.Entities.DAProfCalcSettings, Vanrise.Analytic.Entities","ConfigId":"b3af681b-72ce-4dd8-9090-cc727690f7e0","DataRecordTypeId":"d818c097-822d-4ea5-ae22-f7d208154a78","HideActionRuleRecordFilter":false,"UseChunkTime":false,"ItemsConfig":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.DataAnalysisItemDefinitionConfig, Vanrise.Analytic.Entities]], mscorlib","$values":[{"$type":"Vanrise.Analytic.Entities.DataAnalysisItemDefinitionConfig, Vanrise.Analytic.Entities","TypeId":"39e04643-3c5c-4d11-9d3c-41611c34f7b3","Title":"Record Profiling","Editor":"vr-analytic-recordprofilingoutputsettings-editor","GridDirective":"vr-analytic-dataanalysisitemdefinition-grid"}]}}')
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
('E2CC1450-CED9-4640-B875-FFCB94C3F84A','A8520A8F-B914-44EF-96E4-4E1AFF5358E0','Billing Analysis Profiling','{"$type":"Vanrise.Analytic.Entities.DataAnalysis.ProfilingAndCalculation.OutputDefinitions.RecordProfilingOutputSettings, Vanrise.Analytic.Entities","ItemDefinitionTypeId":"39e04643-3c5c-4d11-9d3c-41611c34f7b3","GroupingFields":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.DAProfCalcGroupingField, Vanrise.Analytic.Entities]], mscorlib","$values":[{"$type":"Vanrise.Analytic.Entities.DAProfCalcGroupingField, Vanrise.Analytic.Entities","FieldName":"Company","FieldTitle":"Company","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","RuntimeEditor":"vr-genericdata-fieldtype-businessentity-runtimeeditor","ViewerEditor":"vr-genericdata-fieldtype-businessentity-viewereditor","BusinessEntityDefinitionId":"9a427357-cf55-4f33-99f7-745206dee7cd","IsNullable":true,"OrderType":1,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","StoreValueSerialized":false,"CanRoundValue":false}},{"$type":"Vanrise.Analytic.Entities.DAProfCalcGroupingField, Vanrise.Analytic.Entities","FieldName":"Branch","FieldTitle":"Branch","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","RuntimeEditor":"vr-genericdata-fieldtype-businessentity-runtimeeditor","ViewerEditor":"vr-genericdata-fieldtype-businessentity-viewereditor","BusinessEntityDefinitionId":"9a427357-cf55-4f33-99f7-745206dee7cd","IsNullable":false,"OrderType":1,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","StoreValueSerialized":false,"CanRoundValue":false}},{"$type":"Vanrise.Analytic.Entities.DAProfCalcGroupingField, Vanrise.Analytic.Entities","FieldName":"User","FieldTitle":"User","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","RuntimeEditor":"vr-genericdata-fieldtype-businessentity-runtimeeditor","ViewerEditor":"vr-genericdata-fieldtype-businessentity-viewereditor","BusinessEntityDefinitionId":"9a427357-cf55-4f33-99f7-745206dee7cd","IsNullable":true,"OrderType":1,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","StoreValueSerialized":false,"CanRoundValue":false}},{"$type":"Vanrise.Analytic.Entities.DAProfCalcGroupingField, Vanrise.Analytic.Entities","FieldName":"ServiceType","FieldTitle":"Service Type","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","RuntimeEditor":"vr-genericdata-fieldtype-businessentity-runtimeeditor","ViewerEditor":"vr-genericdata-fieldtype-businessentity-viewereditor","BusinessEntityDefinitionId":"bfad446f-7129-45b1-94bf-febd290f394d","IsNullable":true,"OrderType":1,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","StoreValueSerialized":false,"CanRoundValue":false}},{"$type":"Vanrise.Analytic.Entities.DAProfCalcGroupingField, Vanrise.Analytic.Entities","FieldName":"Zone","FieldTitle":"Zone","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","RuntimeEditor":"vr-genericdata-fieldtype-businessentity-runtimeeditor","ViewerEditor":"vr-genericdata-fieldtype-businessentity-viewereditor","BusinessEntityDefinitionId":"f650d523-7adb-4787-a2f6-c13168f7e8f7","IsNullable":true,"OrderType":1,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","StoreValueSerialized":false,"CanRoundValue":false}},{"$type":"Vanrise.Analytic.Entities.DAProfCalcGroupingField, Vanrise.Analytic.Entities","FieldName":"InterconnectOperator","FieldTitle":"InterconnectOperator","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","RuntimeEditor":"vr-genericdata-fieldtype-businessentity-runtimeeditor","ViewerEditor":"vr-genericdata-fieldtype-businessentity-viewereditor","BusinessEntityDefinitionId":"a5c1852b-2c92-4d66-b959-e3f49304338a","IsNullable":false,"OrderType":1,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","StoreValueSerialized":false,"CanRoundValue":false}},{"$type":"Vanrise.Analytic.Entities.DAProfCalcGroupingField, Vanrise.Analytic.Entities","FieldName":"TrafficDirection","FieldTitle":"Traffic Direction","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldChoicesType, Vanrise.GenericData.MainExtensions","ConfigId":"eabc41a9-e332-4120-ac85-f0b7e53c0d0d","RuntimeEditor":"vr-genericdata-fieldtype-choices-runtimeeditor","Choices":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.MainExtensions.DataRecordFields.Choice, Vanrise.GenericData.MainExtensions]], mscorlib","$values":[{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.Choice, Vanrise.GenericData.MainExtensions","Value":1,"Text":"IN"},{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.Choice, Vanrise.GenericData.MainExtensions","Value":2,"Text":"Out"}]},"IsNullable":true,"OrderType":1,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","StoreValueSerialized":false,"CanRoundValue":false}}]},"AggregationFields":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.DAProfCalcAggregationField, Vanrise.Analytic.Entities]], mscorlib","$values":[{"$type":"Vanrise.Analytic.Entities.DAProfCalcAggregationField, Vanrise.Analytic.Entities","FieldName":"TotalSaleAmount","FieldTitle":"Sale Amount","RecordAggregate":{"$type":"Vanrise.Analytic.MainExtensions.DARecordAggregates.SumAggregate, Vanrise.Analytic.MainExtensions","ConfigId":"dc962a83-2fda-456f-9940-15e9be787d89","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","ViewerEditor":"vr-genericdata-fieldtype-number-viewereditor","RuntimeEditor":"vr-genericdata-fieldtype-number-runtimeeditor","DataPrecision":0,"DataType":0,"IsNullable":false,"CanRoundValue":true,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false},"SumFieldName":"SaleAmount","CurrencySQLColumnName":"SaleCurrencyId"}},{"$type":"Vanrise.Analytic.Entities.DAProfCalcAggregationField, Vanrise.Analytic.Entities","FieldName":"TotalSaleDurationInSeconds","FieldTitle":"Sale Duration (sec)","RecordAggregate":{"$type":"Vanrise.Analytic.MainExtensions.DARecordAggregates.SumAggregate, Vanrise.Analytic.MainExtensions","ConfigId":"dc962a83-2fda-456f-9940-15e9be787d89","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","ViewerEditor":"vr-genericdata-fieldtype-number-viewereditor","RuntimeEditor":"vr-genericdata-fieldtype-number-runtimeeditor","DataPrecision":0,"DataType":0,"IsNullable":false,"CanRoundValue":true,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false},"SumFieldName":"SaleDurationInSeconds"}},{"$type":"Vanrise.Analytic.Entities.DAProfCalcAggregationField, Vanrise.Analytic.Entities","FieldName":"TotalNumberOfCDRs","FieldTitle":"Number Of CDRs","RecordAggregate":{"$type":"Vanrise.Analytic.MainExtensions.DARecordAggregates.CountAggregate, Vanrise.Analytic.MainExtensions","ConfigId":"dad39edb-65b1-4c40-935c-7e6339267055","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","ViewerEditor":"vr-genericdata-fieldtype-number-viewereditor","RuntimeEditor":"vr-genericdata-fieldtype-number-runtimeeditor","DataPrecision":0,"DataType":2,"IsNullable":false,"CanRoundValue":true,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false}}}]},"CalculationFields":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.DAProfCalcCalculationField, Vanrise.Analytic.Entities]], mscorlib","$values":[{"$type":"Vanrise.Analytic.Entities.DAProfCalcCalculationField, Vanrise.Analytic.Entities","FieldName":"TotalSaleDurationInMinutes","FieldTitle":"Sale Duration (min)","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","ViewerEditor":"vr-genericdata-fieldtype-number-viewereditor","RuntimeEditor":"vr-genericdata-fieldtype-number-runtimeeditor","DataPrecision":0,"DataType":0,"IsNullable":false,"CanRoundValue":true,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false},"Expression":"decimal? totalSaleDurationInSeconds = context.GetAggregateValue(\"TotalSaleDurationInSeconds\");\nreturn totalSaleDurationInSeconds.HasValue? totalSaleDurationInSeconds/60 : 0;"}]},"RecordTypeId":"c0820b92-0762-42cd-b0af-42ac37659e4e","Title":"Billing Analysis Profiling"}')
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
('C0820B92-0762-42CD-B0AF-42AC37659E4E','Billing Analysis',null,'{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities]], mscorlib","$values":[]}','{"$type":"Vanrise.Analytic.Business.DAProfCalcRecordTypeExtraFields, Vanrise.Analytic.Business","ConfigId":"93f44a29-235d-4c3f-900e-6d7fe780cef3","DataAnalysisDefinitionId":"a8520a8f-b914-44ef-96e4-4e1aff5358e0","DataAnalysisItemDefinitionId":"e2cc1450-ced9-4640-b875-ffcb94c3f84a"}','{"$type":"Vanrise.GenericData.Entities.DataRecordTypeSettings, Vanrise.GenericData.Entities"}')
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
('381CBECE-98E1-4801-9A8E-8E45EB9C77C3','Billing Analysis','FDD73530-067F-4160-AB71-7852303C785C','{"$type":"Vanrise.Notification.Entities.VRNotificationTypeSettings, Vanrise.Notification.Entities","VRComponentTypeConfigId":"fdd73530-067f-4160-ab71-7852303c785c","VRAlertLevelDefinitionId":"48fc148c-299a-4717-bd03-401bb79c082e","ExtendedSettings":{"$type":"Vanrise.GenericData.Notification.DataRecordNotificationTypeSettings, Vanrise.GenericData.Notification","ConfigId":"e64c51a2-08e0-4b7d-96f0-9ff1848a72fa","SearchRuntimeEditor":"vr-genericdata-datarecordnotificationtypesettings-searcheditor","BodyRuntimeEditor":"vr-genericdata-datarecordnotificationtypesettings-bodyeditor","DataRecordTypeId":"c0820b92-0762-42cd-b0af-42ac37659e4e","GridColumnDefinitions":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"Company","Header":"Company"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"Branch","Header":"Branch"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"User","Header":"User"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"ServiceType","Header":"Service Type"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"Zone","Header":"Zone"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"InterconnectOperator","Header":"InterconnectOperator"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"TrafficDirection","Header":"Traffic Direction"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"TotalSaleAmount","Header":"Sale Amount"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"TotalNumberOfCDRs","Header":"Number Of CDRs"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"TotalSaleDurationInMinutes","Header":"Sale Duration (min)"}]}},"Security":{"$type":"Vanrise.Notification.Entities.VRNotificationTypeSecurity, Vanrise.Notification.Entities","ViewRequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"219c0512-7a4d-41c9-a93b-97b2df5fc673","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View Alerts"]}}]}},"HideRequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}}}}')
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
('4766C266-9C29-4C76-8B55-E49B97459C6A','Billing Analysis','{"$type":"Vanrise.Entities.VRObjectTypeDefinitionSettings, Vanrise.Entities","ObjectType":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordObjectType, Vanrise.GenericData.MainExtensions","ConfigId":"bbc57155-0412-4371-83e5-1917a8bea468","RecordTypeId":"c0820b92-0762-42cd-b0af-42ac37659e4e"},"Properties":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities]], mscorlib","Company":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Company","PropertyEvaluator":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordFieldPropertyEvaluator, Vanrise.GenericData.MainExtensions","ConfigId":"f663bf74-99db-4746-8cbc-e74198e1786c","FieldName":"Company","UseDescription":true}},"Branch":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Branch","PropertyEvaluator":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordFieldPropertyEvaluator, Vanrise.GenericData.MainExtensions","ConfigId":"f663bf74-99db-4746-8cbc-e74198e1786c","FieldName":"Branch","UseDescription":true}},"User":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"User","PropertyEvaluator":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordFieldPropertyEvaluator, Vanrise.GenericData.MainExtensions","ConfigId":"f663bf74-99db-4746-8cbc-e74198e1786c","FieldName":"User","UseDescription":true}},"ServiceType":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"ServiceType","PropertyEvaluator":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordFieldPropertyEvaluator, Vanrise.GenericData.MainExtensions","ConfigId":"f663bf74-99db-4746-8cbc-e74198e1786c","FieldName":"ServiceType","UseDescription":true}},"Zone":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Zone","PropertyEvaluator":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordFieldPropertyEvaluator, Vanrise.GenericData.MainExtensions","ConfigId":"f663bf74-99db-4746-8cbc-e74198e1786c","FieldName":"Zone","UseDescription":true}},"InterConnectOperator":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"InterConnectOperator","PropertyEvaluator":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordFieldPropertyEvaluator, Vanrise.GenericData.MainExtensions","ConfigId":"f663bf74-99db-4746-8cbc-e74198e1786c","FieldName":"InterconnectOperator","UseDescription":true}},"TrafficDirection":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"TrafficDirection","PropertyEvaluator":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordFieldPropertyEvaluator, Vanrise.GenericData.MainExtensions","ConfigId":"f663bf74-99db-4746-8cbc-e74198e1786c","FieldName":"TrafficDirection","UseDescription":false}},"SaleAmount":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"SaleAmount","PropertyEvaluator":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordFieldPropertyEvaluator, Vanrise.GenericData.MainExtensions","ConfigId":"f663bf74-99db-4746-8cbc-e74198e1786c","FieldName":"TotalSaleAmount","UseDescription":false}},"Sale Duration":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Sale Duration","PropertyEvaluator":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordFieldPropertyEvaluator, Vanrise.GenericData.MainExtensions","ConfigId":"f663bf74-99db-4746-8cbc-e74198e1786c","FieldName":"TotalSaleDurationInMinutes","UseDescription":true}},"NumberOfCDRs":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"NumberOfCDRs","PropertyEvaluator":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordFieldPropertyEvaluator, Vanrise.GenericData.MainExtensions","ConfigId":"f663bf74-99db-4746-8cbc-e74198e1786c","FieldName":"TotalNumberOfCDRs","UseDescription":true}}}}')
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
('5E951568-0CBE-4315-B221-611E4FE72506','Billing Analysis','{"$type":"Vanrise.Entities.VRMailMessageTypeSettings, Vanrise.Entities","Objects":{"$type":"Vanrise.Entities.VRObjectVariableCollection, Vanrise.Entities","Billing CDR Analysis":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"Billing CDR Analysis","VRObjectTypeDefinitionId":"4766c266-9c29-4c76-8b55-e49b97459c6a"},"Action Rule":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"Action Rule","VRObjectTypeDefinitionId":"f79ae668-b844-4121-9046-453e7c2bf041"}}}')
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
('534D0502-2C4A-4355-B1F9-E8F557339D23','Billing Analysis','{"$type":"Vanrise.Analytic.Entities.DAProfCalcAlertRuleTypeSettings, Vanrise.Analytic.Entities","CriteriaEditor":"vr-analytic-daprofcalc-alertrulecriteria","ConfigId":"57033e80-65cb-4359-95f6-22a57084d027","SettingEditor":"vr-analytic-daprofcalc-alertrulesettings","DataAnalysisDefinitionId":"a8520a8f-b914-44ef-96e4-4e1aff5358e0","RawRecordFilterLabel":"Source CDR Filter","SourceRecordStorages":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.DAProfCalcAlertRuleRecordStorage, Vanrise.Analytic.Entities]], mscorlib","$values":[{"$type":"Vanrise.Analytic.Entities.DAProfCalcAlertRuleRecordStorage, Vanrise.Analytic.Entities","DataRecordStorageId":"5cd31703-3bc6-41eb-b204-ef473cb394e4"}]},"DAProfCalcItemNotifications":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.DAProfCalcItemNotification, Vanrise.Analytic.Entities]], mscorlib","$values":[{"$type":"Vanrise.Analytic.Entities.DAProfCalcItemNotification, Vanrise.Analytic.Entities","DataAnalysisItemDefinitionId":"e2cc1450-ced9-4640-b875-ffcb94c3f84a","NotificationTypeId":"381cbece-98e1-4801-9a8e-8e45eb9c77c3"}]},"DAProfCalcSecurity":{"$type":"Vanrise.Analytic.Entities.DAProfCalcAlertRuleTypeSecurity, Vanrise.Analytic.Entities","ViewPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"219c0512-7a4d-41c9-a93b-97b2df5fc673","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View Process Logs"]}}]}},"StartInstancePermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"219c0512-7a4d-41c9-a93b-97b2df5fc673","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Start Process"]}}]}}},"Security":{"$type":"Vanrise.Notification.Entities.AlertRuleTypeSecurity, Vanrise.Notification.Entities","ViewPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"219c0512-7a4d-41c9-a93b-97b2df5fc673","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View Rules"]}}]}},"AddPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"219c0512-7a4d-41c9-a93b-97b2df5fc673","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Add Rules"]}}]}},"EditPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"219c0512-7a4d-41c9-a93b-97b2df5fc673","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Edit Rules"]}}]}}}}')
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


--[sec].[View]--------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank],[IsDeleted])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('F480E751-BC0D-4750-B7FF-56854F69C205','Billing Alerts','Billing Alerts',null,'6471DA6F-E4DD-4B2A-BFB6-F8EA498CD37C',null,null,null,'{"$type":"Vanrise.Notification.Entities.VRNotificationViewSettings, Vanrise.Notification.Entities","Settings":{"$type":"System.Collections.Generic.List`1[[Vanrise.Notification.Entities.VRNotificationViewSettingItem, Vanrise.Notification.Entities]], mscorlib","$values":[{"$type":"Vanrise.Notification.Entities.VRNotificationViewSettingItem, Vanrise.Notification.Entities","VRNotificationTypeId":"381cbece-98e1-4801-9a8e-8e45eb9c77c3"}]}}','A196C40A-30B5-4297-B7B0-4344C41CE5A2',5,0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank],[IsDeleted]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank]
when not matched by target then
	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank],[IsDeleted])
	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank],s.[IsDeleted]);