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

--[Mediation_Generic].[MediationDefinition]---------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [Mediation_Generic].[MediationDefinition] on;
;with cte_data([ID],[Name],[Details])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Mediation Definition','{"$type":"Mediation.Generic.Entities.MediationDefinition, Mediation.Generic.Entities","MediationDefinitionId":1,"Name":"Mediation Definition","ParsedRecordTypeId":"1d7b5d3c-4ac6-42a3-a6e8-5a014e2edf94","CookedRecordTypeId":"eae69abe-1e94-460c-8b6e-77dbc7dab1d6","ParsedRecordIdentificationSetting":{"$type":"Mediation.Generic.Entities.ParsedRecordIdentificationSetting, Mediation.Generic.Entities","SessionIdField":"TC_CALLID","EventTimeField":"TC_TIMESTAMP","StatusMappings":{"$type":"System.Collections.Generic.List`1[[Mediation.Generic.Entities.StatusMapping, Mediation.Generic.Entities]], mscorlib","$values":[{"$type":"Mediation.Generic.Entities.StatusMapping, Mediation.Generic.Entities","Status":0,"FilterExpression":" TC_LOGTYPE  =  START ","FilterGroup":{"$type":"Vanrise.GenericData.Entities.RecordFilterGroup, Vanrise.GenericData.Entities","LogicalOperator":0,"Filters":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.RecordFilter, Vanrise.GenericData.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Entities.StringRecordFilter, Vanrise.GenericData.Entities","CompareOperator":0,"Value":"START","FieldName":"TC_LOGTYPE"}]}}},{"$type":"Mediation.Generic.Entities.StatusMapping, Mediation.Generic.Entities","Status":2,"FilterExpression":" TC_LOGTYPE  =  STOP  OR  TC_LOGTYPE  =  INTERIM ","FilterGroup":{"$type":"Vanrise.GenericData.Entities.RecordFilterGroup, Vanrise.GenericData.Entities","LogicalOperator":1,"Filters":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.RecordFilter, Vanrise.GenericData.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Entities.StringRecordFilter, Vanrise.GenericData.Entities","CompareOperator":0,"Value":"STOP","FieldName":"TC_LOGTYPE"},{"$type":"Vanrise.GenericData.Entities.StringRecordFilter, Vanrise.GenericData.Entities","CompareOperator":0,"Value":"INTERIM","FieldName":"TC_LOGTYPE"}]}}},{"$type":"Mediation.Generic.Entities.StatusMapping, Mediation.Generic.Entities","Status":1}]}},"CookedFromParsedSettings":{"$type":"Mediation.Generic.Entities.UpdateCookedFromParsed, Mediation.Generic.Entities","TransformationDefinitionId":"c8b4bc22-65b7-447d-aca0-633f14c175d4","ParsedRecordName":"parsedCDRs","CookedRecordName":"cookedCDR"},"CookedCDRDataStoreSetting":{"$type":"Mediation.Generic.Entities.CookedCDRDataStoreSetting, Mediation.Generic.Entities","DataRecordStorageId":"0b1837df-c8ce-4b2a-b07e-1a9f75408741"}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Details]))
merge	[Mediation_Generic].[MediationDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Details] = s.[Details]
when not matched by target then
	insert([ID],[Name],[Details])
	values(s.[ID],s.[Name],s.[Details]);
set identity_insert [Mediation_Generic].[MediationDefinition] off;

