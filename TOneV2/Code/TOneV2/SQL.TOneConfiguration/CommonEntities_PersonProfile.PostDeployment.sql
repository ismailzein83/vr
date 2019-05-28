

                 --- [common].[VRDevProject]-------------------------------------------------------------------
                 -----------------------------------------------------------------------------------------------
                 begin
                 
                 set nocount on;
                 
                 ;with cte_data([ID],[Name],[CreatedTime],[LastModifiedTime])
                  as (select* from (values
                 --//////////////////////////////////////////////////////////////////////////////////////////////////
                       ('ead1feb0-78ec-4bb7-bcb9-d3f18fe3e39b','Common Person Profile','2019-05-28 10:35:51.307','2019-05-28 10:35:51.307')
                 --\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
                )c([ID],[Name],[CreatedTime],[LastModifiedTime]))
                merge[common].[VRDevProject] as t
                using  cte_data as s
                on            1=1 and t.[ID]=s.[ID]
                  
                  when matched then
                 update set
                 [ID]=s.[ID] ,[Name]=s.[Name] ,[CreatedTime]=s.[CreatedTime] ,[LastModifiedTime]=s.[LastModifiedTime] 
                 when not matched by target then
                 insert([ID],[Name],[CreatedTime],[LastModifiedTime])
                 values(s.[ID], s.[Name], s.[CreatedTime], s.[LastModifiedTime]);
                  
            ----------------------------------------------------------------------------------------------------
              end
            ----------------------------------------------------------------------------------------------------
              

                 --- [genericdata].[DataRecordType]-------------------------------------------------------------------
                 -----------------------------------------------------------------------------------------------
                 begin
                 
                 set nocount on;
                 
                 ;with cte_data([ID],[Name],[DevProjectID],[Fields],[ExtraFieldsEvaluator],[Settings],[CreatedTime],[LastModifiedTime])
                  as (select* from (values
                 --//////////////////////////////////////////////////////////////////////////////////////////////////
                       ('4bf2a5b8-37f6-4314-b0b1-2d912fb2fb69','Com_PersonProf_Career','ead1feb0-78ec-4bb7-bcb9-d3f18fe3e39b','{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities]], mscorlib","$values":[]}','{"$type":"Vanrise.GenericData.MainExtensions.ParentDataRecordTypeExtraFields, Vanrise.GenericData.MainExtensions","ConfigId":"043e058a-0e0e-40e2-82ab-fe04c896e615","DataRecordTypeId":"157157b3-4aa0-4f96-9217-d4614aba9bbd"}','{"$type":"Vanrise.GenericData.Entities.DataRecordTypeSettings, Vanrise.GenericData.Entities","DateTimeField":"CreatedTime","IdField":"ID"}','2019-05-16 10:44:03.130','2019-05-28 10:37:48.813'),
('a2f976e8-6679-485b-a3b2-6f9d2a1159a3','Com_PersonProf_Salutation','ead1feb0-78ec-4bb7-bcb9-d3f18fe3e39b','{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities]], mscorlib","$values":[]}','{"$type":"Vanrise.GenericData.MainExtensions.ParentDataRecordTypeExtraFields, Vanrise.GenericData.MainExtensions","ConfigId":"043e058a-0e0e-40e2-82ab-fe04c896e615","DataRecordTypeId":"3e50b634-706a-4245-b274-30cfe03f30f6"}','{"$type":"Vanrise.GenericData.Entities.DataRecordTypeSettings, Vanrise.GenericData.Entities","DateTimeField":"CreatedTime","IdField":"ID"}','2019-05-16 09:15:52.517','2019-05-28 10:37:54.670')
                 --\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
                )c([ID],[Name],[DevProjectID],[Fields],[ExtraFieldsEvaluator],[Settings],[CreatedTime],[LastModifiedTime]))
                merge[genericdata].[DataRecordType] as t
                using  cte_data as s
                on            1=1 and t.[ID]=s.[ID]
                  
                  when matched then
                 update set
                 [ID]=s.[ID] ,[Name]=s.[Name] ,[DevProjectID]=s.[DevProjectID] ,[Fields]=s.[Fields] ,[ExtraFieldsEvaluator]=s.[ExtraFieldsEvaluator] ,[Settings]=s.[Settings] ,[CreatedTime]=s.[CreatedTime] ,[LastModifiedTime]=s.[LastModifiedTime] 
                 when not matched by target then
                 insert([ID],[Name],[DevProjectID],[Fields],[ExtraFieldsEvaluator],[Settings],[CreatedTime],[LastModifiedTime])
                 values(s.[ID], s.[Name], s.[DevProjectID], s.[Fields], s.[ExtraFieldsEvaluator], s.[Settings], s.[CreatedTime], s.[LastModifiedTime]);
                  
            ----------------------------------------------------------------------------------------------------
              end
            ----------------------------------------------------------------------------------------------------
              

                 --- [genericdata].[DataRecordStorage]-------------------------------------------------------------------
                 -----------------------------------------------------------------------------------------------
                 begin
                 
                 set nocount on;
                 
                 ;with cte_data([ID],[Name],[DataRecordTypeID],[DataStoreID],[Settings],[CreatedTime],[LastModifiedTime])
                  as (select* from (values
                 --//////////////////////////////////////////////////////////////////////////////////////////////////
                       ('9ab813e4-61ca-481b-bc48-ba5a2864b4f9','Com_PersonProf_Career Table','4bf2a5b8-37f6-4314-b0b1-2d912fb2fb69','ff5bf627-0d23-4d5e-9c2c-6cc2a41e4a61','{"$type":"Vanrise.GenericData.RDBDataStorage.RDBDataRecordStorageSettings, Vanrise.GenericData.RDBDataStorage","TableName":"Career","TableSchema":"Com_PersonProf","Columns":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.RDBDataStorage.RDBDataRecordStorageColumn, Vanrise.GenericData.RDBDataStorage]], mscorlib","$values":[{"$type":"Vanrise.GenericData.RDBDataStorage.RDBDataRecordStorageColumn, Vanrise.GenericData.RDBDataStorage","FieldName":"ID","ColumnName":"ID","DataType":2,"IsUnique":false,"IsIdentity":true},{"$type":"Vanrise.GenericData.RDBDataStorage.RDBDataRecordStorageColumn, Vanrise.GenericData.RDBDataStorage","FieldName":"Name","ColumnName":"Name","DataType":1,"Size":255,"IsUnique":true,"IsIdentity":false},{"$type":"Vanrise.GenericData.RDBDataStorage.RDBDataRecordStorageColumn, Vanrise.GenericData.RDBDataStorage","FieldName":"CreatedTime","ColumnName":"CreatedTime","DataType":5,"IsUnique":false,"IsIdentity":false},{"$type":"Vanrise.GenericData.RDBDataStorage.RDBDataRecordStorageColumn, Vanrise.GenericData.RDBDataStorage","FieldName":"CreatedBy","ColumnName":"CreatedBy","DataType":2,"IsUnique":false,"IsIdentity":false},{"$type":"Vanrise.GenericData.RDBDataStorage.RDBDataRecordStorageColumn, Vanrise.GenericData.RDBDataStorage","FieldName":"LastModifiedTime","ColumnName":"LastModifiedTime","DataType":5,"IsUnique":false,"IsIdentity":false},{"$type":"Vanrise.GenericData.RDBDataStorage.RDBDataRecordStorageColumn, Vanrise.GenericData.RDBDataStorage","FieldName":"LastModifiedBy","ColumnName":"LastModifiedBy","DataType":2,"IsUnique":false,"IsIdentity":false}]},"NullableFields":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.RDBDataStorage.RDBNullableField, Vanrise.GenericData.RDBDataStorage]], mscorlib","$values":[]},"IncludeQueueItemId":false,"DateTimeField":"CreatedTime","LastModifiedByField":"LastModifiedBy","CreatedByField":"CreatedBy","LastModifiedTimeField":"LastModifiedTime","CreatedTimeField":"CreatedTime","EnableUseCaching":true,"RequiredLimitResult":false,"DontReflectToDB":false,"DenyAPICall":true,"RequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}},"FieldsPermissions":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.DataRecordStorageFieldsPermission, Vanrise.GenericData.Entities]], mscorlib","$values":[]}}','2019-05-16 10:45:23.853','2019-05-16 10:45:23.700'),
('f8e465c4-39d6-4155-a924-738f70a55b7e','Com_PersonProf_Salutation Table','a2f976e8-6679-485b-a3b2-6f9d2a1159a3','ff5bf627-0d23-4d5e-9c2c-6cc2a41e4a61','{"$type":"Vanrise.GenericData.RDBDataStorage.RDBDataRecordStorageSettings, Vanrise.GenericData.RDBDataStorage","TableName":"Salutation","TableSchema":"Com_PersonProf","Columns":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.RDBDataStorage.RDBDataRecordStorageColumn, Vanrise.GenericData.RDBDataStorage]], mscorlib","$values":[{"$type":"Vanrise.GenericData.RDBDataStorage.RDBDataRecordStorageColumn, Vanrise.GenericData.RDBDataStorage","FieldName":"ID","ColumnName":"ID","DataType":6,"IsUnique":false,"IsIdentity":false},{"$type":"Vanrise.GenericData.RDBDataStorage.RDBDataRecordStorageColumn, Vanrise.GenericData.RDBDataStorage","FieldName":"Name","ColumnName":"Name","DataType":1,"Size":255,"IsUnique":true,"IsIdentity":false},{"$type":"Vanrise.GenericData.RDBDataStorage.RDBDataRecordStorageColumn, Vanrise.GenericData.RDBDataStorage","FieldName":"CreatedTime","ColumnName":"CreatedTime","DataType":5,"IsUnique":false,"IsIdentity":false},{"$type":"Vanrise.GenericData.RDBDataStorage.RDBDataRecordStorageColumn, Vanrise.GenericData.RDBDataStorage","FieldName":"CreatedBy","ColumnName":"CreatedBy","DataType":2,"IsUnique":false,"IsIdentity":false},{"$type":"Vanrise.GenericData.RDBDataStorage.RDBDataRecordStorageColumn, Vanrise.GenericData.RDBDataStorage","FieldName":"LastModifiedTime","ColumnName":"LastModifiedTime","DataType":5,"IsUnique":false,"IsIdentity":false},{"$type":"Vanrise.GenericData.RDBDataStorage.RDBDataRecordStorageColumn, Vanrise.GenericData.RDBDataStorage","FieldName":"LastModifiedBy","ColumnName":"LastModifiedBy","DataType":2,"IsUnique":false,"IsIdentity":false}]},"NullableFields":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.RDBDataStorage.RDBNullableField, Vanrise.GenericData.RDBDataStorage]], mscorlib","$values":[]},"IncludeQueueItemId":false,"DateTimeField":"CreatedTime","LastModifiedByField":"LastModifiedBy","CreatedByField":"CreatedBy","LastModifiedTimeField":"LastModifiedTime","CreatedTimeField":"CreatedTime","EnableUseCaching":true,"RequiredLimitResult":false,"DontReflectToDB":false,"DenyAPICall":true,"RequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}},"FieldsPermissions":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.DataRecordStorageFieldsPermission, Vanrise.GenericData.Entities]], mscorlib","$values":[]}}','2019-05-16 09:17:09.287','2019-05-16 10:47:19.763')
                 --\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
                )c([ID],[Name],[DataRecordTypeID],[DataStoreID],[Settings],[CreatedTime],[LastModifiedTime]))
                merge[genericdata].[DataRecordStorage] as t
                using  cte_data as s
                on            1=1 and t.[ID]=s.[ID]
                  
                  when matched then
                 update set
                 [ID]=s.[ID] ,[Name]=s.[Name] ,[DataRecordTypeID]=s.[DataRecordTypeID] ,[DataStoreID]=s.[DataStoreID] ,[Settings]=s.[Settings] ,[CreatedTime]=s.[CreatedTime] ,[LastModifiedTime]=s.[LastModifiedTime] 
                 when not matched by target then
                 insert([ID],[Name],[DataRecordTypeID],[DataStoreID],[Settings],[CreatedTime],[LastModifiedTime])
                 values(s.[ID], s.[Name], s.[DataRecordTypeID], s.[DataStoreID], s.[Settings], s.[CreatedTime], s.[LastModifiedTime]);
                  
            ----------------------------------------------------------------------------------------------------
              end
            ----------------------------------------------------------------------------------------------------
              

                 --- [sec].[View]-------------------------------------------------------------------
                 -----------------------------------------------------------------------------------------------
                 begin
                 
                 set nocount on;
                 
                 ;with cte_data([ID],[Name],[DevProjectID],[Title],[Module],[Url],[Type],[Settings],[Rank],[ActionNames],[Content],[Audience],[CreatedTime],[LastModifiedTime],[IsDeleted])
                  as (select* from (values
                 --//////////////////////////////////////////////////////////////////////////////////////////////////
                       ('79236029-bb14-4aae-8347-5315539696c8','Salutations','ead1feb0-78ec-4bb7-bcb9-d3f18fe3e39b','Salutations','89254e36-5d91-4db1-970f-9bfef404679a',NULL,'b99b2b0a-9a80-49fc-b68f-c946e1628595','{"$type":"Vanrise.GenericData.Business.GenericBEViewSettings, Vanrise.GenericData.Business","Settings":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Business.GenericBEViewSettingItem, Vanrise.GenericData.Business]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Business.GenericBEViewSettingItem, Vanrise.GenericData.Business","BusinessEntityDefinitionId":"2f6e55d2-44b6-4bcb-b398-758fcb709e05"}]}}',4,NULL,NULL,NULL,'2019-05-16 09:18:59.483','2019-05-28 10:42:56.313',NULL),
('b6621a10-2665-41da-a0a8-cf6485fb3573','Careers','ead1feb0-78ec-4bb7-bcb9-d3f18fe3e39b','Careers','89254e36-5d91-4db1-970f-9bfef404679a',NULL,'b99b2b0a-9a80-49fc-b68f-c946e1628595','{"$type":"Vanrise.GenericData.Business.GenericBEViewSettings, Vanrise.GenericData.Business","Settings":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Business.GenericBEViewSettingItem, Vanrise.GenericData.Business]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Business.GenericBEViewSettingItem, Vanrise.GenericData.Business","BusinessEntityDefinitionId":"0c36853f-e2dc-4391-913e-420a8d064641"}]}}',NULL,NULL,NULL,NULL,'2019-05-28 11:49:56.823','2019-05-28 11:49:56.823',NULL)
                 --\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
                )c([ID],[Name],[DevProjectID],[Title],[Module],[Url],[Type],[Settings],[Rank],[ActionNames],[Content],[Audience],[CreatedTime],[LastModifiedTime],[IsDeleted]))
                merge[sec].[View] as t
                using  cte_data as s
                on            1=1 and t.[ID]=s.[ID]
                  
                  when matched then
                 update set
                 [ID]=s.[ID] ,[Name]=s.[Name] ,[DevProjectID]=s.[DevProjectID] ,[Title]=s.[Title] ,[Module]=s.[Module] ,[Url]=s.[Url] ,[Type]=s.[Type] ,[Settings]=s.[Settings] ,[Rank]=s.[Rank] ,[ActionNames]=s.[ActionNames] ,[Content]=s.[Content] ,[Audience]=s.[Audience] ,[CreatedTime]=s.[CreatedTime] ,[LastModifiedTime]=s.[LastModifiedTime] ,[IsDeleted]=s.[IsDeleted] 
                 when not matched by target then
                 insert([ID],[Name],[DevProjectID],[Title],[Module],[Url],[Type],[Settings],[Rank],[ActionNames],[Content],[Audience],[CreatedTime],[LastModifiedTime],[IsDeleted])
                 values(s.[ID], s.[Name], s.[DevProjectID], s.[Title], s.[Module], s.[Url], s.[Type], s.[Settings], s.[Rank], s.[ActionNames], s.[Content], s.[Audience], s.[CreatedTime], s.[LastModifiedTime], s.[IsDeleted]);
                  
            ----------------------------------------------------------------------------------------------------
              end
            ----------------------------------------------------------------------------------------------------
              