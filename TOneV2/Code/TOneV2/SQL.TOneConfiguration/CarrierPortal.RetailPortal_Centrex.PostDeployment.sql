﻿--[genericdata].[datastore]-------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[OldID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('7403534B-628F-4114-B1C9-C9EDAB294219',null,'VR Rest API','{"$type":"Vanrise.GenericData.MainExtensions.DataStorages.DataStore.VRRestAPIDataStoreSettings, Vanrise.GenericData.MainExtensions","ConfigId":"4829119d-f86f-4a6c-a6c0-cdb3fc8274c1","ConnectionId":"7a2044f4-b42c-44aa-bfb8-6538904e8e4c","IsRemoteDataStore":true}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[OldID],[Name],[Settings]))
merge	[genericdata].[datastore] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[OldID] = s.[OldID],[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[OldID],[Name],[Settings])
	values(s.[ID],s.[OldID],s.[Name],s.[Settings]);


--[genericdata].[datarecordstorage]-----------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[OldID],[Name],[OldDataRecordTypeID],[DataRecordTypeID],[DataStoreID],[OldDataStoreID],[Settings],[State])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('59BD3A0D-2BA1-4263-B301-4779E0D4D0A2',null,'CDR Table',null,'20946C89-91D2-4895-99F4-E8FF739C0CFF','7403534B-628F-4114-B1C9-C9EDAB294219',null,'{"$type":"Vanrise.GenericData.MainExtensions.DataStorages.DataRecordStorage.VRRestAPIDataRecordStorageSettings, Vanrise.GenericData.MainExtensions","RemoteDataRecordTypeId":"d818c097-822d-4ea5-ae22-f7d208154a78","RemoteDataRecordStorageIds":{"$type":"System.Collections.Generic.List`1[[System.Guid, mscorlib]], mscorlib","$values":["5cd31703-3bc6-41eb-b204-ef473cb394e4","01811e55-6f13-42db-92b7-589aa6ee4b75"]},"VRRestAPIRecordQueryInterceptor":{"$type":"PartnerPortal.CustomerAccess.MainExtensions.VRRestAPIRecordQueryInterceptor.RetailAccountVRRestAPIRecordQueryInterceptor, PartnerPortal.CustomerAccess.MainExtensions","ConfigId":"b3a94a20-92ed-47bf-86d6-1034b720be73"},"DateTimeField":"ConnectDateTime","RequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}}}',null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[OldID],[Name],[OldDataRecordTypeID],[DataRecordTypeID],[DataStoreID],[OldDataStoreID],[Settings],[State]))
merge	[genericdata].[datarecordstorage] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[OldID] = s.[OldID],[Name] = s.[Name],[OldDataRecordTypeID] = s.[OldDataRecordTypeID],[DataRecordTypeID] = s.[DataRecordTypeID],[DataStoreID] = s.[DataStoreID],[OldDataStoreID] = s.[OldDataStoreID],[Settings] = s.[Settings],[State] = s.[State]
when not matched by target then
	insert([ID],[OldID],[Name],[OldDataRecordTypeID],[DataRecordTypeID],[DataStoreID],[OldDataStoreID],[Settings],[State])
	values(s.[ID],s.[OldID],s.[Name],s.[OldDataRecordTypeID],s.[DataRecordTypeID],s.[DataStoreID],s.[OldDataStoreID],s.[Settings],s.[State]);


--[genericdata].[datarecordtype]--------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[OldID],[Name],[ParentID],[OldParentID],[Fields],[ExtraFieldsEvaluator])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('20946C89-91D2-4895-99F4-E8FF739C0CFF',null,'BillingCDR',null,null,'{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"CDRID","Title":"ID","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","DataPrecision":0,"DataType":2,"IsNullable":false,"OrderType":0}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"IDonSwitch","Title":"ID on Switch","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"AttemptDateTime","Title":"Attempt Date Time","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldDateTimeType, Vanrise.GenericData.MainExtensions","ConfigId":"b8712417-83ab-4d4b-9ee1-109d20ceb909","DataType":0,"IsNullable":false,"OrderType":0}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"ConnectDateTime","Title":"Connect Date Time","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldDateTimeType, Vanrise.GenericData.MainExtensions","ConfigId":"b8712417-83ab-4d4b-9ee1-109d20ceb909","DataType":0,"IsNullable":false,"OrderType":0}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"DisconnectDateTime","Title":"Disconnect Date Time","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldDateTimeType, Vanrise.GenericData.MainExtensions","ConfigId":"b8712417-83ab-4d4b-9ee1-109d20ceb909","DataType":0,"IsNullable":false,"OrderType":0}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"DurationInSeconds","Title":"Duration In Seconds","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","DataPrecision":0,"DataType":0,"IsNullable":false,"OrderType":0}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"DisconnectReason","Title":"Disconnect Reason","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"CallProgressState","Title":"Call Progress State","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"ServiceType","Title":"Service Type","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","BusinessEntityDefinitionId":"8e5eaedf-f4f0-49fa-ab46-306c8044317d","IsNullable":false,"OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TrafficDirection","Title":"Traffic Direction","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldChoicesType, Vanrise.GenericData.MainExtensions","ConfigId":"eabc41a9-e332-4120-ac85-f0b7e53c0d0d","Choices":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.MainExtensions.DataRecordFields.Choice, Vanrise.GenericData.MainExtensions]], mscorlib","$values":[{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.Choice, Vanrise.GenericData.MainExtensions","Value":1,"Text":"In"},{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.Choice, Vanrise.GenericData.MainExtensions","Value":2,"Text":"Out"}]},"IsNullable":false,"OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"Calling","Title":"Calling","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"Called","Title":"Called","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"OtherPartyNumber","Title":"Other Party Number","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"InterconnectOperator","Title":"Interconnect Operator","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","BusinessEntityDefinitionId":"c4ba92af-4e77-4f36-9d78-7d715eda8a74","IsNullable":false,"OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"Zone","Title":"Zone","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","BusinessEntityDefinitionId":"48181232-9b13-4f76-8ba6-19ac19753c64","IsNullable":false,"OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"PackageId","Title":"Package","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","BusinessEntityDefinitionId":"5f3c5c17-2d17-46eb-b7f2-dd44e979dbf9","IsNullable":false,"OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"ChargingPolicyId","Title":"Charging Policy","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","BusinessEntityDefinitionId":"56287cbb-376b-42f3-9ba0-04ffbe609a70","IsNullable":false,"OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"Rate","Title":"Rate","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","DataPrecision":1,"DataType":0,"IsNullable":false,"OrderType":0}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"Amount","Title":"Amount","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","DataPrecision":0,"DataType":0,"IsNullable":false,"OrderType":0}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"RateTypeId","Title":"Rate Type","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","DataPrecision":0,"DataType":1,"IsNullable":false,"OrderType":0}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"CurrencyId","Title":"Currency","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","BusinessEntityDefinitionId":"aeb7bdc9-8a66-4297-8a11-0865515df4e6","IsNullable":false,"OrderType":1}}]}',null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[OldID],[Name],[ParentID],[OldParentID],[Fields],[ExtraFieldsEvaluator]))
merge	[genericdata].[datarecordtype] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[OldID] = s.[OldID],[Name] = s.[Name],[ParentID] = s.[ParentID],[OldParentID] = s.[OldParentID],[Fields] = s.[Fields],[ExtraFieldsEvaluator] = s.[ExtraFieldsEvaluator]
when not matched by target then
	insert([ID],[OldID],[Name],[ParentID],[OldParentID],[Fields],[ExtraFieldsEvaluator])
	values(s.[ID],s.[OldID],s.[Name],s.[ParentID],s.[OldParentID],s.[Fields],s.[ExtraFieldsEvaluator]);


--[genericdata].[BusinessEntityDefinition]----------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[OldID],[Name],[Title],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('56287CBB-376B-42F3-9BA0-04FFBE609A70',null,'Remote_BE_ChargingPolicy','Charging Policy','{"$type":"Vanrise.GenericData.Business.VRRestAPIBEDefinitionSettings, Vanrise.GenericData.Business","ConfigId":"b4f22ffb-b663-4f5f-af53-acbef7224dfb","IdType":"System.Int32","DefinitionEditor":"vr-genericdata-restapibedefinitions-editor","SelectorUIControl":"vr-genericdata-businessentity-remoteselector","ManagerFQTN":"Vanrise.GenericData.Business.VRRestAPIBusinessEntityManager, Vanrise.GenericData.Business","ConnectionId":"7a2044f4-b42c-44aa-bfb8-6538904e8e4c","RemoteBEDefinitionId":"d0ee9bf8-385e-48ef-b989-a87666a00072","SingularTitle":"Charging Policy","PluralTitle":"Charging Policies"}'),
('AEB7BDC9-8A66-4297-8A11-0865515DF4E6',null,'Remote_BE_Currency','Currency','{"$type":"Vanrise.GenericData.Business.VRRestAPIBEDefinitionSettings, Vanrise.GenericData.Business","ConfigId":"b4f22ffb-b663-4f5f-af53-acbef7224dfb","IdType":"System.Int32","DefinitionEditor":"vr-genericdata-restapibedefinitions-editor","SelectorUIControl":"vr-genericdata-businessentity-remoteselector","ManagerFQTN":"Vanrise.GenericData.Business.VRRestAPIBusinessEntityManager, Vanrise.GenericData.Business","ConnectionId":"7a2044f4-b42c-44aa-bfb8-6538904e8e4c","RemoteBEDefinitionId":"d41ea344-c3c0-4203-8583-019b6b3edb76","SingularTitle":"Currency","PluralTitle":"Currencies"}'),
('48181232-9B13-4F76-8BA6-19AC19753C64',null,'Remote_BE_Zone','Zone','{"$type":"Vanrise.GenericData.Business.VRRestAPIBEDefinitionSettings, Vanrise.GenericData.Business","ConfigId":"b4f22ffb-b663-4f5f-af53-acbef7224dfb","IdType":"System.Int64","DefinitionEditor":"vr-genericdata-restapibedefinitions-editor","SelectorUIControl":"vr-genericdata-businessentity-remoteselector","ManagerFQTN":"Vanrise.GenericData.Business.VRRestAPIBusinessEntityManager, Vanrise.GenericData.Business","ConnectionId":"7a2044f4-b42c-44aa-bfb8-6538904e8e4c","RemoteBEDefinitionId":"10740f30-5a20-4718-b5af-0e2e160b21c2","SingularTitle":"Zone","PluralTitle":"Zones"}'),
('8E5EAEDF-F4F0-49FA-AB46-306C8044317D',null,'Remote_BE_ServiceType','Service Type','{"$type":"Vanrise.GenericData.Business.VRRestAPIBEDefinitionSettings, Vanrise.GenericData.Business","ConfigId":"b4f22ffb-b663-4f5f-af53-acbef7224dfb","IdType":"System.Guid","DefinitionEditor":"vr-genericdata-restapibedefinitions-editor","SelectorUIControl":"vr-genericdata-businessentity-remoteselector","ManagerFQTN":"Vanrise.GenericData.Business.VRRestAPIBusinessEntityManager, Vanrise.GenericData.Business","ConnectionId":"7a2044f4-b42c-44aa-bfb8-6538904e8e4c","RemoteBEDefinitionId":"bfad446f-7129-45b1-94bf-febd290f394d","SingularTitle":"Service Type","PluralTitle":"Service Types"}'),
('C4BA92AF-4E77-4F36-9D78-7D715EDA8A74',null,'Remote_BE_InterconnectOperator','Interconnect Operator','{"$type":"Vanrise.GenericData.Business.VRRestAPIBEDefinitionSettings, Vanrise.GenericData.Business","ConfigId":"b4f22ffb-b663-4f5f-af53-acbef7224dfb","IdType":"System.Int64","DefinitionEditor":"vr-genericdata-restapibedefinitions-editor","SelectorUIControl":"vr-genericdata-businessentity-remoteselector","ManagerFQTN":"Vanrise.GenericData.Business.VRRestAPIBusinessEntityManager, Vanrise.GenericData.Business","ConnectionId":"7a2044f4-b42c-44aa-bfb8-6538904e8e4c","RemoteBEDefinitionId":"a5c1852b-2c92-4d66-b959-e3f49304338a","SingularTitle":"Interconnect Operator","PluralTitle":"Interconnect Operators"}'),
('5F3C5C17-2D17-46EB-B7F2-DD44E979DBF9',null,'Remote_BE_Package','Package','{"$type":"Vanrise.GenericData.Business.VRRestAPIBEDefinitionSettings, Vanrise.GenericData.Business","ConfigId":"b4f22ffb-b663-4f5f-af53-acbef7224dfb","IdType":"System.Int32","DefinitionEditor":"vr-genericdata-restapibedefinitions-editor","SelectorUIControl":"vr-genericdata-businessentity-remoteselector","ManagerFQTN":"Vanrise.GenericData.Business.VRRestAPIBusinessEntityManager, Vanrise.GenericData.Business","ConnectionId":"7a2044f4-b42c-44aa-bfb8-6538904e8e4c","RemoteBEDefinitionId":"c0c76db1-4876-4e0d-9b59-ca89120e6be9","SingularTitle":"Package","PluralTitle":"Packages"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[OldID],[Name],[Title],[Settings]))
merge	[genericdata].[BusinessEntityDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[OldID] = s.[OldID],[Name] = s.[Name],[Title] = s.[Title],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[OldID],[Name],[Title],[Settings])
	values(s.[ID],s.[OldID],s.[Name],s.[Title],s.[Settings]);


--[Analytic].[AnalyticReport]-----------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[OldID],[UserID],[Name],[AccessType],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('EBEB4ECA-8A9D-4A39-A424-EC841C3C7288',null,1,'CDR Log',0,'{"$type":"Vanrise.Analytic.Entities.DataRecordSearchPageSettings, Vanrise.Analytic.Entities","ConfigId":"82aa89f6-4d19-4168-a499-cdd2875f1702","Sources":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.DRSearchPageStorageSource, Vanrise.Analytic.Entities]], mscorlib","$values":[{"$type":"Vanrise.Analytic.Entities.DRSearchPageStorageSource, Vanrise.Analytic.Entities","Title":"CDRs","Name":"MainCDRs","DataRecordTypeId":"20946c89-91d2-4895-99f4-e8ff739c0cff","RecordStorageIds":{"$type":"System.Collections.Generic.List`1[[System.Guid, mscorlib]], mscorlib","$values":["59bd3a0d-2ba1-4263-b301-4779e0d4d0a2"]},"GridColumns":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.DRSearchPageGridColumn, Vanrise.Analytic.Entities]], mscorlib","$values":[{"$type":"Vanrise.Analytic.Entities.DRSearchPageGridColumn, Vanrise.Analytic.Entities","FieldName":"CDRID","FieldTitle":"ID","ColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Normal"}},{"$type":"Vanrise.Analytic.Entities.DRSearchPageGridColumn, Vanrise.Analytic.Entities","FieldName":"ServiceType","FieldTitle":"Service Type","ColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Normal"}},{"$type":"Vanrise.Analytic.Entities.DRSearchPageGridColumn, Vanrise.Analytic.Entities","FieldName":"TrafficDirection","FieldTitle":"Direction","ColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Normal"}},{"$type":"Vanrise.Analytic.Entities.DRSearchPageGridColumn, Vanrise.Analytic.Entities","FieldName":"ConnectDateTime","FieldTitle":"Connect","ColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Normal"}},{"$type":"Vanrise.Analytic.Entities.DRSearchPageGridColumn, Vanrise.Analytic.Entities","FieldName":"DisconnectDateTime","FieldTitle":"Disconnect","ColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Normal"}},{"$type":"Vanrise.Analytic.Entities.DRSearchPageGridColumn, Vanrise.Analytic.Entities","FieldName":"DurationInSeconds","FieldTitle":"Duration (sec)","ColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Normal"}},{"$type":"Vanrise.Analytic.Entities.DRSearchPageGridColumn, Vanrise.Analytic.Entities","FieldName":"Calling","FieldTitle":"Calling","ColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Normal"}},{"$type":"Vanrise.Analytic.Entities.DRSearchPageGridColumn, Vanrise.Analytic.Entities","FieldName":"Called","FieldTitle":"Called","ColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Normal"}},{"$type":"Vanrise.Analytic.Entities.DRSearchPageGridColumn, Vanrise.Analytic.Entities","FieldName":"Zone","FieldTitle":"Zone","ColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Normal"}},{"$type":"Vanrise.Analytic.Entities.DRSearchPageGridColumn, Vanrise.Analytic.Entities","FieldName":"InterconnectOperator","FieldTitle":"Operator","ColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Normal"}},{"$type":"Vanrise.Analytic.Entities.DRSearchPageGridColumn, Vanrise.Analytic.Entities","FieldName":"PackageId","FieldTitle":"Package","ColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Normal"}},{"$type":"Vanrise.Analytic.Entities.DRSearchPageGridColumn, Vanrise.Analytic.Entities","FieldName":"Rate","FieldTitle":"Rate","ColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Normal"}},{"$type":"Vanrise.Analytic.Entities.DRSearchPageGridColumn, Vanrise.Analytic.Entities","FieldName":"Amount","FieldTitle":"Amount","ColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Normal"}},{"$type":"Vanrise.Analytic.Entities.DRSearchPageGridColumn, Vanrise.Analytic.Entities","FieldName":"RateTypeId","FieldTitle":"Rate Type","ColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Normal"}},{"$type":"Vanrise.Analytic.Entities.DRSearchPageGridColumn, Vanrise.Analytic.Entities","FieldName":"CurrencyId","FieldTitle":"Currency","ColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Normal"}}]},"ItemDetails":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.DRSearchPageItemDetail, Vanrise.Analytic.Entities]], mscorlib","$values":[{"$type":"Vanrise.Analytic.Entities.DRSearchPageItemDetail, Vanrise.Analytic.Entities","FieldName":"CDRID","FieldTitle":"ID","ColumnWidth":1},{"$type":"Vanrise.Analytic.Entities.DRSearchPageItemDetail, Vanrise.Analytic.Entities","FieldName":"IDonSwitch","FieldTitle":"ID on Switch","ColumnWidth":1},{"$type":"Vanrise.Analytic.Entities.DRSearchPageItemDetail, Vanrise.Analytic.Entities","FieldName":"DisconnectReason","FieldTitle":"Disconnect Reason","ColumnWidth":1},{"$type":"Vanrise.Analytic.Entities.DRSearchPageItemDetail, Vanrise.Analytic.Entities","FieldName":"CallProgressState","FieldTitle":"Call Progress State","ColumnWidth":1}]},"SortColumns":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.DRSearchPageSortColumn, Vanrise.Analytic.Entities]], mscorlib","$values":[{"$type":"Vanrise.Analytic.Entities.DRSearchPageSortColumn, Vanrise.Analytic.Entities","FieldName":"CDRID","IsDescending":false}]}}]},"NumberOfRecords":100}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[OldID],[UserID],[Name],[AccessType],[Settings]))
merge	[Analytic].[AnalyticReport] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[OldID] = s.[OldID],[UserID] = s.[UserID],[Name] = s.[Name],[AccessType] = s.[AccessType],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[OldID],[UserID],[Name],[AccessType],[Settings])
	values(s.[ID],s.[OldID],s.[UserID],s.[Name],s.[AccessType],s.[Settings]);

--[sec].[Module]------------------------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('6471DA6F-E4DD-4B2A-BFB6-F8EA498CD37C','Billing'				,null,null,'/images/menu-icons/billing.png',9,0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic]))
merge	[sec].[Module] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Url] = s.[Url],[ParentId] = s.[ParentId],[Icon] = s.[Icon],[Rank] = s.[Rank],[AllowDynamic] = s.[AllowDynamic]
when not matched by target then
	insert([ID],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
	values(s.[ID],s.[Name],s.[Url],s.[ParentId],s.[Icon],s.[Rank],s.[AllowDynamic]);
----------------------------------------------------------------------------------------------------
END

--[sec].[View]--------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank],[IsDeleted])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('ACFDFC22-FE0B-491E-B5D1-0A9541B822FE','Account Statement','Account Statement',null,'6471DA6F-E4DD-4B2A-BFB6-F8EA498CD37C',null,null,null,'{"$type":"PartnerPortal.CustomerAccess.Entities.AccountStatementViewSettings, PartnerPortal.CustomerAccess.Entities","AccountStatementViewData":{"$type":"PartnerPortal.CustomerAccess.Entities.AccountStatementViewData, PartnerPortal.CustomerAccess.Entities","VRConnectionId":"7a2044f4-b42c-44aa-bfb8-6538904e8e4c","AccountTypeId":"20b0c83e-6f53-49c7-b52f-828a19e6dc2a","AccountStatementHandler":{"$type":"PartnerPortal.CustomerAccess.MainExtensions.AccountStatement.RetailAccountUser, PartnerPortal.CustomerAccess.MainExtensions","ConfigId":"a8085279-37bf-40c5-941b-a1e46f83dfab"}}}','A0709FCC-0344-4B64-BC0D-50471D052D0F',null,null,null),
('91164CB8-B3F3-4CE6-9B62-183AE3EE79CF','CDR Log','CDR Log',null,'6471DA6F-E4DD-4B2A-BFB6-F8EA498CD37C',null,null,null,'{"$type":"Vanrise.Analytic.Entities.AnalyticReportViewSettings, Vanrise.Analytic.Entities","TypeId":"82aa89f6-4d19-4168-a499-cdd2875f1702","AnalyticReportId":"ebeb4eca-8a9d-4a39-a424-ec841c3c7288"}','82FF3B8A-0C39-4376-9602-B84A240FBF82',null,null,null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank],[IsDeleted]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[OldType] = s.[OldType],[Rank] = s.[Rank],[IsDeleted] = s.[IsDeleted]
when not matched by target then
	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank],[IsDeleted])
	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[OldType],s.[Rank],s.[IsDeleted]);


--[common].[Connection]-----------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('7A2044F4-B42C-44AA-BFB8-6538904E8E4C','Retail Connection','{"$type":"Vanrise.Common.Business.VRInterAppRestConnection, Vanrise.Common.Business","ConfigId":"5cd2aac3-1c74-401f-8010-8b9b5fd9c011"}')
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