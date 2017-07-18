--[genericdata].[DataTransformationDefinition]------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Title],[Details])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('5289D91B-DDE7-44CD-859A-E45353343206','AssignCostToCDR','Assign Cost To CDR','{"$type":"Vanrise.GenericData.Transformation.Entities.DataTransformationDefinition, Vanrise.GenericData.Transformation.Entities","DataTransformationDefinitionId":"5289d91b-dde7-44cd-859a-e45353343206","Name":"AssignCostToCDR","Title":"Assign Cost To CDR","RecordTypes":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Transformation.Entities.DataTransformationRecordType, Vanrise.GenericData.Transformation.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Transformation.Entities.DataTransformationRecordType, Vanrise.GenericData.Transformation.Entities","RecordName":"BillingCDRs","DataRecordTypeId":"d818c097-822d-4ea5-ae22-f7d208154a78","IsArray":true}]},"MappingSteps":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Transformation.Entities.MappingStep, Vanrise.GenericData.Transformation.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.ExecuteExpressionStep, Vanrise.GenericData.Transformation.MainExtensions","ConfigId":"ce33aab3-2c0a-4f8c-8fbd-ff6b91677c8f","Expression":"new Retail.Cost.Business.CDRCostManager().AssignCostToCDR(BillingCDRs, \"AttemptDateTime\", \"Calling\", \"Called\",\"DurationInSeconds\",\n new Retail.Cost.Business.CDRCostFieldNames(){\nCostAmount=\"CostAmount\",\nCostRate =\"CostRate\",\nSupplierName =\"SupplierName\",\nCostCurrency =\"CostCurrency\",\nCDRCostId =\"CDRCostId\"\n});"}]}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Details]))
merge	[genericdata].[DataTransformationDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Details] = s.[Details]
when not matched by target then
	insert([ID],[Name],[Title],[Details])
	values(s.[ID],s.[Name],s.[Title],s.[Details]);
----------------------------------------------------------------------------------------------------
END


--[genericdata].[DataRecordType]--------------------------------------------------------------------
Delete From [genericdata].[DataRecordType] Where [ID] in ('548613D7-17EE-41FD-A3C3-0D82357EE5E7','A2913B86-CA10-422F-A8BE-30D9CC2AD115','7F3EEA5C-6480-4FDE-AE26-448EA83035DE','4F694A53-D78A-4BAA-8C60-47DC8B01D607','976B6B04-BD15-4200-BB18-4859D8F2F33B','92010E40-1EF1-41C0-9DBF-533883F7D5F9','C4FAD850-CADE-4AA3-88FF-4FC028833904','7FF7337E-FC95-4E8D-BEBD-564BE6DF0395','D221198E-3CC7-4102-BC63-56944DCDAD35','8EE3A64D-6CC7-4C2A-9F41-5AC71E1272ED','4A7F1894-6B33-466B-80D1-74CE46631ABC','C8074CA2-B6F8-4865-B356-74DBAB201220','D0D7E57B-798E-4946-80AF-774BC65923C4','0AB64C3F-A35F-4E8B-9B0F-8CB0440C3554','B7688C91-8B68-4070-8505-9C5DC4B6C656','4ACBF7A3-E000-47F0-AAFC-AD7A9C3EF421','387BCE79-42C8-4EF2-9830-D6E97FA0FCED')
BEGIN
set nocount on;
;with cte_data([ID],[Name],[ParentID],[Fields],[ExtraFieldsEvaluator],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('5C624ED4-4F6A-49E6-A489-488DF161F7BB','CDRCost'			,null,'{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"ID","Title":"ID","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","DataPrecision":0,"DataType":2,"IsNullable":false,"OrderType":0}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"SourceID","Title":"Source ID","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"AttemptDateTime","Title":"Attempt Date Time","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldDateTimeType, Vanrise.GenericData.MainExtensions","ConfigId":"b8712417-83ab-4d4b-9ee1-109d20ceb909","DataType":0,"IsNullable":true,"OrderType":0}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"CGPN","Title":"CGPN","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"CDPN","Title":"CDPN","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"DurationInSeconds","Title":"Duration in Seconds","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","DataPrecision":0,"DataType":0,"IsNullable":true,"OrderType":0}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"Rate","Title":"Rate","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","DataPrecision":1,"DataType":0,"IsNullable":true,"OrderType":0}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"Amount","Title":"Amount","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","DataPrecision":0,"DataType":0,"IsNullable":true,"OrderType":0}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"SupplierName","Title":"Supplier Name","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"Currency","Title":"Currency","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","BusinessEntityDefinitionId":"d41ea344-c3c0-4203-8583-019b6b3edb76","IsNullable":true,"OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"IsReRate","Title":"ReRate","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBooleanType, Vanrise.GenericData.MainExtensions","ConfigId":"a77fad19-d044-40d8-9d04-6362b79b177b","IsNullable":true,"OrderType":0}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"IsDeleted","Title":"Deleted","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBooleanType, Vanrise.GenericData.MainExtensions","ConfigId":"a77fad19-d044-40d8-9d04-6362b79b177b","IsNullable":true,"OrderType":0}}]}',null,'{"$type":"Vanrise.GenericData.Entities.DataRecordTypeSettings, Vanrise.GenericData.Entities","DateTimeField":"AttemptDateTime"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[ParentID],[Fields],[ExtraFieldsEvaluator],[Settings]))merge	[genericdata].[datarecordtype] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[ParentID] = s.[ParentID],[Fields] = s.[Fields],[ExtraFieldsEvaluator] = s.[ExtraFieldsEvaluator],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[ParentID],[Fields],[ExtraFieldsEvaluator],[Settings])	values(s.[ID],s.[Name],s.[ParentID],s.[Fields],s.[ExtraFieldsEvaluator],s.[Settings]);
----------------------------------------------------------------------------------------------------
END

--[genericdata].[DataRecordStorage]-----------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[DataRecordTypeID],[DataStoreID],[Settings],[State])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('32641539-8549-484F-8D84-E55BF49D8B67','CDR Cost Table'			,'5C624ED4-4F6A-49E6-A489-488DF161F7BB','B95CF5D4-7679-42D0-BF8F-1859BA42B503','{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageSettings, Vanrise.GenericData.SQLDataStorage","TableName":"CDRCost","TableSchema":"Retail_CDR","Columns":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage]], mscorlib","$values":[{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"ID","SQLDataType":"bigint","ValueExpression":"ID"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"SourceID","SQLDataType":"varchar(255)","ValueExpression":"SourceID"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"AttemptDateTime","SQLDataType":"datetime","ValueExpression":"AttemptDateTime"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"CGPN","SQLDataType":"varchar(100)","ValueExpression":"CGPN"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"CDPN","SQLDataType":"varchar(100)","ValueExpression":"CDPN"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"SupplierName","SQLDataType":"nvarchar(255)","ValueExpression":"SupplierName"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"DurationInSeconds","SQLDataType":"decimal(20, 4)","ValueExpression":"DurationInSeconds"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"CurrencyId","SQLDataType":"int","ValueExpression":"Currency"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"Rate","SQLDataType":"decimal(20, 8)","ValueExpression":"Rate"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"Amount","SQLDataType":"decimal(22, 6)","ValueExpression":"Amount"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"IsReRate","SQLDataType":"bit","ValueExpression":"IsReRate"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"IsDeleted","SQLDataType":"bit","ValueExpression":"IsDeleted"}]},"NullableFields":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.SQLDataStorage.NullableField, Vanrise.GenericData.SQLDataStorage]], mscorlib","$values":[]},"IncludeQueueItemId":false,"DateTimeField":"AttemptDateTime","RequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}}}',null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[DataRecordTypeID],[DataStoreID],[Settings],[State]))
merge	[genericdata].[DataRecordStorage] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[DataRecordTypeID] = s.[DataRecordTypeID],[DataStoreID] = s.[DataStoreID],[Settings] = s.[Settings],[State] = s.[State]
when not matched by target then
	insert([ID],[Name],[DataRecordTypeID],[DataStoreID],[Settings],[State])
	values(s.[ID],s.[Name],s.[DataRecordTypeID],s.[DataStoreID],s.[Settings],s.[State]);
----------------------------------------------------------------------------------------------------
END

--[reprocess].[ReprocessDefinition]-----------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([Id],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('12BEC621-B3CD-450C-9BA3-A71A61647D0E','Cost CDR Reprocessing','{"$type":"Vanrise.Reprocess.Entities.ReprocessDefinitionSettings, Vanrise.Reprocess.Entities","SourceRecordStorageIds":{"$type":"System.Collections.Generic.List`1[[System.Guid, mscorlib]], mscorlib","$values":["5cd31703-3bc6-41eb-b204-ef473cb394e4"]},"ExecutionFlowDefinitionId":"9af6085e-a28e-4faf-a8b9-47989214c929","StageNames":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Main CDR Storage Stage","Apply Cost","Billing Stats Daily Generation Stage"]},"InitiationStageNames":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Apply Cost"]},"StagesToHoldNames":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Apply Cost"]},"StagesToProcessNames":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Main CDR Storage Stage","Billing Stats Daily Update Stage","Billing Stats Daily Generation Stage"]},"RecordCountPerTransaction":50000}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Settings]))
merge	[reprocess].[ReprocessDefinition] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([Id],[Name],[Settings])
	values(s.[Id],s.[Name],s.[Settings]);
----------------------------------------------------------------------------------------------------
END


--[common].[Setting]--------------------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('4DDFC722-3042-4240-895E-C2F5F3017CD3','Cost','Retail_Cost_CDRCostSettings','General','{"Editor" : "retail-cost-cdrcostsettings-editor"}','{"$type":"Retail.Cost.Entities.CDRCostSettingData, Retail.Cost.Entities","DurationMargin":"00:00:05","AttemptDateTimeMargin":"00:00:05","AttemptDateTimeOffset":"00:00:00","MaxBatchDurationInMinutes":10}',0)
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
----------------------------------------------------------------------------------------------------
END


--[bp].[BPDefinition]-------------------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Title],[FQTN],[Config])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('595CFF67-49C0-4D10-8A11-4121DE586C74','Retail.Cost.BP.Arguments.CostEvaluatorProcessInput','Cost Evaluator','Retail.Cost.BP.CostEvaluatorProcess, Retail.Cost.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"ManualExecEditor":"retail-cost-cdrcostevaluator-process","ScheduledExecEditor":"retail-cost-cdrcostevaluator-task","IsPersistable":false,"HasChildProcesses":true,"HasBusinessRules":false,"NotVisibleInManagementScreen":false,"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"CCD65B5B-53BB-4816-B7EA-D8DC58AA513E","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View Reprocess Logs"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"CCD65B5B-53BB-4816-B7EA-D8DC58AA513E","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Start Reprocess"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"CCD65B5B-53BB-4816-B7EA-D8DC58AA513E","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Start Reprocess"]}}]}}}}')
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

--[runtime].[ScheduleTask]------------------------------------------------------------------------------
BEGIN

set nocount on;
;with cte_data([ID],[Name],[IsEnabled],[TaskType],[TriggerTypeId],[ActionTypeId],[TaskSettings],[OwnerId])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('1ED9CCD7-1DE4-495C-A1F5-29B5054EE334','Data Source Task',0,0,'295B4FAC-DBF9-456F-855E-60D0B176F86B','B7CF41B9-F1B3-4C02-980D-B9FAFB4CFF68','{"$type":"Vanrise.Runtime.Entities.SchedulerTaskSettings, Vanrise.Runtime.Entities","TaskTriggerArgument":{"$type":"Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments.IntervalTimeTaskTriggerArgument, Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments","Interval":1.0,"IntervalType":0,"TimerTriggerTypeFQTN":"Vanrise.Runtime.Triggers.TimeTaskTrigger.IntervalTimeSchedulerTaskTrigger, Vanrise.Runtime.Triggers.TimeTaskTrigger","IgnoreSkippedIntervals":false},"StartEffDate":"2017-07-17T14:41:33.309"}',1)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[IsEnabled],[TaskType],[TriggerTypeId],[ActionTypeId],[TaskSettings],[OwnerId]))
merge	[runtime].[ScheduleTask] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
--when matched then
--	update set
--	[Name] = s.[Name],[IsEnabled] = s.[IsEnabled],[TaskType] = s.[TaskType],[TriggerTypeId] = s.[TriggerTypeId],[ActionTypeId] = s.[ActionTypeId],[TaskSettings] = s.[TaskSettings],[OwnerId] = s.[OwnerId]
when not matched by target then
	insert([ID],[Name],[IsEnabled],[TaskType],[TriggerTypeId],[ActionTypeId],[TaskSettings],[OwnerId])
	values(s.[ID],s.[Name],s.[IsEnabled],s.[TaskType],s.[TriggerTypeId],s.[ActionTypeId],s.[TaskSettings],s.[OwnerId]);
----------------------------------------------------------------------------------------------------
END

--[integration].[DataSource]------------------------------------------------------------------------
BEGIN

set nocount on;

;with cte_data([ID],[Name],[AdapterID],[AdapterState],[TaskId],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('906B69DE-C9A5-42AD-9CDC-2FD16AEF1777','Cost CDR Import Multinet FTP','396A4933-DF4F-49CD-9799-BF605B9F4597',null,'1ED9CCD7-1DE4-495C-A1F5-29B5054EE334','{"$type":"Vanrise.Integration.Entities.DataSourceSettings, Vanrise.Integration.Entities","AdapterArgument":{"$type":"Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument, Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments","Extension":".csv","Mask":"","Directory":"/BCPFiles","ServerIP":"192.168.110.185","UserName":"devftpuser","Password":"P@ssw0rd","ActionAfterImport":0,"BasedOnLastModifiedTime":true,"MaxParallelRuntimeInstances":1},"MapperCustomCode":"LogVerbose(\"Started\");\n            Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));\n            var cdrs = new List<dynamic>();\n\n            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();\n            Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType(\"CDRCost\");\n            var dataRecordVanriseType = new Vanrise.GenericData.Entities.DataRecordVanriseType(\"CDRCost\");\n\n            var currentItemCount = 21;\n            System.IO.StreamReader sr = ImportedData.StreamReader;\n            Vanrise.Common.Business.CurrencyManager currencyManager = new Vanrise.Common.Business.CurrencyManager();\n\n            while (!sr.EndOfStream)\n            {\n                string currentLine = sr.ReadLine();\n                if (string.IsNullOrEmpty(currentLine))\n                    continue;\n\n                string[] rowData = currentLine.Split('','');\n                if (rowData.Length != currentItemCount)\n                    continue;\n\n                string customerName = rowData[18];\n                string durationAsString;\n                string amountAsString;\n\n                if (string.Compare(customerName, \"FLL-Incoming (Dom)\", true) == 0)\n                {\n                    durationAsString = rowData[17];\n                    amountAsString = rowData[14];\n                }\n                else if (string.Compare(customerName, \"FLL-Incoming (IDD)\", true) == 0)\n                {\n                    durationAsString = rowData[16];\n                    amountAsString = rowData[13];\n                }\n                else\n                    continue;\n\n\n                dynamic cdr = Activator.CreateInstance(cdrRuntimeType) as dynamic;\n                cdr.SourceID = rowData[0];\n                cdr.AttemptDateTime = DateTime.ParseExact(rowData[1], \"yyyy-MM-dd HH:mm:ss\", System.Globalization.CultureInfo.InvariantCulture);\n                cdr.CGPN = rowData[4];\n                cdr.CDPN = rowData[5];\n                cdr.DurationInSeconds = !string.IsNullOrEmpty(durationAsString) ? decimal.Parse(durationAsString) : default(decimal?);\n                cdr.Amount = !string.IsNullOrEmpty(amountAsString) ? decimal.Parse(amountAsString) : default(decimal?);\n\n                string rateAsString = rowData[12];\n                cdr.Rate = !string.IsNullOrEmpty(rateAsString) ? decimal.Parse(rateAsString) : default(decimal?);\n\n                cdr.SupplierName = rowData[19];\n                string currencySymbol = rowData[10];\n                if (!string.IsNullOrEmpty(currencySymbol))\n                {\n                    var currency = currencyManager.GetCurrencyBySymbol(currencySymbol);\n                    if (currency != null)\n                        cdr.Currency = currency.CurrencyId;\n                }\n                string statusAsString = rowData[20];\n                if (!string.IsNullOrEmpty(statusAsString) && string.Compare(statusAsString, \"R\", true) == 0)\n                    cdr.IsReRate = true;\n\n                cdrs.Add(cdr);\n            }\n\n            if (cdrs.Count > 0)\n            {\n                long startingId;\n                Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(dataRecordVanriseType, cdrs.Count, out startingId);\n                long currentCDRId = startingId;\n\n                foreach (var cdr in cdrs)\n                {\n                    cdr.ID = currentCDRId;\n                    currentCDRId++;\n                }\n                var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, \"#RECORDSCOUNT# of Raw CDRs\", \"CDRCost\");\n                mappedBatches.Add(\"CDR Cost Storage Stage\", batch);\n            }\n\n            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();\n            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;\n            LogVerbose(\"Finished\");\n\n            return result;","ExecutionFlowId":"8f5414aa-06d1-4bad-b6f2-6bd822b5ed9e"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[AdapterID],[AdapterState],[TaskId],[Settings]))
merge	[integration].[DataSource] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
--when matched then
--	update set
--	[Name] = s.[Name],[AdapterID] = s.[AdapterID],[AdapterState] = s.[AdapterState],[TaskId] = s.[TaskId],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[AdapterID],[AdapterState],[TaskId],[Settings])
	values(s.[ID],s.[Name],s.[AdapterID],s.[AdapterState],s.[TaskId],s.[Settings]);
----------------------------------------------------------------------------------------------------
END

--[queue].[ExecutionFlowDefinition]-----------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([Id],[Name],[Title],[ExecutionTree],[Stages])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('DCDE6A57-5637-4599-AE1D-52DDBE0F4D98','ImportCDRCost','Import CDR Cost',null,'{"$type":"System.Collections.Generic.List`1[[Vanrise.Queueing.Entities.QueueExecutionFlowStage, Vanrise.Queueing.Entities]], mscorlib","$values":[{"$type":"Vanrise.Queueing.Entities.QueueExecutionFlowStage, Vanrise.Queueing.Entities","StageName":"CDR Cost Storage Stage","QueueNameTemplate":"Queue_#FlowId#_#StageName#","QueueTitleTemplate":"#StageName# Queue (#FlowName#)","QueueItemType":{"$type":"Vanrise.GenericData.QueueActivators.DataRecordBatchQueueItemType, Vanrise.GenericData.QueueActivators","DataRecordTypeId":"5c624ed4-4f6a-49e6-a489-488df161f7bb","BatchDescription":"#RECORDSCOUNT# of CDRs"},"QueueActivator":{"$type":"Vanrise.GenericData.QueueActivators.StoreBatchQueueActivator, Vanrise.GenericData.QueueActivators","DataRecordStorageId":"32641539-8549-484f-8d84-e55bf49d8b67","NbOfMaxConcurrentActivators":10,"ConfigId":1},"IsSequential":false}]}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Title],[ExecutionTree],[Stages]))
merge	[queue].[ExecutionFlowDefinition] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[ExecutionTree] = s.[ExecutionTree],[Stages] = s.[Stages]
when not matched by target then
	insert([Id],[Name],[Title],[ExecutionTree],[Stages])
	values(s.[Id],s.[Name],s.[Title],s.[ExecutionTree],s.[Stages]);
----------------------------------------------------------------------------------------------------
END


--[queue].[ExecutionFlow]---------------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([Id],[Name],[ExecutionFlowDefinitionID])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('8F5414AA-06D1-4BAD-B6F2-6BD822B5ED9E','Import CDR Cost','DCDE6A57-5637-4599-AE1D-52DDBE0F4D98')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[ExecutionFlowDefinitionID]))
merge	[queue].[ExecutionFlow] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[ExecutionFlowDefinitionID] = s.[ExecutionFlowDefinitionID]
when not matched by target then
	insert([Id],[Name],[ExecutionFlowDefinitionID])
	values(s.[Id],s.[Name],s.[ExecutionFlowDefinitionID]);
----------------------------------------------------------------------------------------------------
END