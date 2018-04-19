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
delete from [common].[extensionconfiguration] where [ConfigType] = 'WhS_RouteSync_SwitchRouteSynchronizer'
--[common].[extensionconfiguration]---------------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('1EE51230-FE31-4D01-9289-0E27E24D3601','WhS_RouteSync_SwitchRouteSynchronizer_BuiltInIVSwitch','CloudXPoint Switch Synchronizer','WhS_RouteSync_SwitchRouteSynchronizer','{"Editor":"whs-routesync-builtinivswitch-swsync"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[ConfigType],[Settings]))
merge	[common].[extensionconfiguration] as t
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

--[Analytic].[AnalyticTable]------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------

BEGIN
set nocount on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('913411d7-2782-4ac9-a55c-27cd88a55ebf','IVSwitchLive','{"$type":"Vanrise.Analytic.Entities.AnalyticTableSettings, Vanrise.Analytic.Entities","ConnectionStringName":"IVSwitchCDRDBConnString","TableName":"cdrs_buffer","TimeColumnName":"det_date","DataRecordTypeIds":{"$type":"System.Collections.Generic.List`1[[System.Guid, mscorlib]], mscorlib","$values":["6cf5f7ad-5123-45d2-b47f-eca613d454f7"]},"RequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"a611a651-b60b-483d-bc83-1c2b667a120a","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"DataProvider":{"$type":"Vanrise.Analytic.Data.Postgres.PostgresAnalyticDataProvider, Vanrise.Analytic.Data.Postgres","ConfigId":"10631f32-9116-4443-a73d-2d4b77111634"}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings]))
merge	[Analytic].[AnalyticTable] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Settings])
	values(s.[ID],s.[Name],s.[Settings]);
END

--[Analytic].[AnalyticItemConfig]----------------------------------------------------------
----------------------------------------------------------------------------------------------------
BEGIN

set nocount on;
;with cte_data([ID],[TableId],[ItemType],[Name],[Title],[Config])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('485F33B2-0A27-4334-91F4-07A0F0737005','913411d7-2782-4ac9-a55c-27cd88a55ebf',4,'CountConnectedMoreThan07','CountConnectedMoreThan07','{"$type":"Vanrise.Analytic.Entities.AnalyticAggregateConfig, Vanrise.Analytic.Entities","SQLColumn":"case when con_date is not null AND extract(epoch from now()-con_date)  between 7 *60 and 19*60 then 1 else 0 end","AggregateType":2}'),
('DC7A041A-CFAC-4E3F-80D1-0D8CD507F3D2','913411d7-2782-4ac9-a55c-27cd88a55ebf',2,'CountConnectedMoreThan20','20''+','{"$type":"Vanrise.Analytic.Entities.AnalyticMeasureConfig, Vanrise.Analytic.Entities","GetValueMethod":"return context.GetAggregateValue(\"CountConnectedMoreThan20\");","DependentAggregateNames":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["CountConnectedMoreThan20"]},"FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","DataPrecision":0,"DataType":2,"IsNullable":false,"OrderType":0},"RequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}}}'),
('6E9FB41C-015B-4488-9D1D-1BA0768AE7BC','913411d7-2782-4ac9-a55c-27cd88a55ebf',4,'CountConnectedMoreThan20','CountConnectedMoreThan20','{"$type":"Vanrise.Analytic.Entities.AnalyticAggregateConfig, Vanrise.Analytic.Entities","SQLColumn":"case when con_date is not null AND extract(epoch from now()-con_date)  between 20*60 and 59*60 then 1 else 0 end","AggregateType":2}'),
('516C5FA8-5B97-4EE8-8A89-27531280A6F6','913411d7-2782-4ac9-a55c-27cd88a55ebf',4,'CountConnectedMoreThan60','CountConnectedMoreThan60','{"$type":"Vanrise.Analytic.Entities.AnalyticAggregateConfig, Vanrise.Analytic.Entities","SQLColumn":"case when con_date is not null AND extract(epoch from now()-con_date)  between 60*60 and 89*60 then 1 else 0 end","AggregateType":2}'),
('FCA2D58F-1D67-412F-849E-288D3742D04A','913411d7-2782-4ac9-a55c-27cd88a55ebf',1,'Supplier','Supplier','{"$type":"Vanrise.Analytic.Entities.AnalyticDimensionConfig, Vanrise.Analytic.Entities","GetValueMethod":"var routeId= context.GetDimensionValue(\"RouteId\");\nif(routeId != null)\n {\n     // return (new NP.IVSwitch.Business.AccountManager()).GetRouteSwitchAccountName(routeId);\n    return (new NP.IVSwitch.Business.RouteManager()).GetRouteCarrierAccountId(routeId);\n}\nelse\n return null;","DependentDimensions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["RouteId"]},"FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","BusinessEntityDefinitionId":"8c286bcd-5766-487a-8b32-5d167ec342c0","IsNullable":false,"OrderType":1},"DimensionFieldMappings":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.DimensionFieldMapping, Vanrise.Analytic.Entities]], mscorlib","$values":[{"$type":"Vanrise.Analytic.Entities.DimensionFieldMapping, Vanrise.Analytic.Entities","DataRecordTypeId":"6cf5f7ad-5123-45d2-b47f-eca613d454f7"}]}}'),
('C9BCEA23-6865-4951-8A08-359E8AC6B349','913411d7-2782-4ac9-a55c-27cd88a55ebf',2,'CountConnectedMoreThan60','60''+','{"$type":"Vanrise.Analytic.Entities.AnalyticMeasureConfig, Vanrise.Analytic.Entities","GetValueMethod":"return context.GetAggregateValue(\"CountConnectedMoreThan60\");","DependentAggregateNames":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["CountConnectedMoreThan60"]},"FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","DataPrecision":0,"DataType":2,"IsNullable":false,"OrderType":0},"RequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}}}'),
('31DED02E-62B8-4FAA-A7AF-3D53ABFA3179','913411d7-2782-4ac9-a55c-27cd88a55ebf',2,'PDDInSec','PDD (s)','{"$type":"Vanrise.Analytic.Entities.AnalyticMeasureConfig, Vanrise.Analytic.Entities","GetValueMethod":"var sumPDDInSec = context.GetAggregateValue(\"SumPDDInSec\");\nvar countAlerted = context.GetAggregateValue(\"CountAlerted\");\nreturn countAlerted > 0 ? (Decimal?)Math.Round(sumPDDInSec / countAlerted, 2) : default(Decimal?);","DependentAggregateNames":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["SumPDDInSec","CountAlerted"]},"FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","DataPrecision":0,"DataType":0,"IsNullable":false,"OrderType":0},"RequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}}}'),
('C0844F23-5488-4C91-BBC5-43D6B0FAEFBC','913411d7-2782-4ac9-a55c-27cd88a55ebf',4,'CountAlerted','CountAlerted','{"$type":"Vanrise.Analytic.Entities.AnalyticAggregateConfig, Vanrise.Analytic.Entities","SQLColumn":"case when prg_date is not null then 1 else 0 end","AggregateType":2}'),
('C6DAF88C-CCFE-4E76-ACC3-54DF232FAEBA','913411d7-2782-4ac9-a55c-27cd88a55ebf',2,'CountConnectedMoreThan03','3''+','{"$type":"Vanrise.Analytic.Entities.AnalyticMeasureConfig, Vanrise.Analytic.Entities","GetValueMethod":"return context.GetAggregateValue(\"CountConnectedMoreThan03\");","DependentAggregateNames":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["CountConnectedMoreThan03"]},"FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","DataPrecision":0,"DataType":2,"IsNullable":false,"OrderType":0},"RequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}}}'),
('90651116-6AFF-40EA-9703-61F6608546F0','913411d7-2782-4ac9-a55c-27cd88a55ebf',1,'Code','Code','{"$type":"Vanrise.Analytic.Entities.AnalyticDimensionConfig, Vanrise.Analytic.Entities","SQLExpression":"dest_code","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1},"DimensionFieldMappings":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.DimensionFieldMapping, Vanrise.Analytic.Entities]], mscorlib","$values":[{"$type":"Vanrise.Analytic.Entities.DimensionFieldMapping, Vanrise.Analytic.Entities","DataRecordTypeId":"6cf5f7ad-5123-45d2-b47f-eca613d454f7"}]}}'),
('F8E36F39-45E2-4A59-8D7F-63C30E923B80','913411d7-2782-4ac9-a55c-27cd88a55ebf',2,'ACD','ACD','{"$type":"Vanrise.Analytic.Entities.AnalyticMeasureConfig, Vanrise.Analytic.Entities","GetValueMethod":"var successfulAttempts = context.GetAggregateValue(\"CountConnected\");\nreturn (decimal)(successfulAttempts != 0 ? (context.GetAggregateValue(\"SumDurationInSec\") /60) / successfulAttempts : 0);","DependentAggregateNames":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["SumDurationInSec","CountConnected"]},"FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","DataPrecision":0,"DataType":0,"IsNullable":false,"OrderType":0},"RequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}}}'),
('7130536C-11C3-447F-A3F0-6487BA60E789','913411d7-2782-4ac9-a55c-27cd88a55ebf',4,'CountConnectedMoreThan03','CountConnectedMoreThan03','{"$type":"Vanrise.Analytic.Entities.AnalyticAggregateConfig, Vanrise.Analytic.Entities","SQLColumn":"case when con_date is not null AND extract(epoch from now()-con_date)  between 3 *60 and 6 *60 then 1 else 0 end","AggregateType":2}'),
('F195B744-D059-4163-BD22-6AC8DC1FD198','913411d7-2782-4ac9-a55c-27cd88a55ebf',2,'Attempts','Attempts','{"$type":"Vanrise.Analytic.Entities.AnalyticMeasureConfig, Vanrise.Analytic.Entities","GetValueMethod":"return context.GetAggregateValue(\"TotalCount\");","DependentAggregateNames":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["TotalCount"]},"FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","DataPrecision":0,"DataType":2,"IsNullable":false,"OrderType":0},"RequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}}}'),
('B1D04E22-83EB-479E-9BA1-7D4A505CC71E','913411d7-2782-4ac9-a55c-27cd88a55ebf',2,'TotalDuration','Total Duration','{"$type":"Vanrise.Analytic.Entities.AnalyticMeasureConfig, Vanrise.Analytic.Entities","GetValueMethod":"var durInSec = context.GetAggregateValue(\"SumDurationInSec\");\nreturn (long)durInSec / 60;\n/*if(durInSec != null)\n{\nTimeSpan durInTimeSpan = TimeSpan.FromSeconds((int)durInSec);\nreturn durInTimeSpan.ToString();\n}\nelse\nreturn null;*/","DependentAggregateNames":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["SumDurationInSec"]},"FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","DataPrecision":0,"DataType":2,"IsNullable":false,"OrderType":0},"RequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}}}'),
('6FFCAC24-6957-4E3D-8DDC-7E803218999F','913411d7-2782-4ac9-a55c-27cd88a55ebf',2,'CountConnected','Connected','{"$type":"Vanrise.Analytic.Entities.AnalyticMeasureConfig, Vanrise.Analytic.Entities","GetValueMethod":"return context.GetAggregateValue(\"CountConnected\");","DependentAggregateNames":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["CountConnected"]},"FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","DataPrecision":0,"DataType":2,"IsNullable":false,"OrderType":0},"RequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}}}'),
('B20F2E22-834B-475D-B60F-8526AE9B8D94','913411d7-2782-4ac9-a55c-27cd88a55ebf',1,'RouteId','RouteId','{"$type":"Vanrise.Analytic.Entities.AnalyticDimensionConfig, Vanrise.Analytic.Entities","SQLExpression":"route_id","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","DataPrecision":0,"DataType":1,"IsNullable":false,"OrderType":0},"DimensionFieldMappings":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.DimensionFieldMapping, Vanrise.Analytic.Entities]], mscorlib","$values":[{"$type":"Vanrise.Analytic.Entities.DimensionFieldMapping, Vanrise.Analytic.Entities","DataRecordTypeId":"6cf5f7ad-5123-45d2-b47f-eca613d454f7"}]}}'),
('A0220306-49E8-4ADD-AEDD-8E8730338272','913411d7-2782-4ac9-a55c-27cd88a55ebf',1,'DurationRange','Duration Range','{"$type":"Vanrise.Analytic.Entities.AnalyticDimensionConfig, Vanrise.Analytic.Entities","SQLExpression":"case when con_date is null then 0\nwhen extract(epoch from now()-con_date) >= 90*60 then 7\nwhen extract(epoch from now()-con_date) >= 60*60 then 6\nwhen extract(epoch from now()-con_date) >= 20*60 then 5\nwhen extract(epoch from now()-con_date) >= 7 *60 then 4\nwhen extract(epoch from now()-con_date) >= 3 *60 then 3\nelse 2 end","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldChoicesType, Vanrise.GenericData.MainExtensions","ConfigId":"eabc41a9-e332-4120-ac85-f0b7e53c0d0d","Choices":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.MainExtensions.DataRecordFields.Choice, Vanrise.GenericData.MainExtensions]], mscorlib","$values":[{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.Choice, Vanrise.GenericData.MainExtensions","Value":0,"Text":"Not Connected"},{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.Choice, Vanrise.GenericData.MainExtensions","Value":2,"Text":"< 3"},{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.Choice, Vanrise.GenericData.MainExtensions","Value":3,"Text":"3 to 7"},{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.Choice, Vanrise.GenericData.MainExtensions","Value":4,"Text":"7 to 20"},{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.Choice, Vanrise.GenericData.MainExtensions","Value":5,"Text":"20 to 60"},{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.Choice, Vanrise.GenericData.MainExtensions","Value":6,"Text":"60 to 90"},{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.Choice, Vanrise.GenericData.MainExtensions","Value":7,"Text":"> 90"}]},"IsNullable":false,"OrderType":1},"DimensionFieldMappings":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.DimensionFieldMapping, Vanrise.Analytic.Entities]], mscorlib","$values":[{"$type":"Vanrise.Analytic.Entities.DimensionFieldMapping, Vanrise.Analytic.Entities","DataRecordTypeId":"6cf5f7ad-5123-45d2-b47f-eca613d454f7"}]}}'),
('E780519A-325B-4EC5-972A-929CF0CA4C2E','913411d7-2782-4ac9-a55c-27cd88a55ebf',2,'CountConnectedMoreThan07','7''+','{"$type":"Vanrise.Analytic.Entities.AnalyticMeasureConfig, Vanrise.Analytic.Entities","GetValueMethod":"return context.GetAggregateValue(\"CountConnectedMoreThan07\");","DependentAggregateNames":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["CountConnectedMoreThan07"]},"FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","DataPrecision":0,"DataType":2,"IsNullable":false,"OrderType":0},"RequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}}}'),
('831C669F-337E-495B-98CB-97EBB98A2361','913411d7-2782-4ac9-a55c-27cd88a55ebf',2,'CountConnectedMoreThan90','90''+','{"$type":"Vanrise.Analytic.Entities.AnalyticMeasureConfig, Vanrise.Analytic.Entities","GetValueMethod":"return context.GetAggregateValue(\"CountConnectedMoreThan90\");","DependentAggregateNames":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["CountConnectedMoreThan90"]},"FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","DataPrecision":0,"DataType":2,"IsNullable":false,"OrderType":0},"RequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}}}'),
('D91BA478-F942-4E33-AE38-AEC16DF8B577','913411d7-2782-4ac9-a55c-27cd88a55ebf',4,'CountConnectedMoreThan90','CountConnectedMoreThan90','{"$type":"Vanrise.Analytic.Entities.AnalyticAggregateConfig, Vanrise.Analytic.Entities","SQLColumn":"case when con_date is not null AND extract(epoch from now()-con_date)  >= 90*60 then 1 else 0 end","AggregateType":2}'),
('80466020-E895-48B4-BA4E-AF005243DBFA','913411d7-2782-4ac9-a55c-27cd88a55ebf',4,'TotalCount','TotalCount','{"$type":"Vanrise.Analytic.Entities.AnalyticAggregateConfig, Vanrise.Analytic.Entities","SQLColumn":"*","AggregateType":1}'),
('EEEE474A-0E00-47CE-B775-AF4CDA43DDA0','913411d7-2782-4ac9-a55c-27cd88a55ebf',1,'UserId','UserId','{"$type":"Vanrise.Analytic.Entities.AnalyticDimensionConfig, Vanrise.Analytic.Entities","SQLExpression":"user_id","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","DataPrecision":0,"DataType":1,"IsNullable":false,"OrderType":0},"DimensionFieldMappings":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.DimensionFieldMapping, Vanrise.Analytic.Entities]], mscorlib","$values":[{"$type":"Vanrise.Analytic.Entities.DimensionFieldMapping, Vanrise.Analytic.Entities","DataRecordTypeId":"6cf5f7ad-5123-45d2-b47f-eca613d454f7"}]}}'),
('39BE096B-E050-4B99-BF8E-B1133F4B9C67','913411d7-2782-4ac9-a55c-27cd88a55ebf',2,'PercConnected','% Connected','{"$type":"Vanrise.Analytic.Entities.AnalyticMeasureConfig, Vanrise.Analytic.Entities","GetValueMethod":"var attempts = context.GetAggregateValue(\"TotalCount\");\nreturn attempts > 0 ? (int)(context.GetAggregateValue(\"CountConnected\")*100/attempts) : 0;","DependentAggregateNames":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["TotalCount","CountConnected"]},"FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","DataPrecision":0,"DataType":1,"IsNullable":false,"OrderType":0},"RequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}}}'),
('36AD8CEB-0EDF-45AE-9960-C490C8B24F97','913411d7-2782-4ac9-a55c-27cd88a55ebf',4,'SumPDDInSec','SumPDDInSec','{"$type":"Vanrise.Analytic.Entities.AnalyticAggregateConfig, Vanrise.Analytic.Entities","SQLColumn":"case when prg_date is not null then extract(epoch from prg_date - det_date) else 0 end","AggregateType":2}'),
('AC1CFE60-3A8E-4ABC-9F79-D11EE5671816','913411d7-2782-4ac9-a55c-27cd88a55ebf',1,'Zone','Zone','{"$type":"Vanrise.Analytic.Entities.AnalyticDimensionConfig, Vanrise.Analytic.Entities","SQLExpression":"dest_name","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1},"DimensionFieldMappings":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.DimensionFieldMapping, Vanrise.Analytic.Entities]], mscorlib","$values":[{"$type":"Vanrise.Analytic.Entities.DimensionFieldMapping, Vanrise.Analytic.Entities","DataRecordTypeId":"6cf5f7ad-5123-45d2-b47f-eca613d454f7"}]}}'),
('331114FE-59A5-4FA5-8C0B-DEEF30D68167','913411d7-2782-4ac9-a55c-27cd88a55ebf',1,'Country','Country','{"$type":"Vanrise.Analytic.Entities.AnalyticDimensionConfig, Vanrise.Analytic.Entities","GetValueMethod":"var code = context.GetDimensionValue(\"Code\");\nif(code != null)\n {\n     var codeGroup = (new TOne.WhS.BusinessEntity.Business.CodeGroupManager()).GetMatchCodeGroup(code);\n     if(codeGroup != null)\n          return codeGroup.CountryId;\n    else\n          return null;\n}\nelse\n     return null;","DependentDimensions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Code"]},"FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","BusinessEntityDefinitionId":"df5cdc08-ddf1-4d4e-b1f6-d17b3833452f","IsNullable":false,"OrderType":1},"DimensionFieldMappings":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.DimensionFieldMapping, Vanrise.Analytic.Entities]], mscorlib","$values":[{"$type":"Vanrise.Analytic.Entities.DimensionFieldMapping, Vanrise.Analytic.Entities","DataRecordTypeId":"6cf5f7ad-5123-45d2-b47f-eca613d454f7"}]}}'),
('9C0038AD-BA72-407A-A41B-E2742D2A3AA2','913411d7-2782-4ac9-a55c-27cd88a55ebf',4,'CountConnected','CountConnected','{"$type":"Vanrise.Analytic.Entities.AnalyticAggregateConfig, Vanrise.Analytic.Entities","SQLColumn":"case when con_date is null then 0  else 1 end","AggregateType":2}'),
('66037B35-B29C-4542-A9B9-F50155040E1A','913411d7-2782-4ac9-a55c-27cd88a55ebf',1,'Customer','Customer','{"$type":"Vanrise.Analytic.Entities.AnalyticDimensionConfig, Vanrise.Analytic.Entities","GetValueMethod":"var endPointId = context.GetDimensionValue(\"UserId\");\nif(endPointId != null)\n {\n// return (new NP.IVSwitch.Business.AccountManager()).GetEndPointSwitchAccountName(endPointId);\n    return (new NP.IVSwitch.Business.EndPointManager()).GetEndPointCarrierAccountId(endPointId);\n}\nelse\n return null;","DependentDimensions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["UserId"]},"FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","BusinessEntityDefinitionId":"ba5a57bd-1f03-440f-a469-463a48762b8f","IsNullable":false,"OrderType":1},"DimensionFieldMappings":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.DimensionFieldMapping, Vanrise.Analytic.Entities]], mscorlib","$values":[{"$type":"Vanrise.Analytic.Entities.DimensionFieldMapping, Vanrise.Analytic.Entities","DataRecordTypeId":"6cf5f7ad-5123-45d2-b47f-eca613d454f7"}]}}'),
('3159D494-FAA1-4F4A-95AA-FC7A8AB43975','913411d7-2782-4ac9-a55c-27cd88a55ebf',4,'SumDurationInSec','SumDurationInSec','{"$type":"Vanrise.Analytic.Entities.AnalyticAggregateConfig, Vanrise.Analytic.Entities","SQLColumn":"case when con_date is not null then extract(epoch from now()-con_date) else 0 end","AggregateType":2}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[TableId],[ItemType],[Name],[Title],[Config]))
merge	[Analytic].[AnalyticItemConfig] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[TableId] = s.[TableId],[ItemType] = s.[ItemType],[Name] = s.[Name],[Title] = s.[Title],[Config] = s.[Config]
when not matched by target then
	insert([ID],[TableId],[ItemType],[Name],[Title],[Config])
	values(s.[ID],s.[TableId],s.[ItemType],s.[Name],s.[Title],s.[Config]);
END

--[TOneWhS_BE].[Switch]-----------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
begin
set nocount on;
set identity_insert [TOneWhS_BE].[Switch] on;
;with cte_data([ID],[Name],[Settings],[SourceID])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'IVSwitch','{"$type":"TOne.WhS.BusinessEntity.Entities.SwitchSettings, TOne.WhS.BusinessEntity.Entities","RouteSynchronizer":{"$type":"TOne.WhS.RouteSync.IVSwitch.BuiltInIVSwitchSWSync, TOne.WhS.RouteSync.IVSwitch","ConfigId":"1ee51230-fe31-4d01-9289-0e27e24d3601","OwnerName":"postgres","MasterConnectionString":"Server=192.168.110.185;Database=ustn_master;Userid=postgres;Password=postgres;","RouteConnectionString":"Server=192.168.110.185;Database=ustn_route_tables;Userid=postgres;Password=postgres;","TariffConnectionString":"Server=192.168.110.185;Database=ustn_tariffs;Userid=postgres;Password=postgres;","NumberOfOptions":4,"BlockedAccountMapping":"941","Uid":"23ae4765-bbf5-4466-0ef3-c953e562a82d"},"SwitchCDRMappingConfiguration":{"$type":"TOne.WhS.BusinessEntity.Entities.SwitchCDRMappingConfiguration, TOne.WhS.BusinessEntity.Entities"}}',null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings],[SourceID]))
merge	[TOneWhS_BE].[Switch] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
--when matched then
--	update set
--	[Name] = s.[Name],[Settings] = s.[Settings],[SourceID] = s.[SourceID]
when not matched by target then
	insert([ID],[Name],[Settings],[SourceID])
	values(s.[ID],s.[Name],s.[Settings],s.[SourceID])
WHEN NOT MATCHED BY SOURCE
    THEN DELETE;
set identity_insert [TOneWhS_BE].[Switch] off;
END

--[TOneWhS_RouteSync].[RouteSyncDefinition]---------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [TOneWhS_RouteSync].[RouteSyncDefinition] on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Built In IV Switch','{"$type":"TOne.WhS.RouteSync.Entities.RouteSyncDefinitionSettings, TOne.WhS.RouteSync.Entities","RouteReader":{"$type":"TOne.WhS.Routing.Business.Extensions.RouteSyncReader, TOne.WhS.Routing.Business","RangeType":0,"ConfigId":0},"SwitchIds":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["1"]}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings]))
merge	[TOneWhS_RouteSync].[RouteSyncDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Settings])
	values(s.[ID],s.[Name],s.[Settings])
WHEN NOT MATCHED BY SOURCE
    THEN DELETE;
set identity_insert [TOneWhS_RouteSync].[RouteSyncDefinition] off;

Delete from [sec].[View] where [ID] IN ('0D2A2A90-A3B7-42BE-AA37-7C9D2DECE9D3','B26EA2B0-06F6-4FBF-BE49-DA01859073DD')--'Switches','Route Sync Definitions'
--[sec].[View]------------------------------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('9B4C0061-8610-42B0-B522-BCCFD8314B75','Switch Mapping','Switch Mapping','#/view/NP_IVSwitch/Views/SwitchMapping/SwitchMappingManagement','D018C0CD-F15F-486D-80C3-F9B87C3F47B8','NP_IVSwitch/SwitchMapping/GetFilteredSwitchMappings',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',55),

('9F98A163-C563-4836-B53D-212DFF6D6AF5','Translation Rules','Translation Rules','#/view/NP_IVSwitch/Views/TranslationRule/TranslationRuleManagement'	,'89254E36-5D91-4DB1-970F-9BFEF404679A','NP_IVSwitch/TranslationRule/GetFilteredTranslationRules',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',105),
('979EA3A2-EF3C-4227-802E-4C501280BEDA','Codec Profiles','Codec Profiles','#/view/NP_IVSwitch/Views/CodecProfile/CodecProfileManagement'				,'89254E36-5D91-4DB1-970F-9BFEF404679A','NP_IVSwitch/CodecProfile/GetFilteredCodecProfiles',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',110),

('AA625479-74DE-4092-931F-D640E4BC2921','Switch Configuration','Switch Configuration','#/viewwithparams/WhS_BusinessEntity/Views/Switch/SingleSwitchManagement/{"switchId":"1"}','D7146EBA-A2B1-484C-A498-0DDE876A7580','WhS_BE/Switch/GetFilteredSwitches & WhS_BE/Switch/UpdateSwitch',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',4),
('C028E089-4353-4408-BE19-5BB751FFDD73','Firewalls','Firewalls','#/view/NP_IVSwitch/Views/Firewall/FirewallManagement'								,'50624672-CD25-44FD-8580-0E3AC8E34C71','NP_IVSwitch/Firewall/GetFilteredFirewalls',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',115),
('8098e7bb-55ed-4c3b-be5e-72ad74cb62c4','Switch Dashboard','Switch Dashboard','#/view/NP_IVSwitch/Views/SwitchDashboard/SwitchDashboard','3246ccb2-88d4-473e-a229-dc1c7de22f8c',null,null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',1)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank]
when not matched by target then
	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank]);
--------------------------------------------------------------------------------------------------------------
end

--[sec].[SystemAction]------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([Name],[RequiredPermissions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('NP_IVSwitch/CodecProfile/GetFilteredCodecProfiles','NP_IVSwitch_CodecProfile: View'),
('NP_IVSwitch/CodecProfile/GetCodecProfile',null),
('NP_IVSwitch/CodecProfile/AddCodecProfile','NP_IVSwitch_CodecProfile: Add'),
('NP_IVSwitch/CodecProfile/UpdateCodecProfile','NP_IVSwitch_CodecProfile: Edit'),
('NP_IVSwitch/CodecProfile/GetCodecProfileEditorRuntime',null),
('NP_IVSwitch/CodecProfile/GetCodecProfilesInfo',null),

('NP_IVSwitch/TranslationRule/GetFilteredTranslationRules','NP_IVSwitch_TranslationRule: View'),
('NP_IVSwitch/TranslationRule/GetTranslationRule',null),
('NP_IVSwitch/TranslationRule/AddTranslationRule','NP_IVSwitch_TranslationRule: Add'),
('NP_IVSwitch/TranslationRule/UpdateTranslationRule','NP_IVSwitch_TranslationRule: Edit'),
('NP_IVSwitch/TranslationRule/GetTranslationRulesInfo',null),

('NP_IVSwitch/Firewall/GetFilteredFirewalls','NP_IVSwitch_Firewall: View'),
('NP_IVSwitch/Firewall/GetFirewall',null),
('NP_IVSwitch/Firewall/AddFirewall','NP_IVSwitch_Firewall: Add'),
('NP_IVSwitch/Firewall/UpdateFirewall','NP_IVSwitch_Firewall: Edit'),

('NP_IVSwitch/SwitchMapping/GetFilteredSwitchMappings','VR_SystemConfiguration: View'),
('NP_IVSwitch/SwitchMapping/LinkCarrierToEndPoints','WhS_BE_Carrier: Edit'),
('NP_IVSwitch/SwitchMapping/LinkCarrierToRoutes','WhS_BE_Carrier: Edit')
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

--[sec].[BusinessEntity]----------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('BA7E48EB-E7CB-4C32-AE2D-47C9FCCBC7B6','NP_IVSwitch_CodecProfile','Codec Profile'			,'8FAF6AA2-6C00-4C48-8EBF-68B87D2DC493',0,'["View", "Add", "Edit"]'),
('D2D07DBA-66EA-4B68-9F00-EBD66B00A740','NP_IVSwitch_TranslationRule','Translation Rule'	,'8FAF6AA2-6C00-4C48-8EBF-68B87D2DC493',0,'["View", "Add", "Edit"]'),

('5437DC96-9CB1-44B2-B680-B1A51D6CA876','NP_IVSwitch_Firewall','Firewall'					,'61451603-E7B9-40C6-AE27-6CBA974E1B3B',0,'["View", "Add", "Edit"]')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions]))
merge	[sec].[BusinessEntity] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[ModuleId] = s.[ModuleId],[BreakInheritance] = s.[BreakInheritance],[PermissionOptions] = s.[PermissionOptions]
when not matched by target then
	insert([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
	values(s.[Id],s.[Name],s.[Title],s.[ModuleId],s.[BreakInheritance],s.[PermissionOptions]);

--[common].[Setting]----------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
Update	s
SET		s.[Data] = REPLACE(s.[Data],'"ProductName":"TOne"','"ProductName":"CLoudXPoint"')
from	[common].[Setting] s
where	ID='509E467B-4562-4CA6-A32E-E50473B74D2C'

--[common].[Currency]-------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [common].[Currency] on;
;with cte_data([ID],[Symbol],[Name],[SourceID])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(155,'USD','United States Dollars','USD')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Symbol],[Name],[SourceID]))
merge	[common].[Currency] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
--when matched then
--	update set
--	[Symbol] = s.[Symbol],[Name] = s.[Name],[SourceID] = s.[SourceID]
when not matched by target then
	insert([ID],[Symbol],[Name],[SourceID])
	values(s.[ID],s.[Symbol],s.[Name],s.[SourceID]);
set identity_insert [common].[Currency] off;

--[common].[Setting]--------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
update s	set
			s.[Data] = '{"$type":"Vanrise.Entities.CurrencySettingData, Vanrise.Entities","CurrencyId":155}'
from		[common].[Setting] s
where		s.ID='1C833B2D-8C97-4CDD-A1C1-C1B4D9D299DE' and s.[Data] is null

--[logging].[LoggableEntity]------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[UniqueName],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('A33DF7F0-25C0-4537-85FF-29CAC5BBEB00','NP_IVSwitch_CodecProfile','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"NP_IVSwitch_CodecProfile_ViewHistoryItem"}'),
('0823D02D-E150-403E-82A1-325CBCBC824D','NP_IVSwitch_Route','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"NP_IVSwitch_Route_ViewHistoryItem"}'),
('BAA954E1-A523-4668-9C9E-59D8493F73CC','NP_IVSwitch_Firewall','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"NP_IVSwitch_Firewall_ViewHistoryItem"}'),
('A656DCD9-2ADA-4EAF-9DF6-735830DE5EFA','NP_IVSwitch_TranslationRule','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"NP_IVSwitch_TranslationRule_ViewHistoryItem"}'),
('B5F4BBB6-509F-45DE-9590-B1BB9AEAB1BA','NP_IVSwitch_EndPoint','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"NP_IVSwitch_EndPoint_ViewHistoryItem"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[UniqueName],[Settings]))
merge	[logging].[LoggableEntity] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[UniqueName] = s.[UniqueName],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[UniqueName],[Settings])
	values(s.[ID],s.[UniqueName],s.[Settings]);