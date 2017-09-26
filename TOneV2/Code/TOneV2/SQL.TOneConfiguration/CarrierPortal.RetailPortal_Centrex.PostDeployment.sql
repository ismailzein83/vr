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



--[genericdata].[BusinessEntityDefinition]----------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Title],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('C4BA92AF-4E77-4F36-9D78-7D715EDA8A74','Remote_BE_InterconnectOperator','Interconnect Operator','{"$type":"Vanrise.GenericData.Business.VRRestAPIBEDefinitionSettings, Vanrise.GenericData.Business","ConfigId":"b4f22ffb-b663-4f5f-af53-acbef7224dfb","IdType":"System.Int64","DefinitionEditor":"vr-genericdata-restapibedefinitions-editor","SelectorUIControl":"vr-genericdata-businessentity-remoteselector","ManagerFQTN":"Vanrise.GenericData.Business.VRRestAPIBusinessEntityManager, Vanrise.GenericData.Business","ConnectionId":"7a2044f4-b42c-44aa-bfb8-6538904e8e4c","RemoteBEDefinitionId":"a5c1852b-2c92-4d66-b959-e3f49304338a","SingularTitle":"Interconnect Operator","PluralTitle":"Interconnect Operators"}')
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



--[genericdata].[datarecordtype]--------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[ParentID],[Fields],[ExtraFieldsEvaluator],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('20946C89-91D2-4895-99F4-E8FF739C0CFF','BillingCDR',null,'{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"CDRID","Title":"ID","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","DataPrecision":0,"DataType":2,"IsNullable":false,"OrderType":0}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"AttemptDateTime","Title":"Attempt Date Time","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldDateTimeType, Vanrise.GenericData.MainExtensions","ConfigId":"b8712417-83ab-4d4b-9ee1-109d20ceb909","DataType":0,"IsNullable":false,"OrderType":0}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"ConnectDateTime","Title":"Connect Date Time","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldDateTimeType, Vanrise.GenericData.MainExtensions","ConfigId":"b8712417-83ab-4d4b-9ee1-109d20ceb909","DataType":0,"IsNullable":false,"OrderType":0}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"DisconnectDateTime","Title":"Disconnect Date Time","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldDateTimeType, Vanrise.GenericData.MainExtensions","ConfigId":"b8712417-83ab-4d4b-9ee1-109d20ceb909","DataType":0,"IsNullable":false,"OrderType":0}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"DurationInSeconds","Title":"Duration In Seconds","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","DataPrecision":0,"DataType":0,"IsNullable":false,"OrderType":0}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"DisconnectReason","Title":"Disconnect Reason","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"CallProgressState","Title":"Call Progress State","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"ServiceType","Title":"Service Type","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","BusinessEntityDefinitionId":"8e5eaedf-f4f0-49fa-ab46-306c8044317d","IsNullable":false,"OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TrafficDirection","Title":"Traffic Direction","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldChoicesType, Vanrise.GenericData.MainExtensions","ConfigId":"eabc41a9-e332-4120-ac85-f0b7e53c0d0d","Choices":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.MainExtensions.DataRecordFields.Choice, Vanrise.GenericData.MainExtensions]], mscorlib","$values":[{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.Choice, Vanrise.GenericData.MainExtensions","Value":1,"Text":"In"},{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.Choice, Vanrise.GenericData.MainExtensions","Value":2,"Text":"Out"}]},"IsNullable":false,"OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"Calling","Title":"Calling","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"Called","Title":"Called","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"OtherPartyNumber","Title":"Other Party Number","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"InterconnectOperator","Title":"Interconnect Operator","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","BusinessEntityDefinitionId":"c4ba92af-4e77-4f36-9d78-7d715eda8a74","IsNullable":false,"OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"Zone","Title":"Zone","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","BusinessEntityDefinitionId":"48181232-9b13-4f76-8ba6-19ac19753c64","IsNullable":false,"OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"PackageId","Title":"Package","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","BusinessEntityDefinitionId":"5f3c5c17-2d17-46eb-b7f2-dd44e979dbf9","IsNullable":false,"OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"ChargingPolicyId","Title":"Charging Policy","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","BusinessEntityDefinitionId":"56287cbb-376b-42f3-9ba0-04ffbe609a70","IsNullable":false,"OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"SaleRate","Title":"Sale Rate","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","DataPrecision":1,"DataType":0,"IsNullable":false,"OrderType":0}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"SaleAmount","Title":"Sale Amount","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","DataPrecision":0,"DataType":0,"IsNullable":false,"OrderType":0}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"SaleRateTypeId","Title":"Sale Rate Type","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","DataPrecision":0,"DataType":1,"IsNullable":false,"OrderType":0}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"SaleCurrencyId","Title":"Sale Currency","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","BusinessEntityDefinitionId":"aeb7bdc9-8a66-4297-8a11-0865515df4e6","IsNullable":false,"OrderType":1}}]}',null, '{"$type":"Vanrise.GenericData.Entities.DataRecordTypeSettings, Vanrise.GenericData.Entities"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[ParentID],[Fields],[ExtraFieldsEvaluator],[Settings]))
merge	[genericdata].[datarecordtype] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[ParentID] = s.[ParentID],[Fields] = s.[Fields],[ExtraFieldsEvaluator] = s.[ExtraFieldsEvaluator],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[ParentID],[Fields],[ExtraFieldsEvaluator],[Settings])
	values(s.[ID],s.[Name],s.[ParentID],s.[Fields],s.[ExtraFieldsEvaluator],s.[Settings]);



--[genericdata].[datarecordstorage]-----------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[DataRecordTypeID],[DataStoreID],[Settings],[State])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('59BD3A0D-2BA1-4263-B301-4779E0D4D0A2','CDR Table','20946C89-91D2-4895-99F4-E8FF739C0CFF','7403534B-628F-4114-B1C9-C9EDAB294219','{"$type":"Vanrise.GenericData.MainExtensions.DataStorages.DataRecordStorage.VRRestAPIDataRecordStorageSettings, Vanrise.GenericData.MainExtensions","RemoteDataRecordTypeId":"d818c097-822d-4ea5-ae22-f7d208154a78","RemoteDataRecordStorageIds":{"$type":"System.Collections.Generic.List`1[[System.Guid, mscorlib]], mscorlib","$values":["5cd31703-3bc6-41eb-b204-ef473cb394e4","df2e0c11-750b-4c9b-b232-829421f1187d"]},"VRRestAPIRecordQueryInterceptor":{"$type":"PartnerPortal.CustomerAccess.MainExtensions.VRRestAPIRecordQueryInterceptor.RetailAccountVRRestAPIRecordQueryInterceptor, PartnerPortal.CustomerAccess.MainExtensions","ConfigId":"b3a94a20-92ed-47bf-86d6-1034b720be73","AccountFieldName":"FinancialAccountId"},"DateTimeField":"ConnectDateTime","RequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}}}',null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[DataRecordTypeID],[DataStoreID],[Settings],[State]))
merge	[genericdata].[datarecordstorage] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[DataRecordTypeID] = s.[DataRecordTypeID],[DataStoreID] = s.[DataStoreID],[Settings] = s.[Settings],[State] = s.[State]
when not matched by target then
	insert([ID],[Name],[DataRecordTypeID],[DataStoreID],[Settings],[State])
	values(s.[ID],s.[Name],s.[DataRecordTypeID],s.[DataStoreID],s.[Settings],s.[State]);



--[Analytic].[AnalyticReport]-----------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[UserID],[Name],[AccessType],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('EBEB4ECA-8A9D-4A39-A424-EC841C3C7288',1,'CDR Log',0,'{"$type":"Vanrise.Analytic.Entities.DataRecordSearchPageSettings, Vanrise.Analytic.Entities","ConfigId":"82aa89f6-4d19-4168-a499-cdd2875f1702","Sources":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.DRSearchPageStorageSource, Vanrise.Analytic.Entities]], mscorlib","$values":[{"$type":"Vanrise.Analytic.Entities.DRSearchPageStorageSource, Vanrise.Analytic.Entities","Title":"CDRs","Name":"MainCDRs","DataRecordTypeId":"20946c89-91d2-4895-99f4-e8ff739c0cff","RecordStorageIds":{"$type":"System.Collections.Generic.List`1[[System.Guid, mscorlib]], mscorlib","$values":["59bd3a0d-2ba1-4263-b301-4779e0d4d0a2"]},"GridColumns":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.DRSearchPageGridColumn, Vanrise.Analytic.Entities]], mscorlib","$values":[{"$type":"Vanrise.Analytic.Entities.DRSearchPageGridColumn, Vanrise.Analytic.Entities","FieldName":"CDRID","FieldTitle":"ID","ColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Normal"}},{"$type":"Vanrise.Analytic.Entities.DRSearchPageGridColumn, Vanrise.Analytic.Entities","FieldName":"ConnectDateTime","FieldTitle":"Connect","ColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Normal"}},{"$type":"Vanrise.Analytic.Entities.DRSearchPageGridColumn, Vanrise.Analytic.Entities","FieldName":"DisconnectDateTime","FieldTitle":"Disconnect","ColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Normal"}},{"$type":"Vanrise.Analytic.Entities.DRSearchPageGridColumn, Vanrise.Analytic.Entities","FieldName":"ServiceType","FieldTitle":"Service Type","ColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Normal"}},{"$type":"Vanrise.Analytic.Entities.DRSearchPageGridColumn, Vanrise.Analytic.Entities","FieldName":"TrafficDirection","FieldTitle":"Direction","ColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Normal"}},{"$type":"Vanrise.Analytic.Entities.DRSearchPageGridColumn, Vanrise.Analytic.Entities","FieldName":"DurationInSeconds","FieldTitle":"Duration (sec)","ColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Normal"}},{"$type":"Vanrise.Analytic.Entities.DRSearchPageGridColumn, Vanrise.Analytic.Entities","FieldName":"Calling","FieldTitle":"Calling","ColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Normal"}},{"$type":"Vanrise.Analytic.Entities.DRSearchPageGridColumn, Vanrise.Analytic.Entities","FieldName":"Called","FieldTitle":"Called","ColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Normal"}},{"$type":"Vanrise.Analytic.Entities.DRSearchPageGridColumn, Vanrise.Analytic.Entities","FieldName":"Zone","FieldTitle":"Zone","ColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Normal"}},{"$type":"Vanrise.Analytic.Entities.DRSearchPageGridColumn, Vanrise.Analytic.Entities","FieldName":"InterconnectOperator","FieldTitle":"Operator","ColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Normal"}},{"$type":"Vanrise.Analytic.Entities.DRSearchPageGridColumn, Vanrise.Analytic.Entities","FieldName":"PackageId","FieldTitle":"Package","ColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Normal"}},{"$type":"Vanrise.Analytic.Entities.DRSearchPageGridColumn, Vanrise.Analytic.Entities","FieldName":"SaleRate","FieldTitle":"Sale Rate","ColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Normal"}},{"$type":"Vanrise.Analytic.Entities.DRSearchPageGridColumn, Vanrise.Analytic.Entities","FieldName":"SaleAmount","FieldTitle":"Sale Amount","ColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Normal"}},{"$type":"Vanrise.Analytic.Entities.DRSearchPageGridColumn, Vanrise.Analytic.Entities","FieldName":"SaleCurrencyId","FieldTitle":"Sale Currency","ColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Normal"}}]},"ItemDetails":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.DRSearchPageItemDetail, Vanrise.Analytic.Entities]], mscorlib","$values":[{"$type":"Vanrise.Analytic.Entities.DRSearchPageItemDetail, Vanrise.Analytic.Entities","FieldName":"CDRID","FieldTitle":"ID","ColumnWidth":1},{"$type":"Vanrise.Analytic.Entities.DRSearchPageItemDetail, Vanrise.Analytic.Entities","FieldName":"DisconnectReason","FieldTitle":"Disconnect Reason","ColumnWidth":1},{"$type":"Vanrise.Analytic.Entities.DRSearchPageItemDetail, Vanrise.Analytic.Entities","FieldName":"CallProgressState","FieldTitle":"Call Progress State","ColumnWidth":1}]},"SortColumns":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.DRSearchPageSortColumn, Vanrise.Analytic.Entities]], mscorlib","$values":[{"$type":"Vanrise.Analytic.Entities.DRSearchPageSortColumn, Vanrise.Analytic.Entities","FieldName":"CDRID","IsDescending":false}]}}]},"NumberOfRecords":100}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[UserID],[Name],[AccessType],[Settings]))
merge	[Analytic].[AnalyticReport] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[UserID] = s.[UserID],[Name] = s.[Name],[AccessType] = s.[AccessType],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[UserID],[Name],[AccessType],[Settings])
	values(s.[ID],s.[UserID],s.[Name],s.[AccessType],s.[Settings]);



--[sec].[View]--------------------------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank],[IsDeleted])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('91164CB8-B3F3-4CE6-9B62-183AE3EE79CF','CDR Log','CDR Log',null					,'6471DA6F-E4DD-4B2A-BFB6-F8EA498CD37C',null,null,null,'{"$type":"Vanrise.Analytic.Entities.AnalyticReportViewSettings, Vanrise.Analytic.Entities","TypeId":"82aa89f6-4d19-4168-a499-cdd2875f1702","AnalyticReportId":"ebeb4eca-8a9d-4a39-a424-ec841c3c7288"}','82FF3B8A-0C39-4376-9602-B84A240FBF82',100,0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank],[IsDeleted]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank],[IsDeleted] = s.[IsDeleted]
when not matched by target then
	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank],[IsDeleted])
	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank],s.[IsDeleted]);
----------------------------------------------------------------------------------------------------
END
