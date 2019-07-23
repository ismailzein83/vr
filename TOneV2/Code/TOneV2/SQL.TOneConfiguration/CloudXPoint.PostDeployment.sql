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

--[sec].[Module]------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Url],[DefaultViewId],[ParentId],[Icon],[Rank],[AllowDynamic],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('148EC616-4E67-4343-A3D4-5FA535238249','Switch Manager',null,null,null,'/Client/Images/menu-icons/Business Entities.png',7,0,null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Url],[DefaultViewId],[ParentId],[Icon],[Rank],[AllowDynamic],[Settings]))
merge	[sec].[Module] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Url] = s.[Url],[DefaultViewId] = s.[DefaultViewId],[ParentId] = s.[ParentId],[Icon] = s.[Icon],[Rank] = s.[Rank],[AllowDynamic] = s.[AllowDynamic],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Url],[DefaultViewId],[ParentId],[Icon],[Rank],[AllowDynamic],[Settings])
	values(s.[ID],s.[Name],s.[Url],s.[DefaultViewId],s.[ParentId],s.[Icon],s.[Rank],s.[AllowDynamic],s.[Settings]);


Delete from [sec].[View] where [ID] IN ('0D2A2A90-A3B7-42BE-AA37-7C9D2DECE9D3','B26EA2B0-06F6-4FBF-BE49-DA01859073DD')--'Switches','Route Sync Definitions'
--[sec].[View]------------------------------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('9B4C0061-8610-42B0-B522-BCCFD8314B75','Switch Mapping','Switch Mapping','#/view/NP_IVSwitch/Views/SwitchMapping/SwitchMappingManagement'													,'D018C0CD-F15F-486D-80C3-F9B87C3F47B8','NP_IVSwitch/SwitchMapping/GetFilteredSwitchMappings',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',55),

('8098e7bb-55ed-4c3b-be5e-72ad74cb62c4','Switch Dashboard','Switch Dashboard','#/view/NP_IVSwitch/Views/SwitchDashboard/SwitchDashboard'													,'148EC616-4E67-4343-A3D4-5FA535238249','NP_IVSwitch/SwitchDashboard/GetSwitchDashboardManagerResult',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',3),
('AA625479-74DE-4092-931F-D640E4BC2921','Switch Configuration','Switch Configuration','#/viewwithparams/WhS_BusinessEntity/Views/Switch/SingleSwitchManagement/{"switchId":"1"}'			,'148EC616-4E67-4343-A3D4-5FA535238249','WhS_BE/Switch/GetFilteredSwitches & WhS_BE/Switch/UpdateSwitch',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',5),
('C8B12E9A-2B11-4B60-BBE2-2023330B02E2','Route Table','Route Table','#/viewwithparams/NP_IVSwitch/Views/RouteTable/RouteTableManagement/{"viewId":"c8b12e9a-2b11-4b60-bbe2-2023330b02e2"}'	,'148EC616-4E67-4343-A3D4-5FA535238249','NP_IVSwitch/RouteTable/GetFilteredRouteTables',null,null,'{"$type":"NP.IVSwitch.Entities.RouteTableViewSettings, NP.IVSwitch.Entities","Types":[0,1,2]}','372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',7),
('37714ED2-FCA7-4C9C-A75B-720424B947AF','CLI Groups','CLI Groups',null																														,'148EC616-4E67-4343-A3D4-5FA535238249',null,null,null,'{"$type":"Vanrise.GenericData.Business.GenericBEViewSettings, Vanrise.GenericData.Business","Settings":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Business.GenericBEViewSettingItem, Vanrise.GenericData.Business]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Business.GenericBEViewSettingItem, Vanrise.GenericData.Business","BusinessEntityDefinitionId":"94820cf8-c9a3-40a9-b90e-ba25a2a96d73"}]}}','B99B2B0A-9A80-49FC-B68F-C946E1628595',9),
('9F98A163-C563-4836-B53D-212DFF6D6AF5','Translation Rules','Translation Rules','#/view/NP_IVSwitch/Views/TranslationRule/TranslationRuleManagement'										,'148EC616-4E67-4343-A3D4-5FA535238249','NP_IVSwitch/TranslationRule/GetFilteredTranslationRules',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',10),
('979EA3A2-EF3C-4227-802E-4C501280BEDA','Codec Profiles','Codec Profiles','#/view/NP_IVSwitch/Views/CodecProfile/CodecProfileManagement'													,'148EC616-4E67-4343-A3D4-5FA535238249','NP_IVSwitch/CodecProfile/GetFilteredCodecProfiles',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',12),
('C028E089-4353-4408-BE19-5BB751FFDD73','Firewalls','Firewalls','#/view/NP_IVSwitch/Views/Firewall/FirewallManagement'																		,'148EC616-4E67-4343-A3D4-5FA535238249','NP_IVSwitch/Firewall/GetFilteredFirewalls',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',14)
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
--------------------------------------------------------------------------------------------------------------
end

---------------move views related to switch manager from administration/switches to switch manager module
UPDATE	[sec].[View] 
SET		[Module] = '148EC616-4E67-4343-A3D4-5FA535238249'
WHERE	[ID] IN (	'11603625-2CF3-436D-9FAA-B5D04CB42F09',--'Switch Connectivity'
				   '3F7327B6-0742-4C6D-8B2C-BE9B5D4DECD4',--'Switch Release Cause'
				   'a01100f6-e5ca-43f2-8944-d78bd95cb4ee')--,'Point Of Interconnects'

UPDATE	[sec].[BusinessEntity]
SET		[ModuleId] = '475F9CED-6D87-4B16-B9D8-ED06F60F3465'
WHERE	[ID] IN (	'18DAE80F-E3FE-4E4B-A92E-1ED8FDC28AB6',--'WhS_BE_Switch'
					'E3CF3CB5-0F44-49B7-87E6-D05B3DE54B8D',--'WhS_BE_SwitchReleaseCode'
					'DBCC03FB-EEEE-4C96-962B-5E68CF019771',--'WhS_BE_SwitchConnectivity'
					'20688FC8-12A8-49E7-B68F-7ABA0BF21CD6')--,'WHS_BE_POI','Point Of Interconnects'	

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
('NP_IVSwitch/SwitchMapping/LinkCarrierToRoutes','WhS_BE_Carrier: Edit'),
('NP_IVSwitch/RouteTable/AddRouteTable','NP_IVSwitch_RouteTable: Add'),
('NP_IVSwitch/RouteTable/UpdateRouteTable','NP_IVSwitch_RouteTable: Edit'),
('NP_IVSwitch/RouteTable/DeleteRouteTable','NP_IVSwitch_RouteTable: Delete'),
('NP_IVSwitch/RouteTableRoute/AddRouteTableRoutes','NP_IVSwitch_RouteTable: Add'),
('NP_IVSwitch/RouteTableRoute/UpdateRouteTableRoute','NP_IVSwitch_RouteTable: Edit'),
('NP_IVSwitch/RouteTableRoute/DeleteRouteTableRoute','NP_IVSwitch_RouteTable: Delete'),
('NP_IVSwitch/RouteTableRoute/GetFilteredRouteTableRoutes','NP_IVSwitch_RouteTable: View'),
('NP_IVSwitch/RouteTable/GetFilteredRouteTables','NP_IVSwitch_RouteTable: View'),
('NP_IVSwitch/TranslationRule/DeleteTranslationRule','NP_IVSwitch_TranslationRule: Delete'),
('NP_IVSwitch/EndPoint/GetFilteredEndPoints','NP_IVSwitch_EndPoint: View'),
('NP_IVSwitch/EndPoint/AddEndPoint','NP_IVSwitch_EndPoint: Add'),
('NP_IVSwitch/EndPoint/UpdateEndPoint','NP_IVSwitch_EndPoint: Edit'),
('NP_IVSwitch/EndPoint/DeleteEndPoint','NP_IVSwitch_EndPoint: Delete'),
('NP_IVSwitch/Route/GetFilteredRoutes','NP_IVSwitch_Route: View'),
('NP_IVSwitch/Route/AddRoute','NP_IVSwitch_Route: Add'),
('NP_IVSwitch/Route/UpdateRoute','NP_IVSwitch_Route: Edit'),
('NP_IVSwitch/Route/DeleteRoute','NP_IVSwitch_Route: Delete'),
('NP_IVSwitch/SwitchDashboard/GetSwitchDashboardManagerResult','NP_IVSwitch_SwitchDashboard:View'),
('NP_IVSwitch/EndPoint/BlockEndPoint','NP_IVSwitch_EndPoint: Block'),
('NP_IVSwitch/EndPoint/InActivateEndPoint','NP_IVSwitch_EndPoint: InActivate'),
('NP_IVSwitch/EndPoint/ActivateEndPoint','NP_IVSwitch_EndPoint: Activate'),
('NP_IVSwitch/Route/BlockRoute','NP_IVSwitch_Route: Block'),
('NP_IVSwitch/Route/ActivateRoute','NP_IVSwitch_Route: Activate'),
('NP_IVSwitch/Route/InActivateRoute','NP_IVSwitch_Route: InActivate')
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

--[sec].[BusinessEntityModule]----------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[ParentId],[BreakInheritance])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('475F9CED-6D87-4B16-B9D8-ED06F60F3465','Switch Manager','5A9E78AE-229E-41B9-9DBF-492997B42B61',0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[ParentId],[BreakInheritance]))
merge	[sec].[BusinessEntityModule] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[ParentId] = s.[ParentId],[BreakInheritance] = s.[BreakInheritance]
when not matched by target then
	insert([ID],[Name],[ParentId],[BreakInheritance])
	values(s.[ID],s.[Name],s.[ParentId],s.[BreakInheritance]);

--[sec].[BusinessEntity]----------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('BA7E48EB-E7CB-4C32-AE2D-47C9FCCBC7B6','NP_IVSwitch_CodecProfile','Codec Profile'			,'475F9CED-6D87-4B16-B9D8-ED06F60F3465',0,'["View", "Add", "Edit"]'),
('D2D07DBA-66EA-4B68-9F00-EBD66B00A740','NP_IVSwitch_TranslationRule','Translation Rule'	,'475F9CED-6D87-4B16-B9D8-ED06F60F3465',0,'["View", "Add", "Edit","Delete"]'),
('5437DC96-9CB1-44B2-B680-B1A51D6CA876','NP_IVSwitch_Firewall','Firewall'					,'475F9CED-6D87-4B16-B9D8-ED06F60F3465',0,'["View", "Add", "Edit"]'),
('4145E135-96B2-412D-9F4E-D3CEE79741F5','NP_IVSwitch_RouteTable','Route Table'				,'475F9CED-6D87-4B16-B9D8-ED06F60F3465',0,'["View","Add","Edit","Delete"]'),
('F88A6489-C43B-4E7E-B910-46B62E040EC5','NP_IVSwitch_Route','Route'							,'475F9CED-6D87-4B16-B9D8-ED06F60F3465',0,'["View","Add","Edit","Delete"]'),
('19FC9866-775E-4114-8D00-6FF123E67E78','NP_IVSwitch_EndPoint','End Point'					,'475F9CED-6D87-4B16-B9D8-ED06F60F3465',0,'["View","Add","Edit","Delete"]'),
('d84c7f2a-39c5-41b9-aff6-bade7a56e8e7','NP_IVSwitch_CLIGroup','CLIGroup'					,'475f9ced-6d87-4b16-b9d8-ed06f60f3465',0,'["View","Add","Edit","Delete"]'),
('8858303c-e9ed-4709-a050-6b82c04780a0','NP_IVSwitch_SwitchDashboard','Switch Dashboard'	,'475f9ced-6d87-4b16-b9d8-ed06f60f3465',0,'["View"]')
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
SET		s.[Data] = REPLACE(s.[Data],'"ProductName":"T.One"','"ProductName":"CLoudXPoint"')
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


--[genericdata].[DataRecordType]--------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[ParentID],[Fields],[ExtraFieldsEvaluator],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('28547015-7E3B-43AE-B051-A94E78B24FF6','Pool-Based CLI Group',null,'{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"ID","Title":"ID","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","ViewerEditor":"vr-genericdata-fieldtype-number-viewereditor","RuntimeEditor":"vr-genericdata-fieldtype-number-runtimeeditor","DataPrecision":0,"DataType":2,"IsNullable":false,"CanRoundValue":true,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"Name","Title":"Name","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","RuntimeEditor":"vr-genericdata-fieldtype-text-runtimeeditor","OrderType":1,"ViewerEditor":"vr-genericdata-fieldtype-text-viewereditor","DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","StoreValueSerialized":false,"CanRoundValue":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"CLIPatterns","Title":"CLI Patterns","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldCustomObjectType, Vanrise.GenericData.MainExtensions","ConfigId":"28411d23-ea66-47ac-a323-106be0b9da7e","ViewerEditor":"vr-genericdata-fieldtype-customobject-viewereditor","IsNullable":false,"Settings":{"$type":"NP.IVSwitch.Business.PoolBasedCLIGroupCustomObjectTypeSettings, NP.IVSwitch.Business","ConfigId":"7cc89311-c9aa-4f9a-9beb-4549362aea4f"},"StoreValueSerialized":true,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"CanRoundValue":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"CreatedTime","Title":"Created Time","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldDateTimeType, Vanrise.GenericData.MainExtensions","ConfigId":"b8712417-83ab-4d4b-9ee1-109d20ceb909","RuntimeEditor":"vr-genericdata-fieldtype-datetime-runtimeeditor","ViewerEditor":"vr-genericdata-fieldtype-datetime-viewereditor","DataType":0,"IsNullable":false,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false,"CanRoundValue":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"CreatedBy","Title":"Created By","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","RuntimeEditor":"vr-genericdata-fieldtype-businessentity-runtimeeditor","ViewerEditor":"vr-genericdata-fieldtype-businessentity-viewereditor","BusinessEntityDefinitionId":"217a8f71-1dd6-4613-8ae2-540a510f5ff5","IsNullable":false,"OrderType":1,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","StoreValueSerialized":false,"CanRoundValue":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"LastModifiedTime","Title":"Last Modified Time","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldDateTimeType, Vanrise.GenericData.MainExtensions","ConfigId":"b8712417-83ab-4d4b-9ee1-109d20ceb909","RuntimeEditor":"vr-genericdata-fieldtype-datetime-runtimeeditor","ViewerEditor":"vr-genericdata-fieldtype-datetime-viewereditor","DataType":0,"IsNullable":false,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false,"CanRoundValue":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"LastModifiedBy","Title":"Last Modified By","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","RuntimeEditor":"vr-genericdata-fieldtype-businessentity-runtimeeditor","ViewerEditor":"vr-genericdata-fieldtype-businessentity-viewereditor","BusinessEntityDefinitionId":"217a8f71-1dd6-4613-8ae2-540a510f5ff5","IsNullable":false,"OrderType":1,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","StoreValueSerialized":false,"CanRoundValue":false},"IsInheritedFromExtraField":false}]}',null,'{"$type":"Vanrise.GenericData.Entities.DataRecordTypeSettings, Vanrise.GenericData.Entities","IdField":"ID"}')
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


--[genericdata].[DataRecordStorage]-----------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[DataRecordTypeID],[DataStoreID],[Settings],[State])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('E3E5E4F9-2F8C-4CE5-8645-9A1C0386631E','Pool-Based CLI Group','28547015-7E3B-43AE-B051-A94E78B24FF6','FF21AE78-F1D2-44B5-B12D-8B380F8D4F42','{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageSettings, Vanrise.GenericData.SQLDataStorage","TableName":"PoolBasedCLIGroup","TableSchema":"NP_IVSwitch","Columns":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage]], mscorlib","$values":[{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"ID","SQLDataType":"BIGINT","ValueExpression":"ID","IsUnique":true,"IsIdentity":true},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"Name","SQLDataType":"NVARCHAR(MAX)","ValueExpression":"Name","IsUnique":true,"IsIdentity":false},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"CreatedTime","SQLDataType":"DATETIME","ValueExpression":"CreatedTime","IsUnique":false,"IsIdentity":false},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"CreatedBy","SQLDataType":"INT","ValueExpression":"CreatedBy","IsUnique":false,"IsIdentity":false},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"LastModifiedTime","SQLDataType":"DATETIME","ValueExpression":"LastModifiedTime","IsUnique":false,"IsIdentity":false},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"LastModifiedBy","SQLDataType":"INT","ValueExpression":"LastModifiedBy","IsUnique":false,"IsIdentity":false},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"CLIPatterns","SQLDataType":"NVARCHAR(MAX)","ValueExpression":"CLIPatterns","IsUnique":false,"IsIdentity":false}]},"NullableFields":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.SQLDataStorage.NullableField, Vanrise.GenericData.SQLDataStorage]], mscorlib","$values":[]},"IncludeQueueItemId":false,"DateTimeField":"CreatedTime","LastModifiedByField":"LastModifiedBy","CreatedByField":"CreatedBy","LastModifiedTimeField":"LastModifiedTime","CreatedTimeField":"CreatedTime","EnableUseCaching":true,"RequiredLimitResult":false,"DontReflectToDB":false,"DenyAPICall":false,"RequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}},"FieldsPermissions":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.DataRecordStorageFieldsPermission, Vanrise.GenericData.Entities]], mscorlib","$values":[]}}',null)
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


	
--[genericdata].[BusinessEntityDefinition]----------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Title],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('94820CF8-C9A3-40A9-B90E-BA25A2A96D73','PoolBasedCLIGroup','Pool-Based CLI Group','{"$type":"Vanrise.GenericData.Business.GenericBEDefinitionSettings, Vanrise.GenericData.Business","ConfigId":"6f3fbd7b-275a-4d92-8e06-ad7f7b04c7d6","DefinitionEditor":"vr-genericdata-genericbusinessentity-editor","ViewerEditor":"vr-genericdata-genericbusinessentity-runtimeeditor","IdType":"System.Int64","SelectorUIControl":"vr-genericdata-genericbusinessentity-selector","ManagerFQTN":"Vanrise.GenericData.Business.GenericBusinessEntityManager, Vanrise.GenericData.Business","GenericBEType":0,"HideAddButton":false,"SelectorSingularTitle":"Group","SelectorPluralTitle":"Groups","Security":{"$type":"Vanrise.GenericData.Business.GenericBEDefinitionSecurity, Vanrise.GenericData.Business","ViewRequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"d84c7f2a-39c5-41b9-aff6-bade7a56e8e7","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"AddRequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"d84c7f2a-39c5-41b9-aff6-bade7a56e8e7","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Add"]}}]}},"EditRequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"d84c7f2a-39c5-41b9-aff6-bade7a56e8e7","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Edit"]}}]}}},"EditorSize":1,"DataRecordTypeId":"28547015-7e3b-43ae-b051-a94e78b24ff6","DataRecordStorageId":"e3e5e4f9-2f8c-4ce5-8645-9a1c0386631e","TitleFieldName":"Name","GenericBEActions":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Business.GenericBEAction, Vanrise.GenericData.Business]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Business.GenericBEAction, Vanrise.GenericData.Business","GenericBEActionId":"91ded9e5-01e2-8bf0-dfe9-e1109033bd4e","Name":"Edit","Settings":{"$type":"Vanrise.GenericData.MainExtensions.EditGenericBEAction, Vanrise.GenericData.MainExtensions","ConfigId":"293b2fab-6abe-4be7-ad58-7d9fa0ba9524","ActionTypeName":"EditGenericBEAction"}},{"$type":"Vanrise.GenericData.Business.GenericBEAction, Vanrise.GenericData.Business","GenericBEActionId":"9ae7cd8f-2d6c-3737-4ea5-9809536bdd66","Name":"Delete","Settings":{"$type":"Vanrise.GenericData.MainExtensions.DeleteGenericBEAction, Vanrise.GenericData.MainExtensions","ConfigId":"6bc1fb84-f28d-476a-81fa-a11fc4e5cc06","ActionTypeName":"DeleteGenericBEAction"}}]},"GridDefinition":{"$type":"Vanrise.GenericData.Business.GenericBEGridDefinition, Vanrise.GenericData.Business","ColumnDefinitions":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Business.GenericBEGridColumn, Vanrise.GenericData.Business]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Business.GenericBEGridColumn, Vanrise.GenericData.Business","FieldName":"ID","FieldTitle":"ID","GridColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Normal"}},{"$type":"Vanrise.GenericData.Business.GenericBEGridColumn, Vanrise.GenericData.Business","FieldName":"Name","FieldTitle":"CLI Group","GridColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Normal"}},{"$type":"Vanrise.GenericData.Business.GenericBEGridColumn, Vanrise.GenericData.Business","FieldName":"CLIPatterns","FieldTitle":"CLI Numbers","GridColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Normal"}},{"$type":"Vanrise.GenericData.Business.GenericBEGridColumn, Vanrise.GenericData.Business","FieldName":"CreatedTime","FieldTitle":"Created Time","GridColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Normal"}}]},"GenericBEGridActions":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Business.GenericBEGridAction, Vanrise.GenericData.Business]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Business.GenericBEGridAction, Vanrise.GenericData.Business","GenericBEGridActionId":"d882496c-653f-b40c-b75c-600f05cdffee","GenericBEActionId":"91ded9e5-01e2-8bf0-dfe9-e1109033bd4e","Title":"Edit","ReloadGridItem":false},{"$type":"Vanrise.GenericData.Business.GenericBEGridAction, Vanrise.GenericData.Business","GenericBEGridActionId":"25644703-00e3-b8e9-27be-388ff5270f47","GenericBEActionId":"9ae7cd8f-2d6c-3737-4ea5-9809536bdd66","Title":"Delete","ReloadGridItem":false}]},"GenericBEGridViews":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Business.GenericBEViewDefinition, Vanrise.GenericData.Business]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Business.GenericBEViewDefinition, Vanrise.GenericData.Business","GenericBEViewDefinitionId":"015be2a4-b073-f9bc-ae8e-1e763000f365","Name":"History","Settings":{"$type":"Vanrise.GenericData.MainExtensions.HistoryGenericBEDefinitionView, Vanrise.GenericData.MainExtensions","ConfigId":"77f7dcb8-e42f-4ec3-8f46-0d655fd519b0","RuntimeDirective":"vr-genericdata-genericbe-historygridview-runtime"}}]}},"EditorDefinition":{"$type":"Vanrise.GenericData.Business.GenericBEEditorDefinition, Vanrise.GenericData.Business","Settings":{"$type":"Vanrise.GenericData.MainExtensions.StaticEditorDefinitionSetting, Vanrise.GenericData.MainExtensions","ConfigId":"ec8b54d7-28ac-474f-b40a-d7ac02d89630","DirectiveName":"np-ivswitch-translationrule-poolbasedcligroup-staticeditor","RuntimeEditor":"vr-genericdata-staticeditor-runtime"}},"FilterDefinition":{"$type":"Vanrise.GenericData.Business.GenericBEFilterDefinition, Vanrise.GenericData.Business","Settings":{"$type":"Vanrise.GenericData.MainExtensions.GenericFilterDefinitionSettings, Vanrise.GenericData.MainExtensions","ConfigId":"6d005236-ece6-43a1-b8ea-281bc0e7643e","RuntimeEditor":"vr-genericdata-genericbe-filterruntime-generic","FieldName":"Name","FieldTitle":"CLI Group","IsRequired":false}},"ShowUpload":false}'),
('CACE00E6-31C2-4FE5-B7BB-636BA1757981','EndPointandRoutesStatusDefinition','EndPointandRoutesStatusDefinition','{"$type":"Vanrise.Common.Business.StatusDefinitionBESettings, Vanrise.Common.Business","ConfigId":"3f26b7e1-33d8-4428-9a3c-986805289c91","DefinitionEditor":"vr-common-statusdefinitionbe-editor","IdType":"System.Guid","ManagerFQTN":"Vanrise.Common.Business.StatusDefinitionManager, Vanrise.Common.Business","SelectorUIControl":"vr-common-statusdefinition-selector"}'),
('959e6f3e-e16e-4ab6-a939-f592e08dfe61','EndPoint','End Point','{"$type":"Vanrise.GenericData.Entities.StaticBEDefinitionSettings, Vanrise.GenericData.Entities","ManagerFQTN":""}'),
('fdd8a75c-5c6c-46d6-a00a-b99086a01194','Route','Route','{"$type":"Vanrise.GenericData.Entities.StaticBEDefinitionSettings, Vanrise.GenericData.Entities","ManagerFQTN":""}')
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

--[integration].[DataSource]-----------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[AdapterID],[AdapterState],[TaskId],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('DE0924D2-E9C2-476D-9FF1-AC5C032E8E90','IV Switch Invalid CDR Import - Sample Datasource','105A2927-10C2-4F20-97CE-EDBA90198553','{"$type":"Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments.DBAdapterState, Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments","LastImportedId":0}','A1628B86-BFE5-4E0E-8746-BD396C02F7DC','{"$type":"Vanrise.Integration.Entities.DataSourceSettings, Vanrise.Integration.Entities","AdapterArgument":{"$type":"Vanrise.Integration.Adapters.PostgresReceiveAdapter.Arguments.PostgresAdapterArgument, Vanrise.Integration.Adapters.PostgresReceiveAdapter.Arguments","ConnectionString":"Server=192.168.25.231;Database=ustn_cdrs;Userid=postgres;Password=postgres;","Query":"with RECURSIVE\n\nFailedCDR as \n(\n\tSELECT * FROM public.cdrs_failed\n\tWhere  \n\t(CDR_ID > coalesce(@RangeStart, 0))\n\tAND (CDR_ID <= coalesce(@RangeEnd,9999999999999999))\n\tORDER BY CDR_ID\tASC \n\t#TopRows#\n)\n,grouppedCDRs as \n(\n    select MAX(cdr_id) cdr_id,chan_id from FailedCDR \n    group by chan_id\n)\n,\ncdrs as\n(\nselect FailedCDR.*,CASE  WHEN  master.chan_id is null THEN  0 ELSE 1 END IsRerouted\nfrom FailedCDR \nleft join (select chan_id from cdrs_master) master on FailedCDR.chan_id = master.chan_id\n) \n\nselect \n\t cdrs.cdr_id\n\t,cdrs.det_date\n\t,cdrs.dis_date\n\t,cdrs.user_id\n\t,cdrs.src_ip\n\t,cdrs.route_id\n\t,cdrs.route_ip\n\t,cdrs.cli\n\t,cdrs.dest_number\n\t,cdrs.dis_q850\n\t,cdrs.dis_disposition\n\t, case when cdrs.IsRerouted=1 then 1 else (case when grouppedCDRs.cdr_id is null then 1 else 0 end) end IsRerouted\nfrom cdrs\nleft join grouppedCDRs on cdrs.cdr_id = grouppedCDRs.cdr_id\nORDER BY CDR_ID\tASC","IdentifierColumnName":"cdr_id","NumberOfParallelReader":5,"NumberOffSet":50000,"MaxParallelRuntimeInstances":5,"CommandTimeoutInSeconds":600},"MapperCustomCode":"LogVerbose(\"Started\");\n        var cdrs = new List<dynamic>();\n        var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();\n        Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType(\"CDR\");\n        int maximumBatchSize = 50000;\n        var dataRecordVanriseType = new Vanrise.GenericData.Entities.DataRecordVanriseType(\"CDR\");\n        var importedData = ((Vanrise.Integration.Entities.DBReaderImportedData)(data));\n        IDataReader reader = importedData.Reader;\n        long failedCdrId = 0;\n        int rowCount = 0;\n\n        while (reader.Read())\n        {\n            dynamic cdr = Activator.CreateInstance(cdrRuntimeType) as dynamic;\n            cdr.SwitchId = 83;\n            failedCdrId = (Utils.GetReaderValue<int>(reader, \"cdr_id\"));\n            cdr.IDonSwitch = failedCdrId * (-1);\n            cdr.Tag = cdr.IDonSwitch.ToString();\n            cdr.AttemptDateTime = (DateTime)reader[\"det_date\"];\n            cdr.AlertDateTime = cdr.AttemptDateTime;\n            cdr.DisconnectDateTime = Utils.GetReaderValue<DateTime>(reader, \"dis_date\");\n            cdr.DurationInSeconds = 0;\n            cdr.InCarrier = Utils.GetReaderValue<int>(reader, \"user_id\").ToString();\n            cdr.InIP = reader[\"src_ip\"] as string;\n            cdr.OutCarrier = Utils.GetReaderValue<int>(reader, \"route_id\").ToString();\n            cdr.OutIP = reader[\"route_ip\"] as string;\n            cdr.CGPN = reader[\"cli\"] as string;\n            cdr.CDPN = reader[\"dest_number\"] as string;\n            cdr.CauseFromReleaseCode = reader[\"dis_q850\"] as string;\n            cdr.CauseToReleaseCode = reader[\"dis_q850\"] as string;\n            string cause = reader[\"dis_disposition\"] as string;\n            cause = cause.Trim();\n            string causeFrom = \"\";\n            string causeTo = \"\";\n            if (!string.IsNullOrEmpty(cause))\n            {\n                switch (cause)\n                {\n                    case \"sb\":\n                        causeFrom = \"A\";\n                        causeTo = \"B\";\n                        break;\n                    case \"rb\":\n                        causeFrom = \"B\";\n                        causeTo = \"A\";\n                        break;\n                    case \"sc\":\n                    case \"rr\":\n                        causeFrom = \"S\";\n                        causeTo = \"S\";\n                        break;\n                    default:\n                        break;\n                }\n            }\n            cdr.CauseTo = causeFrom;\n            cdr.CauseFrom = causeTo;\n            cdr.IsRerouted = false;\n            cdrs.Add(cdr);\n            importedData.LastImportedId = failedCdrId;\n            rowCount++;\n            if (rowCount == maximumBatchSize)\n                break;\n        }\n        long startingId;\n        Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(dataRecordVanriseType, rowCount, out startingId);\n        long currentCDRId = startingId;\n        foreach (var cdr in cdrs)\n        {\n            cdr.Id = currentCDRId;\n            currentCDRId++;\n        }\n        if (cdrs.Count > 0)\n        {\n            var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, \"#RECORDSCOUNT# of Raw CDRs\", \"CDR\");\n            mappedBatches.Add(\"Distribute Raw CDRs Stage\", batch);\n        }\n        else\n            importedData.IsEmpty = true;\n\n        Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();\n        result.Result = Vanrise.Integration.Entities.MappingResult.Valid;\n        LogVerbose(\"Finished\");\n        return result;","ExecutionFlowId":"2799f9d0-8b71-47a1-861f-ab7c2850e8fc"}'),
('A1C364F5-A36F-4410-8C7C-B19FF105B72C','IV Switch Main CDR Import - Sample Datasource','105A2927-10C2-4F20-97CE-EDBA90198553','{"$type":"Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments.DBAdapterState, Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments","LastImportedId":0}','3AD3A313-F0D6-46FD-9673-52511975223E','{"$type":"Vanrise.Integration.Entities.DataSourceSettings, Vanrise.Integration.Entities","AdapterArgument":{"$type":"Vanrise.Integration.Adapters.PostgresReceiveAdapter.Arguments.PostgresAdapterArgument, Vanrise.Integration.Adapters.PostgresReceiveAdapter.Arguments","NumberOffSet":1000000,"ConnectionString":"Server=192.168.25.33;Database=ustn_cdrs;Userid=postgres;Password=postgres;","Query":"SELECT * FROM public.cdrs_master\nWhere  \n(CDR_ID > coalesce(@RangeStart, 0))\nAND (CDR_ID <= coalesce(@RangeEnd,9999999999999999))\nORDER BY CDR_ID\nASC \n#TopRows#","CommandTimeoutInSeconds":60,"IdentifierColumnName":"CDR_ID","NumberOfParallelReader":5,"MaxParallelRuntimeInstances":5},"MapperCustomCode":"LogVerbose(\"Started\");\n\n        var cdrs = new List<dynamic>();\n        var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();\n        Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType(\"CDR\");\n\n        int maximumBatchSize = 50000;\n         var dataRecordVanriseType = new Vanrise.GenericData.Entities.DataRecordVanriseType(\"CDR\")\n\n        var importedData = ((Vanrise.Integration.Entities.DBReaderImportedData)(data));\n\n        IDataReader reader = importedData.Reader;\n        int rowCount = 0;\n\n\n        while (reader.Read())\n        {\n            dynamic cdr = Activator.CreateInstance(cdrRuntimeType) as dynamic;\n            cdr.SwitchId = 1;\n            cdr.IDonSwitch = Utils.GetReaderValue<int>(reader, \"cdr_id\");\n            cdr.Tag = cdr.IDonSwitch.ToString();\n            cdr.AttemptDateTime = (DateTime)reader[\"det_date\"];\n            cdr.AlertDateTime = cdr.AttemptDateTime.AddMilliseconds((int)reader[\"pdd\"]);\n            cdr.ConnectDateTime = Utils.GetReaderValue<DateTime>(reader, \"con_date\");\n            cdr.DisconnectDateTime = Utils.GetReaderValue<DateTime>(reader, \"dis_date\");\n            cdr.DurationInSeconds = Utils.GetReaderValue<int>(reader, \"dura\");\n\n\n            cdr.InCarrier = Utils.GetReaderValue<int>(reader, \"user_id\").ToString();\n            cdr.InIP = reader[\"src_ip\"] as string;\n\n\n            cdr.OutCarrier = Utils.GetReaderValue<int>(reader, \"route_id\").ToString();\n            cdr.OutIP = reader[\"route_ip\"] as string;\n\n            cdr.CGPN = reader[\"cli\"] as string;\n            cdr.CDPN = reader[\"dest_number\"] as string;\n            cdr.CauseFromReleaseCode = reader[\"dis_q850\"] as string;\n            cdr.CauseToReleaseCode = reader[\"dis_q850\"] as string;\n\n            string cause = reader[\"dis_disposition\"] as string;\n            cause = cause.Trim();\n            string causeFrom = \"\";\n            string causeTo = \"\";\n            if (!string.IsNullOrEmpty(cause))\n            {\n                switch (cause)\n                {\n                    case \"sb\":\n                        causeFrom = \"A\";\n                        causeTo = \"B\";\n                        break;\n                    case \"rb\":\n                        causeFrom = \"B\";\n                        causeTo = \"A\";\n                        break;\n                    case \"sc\":\n                    case \"rr\":\n                        causeFrom = \"S\";\n                        causeTo = \"S\";\n                        break;\n                    default:\n                        break;\n                }\n\n            }\n            cdr.CauseTo = causeTo;\n            cdr.CauseFrom =causeFrom ;\n            cdr.IsRerouted = false;\n            cdrs.Add(cdr);\n\n            importedData.LastImportedId = cdr.IDonSwitch;\n\n            rowCount++;\n            if (rowCount == maximumBatchSize)\n                break;\n        }\n\n        long startingId;\n        Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(dataRecordVanriseType, rowCount, out startingId);\n        long currentCDRId = startingId;\n\n        foreach (var cdr in cdrs)\n        {\n            cdr.Id = currentCDRId;\n            currentCDRId++;\n        }\n\n\n        if (cdrs.Count > 0)\n        { \n\t\t\tvar batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, \"#RECORDSCOUNT# of Raw CDRs\", \"CDR\");\n            mappedBatches.Add(\"Distribute Raw CDRs Stage\", batch);\n        }\n        else\n            importedData.IsEmpty = true;\n\n        Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();\n        result.Result = Vanrise.Integration.Entities.MappingResult.Valid;\n        LogVerbose(\"Finished\");\n\n        return result;","ExecutionFlowId":"2799f9d0-8b71-47a1-861f-ab7c2850e8fc"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[AdapterID],[AdapterState],[TaskId],[Settings]))
merge	[integration].[DataSource] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[AdapterID] = s.[AdapterID],[AdapterState] = s.[AdapterState],[TaskId] = s.[TaskId],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[AdapterID],[AdapterState],[TaskId],[Settings])
	values(s.[ID],s.[Name],s.[AdapterID],s.[AdapterState],s.[TaskId],s.[Settings]);

--[runtime].[ScheduleTask]-------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([Id],[Name],[IsEnabled],[TaskType],[TriggerTypeId],[ActionTypeId],[TaskSettings],[OwnerId])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('3AD3A313-F0D6-46FD-9673-52511975223E','Data Source Task',0,0,'295B4FAC-DBF9-456F-855E-60D0B176F86B','B7CF41B9-F1B3-4C02-980D-B9FAFB4CFF68','{"$type":"Vanrise.Runtime.Entities.SchedulerTaskSettings, Vanrise.Runtime.Entities","TaskTriggerArgument":{"$type":"Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments.IntervalTimeTaskTriggerArgument, Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments","Interval":30.0,"IntervalType":2,"TimerTriggerTypeFQTN":"Vanrise.Runtime.Triggers.TimeTaskTrigger.IntervalTimeSchedulerTaskTrigger, Vanrise.Runtime.Triggers.TimeTaskTrigger","IgnoreSkippedIntervals":false},"StartEffDate":"2016-12-29T09:32:42.413"}',1),
('A1628B86-BFE5-4E0E-8746-BD396C02F7DC','Data Source Task',0,0,'295B4FAC-DBF9-456F-855E-60D0B176F86B','B7CF41B9-F1B3-4C02-980D-B9FAFB4CFF68','{"$type":"Vanrise.Runtime.Entities.SchedulerTaskSettings, Vanrise.Runtime.Entities","TaskTriggerArgument":{"$type":"Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments.IntervalTimeTaskTriggerArgument, Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments","Interval":30.0,"IntervalType":2,"TimerTriggerTypeFQTN":"Vanrise.Runtime.Triggers.TimeTaskTrigger.IntervalTimeSchedulerTaskTrigger, Vanrise.Runtime.Triggers.TimeTaskTrigger","IgnoreSkippedIntervals":false},"StartEffDate":"2017-01-12T13:04:00.138"}',1)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[IsEnabled],[TaskType],[TriggerTypeId],[ActionTypeId],[TaskSettings],[OwnerId]))
merge	[runtime].[ScheduleTask] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[IsEnabled] = s.[IsEnabled],[TaskType] = s.[TaskType],[TriggerTypeId] = s.[TriggerTypeId],[ActionTypeId] = s.[ActionTypeId],[TaskSettings] = s.[TaskSettings],[OwnerId] = s.[OwnerId]
when not matched by target then
	insert([Id],[Name],[IsEnabled],[TaskType],[TriggerTypeId],[ActionTypeId],[TaskSettings],[OwnerId])
	values(s.[Id],s.[Name],s.[IsEnabled],s.[TaskType],s.[TriggerTypeId],s.[ActionTypeId],s.[TaskSettings],s.[OwnerId]);

--[common].[VREventHandler]-------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Settings],[BED],[EED])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('511EE882-1221-4CFD-9FA8-B7116F0E480B','ReevaluateEndPointRouteStatusChangedHandler','{"$type":"Vanrise.Entities.VREventHandlerSettings, Vanrise.Entities","ExtendedSettings":{"$type":"TOne.WhS.BusinessEntity.Business.EventHandler.ReevaluateEndPointRouteStatusChangedHandler, TOne.WhS.BusinessEntity.Business","ConfigId":"1366F843-DFCF-460D-922D-B3799BA69F58"}}','2001-10-10 00:00:00.000',null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings],[BED],[EED]))
merge	[common].[VREventHandler] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings],[BED] = s.[BED],[EED] = s.[EED]
when not matched by target then
	insert([ID],[Name],[Settings],[BED],[EED])
	values(s.[ID],s.[Name],s.[Settings],s.[BED],s.[EED]);


--[sec].[Permission]--------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([HolderType],[HolderId],[EntityType],[EntityId],[PermissionFlags])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(0,'174',1,'19fc9866-775e-4114-8d00-6ff123e67e78','[{"Name":"View","Value":1},{"Name":"Add","Value":1},{"Name":"Edit","Value":1},{"Name":"Delete","Value":1},{"Name":"Block","Value":1},{"Name":"Activate","Value":1},{"Name":"InActivate","Value":1}]'),
(0,'174',1,'f88a6489-c43b-4e7e-b910-46b62e040ec5','[{"Name":"View","Value":1},{"Name":"Add","Value":1},{"Name":"Edit","Value":1},{"Name":"Block","Value":2},{"Name":"Activate","Value":2},{"Name":"InActivate","Value":1}]')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([HolderType],[HolderId],[EntityType],[EntityId],[PermissionFlags]))
merge	[sec].[Permission] as t
using	cte_data as s
on		1=1 and t.[HolderType] = s.[HolderType] and t.[HolderId] = s.[HolderId] and t.[EntityType] = s.[EntityType] and t.[EntityId] = s.[EntityId]
when matched then
	update set
	[PermissionFlags] = s.[PermissionFlags]
when not matched by target then
	insert([HolderType],[HolderId],[EntityType],[EntityId],[PermissionFlags])
	values(s.[HolderType],s.[HolderId],s.[EntityType],s.[EntityId],s.[PermissionFlags]);


	--[common].[StatusDefinition]-----------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[BusinessEntityDefinitionID],[Settings],[CreatedBy],[LastModifiedBy],[LastModifiedTime])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('03AA4967-66EA-4992-B9C9-3F014EFF79F2','Active','CACE00E6-31C2-4FE5-B7BB-636BA1757981','{"$type":"Vanrise.Entities.StatusDefinitionSettings, Vanrise.Entities","StyleDefinitionId":"1e644b07-528a-47b5-a40a-a9e8a0fc868a","HasInitialCharge":false,"HasRecurringCharge":false,"IsActive":true,"IsInvoiceActive":false,"IsAccountBalanceActive":false}',95,95,'2019-05-20 15:19:21.870'),
('736137E5-6E5A-4722-BA79-AAEF0CC5B07F','Block','CACE00E6-31C2-4FE5-B7BB-636BA1757981','{"$type":"Vanrise.Entities.StatusDefinitionSettings, Vanrise.Entities","StyleDefinitionId":"61a682f3-e00c-4b31-b2f5-26dd5f5e4c2f","HasInitialCharge":false,"HasRecurringCharge":false,"IsActive":false,"IsInvoiceActive":false,"IsAccountBalanceActive":false}',95,95,'2019-05-21 14:51:24.643'),
('B61884DE-DA86-4DE6-8630-DD67A1146B78','InActive','CACE00E6-31C2-4FE5-B7BB-636BA1757981','{"$type":"Vanrise.Entities.StatusDefinitionSettings, Vanrise.Entities","StyleDefinitionId":"fac30bbc-68b1-4e8e-b5de-93015285c012","HasInitialCharge":false,"HasRecurringCharge":false,"IsActive":false,"IsInvoiceActive":false,"IsAccountBalanceActive":false}',95,95,'2019-05-21 14:51:59.060')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[BusinessEntityDefinitionID],[Settings],[CreatedBy],[LastModifiedBy],[LastModifiedTime]))
merge	[common].[StatusDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[BusinessEntityDefinitionID] = s.[BusinessEntityDefinitionID],[Settings] = s.[Settings],[CreatedBy] = s.[CreatedBy],[LastModifiedBy] = s.[LastModifiedBy],[LastModifiedTime] = s.[LastModifiedTime]
when not matched by target then
	insert([ID],[Name],[BusinessEntityDefinitionID],[Settings],[CreatedBy],[LastModifiedBy],[LastModifiedTime])
	values(s.[ID],s.[Name],s.[BusinessEntityDefinitionID],s.[Settings],s.[CreatedBy],s.[LastModifiedBy],s.[LastModifiedTime]);

