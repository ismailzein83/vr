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


SET IDENTITY_INSERT [Retail].[Account] ON 

GO
INSERT [Retail].[Account] ([ID], [Name], [OldTypeID], [TypeID], [Settings], [ParentID], [StatusID], [SourceID], [ExecutedActionsData], [ExtendedSettings], [CreatedTime]) VALUES (1, N'Pakistan Fixed', NULL, N'954557d8-c871-4636-9ec9-1677485543a9', N'{"$type":"Retail.BusinessEntity.Entities.AccountSettings, Retail.BusinessEntity.Entities","Parts":{"$type":"Retail.BusinessEntity.Entities.AccountPartCollection, Retail.BusinessEntity.Entities","83715d18-0db8-4af4-a9cd-e3ce321e71ad":{"$type":"Retail.BusinessEntity.Entities.AccountPart, Retail.BusinessEntity.Entities","Settings":{"$type":"Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartCompanyProfile, Retail.BusinessEntity.MainExtensions","ConfigId":"b0717c4f-e409-4ae2-8c00-5add4ca828c5","Contacts":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Retail.BusinessEntity.MainExtensions.AccountParts.AccountCompanyContact, Retail.BusinessEntity.MainExtensions]], mscorlib"}}},"158359de-da10-409d-bf51-f0f23f6bea7d":{"$type":"Retail.BusinessEntity.Entities.AccountPart, Retail.BusinessEntity.Entities","Settings":{"$type":"Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartOperatorSetting, Retail.BusinessEntity.MainExtensions","ConfigId":"a2f223cf-3570-4469-bfb6-4a918acf016b","IsMobileOperator":false}}}}', NULL, N'dadc2977-a348-4504-89c9-c92f8f9008dd', NULL, NULL, NULL, CAST(N'2017-06-23 11:12:12.553' AS DateTime))
GO
INSERT [Retail].[Account] ([ID], [Name], [OldTypeID], [TypeID], [Settings], [ParentID], [StatusID], [SourceID], [ExecutedActionsData], [ExtendedSettings], [CreatedTime]) VALUES (2, N'Pakistan-Islamabad-Fixed', NULL, N'954557d8-c871-4636-9ec9-1677485543a9', N'{"$type":"Retail.BusinessEntity.Entities.AccountSettings, Retail.BusinessEntity.Entities","Parts":{"$type":"Retail.BusinessEntity.Entities.AccountPartCollection, Retail.BusinessEntity.Entities","83715d18-0db8-4af4-a9cd-e3ce321e71ad":{"$type":"Retail.BusinessEntity.Entities.AccountPart, Retail.BusinessEntity.Entities","Settings":{"$type":"Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartCompanyProfile, Retail.BusinessEntity.MainExtensions","ConfigId":"b0717c4f-e409-4ae2-8c00-5add4ca828c5","Contacts":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Retail.BusinessEntity.MainExtensions.AccountParts.AccountCompanyContact, Retail.BusinessEntity.MainExtensions]], mscorlib"}}},"158359de-da10-409d-bf51-f0f23f6bea7d":{"$type":"Retail.BusinessEntity.Entities.AccountPart, Retail.BusinessEntity.Entities","Settings":{"$type":"Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartOperatorSetting, Retail.BusinessEntity.MainExtensions","ConfigId":"a2f223cf-3570-4469-bfb6-4a918acf016b","IsMobileOperator":false}}}}', NULL, N'dadc2977-a348-4504-89c9-c92f8f9008dd', NULL, NULL, NULL, CAST(N'2017-06-23 11:12:25.073' AS DateTime))
GO
INSERT [Retail].[Account] ([ID], [Name], [OldTypeID], [TypeID], [Settings], [ParentID], [StatusID], [SourceID], [ExecutedActionsData], [ExtendedSettings], [CreatedTime]) VALUES (3, N'Pakistan-Karachi-Fixed', NULL, N'954557d8-c871-4636-9ec9-1677485543a9', N'{"$type":"Retail.BusinessEntity.Entities.AccountSettings, Retail.BusinessEntity.Entities","Parts":{"$type":"Retail.BusinessEntity.Entities.AccountPartCollection, Retail.BusinessEntity.Entities","83715d18-0db8-4af4-a9cd-e3ce321e71ad":{"$type":"Retail.BusinessEntity.Entities.AccountPart, Retail.BusinessEntity.Entities","Settings":{"$type":"Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartCompanyProfile, Retail.BusinessEntity.MainExtensions","ConfigId":"b0717c4f-e409-4ae2-8c00-5add4ca828c5","Contacts":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Retail.BusinessEntity.MainExtensions.AccountParts.AccountCompanyContact, Retail.BusinessEntity.MainExtensions]], mscorlib"}}},"158359de-da10-409d-bf51-f0f23f6bea7d":{"$type":"Retail.BusinessEntity.Entities.AccountPart, Retail.BusinessEntity.Entities","Settings":{"$type":"Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartOperatorSetting, Retail.BusinessEntity.MainExtensions","ConfigId":"a2f223cf-3570-4469-bfb6-4a918acf016b","IsMobileOperator":false}}}}', NULL, N'dadc2977-a348-4504-89c9-c92f8f9008dd', NULL, NULL, NULL, CAST(N'2017-06-23 11:12:33.637' AS DateTime))
GO
INSERT [Retail].[Account] ([ID], [Name], [OldTypeID], [TypeID], [Settings], [ParentID], [StatusID], [SourceID], [ExecutedActionsData], [ExtendedSettings], [CreatedTime]) VALUES (4, N'Pakistan-Lahore-Fixed', NULL, N'954557d8-c871-4636-9ec9-1677485543a9', N'{"$type":"Retail.BusinessEntity.Entities.AccountSettings, Retail.BusinessEntity.Entities","Parts":{"$type":"Retail.BusinessEntity.Entities.AccountPartCollection, Retail.BusinessEntity.Entities","83715d18-0db8-4af4-a9cd-e3ce321e71ad":{"$type":"Retail.BusinessEntity.Entities.AccountPart, Retail.BusinessEntity.Entities","Settings":{"$type":"Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartCompanyProfile, Retail.BusinessEntity.MainExtensions","ConfigId":"b0717c4f-e409-4ae2-8c00-5add4ca828c5","Contacts":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Retail.BusinessEntity.MainExtensions.AccountParts.AccountCompanyContact, Retail.BusinessEntity.MainExtensions]], mscorlib"}}},"158359de-da10-409d-bf51-f0f23f6bea7d":{"$type":"Retail.BusinessEntity.Entities.AccountPart, Retail.BusinessEntity.Entities","Settings":{"$type":"Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartOperatorSetting, Retail.BusinessEntity.MainExtensions","ConfigId":"a2f223cf-3570-4469-bfb6-4a918acf016b","IsMobileOperator":false}}}}', NULL, N'dadc2977-a348-4504-89c9-c92f8f9008dd', NULL, NULL, NULL, CAST(N'2017-06-23 11:12:41.430' AS DateTime))
GO
INSERT [Retail].[Account] ([ID], [Name], [OldTypeID], [TypeID], [Settings], [ParentID], [StatusID], [SourceID], [ExecutedActionsData], [ExtendedSettings], [CreatedTime]) VALUES (5, N'Pakistan Mobile_SCO', NULL, N'954557d8-c871-4636-9ec9-1677485543a9', N'{"$type":"Retail.BusinessEntity.Entities.AccountSettings, Retail.BusinessEntity.Entities","Parts":{"$type":"Retail.BusinessEntity.Entities.AccountPartCollection, Retail.BusinessEntity.Entities","83715d18-0db8-4af4-a9cd-e3ce321e71ad":{"$type":"Retail.BusinessEntity.Entities.AccountPart, Retail.BusinessEntity.Entities","Settings":{"$type":"Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartCompanyProfile, Retail.BusinessEntity.MainExtensions","ConfigId":"b0717c4f-e409-4ae2-8c00-5add4ca828c5","Contacts":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Retail.BusinessEntity.MainExtensions.AccountParts.AccountCompanyContact, Retail.BusinessEntity.MainExtensions]], mscorlib"}}},"158359de-da10-409d-bf51-f0f23f6bea7d":{"$type":"Retail.BusinessEntity.Entities.AccountPart, Retail.BusinessEntity.Entities","Settings":{"$type":"Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartOperatorSetting, Retail.BusinessEntity.MainExtensions","ConfigId":"a2f223cf-3570-4469-bfb6-4a918acf016b","IsMobileOperator":true}}}}', NULL, N'dadc2977-a348-4504-89c9-c92f8f9008dd', NULL, NULL, NULL, CAST(N'2017-06-23 11:12:56.507' AS DateTime))
GO
INSERT [Retail].[Account] ([ID], [Name], [OldTypeID], [TypeID], [Settings], [ParentID], [StatusID], [SourceID], [ExecutedActionsData], [ExtendedSettings], [CreatedTime]) VALUES (6, N'Pakistan-Mobilink-Mobile', NULL, N'954557d8-c871-4636-9ec9-1677485543a9', N'{"$type":"Retail.BusinessEntity.Entities.AccountSettings, Retail.BusinessEntity.Entities","Parts":{"$type":"Retail.BusinessEntity.Entities.AccountPartCollection, Retail.BusinessEntity.Entities","83715d18-0db8-4af4-a9cd-e3ce321e71ad":{"$type":"Retail.BusinessEntity.Entities.AccountPart, Retail.BusinessEntity.Entities","Settings":{"$type":"Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartCompanyProfile, Retail.BusinessEntity.MainExtensions","ConfigId":"b0717c4f-e409-4ae2-8c00-5add4ca828c5","Contacts":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Retail.BusinessEntity.MainExtensions.AccountParts.AccountCompanyContact, Retail.BusinessEntity.MainExtensions]], mscorlib"}}},"158359de-da10-409d-bf51-f0f23f6bea7d":{"$type":"Retail.BusinessEntity.Entities.AccountPart, Retail.BusinessEntity.Entities","Settings":{"$type":"Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartOperatorSetting, Retail.BusinessEntity.MainExtensions","ConfigId":"a2f223cf-3570-4469-bfb6-4a918acf016b","IsMobileOperator":true}}}}', NULL, N'dadc2977-a348-4504-89c9-c92f8f9008dd', NULL, NULL, NULL, CAST(N'2017-06-23 11:13:07.450' AS DateTime))
GO
INSERT [Retail].[Account] ([ID], [Name], [OldTypeID], [TypeID], [Settings], [ParentID], [StatusID], [SourceID], [ExecutedActionsData], [ExtendedSettings], [CreatedTime]) VALUES (7, N'Pakistan-Other-Mobile', NULL, N'954557d8-c871-4636-9ec9-1677485543a9', N'{"$type":"Retail.BusinessEntity.Entities.AccountSettings, Retail.BusinessEntity.Entities","Parts":{"$type":"Retail.BusinessEntity.Entities.AccountPartCollection, Retail.BusinessEntity.Entities","83715d18-0db8-4af4-a9cd-e3ce321e71ad":{"$type":"Retail.BusinessEntity.Entities.AccountPart, Retail.BusinessEntity.Entities","Settings":{"$type":"Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartCompanyProfile, Retail.BusinessEntity.MainExtensions","ConfigId":"b0717c4f-e409-4ae2-8c00-5add4ca828c5","Contacts":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Retail.BusinessEntity.MainExtensions.AccountParts.AccountCompanyContact, Retail.BusinessEntity.MainExtensions]], mscorlib"}}},"158359de-da10-409d-bf51-f0f23f6bea7d":{"$type":"Retail.BusinessEntity.Entities.AccountPart, Retail.BusinessEntity.Entities","Settings":{"$type":"Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartOperatorSetting, Retail.BusinessEntity.MainExtensions","ConfigId":"a2f223cf-3570-4469-bfb6-4a918acf016b","IsMobileOperator":true}}}}', NULL, N'dadc2977-a348-4504-89c9-c92f8f9008dd', NULL, NULL, NULL, CAST(N'2017-06-23 11:13:17.207' AS DateTime))
GO
INSERT [Retail].[Account] ([ID], [Name], [OldTypeID], [TypeID], [Settings], [ParentID], [StatusID], [SourceID], [ExecutedActionsData], [ExtendedSettings], [CreatedTime]) VALUES (8, N'Pakistan-Telenor-Mobile', NULL, N'954557d8-c871-4636-9ec9-1677485543a9', N'{"$type":"Retail.BusinessEntity.Entities.AccountSettings, Retail.BusinessEntity.Entities","Parts":{"$type":"Retail.BusinessEntity.Entities.AccountPartCollection, Retail.BusinessEntity.Entities","83715d18-0db8-4af4-a9cd-e3ce321e71ad":{"$type":"Retail.BusinessEntity.Entities.AccountPart, Retail.BusinessEntity.Entities","Settings":{"$type":"Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartCompanyProfile, Retail.BusinessEntity.MainExtensions","ConfigId":"b0717c4f-e409-4ae2-8c00-5add4ca828c5","Contacts":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Retail.BusinessEntity.MainExtensions.AccountParts.AccountCompanyContact, Retail.BusinessEntity.MainExtensions]], mscorlib"}}},"158359de-da10-409d-bf51-f0f23f6bea7d":{"$type":"Retail.BusinessEntity.Entities.AccountPart, Retail.BusinessEntity.Entities","Settings":{"$type":"Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartOperatorSetting, Retail.BusinessEntity.MainExtensions","ConfigId":"a2f223cf-3570-4469-bfb6-4a918acf016b","IsMobileOperator":true}}}}', NULL, N'dadc2977-a348-4504-89c9-c92f8f9008dd', NULL, NULL, NULL, CAST(N'2017-06-23 11:13:29.310' AS DateTime))
GO
INSERT [Retail].[Account] ([ID], [Name], [OldTypeID], [TypeID], [Settings], [ParentID], [StatusID], [SourceID], [ExecutedActionsData], [ExtendedSettings], [CreatedTime]) VALUES (9, N'Pakistan-Ufone-Mobile', NULL, N'954557d8-c871-4636-9ec9-1677485543a9', N'{"$type":"Retail.BusinessEntity.Entities.AccountSettings, Retail.BusinessEntity.Entities","Parts":{"$type":"Retail.BusinessEntity.Entities.AccountPartCollection, Retail.BusinessEntity.Entities","83715d18-0db8-4af4-a9cd-e3ce321e71ad":{"$type":"Retail.BusinessEntity.Entities.AccountPart, Retail.BusinessEntity.Entities","Settings":{"$type":"Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartCompanyProfile, Retail.BusinessEntity.MainExtensions","ConfigId":"b0717c4f-e409-4ae2-8c00-5add4ca828c5","Contacts":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Retail.BusinessEntity.MainExtensions.AccountParts.AccountCompanyContact, Retail.BusinessEntity.MainExtensions]], mscorlib"}}},"158359de-da10-409d-bf51-f0f23f6bea7d":{"$type":"Retail.BusinessEntity.Entities.AccountPart, Retail.BusinessEntity.Entities","Settings":{"$type":"Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartOperatorSetting, Retail.BusinessEntity.MainExtensions","ConfigId":"a2f223cf-3570-4469-bfb6-4a918acf016b","IsMobileOperator":true}}}}', NULL, N'dadc2977-a348-4504-89c9-c92f8f9008dd', NULL, NULL, NULL, CAST(N'2017-06-23 11:13:38.830' AS DateTime))
GO
INSERT [Retail].[Account] ([ID], [Name], [OldTypeID], [TypeID], [Settings], [ParentID], [StatusID], [SourceID], [ExecutedActionsData], [ExtendedSettings], [CreatedTime]) VALUES (10, N'Pakistan-Warid-Mobile', NULL, N'954557d8-c871-4636-9ec9-1677485543a9', N'{"$type":"Retail.BusinessEntity.Entities.AccountSettings, Retail.BusinessEntity.Entities","Parts":{"$type":"Retail.BusinessEntity.Entities.AccountPartCollection, Retail.BusinessEntity.Entities","83715d18-0db8-4af4-a9cd-e3ce321e71ad":{"$type":"Retail.BusinessEntity.Entities.AccountPart, Retail.BusinessEntity.Entities","Settings":{"$type":"Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartCompanyProfile, Retail.BusinessEntity.MainExtensions","ConfigId":"b0717c4f-e409-4ae2-8c00-5add4ca828c5","Contacts":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Retail.BusinessEntity.MainExtensions.AccountParts.AccountCompanyContact, Retail.BusinessEntity.MainExtensions]], mscorlib"}}},"158359de-da10-409d-bf51-f0f23f6bea7d":{"$type":"Retail.BusinessEntity.Entities.AccountPart, Retail.BusinessEntity.Entities","Settings":{"$type":"Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartOperatorSetting, Retail.BusinessEntity.MainExtensions","ConfigId":"a2f223cf-3570-4469-bfb6-4a918acf016b","IsMobileOperator":true}}}}', NULL, N'dadc2977-a348-4504-89c9-c92f8f9008dd', NULL, NULL, NULL, CAST(N'2017-06-23 11:13:48.347' AS DateTime))
GO
INSERT [Retail].[Account] ([ID], [Name], [OldTypeID], [TypeID], [Settings], [ParentID], [StatusID], [SourceID], [ExecutedActionsData], [ExtendedSettings], [CreatedTime]) VALUES (11, N'Pakistan-Zong-Mobile', NULL, N'954557d8-c871-4636-9ec9-1677485543a9', N'{"$type":"Retail.BusinessEntity.Entities.AccountSettings, Retail.BusinessEntity.Entities","Parts":{"$type":"Retail.BusinessEntity.Entities.AccountPartCollection, Retail.BusinessEntity.Entities","83715d18-0db8-4af4-a9cd-e3ce321e71ad":{"$type":"Retail.BusinessEntity.Entities.AccountPart, Retail.BusinessEntity.Entities","Settings":{"$type":"Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartCompanyProfile, Retail.BusinessEntity.MainExtensions","ConfigId":"b0717c4f-e409-4ae2-8c00-5add4ca828c5","Contacts":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Retail.BusinessEntity.MainExtensions.AccountParts.AccountCompanyContact, Retail.BusinessEntity.MainExtensions]], mscorlib"}}},"158359de-da10-409d-bf51-f0f23f6bea7d":{"$type":"Retail.BusinessEntity.Entities.AccountPart, Retail.BusinessEntity.Entities","Settings":{"$type":"Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartOperatorSetting, Retail.BusinessEntity.MainExtensions","ConfigId":"a2f223cf-3570-4469-bfb6-4a918acf016b","IsMobileOperator":true}}}}', NULL, N'dadc2977-a348-4504-89c9-c92f8f9008dd', NULL, NULL, NULL, CAST(N'2017-06-23 11:14:01.913' AS DateTime))
GO

SET IDENTITY_INSERT [Retail].[Account] OFF

--[rules].[RuleType]------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------BEGINset nocount on;set identity_insert [rules].[RuleType] on;;with cte_data([ID],[Type])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////(6,'Vanrise.GenericData.Entities.GenericRule'),(5,'Vanrise.GenericData.Normalization.NormalizationRule'),(3,'Vanrise.GenericData.Pricing.RateTypeRule'),(2,'Vanrise.GenericData.Pricing.RateValueRule'),(1,'Vanrise.GenericData.Pricing.TariffRule'),(4,'Vanrise.GenericData.Transformation.Entities.MappingRule')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Type]))merge	[rules].[RuleType] as tusing	cte_data as son		1=1 and t.[Type] = s.[Type]--when matched then--	update set--	[Type] = s.[Type]when not matched by target then	insert([ID],[Type])	values(s.[ID],s.[Type]);set identity_insert [rules].[RuleType] off;END

SET IDENTITY_INSERT [rules].[Rule] ON 

GO
INSERT [rules].[Rule] ([ID], [TypeID], [RuleDetails], [BED], [EED], [SourceID]) VALUES (1, 4, N'{"$type":"Vanrise.GenericData.Transformation.Entities.MappingRule, Vanrise.GenericData.Transformation.Entities","Settings":{"$type":"Vanrise.GenericData.Transformation.Entities.MappingRuleSettings, Vanrise.GenericData.Transformation.Entities","Value":true},"DefinitionId":"f67a1a52-e16e-4b81-839f-d5a2e2d70317","RuleId":1,"BeginEffectiveTime":"2000-06-23T00:00:00","RefreshTimeSpan":"01:00:00"}', CAST(N'2000-06-23 00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [rules].[Rule] ([ID], [TypeID], [RuleDetails], [BED], [EED], [SourceID]) VALUES (2, 4, N'{"$type":"Vanrise.GenericData.Transformation.Entities.MappingRule, Vanrise.GenericData.Transformation.Entities","Settings":{"$type":"Vanrise.GenericData.Transformation.Entities.MappingRuleSettings, Vanrise.GenericData.Transformation.Entities","Value":false},"DefinitionId":"f67a1a52-e16e-4b81-839f-d5a2e2d70317","Criteria":{"$type":"Vanrise.GenericData.Entities.GenericRuleCriteria, Vanrise.GenericData.Entities","FieldsValues":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Vanrise.GenericData.Entities.GenericRuleCriteriaFieldValues, Vanrise.GenericData.Entities]], mscorlib","NumberPrefix":{"$type":"Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues.StaticValues, Vanrise.GenericData.MainExtensions","Values":{"$type":"System.Collections.Generic.List`1[[System.Object, mscorlib]], mscorlib","$values":["92"]}}}},"RuleId":2,"BeginEffectiveTime":"2000-06-23T00:00:00","RefreshTimeSpan":"01:00:00"}', CAST(N'2000-06-23 00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [rules].[Rule] ([ID], [TypeID], [RuleDetails], [BED], [EED], [SourceID]) VALUES (3, 4, N'{"$type":"Vanrise.GenericData.Transformation.Entities.MappingRule, Vanrise.GenericData.Transformation.Entities","Settings":{"$type":"Vanrise.GenericData.Transformation.Entities.MappingRuleSettings, Vanrise.GenericData.Transformation.Entities","Value":1},"DefinitionId":"808a5c38-697d-4256-891e-a63005af3f01","Criteria":{"$type":"Vanrise.GenericData.Entities.GenericRuleCriteria, Vanrise.GenericData.Entities","FieldsValues":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Vanrise.GenericData.Entities.GenericRuleCriteriaFieldValues, Vanrise.GenericData.Entities]], mscorlib","NumberPrefix":{"$type":"Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues.StaticValues, Vanrise.GenericData.MainExtensions","Values":{"$type":"System.Collections.Generic.List`1[[System.Object, mscorlib]], mscorlib","$values":["92"]}}}},"RuleId":0,"BeginEffectiveTime":"2000-06-23T00:00:00","RefreshTimeSpan":"01:00:00"}', CAST(N'2000-06-23 00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [rules].[Rule] ([ID], [TypeID], [RuleDetails], [BED], [EED], [SourceID]) VALUES (4, 4, N'{"$type":"Vanrise.GenericData.Transformation.Entities.MappingRule, Vanrise.GenericData.Transformation.Entities","Settings":{"$type":"Vanrise.GenericData.Transformation.Entities.MappingRuleSettings, Vanrise.GenericData.Transformation.Entities","Value":2},"DefinitionId":"808a5c38-697d-4256-891e-a63005af3f01","Criteria":{"$type":"Vanrise.GenericData.Entities.GenericRuleCriteria, Vanrise.GenericData.Entities","FieldsValues":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Vanrise.GenericData.Entities.GenericRuleCriteriaFieldValues, Vanrise.GenericData.Entities]], mscorlib","NumberPrefix":{"$type":"Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues.StaticValues, Vanrise.GenericData.MainExtensions","Values":{"$type":"System.Collections.Generic.List`1[[System.Object, mscorlib]], mscorlib","$values":["9251"]}}}},"RuleId":0,"BeginEffectiveTime":"2000-06-23T00:00:00","RefreshTimeSpan":"01:00:00"}', CAST(N'2000-06-23 00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [rules].[Rule] ([ID], [TypeID], [RuleDetails], [BED], [EED], [SourceID]) VALUES (5, 4, N'{"$type":"Vanrise.GenericData.Transformation.Entities.MappingRule, Vanrise.GenericData.Transformation.Entities","Settings":{"$type":"Vanrise.GenericData.Transformation.Entities.MappingRuleSettings, Vanrise.GenericData.Transformation.Entities","Value":3},"DefinitionId":"808a5c38-697d-4256-891e-a63005af3f01","Criteria":{"$type":"Vanrise.GenericData.Entities.GenericRuleCriteria, Vanrise.GenericData.Entities","FieldsValues":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Vanrise.GenericData.Entities.GenericRuleCriteriaFieldValues, Vanrise.GenericData.Entities]], mscorlib","NumberPrefix":{"$type":"Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues.StaticValues, Vanrise.GenericData.MainExtensions","Values":{"$type":"System.Collections.Generic.List`1[[System.Object, mscorlib]], mscorlib","$values":["9221"]}}}},"RuleId":0,"BeginEffectiveTime":"2000-06-23T00:00:00","RefreshTimeSpan":"01:00:00"}', CAST(N'2000-06-23 00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [rules].[Rule] ([ID], [TypeID], [RuleDetails], [BED], [EED], [SourceID]) VALUES (6, 4, N'{"$type":"Vanrise.GenericData.Transformation.Entities.MappingRule, Vanrise.GenericData.Transformation.Entities","Settings":{"$type":"Vanrise.GenericData.Transformation.Entities.MappingRuleSettings, Vanrise.GenericData.Transformation.Entities","Value":4},"DefinitionId":"808a5c38-697d-4256-891e-a63005af3f01","Criteria":{"$type":"Vanrise.GenericData.Entities.GenericRuleCriteria, Vanrise.GenericData.Entities","FieldsValues":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Vanrise.GenericData.Entities.GenericRuleCriteriaFieldValues, Vanrise.GenericData.Entities]], mscorlib","NumberPrefix":{"$type":"Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues.StaticValues, Vanrise.GenericData.MainExtensions","Values":{"$type":"System.Collections.Generic.List`1[[System.Object, mscorlib]], mscorlib","$values":["9242"]}}}},"RuleId":0,"BeginEffectiveTime":"2000-06-23T00:00:00","RefreshTimeSpan":"01:00:00"}', CAST(N'2000-06-23 00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [rules].[Rule] ([ID], [TypeID], [RuleDetails], [BED], [EED], [SourceID]) VALUES (7, 4, N'{"$type":"Vanrise.GenericData.Transformation.Entities.MappingRule, Vanrise.GenericData.Transformation.Entities","Settings":{"$type":"Vanrise.GenericData.Transformation.Entities.MappingRuleSettings, Vanrise.GenericData.Transformation.Entities","Value":5},"DefinitionId":"808a5c38-697d-4256-891e-a63005af3f01","Criteria":{"$type":"Vanrise.GenericData.Entities.GenericRuleCriteria, Vanrise.GenericData.Entities","FieldsValues":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Vanrise.GenericData.Entities.GenericRuleCriteriaFieldValues, Vanrise.GenericData.Entities]], mscorlib","NumberPrefix":{"$type":"Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues.StaticValues, Vanrise.GenericData.MainExtensions","Values":{"$type":"System.Collections.Generic.List`1[[System.Object, mscorlib]], mscorlib","$values":["9235"]}}}},"RuleId":0,"BeginEffectiveTime":"2000-06-23T00:00:00","RefreshTimeSpan":"01:00:00"}', CAST(N'2000-06-23 00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [rules].[Rule] ([ID], [TypeID], [RuleDetails], [BED], [EED], [SourceID]) VALUES (8, 4, N'{"$type":"Vanrise.GenericData.Transformation.Entities.MappingRule, Vanrise.GenericData.Transformation.Entities","Settings":{"$type":"Vanrise.GenericData.Transformation.Entities.MappingRuleSettings, Vanrise.GenericData.Transformation.Entities","Value":6},"DefinitionId":"808a5c38-697d-4256-891e-a63005af3f01","Criteria":{"$type":"Vanrise.GenericData.Entities.GenericRuleCriteria, Vanrise.GenericData.Entities","FieldsValues":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Vanrise.GenericData.Entities.GenericRuleCriteriaFieldValues, Vanrise.GenericData.Entities]], mscorlib","NumberPrefix":{"$type":"Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues.StaticValues, Vanrise.GenericData.MainExtensions","Values":{"$type":"System.Collections.Generic.List`1[[System.Object, mscorlib]], mscorlib","$values":["9230"]}}}},"RuleId":0,"BeginEffectiveTime":"2000-06-23T00:00:00","RefreshTimeSpan":"01:00:00"}', CAST(N'2000-06-23 00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [rules].[Rule] ([ID], [TypeID], [RuleDetails], [BED], [EED], [SourceID]) VALUES (9, 4, N'{"$type":"Vanrise.GenericData.Transformation.Entities.MappingRule, Vanrise.GenericData.Transformation.Entities","Settings":{"$type":"Vanrise.GenericData.Transformation.Entities.MappingRuleSettings, Vanrise.GenericData.Transformation.Entities","Value":7},"DefinitionId":"808a5c38-697d-4256-891e-a63005af3f01","Criteria":{"$type":"Vanrise.GenericData.Entities.GenericRuleCriteria, Vanrise.GenericData.Entities","FieldsValues":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Vanrise.GenericData.Entities.GenericRuleCriteriaFieldValues, Vanrise.GenericData.Entities]], mscorlib","NumberPrefix":{"$type":"Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues.StaticValues, Vanrise.GenericData.MainExtensions","Values":{"$type":"System.Collections.Generic.List`1[[System.Object, mscorlib]], mscorlib","$values":["923"]}}}},"RuleId":0,"BeginEffectiveTime":"2000-06-23T00:00:00","RefreshTimeSpan":"01:00:00"}', CAST(N'2000-06-23 00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [rules].[Rule] ([ID], [TypeID], [RuleDetails], [BED], [EED], [SourceID]) VALUES (10, 4, N'{"$type":"Vanrise.GenericData.Transformation.Entities.MappingRule, Vanrise.GenericData.Transformation.Entities","Settings":{"$type":"Vanrise.GenericData.Transformation.Entities.MappingRuleSettings, Vanrise.GenericData.Transformation.Entities","Value":8},"DefinitionId":"808a5c38-697d-4256-891e-a63005af3f01","Criteria":{"$type":"Vanrise.GenericData.Entities.GenericRuleCriteria, Vanrise.GenericData.Entities","FieldsValues":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Vanrise.GenericData.Entities.GenericRuleCriteriaFieldValues, Vanrise.GenericData.Entities]], mscorlib","NumberPrefix":{"$type":"Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues.StaticValues, Vanrise.GenericData.MainExtensions","Values":{"$type":"System.Collections.Generic.List`1[[System.Object, mscorlib]], mscorlib","$values":["9234"]}}}},"RuleId":0,"BeginEffectiveTime":"2000-06-23T00:00:00","RefreshTimeSpan":"01:00:00"}', CAST(N'2000-06-23 00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [rules].[Rule] ([ID], [TypeID], [RuleDetails], [BED], [EED], [SourceID]) VALUES (11, 4, N'{"$type":"Vanrise.GenericData.Transformation.Entities.MappingRule, Vanrise.GenericData.Transformation.Entities","Settings":{"$type":"Vanrise.GenericData.Transformation.Entities.MappingRuleSettings, Vanrise.GenericData.Transformation.Entities","Value":9},"DefinitionId":"808a5c38-697d-4256-891e-a63005af3f01","Criteria":{"$type":"Vanrise.GenericData.Entities.GenericRuleCriteria, Vanrise.GenericData.Entities","FieldsValues":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Vanrise.GenericData.Entities.GenericRuleCriteriaFieldValues, Vanrise.GenericData.Entities]], mscorlib","NumberPrefix":{"$type":"Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues.StaticValues, Vanrise.GenericData.MainExtensions","Values":{"$type":"System.Collections.Generic.List`1[[System.Object, mscorlib]], mscorlib","$values":["9233"]}}}},"RuleId":0,"BeginEffectiveTime":"2000-06-23T00:00:00","RefreshTimeSpan":"01:00:00"}', CAST(N'2000-06-23 00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [rules].[Rule] ([ID], [TypeID], [RuleDetails], [BED], [EED], [SourceID]) VALUES (12, 4, N'{"$type":"Vanrise.GenericData.Transformation.Entities.MappingRule, Vanrise.GenericData.Transformation.Entities","Settings":{"$type":"Vanrise.GenericData.Transformation.Entities.MappingRuleSettings, Vanrise.GenericData.Transformation.Entities","Value":10},"DefinitionId":"808a5c38-697d-4256-891e-a63005af3f01","Criteria":{"$type":"Vanrise.GenericData.Entities.GenericRuleCriteria, Vanrise.GenericData.Entities","FieldsValues":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Vanrise.GenericData.Entities.GenericRuleCriteriaFieldValues, Vanrise.GenericData.Entities]], mscorlib","NumberPrefix":{"$type":"Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues.StaticValues, Vanrise.GenericData.MainExtensions","Values":{"$type":"System.Collections.Generic.List`1[[System.Object, mscorlib]], mscorlib","$values":["9232"]}}}},"RuleId":0,"BeginEffectiveTime":"2000-06-23T00:00:00","RefreshTimeSpan":"01:00:00"}', CAST(N'2000-06-23 00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [rules].[Rule] ([ID], [TypeID], [RuleDetails], [BED], [EED], [SourceID]) VALUES (13, 4, N'{"$type":"Vanrise.GenericData.Transformation.Entities.MappingRule, Vanrise.GenericData.Transformation.Entities","Settings":{"$type":"Vanrise.GenericData.Transformation.Entities.MappingRuleSettings, Vanrise.GenericData.Transformation.Entities","Value":11},"DefinitionId":"808a5c38-697d-4256-891e-a63005af3f01","Criteria":{"$type":"Vanrise.GenericData.Entities.GenericRuleCriteria, Vanrise.GenericData.Entities","FieldsValues":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Vanrise.GenericData.Entities.GenericRuleCriteriaFieldValues, Vanrise.GenericData.Entities]], mscorlib","NumberPrefix":{"$type":"Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues.StaticValues, Vanrise.GenericData.MainExtensions","Values":{"$type":"System.Collections.Generic.List`1[[System.Object, mscorlib]], mscorlib","$values":["9231"]}}}},"RuleId":0,"BeginEffectiveTime":"2000-06-23T00:00:00","RefreshTimeSpan":"01:00:00"}', CAST(N'2000-06-23 00:00:00.000' AS DateTime), NULL, NULL)
GO

SET IDENTITY_INSERT [rules].[Rule] OFF


--[common].[Type]---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------BEGINset nocount on;set identity_insert [common].[Type] on;;with cte_data([ID],[Type])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////(12,'GenericData_DataRecordType_CDR'),(14,'GenericSummaryTransformationManager_SummaryTransformationDefinitionId_1946c43b-03bc-4a5e-a574-19454c54a13e'),(13,'GenericSummaryTransformationManager_SummaryTransformationDefinitionId_b026ec76-54cd-4c28-a69d-f39a87799dcd'),(6,'Vanrise.BusinessProcess.BPRegulatorRuntimeService, Vanrise.BusinessProcess, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'),(7,'Vanrise.Common.Business.BigDataRuntimeService, Vanrise.Common.Business, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'),(1,'Vanrise.Common.Business.CountryManager, Vanrise.Common.Business, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'),(3,'Vanrise.Integration.Business.DataSourceRuntimeService, Vanrise.Integration.Business, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'),(11,'Vanrise.NumberingPlan.Business.SaleCodeManager, Vanrise.NumberingPlan.Business, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'),(10,'Vanrise.NumberingPlan.Business.SaleZoneManager, Vanrise.NumberingPlan.Business, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'),(5,'Vanrise.Queueing.QueueActivationRuntimeService, Vanrise.Queueing, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'),(2,'Vanrise.Queueing.QueueRegulatorRuntimeService, Vanrise.Queueing, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'),(4,'Vanrise.Queueing.SummaryQueueActivationRuntimeService, Vanrise.Queueing, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'),(9,'Vanrise.Runtime.SchedulerService, Vanrise.Runtime, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'),(8,'VR_BusinessProcess_BusinessProcessService')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Type]))merge	[common].[Type] as tusing	cte_data as son		1=1 and t.[Type] = s.[Type]--when matched then--	update set--	[Type] = s.[Type]when not matched by target then	insert([ID],[Type])	values(s.[ID],s.[Type]);set identity_insert [common].[Type] off;END--[common].[IDManager]----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------BEGINset nocount on;;with cte_data([TypeID],[LastTakenID])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////(1,254),(10,1511),(11,50848)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([TypeID],[LastTakenID]))merge	[common].[IDManager] as tusing	cte_data as son		1=1 and t.[TypeID] = s.[TypeID]--when matched then--	update set--	[LastTakenID] = s.[LastTakenID]when not matched by target then	insert([TypeID],[LastTakenID])	values(s.[TypeID],s.[LastTakenID]);END--[common].[Currency]-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------BEGINset nocount on;set identity_insert [common].[Currency] on;;with cte_data([ID],[Symbol],[Name],[SourceID])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////(118,'PKR','Pakistan Rupees','1')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Symbol],[Name],[SourceID]))merge	[common].[Currency] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Symbol] = s.[Symbol],[Name] = s.[Name],[SourceID] = s.[SourceID]when not matched by target then	insert([ID],[Symbol],[Name],[SourceID])	values(s.[ID],s.[Symbol],s.[Name],s.[SourceID]);set identity_insert [common].[Currency] off;
END--[common].[Country]------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------BEGINset nocount on;;with cte_data([ID],[Name],[SourceID])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////(1,'Afghanistan','afghanistan'),(2,'Albania','albania'),(3,'Algeria','algeria'),(4,'American Samoa','american samoa'),(5,'Andorra ','andorra'),(6,'Angola ','angola'),(7,'Anguilla','anguilla'),(8,'Antarctic Global Networks','antarctic global networks'),(9,'Antarctica / Norfolk Island / Australian External Territories','antarctica / norfolk island / australian external territories'),(10,'Antigua / Barbuda','antigua / barbuda'),(11,'Argentina','argentina'),(12,'Armenia ','armenia'),(13,'Aruba','aruba'),(14,'Ascension Island ','ascension island'),(15,'Austria','austria'),(16,'Azerbaijan','azerbaijan'),(17,'Bahamac','bahamac'),(18,'Bahrain','bahrain'),(19,'Bangladesh','bangladesh'),(20,'Barbados','barbados'),(21,'Belarus','belarus'),(22,'Belgium','belgium'),(23,'Belize','belize'),(24,'Benin','benin'),(25,'Bermuda','bermuda'),(26,'Bhutan','bhutan'),(27,'Bolivia ','bolivia'),(28,'Bosnia & Herzegovina ','bosnia & herzegovina'),(29,'Botswana ','botswana'),(30,'Brazil ','brazil'),(31,'British Indian Ocean Territory','british indian ocean territory'),(32,'British Virgin Islands','british virgin islands'),(33,'Brunei Darussalam','brunei darussalam'),(34,'Bulgaria','bulgaria'),(35,'Burkina Faso ','burkina faso'),(36,'Burundi','burundi'),(37,'Cambodia','cambodia'),(38,'Cameroon','cameroon'),(39,'Cape Verde Islands','cape verde islands'),(40,'Cayman Islands','cayman islands'),(41,'Central African Republic','central african republic'),(42,'Chad ','chad'),(43,'Chile / Easter Island','chile / easter island'),(44,'China (PRC)','china (prc)'),(45,'Cocos-Keeling Islands ','cocos-keeling islands'),(46,'Colombia ','colombia'),(47,'Comoros / Mayotte Island','comoros / mayotte island'),(48,'Cook Islands','cook islands'),(49,'Costa Rica','costa rica'),(50,'Croatia','croatia'),(51,'Cuba / Christmas Island / Guatanamo Bay','cuba / christmas island / guatanamo bay'),(52,'Curaçao / Netherlands Antilles','curaçao / netherlands antilles'),(53,'Cyprus','cyprus'),(54,'Czech Republic','czech republic'),(55,'Democratic Republic of the Congo ','democratic republic of the congo'),(56,'Denmark','denmark'),(57,'Djibouti','djibouti'),(58,'Dominica','dominica'),(59,'Dominican Republic ','dominican republic'),(60,'East Timor','east timor'),(61,'Ecuador ','ecuador'),(62,'Egypt','egypt'),(63,'El Salvador','el salvador'),(64,'Ellipso (Mobile Satellite service)','ellipso (mobile satellite service)'),(65,'EMSAT (Mobile Satellite service)','emsat (mobile satellite service)'),(66,'Equatorial Guinea','equatorial guinea'),(67,'Eritrea','eritrea'),(68,'Estonia','estonia'),(69,'Ethiopia','ethiopia'),(70,'Euro SAT','euro sat'),(71,'Falkland Islands (Malvinas)','falkland islands (malvinas)'),(72,'Faroe Islands','faroe islands'),(73,'Fiji Islands','fiji islands'),(74,'Finland','finland'),(75,'France','france'),(76,'French Antilles / Martinique','french antilles / martinique'),(77,'French Guiana','french guiana'),(78,'French Polynesia','french polynesia'),(79,'Gabonese Republic','gabonese republic'),(80,'Gambia','gambia'),(81,'Georgia','georgia'),(82,'Germany','germany'),(83,'Ghana ','ghana'),(84,'Gibraltar ','gibraltar'),(85,'Global Mobile Satellite System (GMSS)','global mobile satellite system (gmss)'),(86,'Global Satellite','global satellite'),(87,'Globalstar (Mobile Satellite Service)','globalstar (mobile satellite service)'),(88,'Greece ','greece'),(89,'Greenland ','greenland'),(90,'Grenada','grenada'),(91,'Guadeloupe','guadeloupe'),(92,'Guam','guam'),(93,'Guantanamo Bay','guantanamo bay'),(94,'Guatemala ','guatemala'),(95,'Guinea','guinea'),(96,'Guinea-Bissau ','guinea-bissau'),(97,'Guyana','guyana'),(98,'Haiti ','haiti'),(99,'Honduras','honduras'),(100,'Hong Kong','hong kong'),(101,'Hungary ','hungary'),(102,'Iceland','iceland'),(103,'ICO Global (Mobile Satellite Service)','ico global (mobile satellite service)'),(104,'India','india'),(105,'Indonesia','indonesia'),(106,'Inmarsat (Atlantic Ocean - East)','inmarsat (atlantic ocean - east)'),(107,'Inmarsat (Atlantic Ocean - West)','inmarsat (atlantic ocean - west)'),(108,'Inmarsat (Indian Ocean)','inmarsat (indian ocean)'),(109,'Inmarsat (Pacific Ocean)','inmarsat (pacific ocean)'),(110,'Inmarsat ROC','inmarsat roc'),(111,'Inmarsat SNAC','inmarsat snac'),(112,'International Freephone Service','international freephone service'),(113,'International Networks 1','international networks 1'),(114,'Iran','iran'),(115,'Iraq','iraq'),(116,'Ireland','ireland'),(117,'Ivory Coast (Côte d''Ivoire)','ivory coast (côte d''ivoire)'),(118,'Jamaica ','jamaica'),(119,'Japan ','japan'),(120,'Jordan','jordan'),(121,'Kenya','kenya'),(122,'Kiribati ','kiribati'),(123,'Korea (North)','korea (north)'),(124,'Korea (South)','korea (south)'),(125,'Kuwait ','kuwait'),(126,'Kyrgyz Republic','kyrgyz republic'),(127,'Laos','laos'),(128,'Latvia ','latvia'),(129,'Lebanon','lebanon'),(130,'Lesotho','lesotho'),(131,'Liberia','liberia'),(132,'Libya','libya'),(133,'Liechtenstein','liechtenstein'),(134,'Lithuania ','lithuania'),(135,'Luxembourg','luxembourg'),(136,'Macao','macao'),(137,'Macedonia (Former Yugoslav Rep of.)','macedonia (former yugoslav rep of.)'),(138,'Madagascar','madagascar'),(139,'Malawi ','malawi'),(140,'Malaysia','malaysia'),(141,'Maldives','maldives'),(142,'Mali Republic','mali republic'),(143,'Malta','malta'),(144,'Marshall Islands','marshall islands'),(145,'Mauritania','mauritania'),(146,'Mauritius','mauritius'),(147,'Mexico','mexico'),(148,'Micronesia, (Federal States of)','micronesia, (federal states of)'),(149,'Midway Islands','midway islands'),(150,'Moldova ','moldova'),(151,'Monaco','monaco'),(152,'Mongolia ','mongolia'),(153,'Montenegro','montenegro'),(154,'Montserrat ','montserrat'),(155,'Morocco','morocco'),(156,'Mozambique','mozambique'),(157,'Myanmar','myanmar'),(158,'Namibia','namibia'),(159,'Nauru','nauru'),(160,'Nepal ','nepal'),(161,'Netherlands','netherlands'),(162,'New Caledonia','new caledonia'),(163,'New Zealand','new zealand'),(164,'Nicaragua','nicaragua'),(165,'Niger','niger'),(166,'Nigeria','nigeria'),(167,'Niue','niue'),(168,'North Yemen','north yemen'),(169,'Northern Marianas Islands (Saipan, Rota, & Tinian)','northern marianas islands (saipan, rota, & tinian)'),(170,'Norway ','norway'),(171,'Oman','oman'),(172,'Pakistan','pakistan'),(173,'Palastin','palastin'),(174,'Palau','palau'),(175,'Palestinian Settlements','palestinian settlements'),(176,'Panama','panama'),(177,'Papua New Guinea','papua new guinea'),(178,'Paraguay','paraguay'),(179,'Peru','peru'),(180,'Philippines','philippines'),(181,'Poland','poland'),(182,'Portugal','portugal'),(183,'Puerto Rico','puerto rico'),(184,'Qatar','qatar'),(185,'Republic of the Congo','republic of the congo'),(186,'Réunion Island','réunion island'),(187,'Romania','romania'),(188,'Russia / Kazakhstan','russia / kazakhstan'),(189,'Rwandese Republic','rwandese republic'),(190,'Samoa','samoa'),(191,'San Marino','san marino'),(192,'Sao Tomé and Principe','sao tomé and principe'),(193,'Saudi Arabia','saudi arabia'),(194,'Senegal ','senegal'),(195,'Serbia','serbia'),(196,'Seychelles Republic','seychelles republic'),(197,'Sierra Leone','sierra leone'),(198,'Singapore','singapore'),(199,'Slovak Republic','slovak republic'),(200,'Slovenia ','slovenia'),(201,'Solomon Islands','solomon islands'),(202,'Somali Democratic Republic','somali democratic republic'),(203,'South Africa','south africa'),(204,'South Sudan','south sudan'),(205,'Spain','spain'),(206,'Sri Lanka','sri lanka'),(207,'St. Helena','st. helena'),(208,'St. Kitts/Nevis','st. kitts/nevis'),(209,'St. Lucia','st. lucia'),(210,'St. Pierre & Miquelon','st. pierre & miquelon'),(211,'St. Vincent & Grenadines','st. vincent & grenadines'),(212,'Sudan','sudan'),(213,'Suriname ','suriname'),(214,'Swaziland','swaziland'),(215,'Sweden','sweden'),(216,'Switzerland','switzerland'),(217,'Syria','syria'),(218,'Taiwan','taiwan'),(219,'Tajikistan','tajikistan'),(220,'Tanzania','tanzania'),(221,'Telecommunications for Disaster Relief','telecommunications for disaster relief'),(222,'testGroup','testgroup'),(223,'testNX','testnx'),(224,'Thailand','thailand'),(225,'Thuraya (Mobile Satellite service)','thuraya (mobile satellite service)'),(226,'Togolese Republic','togolese republic'),(227,'Tokelau','tokelau'),(228,'Tonga Islands','tonga islands'),(229,'Trinidad & Tobago','trinidad & tobago'),(230,'Tristan da Cunha','tristan da cunha'),(231,'Tunisia','tunisia'),(232,'Turkey','turkey'),(233,'Turkmenistan ','turkmenistan'),(234,'Turks and Caicos Islands','turks and caicos islands'),(235,'Tuvalu','tuvalu'),(236,'Uganda','uganda'),(237,'Ukraine','ukraine'),(238,'United Arab Emirates','united arab emirates'),(239,'United Kingdom','united kingdom'),(240,'United States / Canada','united states / canada'),(241,'Universal Personal Telecommunications (UPT)','universal personal telecommunications (upt)'),(242,'Unknown','unknown'),(243,'Uruguay','uruguay'),(244,'US Virgin Islands','us virgin islands'),(245,'Uzbekistan','uzbekistan'),(246,'Vanuatu','vanuatu'),(247,'Vatican City','vatican city'),(248,'Venezuela','venezuela'),(249,'Vietnam','vietnam'),(250,'Wake Island','wake island'),(251,'Wallis and Futuna Islands','wallis and futuna islands'),(252,'Yemen','yemen'),(253,'Zambia','zambia'),(254,'Zimbabwe','zimbabwe')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[SourceID]))merge	[common].[Country] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]--when matched then--	update set--	[Name] = s.[Name],[SourceID] = s.[SourceID]when not matched by target then	insert([ID],[Name],[SourceID])	values(s.[ID],s.[Name],s.[SourceID]);END

--[common].[Region]-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------BEGINset nocount on;set identity_insert [common].[Region] on;;with cte_data([ID],[Name],[CountryID],[Settings],[SourceID])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////(1,'Sindh',172,null,'1'),(2,'KPK',172,null,'21'),(3,'Punjab',172,null,'22'),(4,'Islamabad',172,null,'101'),(5,'Baluchistan',172,null,'102')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[CountryID],[Settings],[SourceID]))merge	[common].[Region] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]--when matched then--	update set--	[Name] = s.[Name],[CountryID] = s.[CountryID],[Settings] = s.[Settings],[SourceID] = s.[SourceID]when not matched by target then	insert([ID],[Name],[CountryID],[Settings],[SourceID])	values(s.[ID],s.[Name],s.[CountryID],s.[Settings],s.[SourceID]);set identity_insert [common].[Region] off;
END--[common].[City]---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------BEGINset nocount on;set identity_insert [common].[City] on;;with cte_data([ID],[Name],[CountryID],[Settings],[SourceId])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////(1,'Karachi',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','1'),(2,'Hyderabad',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','21'),(3,'Faisalabad',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','23'),(4,'Lahore',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','24'),(5,'Multan',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','25'),(6,'Islamabad',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":4}','26'),(7,'Peshawar',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":2}','27'),(8,'Quetta',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":5}','28'),(9,'Sialkot',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','29'),(10,'Abbottabad',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":2}','32'),(11,'Ahmed Pur East',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','33'),(12,'Ali Pur',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','34'),(13,'Arifwala',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','35'),(14,'Attock',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','36'),(15,'Badin',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','37'),(16,'Bahawalnagar',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','38'),(17,'Bhawalpur',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','39'),(18,'Bannu',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":2}','40'),(19,'Bhai Pheru',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','41'),(20,'Bhakar  Jhang',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','42'),(21,'Bhalwal',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','43'),(22,'Bhelli Bhattar',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":2}','44'),(23,'Bhit Shah  Sind',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','45'),(24,'Burewala',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','46'),(25,'Chakwal',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":2}','47'),(26,'Chaman',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":5}','48'),(27,'Chawala',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":2}','49'),(28,'Chichawatni',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','50'),(29,'chiniot',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','51'),(30,'Chistian',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','52'),(31,'Chitral',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":2}','53'),(32,'Chunian',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','54'),(33,'D.G Khan',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','55'),(34,'D.I.Khan',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":2}','56'),(35,'Dadu',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','57'),(36,'Daska',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','58'),(37,'Dera Murad Jamali',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":5}','59'),(38,'Dharaki',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','60'),(39,'Dherki',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','61'),(40,'Digri',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','62'),(41,'Dina',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":2}','63'),(42,'Dinga',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','64'),(43,'Dunyapur',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','65'),(44,'Fatah jang',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','66'),(45,'Fazil Pur',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','67'),(46,'Fort Abbas',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','68'),(47,'Gawadar',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":5}','69'),(48,'Ghotki',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','70'),(49,'Gilgit ',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":2}','71'),(50,'Gojra ',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','72'),(51,'Gujar Khan',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":2}','73'),(52,'Gujranwala',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','74'),(53,'Gujrat',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','75'),(54,'Hafizabad',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','76'),(55,'Hamidpur',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','77'),(56,'Haripur',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":2}','78'),(57,'Haroonabad',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','79'),(58,'Hasan Abdal',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":2}','80'),(59,'Hasil Pur',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','81'),(60,'Hattar',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":2}','82'),(61,'Hazro',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','83'),(62,'Hub',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":5}','84'),(63,'Hunza',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":2}','85'),(64,'Jacobabad',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','86'),(65,'Jalapur Peerwala',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','87'),(66,'Jhelum',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','88'),(67,'Jhang',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','89'),(68,'Jhuddo',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','90'),(69,'Jiranwala',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','91'),(70,'Kalat',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":5}','92'),(71,'Kallar Syedan',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":2}','93'),(72,'Kamalia',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','94'),(73,'Kamonki',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','95'),(74,'Kamra',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','96'),(75,'Kandyaro',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','97'),(76,'Kandhkot',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','98'),(77,'Kanganpur',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','99'),(78,'Kashmore',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','100'),(79,'Kasur',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','101'),(80,'Kehor Paka',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":2}','102'),(81,'Khairpur',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','103'),(82,'Khan Bela',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','104'),(83,'Khan Pur',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','105'),(84,'Khanewal',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','106'),(85,'Kharian',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','107'),(86,'Khipro',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','108'),(87,'Khushab',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','109'),(88,'Khuzdar',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":5}','110'),(89,'Kohat',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":2}','111'),(90,'Kotli',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":2}','112'),(91,'Kotri',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','113'),(92,'Kuchluk',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','114'),(93,'Kunri',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','115'),(94,'Lahore -',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','116'),(95,'Larkana',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','117'),(96,'lasbela',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','118'),(97,'Liaquatpur',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','119'),(98,'Lodhran',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','120'),(99,'Lora Lai',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":5}','121'),(100,'Lyyah',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','122'),(101,'Mach',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','123'),(102,'Matli',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','124'),(103,'Mandi bahauddin',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','125'),(104,'Mansehra',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":2}','126'),(105,'Mardan',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":2}','127'),(106,'Mastung',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":5}','128'),(107,'Mehar',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','129'),(108,'Mehrab Pur',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','130'),(109,'Mian Chanu',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','131'),(110,'Mianwali',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','132'),(111,'Mir Pur',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','133'),(112,'MIR PUR KHAS',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','134'),(113,'Mir Pur Mathelo',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','135'),(114,'Mirpur',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','136'),(115,'Mirpur AJk',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','137'),(116,'Mirpur khas',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','138'),(117,'Moro',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','139'),(118,'Multan -',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','140'),(119,'Muree',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','141'),(120,'Muridke',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','142'),(121,'Muslim Bagh',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','143'),(122,'Muzafarabad',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','144'),(123,'Muzafargarh',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','145'),(124,'Nankana Sb',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','148'),(125,'Nawabshah',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','150'),(126,'Noshehra',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','151'),(127,'Okara',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','153'),(128,'Pano Aqil',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','154'),(129,'Pasrur',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','155'),(130,'Pattoki',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','156'),(131,'Pindgheb',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','157'),(132,'Pir Jo Goth',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','158'),(133,'Pishin',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','159'),(134,'Qila Didar Singh',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','160'),(135,'Qilla Saif Ullah',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','161'),(136,'Rahim Yar Khan',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','162'),(137,'Rani Pur',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','163'),(138,'RATO DERO',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','164'),(139,'Ratta',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','165'),(140,'Ratta Cross',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','166'),(141,'Rawalpindi',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','167'),(142,'Rawat',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":2}','168'),(143,'Rohri',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','169'),(144,'Sadiqabad',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','170'),(145,'Sahiwal',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','171'),(146,'Sialkot -',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','172'),(147,'Sakrand',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','173'),(148,'Sanghar',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','174'),(149,'Sangla Hill',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','175'),(150,'Sargodha',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','176'),(151,'Sawabi',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":2}','177'),(152,'Sehwan',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','178'),(153,'Shabqadr',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":2}','179'),(154,'Shadadpur',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','180'),(155,'Shahdad Kot',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','181'),(156,'Shaikhupura',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','182'),(157,'Shikarpur',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','183'),(158,'Siakh',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','184'),(159,'Sibi',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":5}','185'),(160,'Skardu',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":2}','186'),(161,'Swabi',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":2}','188'),(162,'Swat',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":2}','189'),(163,'Talla Gang',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":2}','190'),(164,'Tando M. Khan',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','191'),(165,'Taxila',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":2}','192'),(166,'Thatta',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','193'),(167,'Thatta Jatli',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','194'),(168,'Toba Tek Singh',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','195'),(169,'Ubaro',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','196'),(170,'Umer Kot',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','197'),(171,'Usta Muhammad',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":5}','198'),(172,'Vehari',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','199'),(173,'Wahcantt',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','200'),(174,'Wazirabad',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','201'),(175,'Yazman',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','202'),(176,'Zahir Peer',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":2}','203'),(177,'Zhob',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":5}','204'),(178,'Dera Ghazi Khan',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":3}','222'),(179,'Bhakar',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":2}','227'),(180,'Bhit Shah',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','232'),(181,'Ghotki -',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','233'),(182,'Sukkur',172,'{"$type":"Vanrise.Entities.CitySettings, Vanrise.Entities","RegionId":1}','236')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[CountryID],[Settings],[SourceId]))merge	[common].[City] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]--when matched then--	update set--	[Name] = s.[Name],[CountryID] = s.[CountryID],[Settings] = s.[Settings],[SourceId] = s.[SourceId]when not matched by target then	insert([ID],[Name],[CountryID],[Settings],[SourceId])	values(s.[ID],s.[Name],s.[CountryID],s.[Settings],s.[SourceId]);set identity_insert [common].[City] off;END--[VR_NumberingPlan].[CodeGroup]------------------------------------------------------------------------------------------------------------------------------------------------------------------------BEGINset nocount on;set identity_insert [VR_NumberingPlan].[CodeGroup] on;;with cte_data([ID],[CountryID],[Code],[SourceID])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////(1,1,'93',null),(2,2,'355',null),(3,3,'213',null),(4,4,'1684',null),(5,5,'376',null),(6,6,'244',null),(7,7,'1264',null),(8,8,'88234',null),(9,9,'672',null),(10,10,'1268',null),(11,11,'54',null),(12,12,'374',null),(13,13,'297',null),(14,14,'247',null),(15,15,'43',null),(16,16,'994',null),(17,17,'1242',null),(18,18,'973',null),(19,19,'880',null),(20,20,'1246',null),(21,21,'375',null),(22,22,'32',null),(23,23,'501',null),(24,24,'229',null),(25,25,'1441',null),(26,26,'975',null),(27,27,'591',null),(28,28,'387',null),(29,29,'267',null),(30,30,'55',null),(31,31,'246',null),(32,32,'1284',null),(33,33,'673',null),(34,34,'359',null),(35,35,'226',null),(36,36,'257',null),(37,37,'855',null),(38,38,'237',null),(39,39,'238',null),(40,40,'1345',null),(41,41,'236',null),(42,42,'235',null),(43,43,'56',null),(44,44,'86',null),(45,45,'61',null),(46,46,'57',null),(47,47,'269',null),(48,48,'682',null),(49,49,'506',null),(50,50,'385',null),(51,51,'53',null),(52,52,'599',null),(53,53,'357',null),(54,54,'420',null),(55,55,'243',null),(56,56,'45',null),(57,57,'253',null),(58,58,'1767',null),(59,59,'1809',null),(60,59,'1829',null),(61,59,'1849',null),(62,60,'670',null),(63,61,'593',null),(64,62,'20',null),(65,63,'503',null),(66,64,'8812',null),(67,64,'8813',null),(68,65,'88213',null),(69,66,'240',null),(70,67,'291',null),(71,68,'372',null),(72,69,'251',null),(73,70,'388',null),(74,71,'500',null),(75,72,'298',null),(76,73,'679',null),(77,74,'358',null),(78,75,'33',null),(79,76,'596',null),(80,77,'594',null),(81,78,'689',null),(82,79,'241',null),(83,80,'220',null),(84,81,'995',null),(85,82,'49',null),(86,83,'233',null),(87,84,'350',null),(88,85,'881',null),(89,86,'882',null),(90,87,'8818',null),(91,87,'8819',null),(92,88,'30',null),(93,89,'299',null),(94,90,'1473',null),(95,91,'590',null),(96,92,'1671',null),(97,93,'5399',null),(98,94,'502',null),(99,95,'224',null),(100,96,'245',null),(101,97,'592',null),(102,98,'509',null),(103,99,'504',null),(104,100,'852',null),(105,101,'36',null),(106,102,'354',null),(107,103,'8810',null),(108,104,'91',null),(109,105,'62',null),(110,106,'871',null),(111,107,'874',null),(112,108,'873',null),(113,109,'872',null),(114,110,'87',null),(115,111,'870',null),(116,112,'800',null),(117,113,'883',null),(118,114,'98',null),(119,115,'964',null),(120,116,'353',null),(121,117,'225',null),(122,118,'1876',null),(123,119,'81',null),(124,120,'962',null),(125,121,'254',null),(126,122,'686',null),(127,123,'850',null),(128,124,'82',null),(129,125,'965',null),(130,126,'996',null),(131,127,'856',null),(132,128,'371',null),(133,129,'961',null),(134,130,'266',null),(135,131,'231',null),(136,132,'218',null),(137,133,'423',null),(138,134,'370',null),(139,135,'352',null),(140,136,'853',null),(141,137,'389',null),(142,138,'261',null),(143,139,'265',null),(144,140,'60',null),(145,141,'960',null),(146,142,'223',null),(147,143,'356',null),(148,144,'692',null),(149,145,'222',null),(150,146,'230',null),(151,147,'52',null),(152,148,'691',null),(153,149,'6998',null),(154,150,'373',null),(155,151,'377',null),(156,152,'976',null),(157,153,'382',null),(158,154,'1664',null),(159,155,'212',null),(160,156,'258',null),(161,157,'95',null),(162,158,'264',null),(163,159,'674',null),(164,160,'977',null),(165,161,'31',null),(166,162,'687',null),(167,163,'64',null),(168,164,'505',null),(169,165,'227',null),(170,166,'234',null),(171,167,'683',null),(172,168,'969',null),(173,169,'1670',null),(174,170,'47',null),(175,171,'968',null),(176,172,'92',null),(177,173,'972',null),(178,174,'680',null),(179,175,'970',null),(180,176,'507',null),(181,177,'675',null),(182,178,'595',null),(183,179,'51',null),(184,180,'63',null),(185,181,'48',null),(186,182,'351',null),(187,183,'1787',null),(188,183,'1939',null),(189,184,'974',null),(190,185,'242',null),(191,186,'262',null),(192,187,'40',null),(193,188,'7',null),(194,189,'250',null),(195,190,'685',null),(196,191,'378',null),(197,192,'239',null),(198,193,'966',null),(199,194,'221',null),(200,195,'381',null),(201,196,'248',null),(202,197,'232',null),(203,198,'65',null),(204,199,'421',null),(205,200,'386',null),(206,201,'677',null),(207,202,'252',null),(208,203,'27',null),(209,204,'211',null),(210,205,'34',null),(211,206,'94',null),(212,207,'290',null),(213,208,'1869',null),(214,209,'1758',null),(215,210,'508',null),(216,211,'1784',null),(217,212,'249',null),(218,213,'597',null),(219,214,'268',null),(220,215,'46',null),(221,216,'41',null),(222,217,'963',null),(223,218,'886',null),(224,219,'992',null),(225,220,'255',null),(226,221,'888',null),(227,222,'83',null),(228,223,'3',null),(229,224,'66',null),(230,225,'88216',null),(231,226,'228',null),(232,227,'690',null),(233,228,'676',null),(234,229,'1868',null),(235,230,'289',null),(236,231,'216',null),(237,232,'90',null),(238,233,'993',null),(239,234,'1649',null),(240,235,'688',null),(241,236,'256',null),(242,237,'380',null),(243,238,'971',null),(244,239,'44',null),(245,240,'1',null),(246,241,'878',null),(247,242,'-',null),(248,243,'598',null),(249,244,'1340',null),(250,245,'998',null),(251,246,'678',null),(252,247,'379',null),(253,247,'39',null),(254,248,'58',null),(255,249,'84',null),(256,250,'808',null),(257,251,'681',null),(258,252,'967',null),(259,253,'260',null),(260,254,'263',null)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[CountryID],[Code],[SourceID]))merge	[VR_NumberingPlan].[CodeGroup] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]--when matched then--	update set--	[CountryID] = s.[CountryID],[Code] = s.[Code],[SourceID] = s.[SourceID]when not matched by target then	insert([ID],[CountryID],[Code],[SourceID])	values(s.[ID],s.[CountryID],s.[Code],s.[SourceID]);set identity_insert [VR_NumberingPlan].[CodeGroup] off;
END


GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1, 1, 235, N'Tuvalu', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (2, 1, 81, N'Georgia - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (3, 1, 81, N'Georgia -Mob GEOCELL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (4, 1, 81, N'Georgia - Fixed Tbilisi', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (5, 1, 81, N'Georgia - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (6, 1, 81, N'Georgia -Mob MAGTIKOM', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (7, 1, 81, N'Georgia - Mobile Geocell', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (8, 1, 81, N'Georgia - Mobile Magtikom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (9, 1, 81, N'Georgia -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (10, 1, 148, N'Micronesia', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (11, 1, 122, N'Kiribati', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (12, 1, 144, N'Marshall Islands', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (13, 1, 174, N'Palau', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (14, 1, 232, N'Turkey - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (15, 1, 232, N'Turkey - Fixed Istanbul', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (16, 1, 232, N'Turkey -CITIES', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (17, 1, 232, N'Turkey - Fixed Izmir', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (18, 1, 232, N'Turkey - Fixed Antalya', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (19, 1, 232, N'Turkey - Fixed Ankara', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (20, 1, 232, N'Turkey - Fixed Adana', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (21, 1, 232, N'Turkey - Northern Cyprus', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (22, 1, 232, N'Turkey - 444 Services', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (23, 1, 232, N'Turkey -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (24, 1, 232, N'Turkey - Mobile Aycell', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (25, 1, 232, N'Turkey - Mobile Istela', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (26, 1, 232, N'Turkey - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (27, 1, 232, N'Turkey - Mobility services', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (28, 1, 232, N'Turkey - Mobile Turkcell', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (29, 1, 232, N'Turkey - Mobile Turkcell Northern Cyprus', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (30, 1, 232, N'Turkey -Mob VODAFONE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (31, 1, 232, N'Turkey - Mobile Telsim', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (32, 1, 232, N'Turkey - Mobile Telsim Northern Cyprus', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (33, 1, 232, N'Turkey -Mob VODAFONE N.CYPRUS', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (34, 1, 232, N'Turkey - Nomadic Services', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (35, 1, 119, N'Japan - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (36, 1, 119, N'Japan - IP Phone', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (37, 1, 119, N'Japan - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (38, 1, 119, N'Japan - Mobile Docomo', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (39, 1, 167, N'Niue', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (40, 1, 177, N'Papua New Guinea - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (41, 1, 177, N'Papua New Guinea - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (42, 1, 177, N'Papua New Guinea - Mobile Digicel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (43, 1, 9, N'Norfolk', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (44, 1, 9, N'Antarctica', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (45, 1, 33, N'Brunei - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (46, 1, 33, N'Brunei - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (47, 1, 33, N'Brunei -Mob DSTCOM', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (48, 1, 33, N'Brunei -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (49, 1, 60, N'East Timor - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (50, 1, 60, N'East Timor - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (51, 1, 60, N'East Timor - Mobile TELKOMCEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (52, 1, 73, N'Fiji - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (53, 1, 73, N'Fiji - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (54, 1, 159, N'Nauru', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (55, 1, 228, N'Tonga - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (56, 1, 228, N'Tonga - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (57, 1, 228, N'Tonga - Mobile Digicel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (58, 1, 173, N'Israel - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (59, 1, 173, N'Israel - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (60, 1, 173, N'Israel - Mobile Cellcom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (61, 1, 173, N'Israel - Mobile Partner', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (62, 1, 173, N'Israel - Fixed Palestine', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (63, 1, 173, N'Israel - Mobile Palestine', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (64, 1, 246, N'Vanuatu - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (65, 1, 246, N'Vanuatu - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (66, 1, 188, N'Russia', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (67, 1, 188, N'Russia - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (68, 1, 188, N'Russia -Fix 3', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (69, 1, 188, N'Russia - Far Zone', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (70, 1, 188, N'Russia - Fixed 2', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (71, 1, 188, N'Russia - Fixed 1', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (72, 1, 188, N'Russia -Fix ORANGE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (73, 1, 188, N'Russia - Fixed Moscow', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (74, 1, 188, N'Kazakhstan - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (75, 1, 188, N'Kazakhstan - Mobile KCELL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (76, 1, 188, N'Kazakhstan - Mobile KARTEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (77, 1, 188, N'Kazakhstan - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (78, 1, 188, N'Kazakhstan - Fixed Astana', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (79, 1, 188, N'Kazakhstan - Fixed Karaganda', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (80, 1, 188, N'Kazakhstan - Fixed Almaty', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (81, 1, 188, N'Kazakhstan', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (82, 1, 188, N'Kazakhstan - Fixed OLO', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (83, 1, 188, N'Kazakhstan - Mobility services', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (84, 1, 188, N'Russia - Fixed St Petersburg', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (85, 1, 188, N'Russia - Abkhazia', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (86, 1, 188, N'Russia - Fixed 5', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (87, 1, 188, N'Russia -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (88, 1, 188, N'Russia - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (89, 1, 188, N'Russia - Mobile Rostel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (90, 1, 188, N'Russia - Mobile Vympelcom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (91, 1, 188, N'Russia - Mobile Megafon', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (92, 1, 188, N'Russia - Mobile MTS', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (93, 1, 188, N'Russia -Mob MEGAFON', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (94, 1, 188, N'Russia - South Ossetia', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (95, 1, 188, N'Russia - Mobile Abkhazia', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (96, 1, 188, N'Russia - Mobile Global', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (97, 1, 188, N'Russia - Mobile GTNT', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (98, 1, 188, N'Russia - Mobile Iridium', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (99, 1, 188, N'Russia - Mobility Services', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (100, 1, 227, N'Tokelau', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (101, 1, 160, N'Nepal - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (102, 1, 160, N'Nepal - Fixed Kathmandu', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (103, 1, 160, N'Nepal - Mobile Other', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (104, 1, 160, N'Nepal - Mobile Smart', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (105, 1, 160, N'Nepal - Mobile NT', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (106, 1, 160, N'Nepal - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (107, 1, 160, N'Nepal - Mobile SPICE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (108, 1, 160, N'Nepal -Mob ROAMING', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (109, 1, 78, N'French Polynesia - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (110, 1, 78, N'French Polynesia - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (111, 1, 78, N'French Polynesia -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (112, 1, 114, N'Iran - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (113, 1, 114, N'Iran - Fixed Tehran', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (114, 1, 114, N'Iran -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (115, 1, 114, N'Iran - Mobile MTN', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (116, 1, 114, N'Iran - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (117, 1, 163, N'New Zealand - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (118, 1, 163, N'New Zealand -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (119, 1, 163, N'New Zealand - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (120, 1, 163, N'New Zealand - Mobile Vodafone', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (121, 1, 243, N'Uruguay - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (122, 1, 243, N'Uruguay -MONTEVIDEO', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (123, 1, 243, N'Uruguay - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (124, 1, 243, N'Uruguay -Mob ANCEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (125, 1, 243, N'Uruguay -Mob MOVISTAR', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (126, 1, 243, N'Uruguay -Mob CLARO', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (127, 1, 213, N'Suriname', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (128, 1, 213, N'Surinam -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (129, 1, 213, N'Suriname - Mobile Digicel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (130, 1, 213, N'Suriname - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (131, 1, 77, N'French Guiana - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (132, 1, 77, N'French Guiana - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (133, 1, 77, N'French Guiana - Mobile Outremer', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (134, 1, 77, N'French Guiana - Mobile Digicel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (135, 1, 77, N'French Guiana - Mobile Orange', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (136, 1, 77, N'Guiana French -Mob ORANGE CARAIB', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (137, 1, 76, N'Martinique - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (138, 1, 76, N'Martinique - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (139, 1, 76, N'Martinique - Mobile Outre Mer Telecom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (140, 1, 76, N'Martinique - Mobile Digicel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (141, 1, 76, N'Martinique - Mobile Orange', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (142, 1, 76, N'Martinique -Mob DIGICEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (143, 1, 97, N'Guyana - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (144, 1, 97, N'Guyana - Mobile Digicel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (145, 1, 97, N'Guyana - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (146, 1, 48, N'Cook - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (147, 1, 48, N'Cook - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (148, 1, 126, N'Kyrgyzstan - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (149, 1, 126, N'Kyrgyzstan - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (150, 1, 126, N'Kyrgyzstan - Fixed Bishkek', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (151, 1, 126, N'Kirghizistan -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (152, 1, 126, N'Kyrgyzstan - Mobile AYSAT', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (153, 1, 126, N'Kyrgyzstan - Mobile Bimokom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (154, 1, 126, N'Kyrgyzstan - Mobile Nurtel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (155, 1, 248, N'Venezuela - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (156, 1, 248, N'Venezuela - Fixed Caracas', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (157, 1, 248, N'Venezuela - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (158, 1, 248, N'Venezuela - Mobile Digitel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (159, 1, 248, N'Venezuela - Mobile Movistar', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (160, 1, 248, N'Venezuela -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (161, 1, 248, N'Venezuela - Mobility services', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (162, 1, 248, N'Venezuela - Mobile Movilnet', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (163, 1, 93, N'Cuba - Fixed Guantanamo', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (164, 1, 178, N'Paraguay - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (165, 1, 178, N'Paraguay -ASUNCION', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (166, 1, 178, N'Paraguay -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (167, 1, 178, N'Paraguay - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (168, 1, 178, N'Paraguay -Mob VOX', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (169, 1, 178, N'Paraguay - Mobile Personal', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (170, 1, 178, N'Paraguay -Mob TIGO', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (171, 1, 178, N'Paraguay - Mobile TELCEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (172, 1, 238, N'United Arab Emirates - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (173, 1, 238, N'United Arab Emirates - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (174, 1, 238, N'United Arab Emirates - Mobile DU', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (175, 1, 140, N'Malaysia - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (176, 1, 140, N'Malaysia - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (177, 1, 140, N'Malaysia -Mob CELCOM', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (178, 1, 140, N'Malaysia - Mobile Digi', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (179, 1, 140, N'Malaysia - Mobile Celcom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (180, 1, 140, N'Malaysia - Mobile Maxis', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (181, 1, 45, N'Australia - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (182, 1, 45, N'Australia Satellite - Mobile Optus', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (183, 1, 45, N'Australia Satellite - Fixed Telstra', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (184, 1, 45, N'Australia Satellite - Mobile Telstra', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (185, 1, 45, N'Australia - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (186, 1, 45, N'Australia - Fixed City Group', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (187, 1, 45, N'Australia -METRO', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (188, 1, 45, N'Australia -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (189, 1, 45, N'Australia - Mobile Telstra', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (190, 1, 45, N'Australia - Mobile Optus', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (191, 1, 45, N'Australia - Mobile Vodafone', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (192, 1, 45, N'Australia - Mobile Lycatel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (193, 1, 91, N'Guadeloupe - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (194, 1, 91, N'Guadeloupe - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (195, 1, 91, N'Guadeloupe - Mobile Outre Mer Telecom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (196, 1, 91, N'Guadeloupe - Mobile Digicel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (197, 1, 91, N'Guadeloupe -Mob DAUPHIN TEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (198, 1, 91, N'Guadeloupe - Mobile Orange', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (199, 1, 176, N'Panama - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (200, 1, 176, N'Panama - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (201, 1, 176, N'Panama - Mobile Digicel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (202, 1, 176, N'Panama - Mobile CW', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (203, 1, 176, N'Panama - Mobile Telefonica', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (204, 1, 51, N'Cuba - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (205, 1, 51, N'Cuba - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (206, 1, 27, N'Bolivia - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (207, 1, 27, N'Bolivia - Fixed La Paz', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (208, 1, 27, N'Bolivia -RURAL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (209, 1, 27, N'Bolivia -OTHER CITIES', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (210, 1, 27, N'Bolivia - Fixed Santa Cruz', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (211, 1, 27, N'Bolivia -COCHABAMBA', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (212, 1, 27, N'Bolivia -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (213, 1, 27, N'Bolivia - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (214, 1, 27, N'Bolivia - Mobile Entel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (215, 1, 27, N'Bolivia - Mobile Telcel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (216, 1, 27, N'Bolivia -Mob NUEVATEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (217, 1, 27, N'Bolivia -Mob ENTEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (218, 1, 125, N'Kuwait - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (219, 1, 125, N'Kuwait -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (220, 1, 125, N'Kuwait - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (221, 1, 125, N'Kuwait - Mobile UCC', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (222, 1, 125, N'Kuwait -Mob ZAIN', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (223, 1, 125, N'Kuwait - Mobile MTC', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (224, 1, 52, N'Netherlands Antilles - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (225, 1, 52, N'Netherlands Antilles - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (226, 1, 52, N'Netherlands Antilles -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (227, 1, 52, N'Netherlands Antilles - Mobile Digicel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (228, 1, 61, N'Ecuador - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (229, 1, 61, N'Ecuador -QUITO', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (230, 1, 61, N'Ecuador - Fixed Quito', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (231, 1, 61, N'Ecuador - Fixed ETAPA', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (232, 1, 61, N'Ecuador -GUAYAQUIL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (233, 1, 61, N'Ecuador -PORTOVIEJO', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (234, 1, 61, N'Ecuador -CUENCA', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (235, 1, 61, N'Ecuador - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (236, 1, 61, N'Ecuador - Mobile Porta', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (237, 1, 61, N'Ecuador -Mob CLARO', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (238, 1, 61, N'Ecuador - Mobile Otecel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (239, 1, 49, N'Costa Rica - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (240, 1, 49, N'Costa Rica - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (241, 1, 49, N'Costa Rica - Mobile Telefonica', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (242, 1, 49, N'Costa Rica - Mobile Claro', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (243, 1, 49, N'Costa Rica -Mob CLARO', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (244, 1, 99, N'Honduras - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (245, 1, 99, N'Honduras - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (246, 1, 99, N'Honduras - Mobile Claro', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (247, 1, 99, N'Honduras -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (248, 1, 99, N'Honduras - Mobile Celtel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (249, 1, 175, N'Palestine - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (250, 1, 175, N'Palestine - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (251, 1, 98, N'Haiti - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (252, 1, 98, N'Haiti - Fixed Haitel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (253, 1, 98, N'Haiti - Mobile Digicel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (254, 1, 98, N'Haiti -Mob COMCEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (255, 1, 98, N'Haiti - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (256, 1, 98, N'Haiti -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (257, 1, 98, N'Haiti - Mobile Haitel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (258, 1, 98, N'Haiti -Mob DIGICEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (259, 1, 71, N'Falkland Islands - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (260, 1, 71, N'Falkland Islands - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (261, 1, 210, N'St Pierre &amp; Miquelon - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (262, 1, 210, N'St Pierre &amp; Miquelon - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (263, 1, 23, N'Belize - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (264, 1, 23, N'Belize - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (265, 1, 46, N'Colombia - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (266, 1, 46, N'Colombia - Fixed Bogota', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (267, 1, 46, N'Colombia - Fixed Cali', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (268, 1, 46, N'Colombia - LEX', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (269, 1, 46, N'Colombia -CALI', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (270, 1, 46, N'Colombia -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (271, 1, 46, N'Colombia - Mobile TIGO', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (272, 1, 46, N'Colombia - Mobile Comcel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (273, 1, 46, N'Colombia - Mobile Movistar', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (274, 1, 46, N'Colombia - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (275, 1, 46, N'Colombia - Fixed Medellin', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (276, 1, 46, N'Colombia -BARANQUILLA', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (277, 1, 46, N'Colombia -PEREIRA', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (278, 1, 46, N'Colombia - Fixed Pereira', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (279, 1, 46, N'Colombia -ARMENIA', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (280, 1, 46, N'Colombia -MANIZALES', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (281, 1, 46, N'Colombia -BUCARAMANGA', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (282, 1, 164, N'Nicaragua - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (283, 1, 164, N'Nicaragua - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (284, 1, 164, N'Nicaragua - Mobile Telefonica', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (285, 1, 164, N'Nicaragua - Mobility services', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (286, 1, 43, N'Chile - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (287, 1, 43, N'Chile - Fixed Santiago', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (288, 1, 43, N'Chile - Rural', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (289, 1, 43, N'Chile -Rural', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (290, 1, 43, N'Chile - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (291, 1, 82, N'Germany - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (292, 1, 82, N'Germany -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (293, 1, 82, N'Germany - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (294, 1, 82, N'Germany - Mobile T-Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (295, 1, 82, N'Germany - Mobile Vodafone D2', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (296, 1, 82, N'Germany - Mobile Eplus', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (297, 1, 82, N'Germany -Mob E PLUS', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (298, 1, 82, N'Germany - Mobile O2', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (299, 1, 82, N'Germany - IP Phone', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (300, 1, 82, N'Germany - Personal Numbering', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (301, 1, 147, N'Mexico - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (302, 1, 147, N'Mexico - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (303, 1, 147, N'Mexico -Mob EQUAL ACCESS', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (304, 1, 147, N'Mexico -Mob GUADALAJARA', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (305, 1, 147, N'Mexico -Mob MEXICO CITY', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (306, 1, 147, N'Mexico -Mob MONTERREY', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (307, 1, 147, N'Mexico - Equal Access', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (308, 1, 147, N'Mexico - Guadalajara', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (309, 1, 147, N'Mexico Mexico City - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (310, 1, 147, N'Mexico Monterrey - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (311, 1, 63, N'El Salvador - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (312, 1, 63, N'El Salvador - Mobile Telemovil', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (313, 1, 63, N'El Salvador - Mobile Saltel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (314, 1, 63, N'El Salvador - Mobile Digicel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (315, 1, 63, N'El Salvador - Mobile Claro', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (316, 1, 63, N'El Salvador - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (317, 1, 198, N'Singapore - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (318, 1, 198, N'Singapore -IP PHONE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (319, 1, 198, N'Singapore - Fixed Starhub', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (320, 1, 198, N'Singapore - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (321, 1, 198, N'Singapore - Mobile MobileOne', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (322, 1, 198, N'Singapore - Mobile Starhub', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (323, 1, 198, N'Singapore - Mobile Singtel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (324, 1, 198, N'Singapore -Mob MOBILE ONE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (325, 1, 133, N'Liechtenstein - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (326, 1, 133, N'Liechtenstein - Mobility Services', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (327, 1, 133, N'Liechtenstein - Voice Mail', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (328, 1, 133, N'Liechtenstein -Mob ORANGE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (329, 1, 133, N'Liechtenstein -Mob SWISSCOM FL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (330, 1, 133, N'Liechtenstein - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (331, 1, 133, N'Liechtenstein - Mobile Type A', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (332, 1, 133, N'Liechtenstein -Fix SCES Vas 1', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (333, 1, 199, N'Slovakia - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (334, 1, 199, N'Slovakia - Fixed Short Numbers', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (335, 1, 199, N'Slovakia - Mobile Eurotel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (336, 1, 199, N'Slovakia - Mobile Orange', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (337, 1, 199, N'Slovakia - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (338, 1, 199, N'Slovakia - Mobile O2', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (339, 1, 54, N'Czech Republic - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (340, 1, 54, N'Czech Republic - Mobile Vodafone', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (341, 1, 54, N'Czech Republic - Mobile CZETEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (342, 1, 54, N'Czech Republic - Mobile T-Mobil', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (343, 1, 54, N'Czech Republic - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (344, 1, 54, N'Czech Republic - Mobile Travel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (345, 1, 54, N'Czech Rep -Fix SPECIAL SERVICES', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (346, 1, 54, N'Czech Republic - Mobile Travel Type A', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (347, 1, 54, N'Czech Rep -Mob TELEFONICA O2', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (348, 1, 215, N'Sweden - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (349, 1, 215, N'Sweden - Mobile Comviq', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (350, 1, 215, N'Sweden - Mobile Telia', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (351, 1, 215, N'Sweden - Mobile Vodafone', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (352, 1, 215, N'Sweden - Mobile HI3G', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (353, 1, 215, N'Sweden - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (354, 1, 215, N'Sweden -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (355, 1, 215, N'Sweden -Mob TELIA', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (356, 1, 215, N'Sweden -Mob TELENOR', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (357, 1, 215, N'Sweden -Mob COMVIQ', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (358, 1, 215, N'Sweden -Mob HI3G', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (359, 1, 215, N'Sweden - Fixed Stockholm', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (360, 1, 216, N'Switzerland - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (361, 1, 216, N'Switzerland - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (362, 1, 216, N'Switzerland - Mobile Swisscom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (363, 1, 216, N'Switzerland - Mobile Sunrise', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (364, 1, 216, N'Switzerland -Mob IN PHONE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (365, 1, 216, N'Switzerland - Mobile Lycatel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (366, 1, 216, N'Switzerland -Mob LYCAMOBILE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (367, 1, 216, N'Switzerland - Mobile Orange', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (368, 1, 216, N'Switzerland -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (369, 1, 170, N'Norway - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (370, 1, 170, N'Norway - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (371, 1, 170, N'Norway - Mobile Netcom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (372, 1, 170, N'Norway - Mobile Telenor', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (373, 1, 170, N'Norway -Mob NETCOM', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (374, 1, 170, N'Norway - Mobile Network', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (375, 1, 170, N'Norway -Mob TELENOR', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (376, 1, 170, N'Norway - Mobile Tele2', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (377, 1, 65, N'Emsat', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (378, 1, 247, N'Italy - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (379, 1, 247, N'Italy - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (380, 1, 247, N'Italy - Mobile Elsacom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (381, 1, 247, N'Italy - Mobile INTERM', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (382, 1, 247, N'Italy -Mob WIND', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (383, 1, 247, N'Italy - Mobile Wind', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (384, 1, 247, N'Italy -Mob TIM', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (385, 1, 247, N'Italy - Mobile TIM', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (386, 1, 247, N'Italy - Mobile Vodafone Omnitel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (387, 1, 247, N'Italy - Mobile H3G', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (388, 1, 247, N'Italy -Mob HI3G', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (389, 1, 184, N'Qatar - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (390, 1, 184, N'Qatar -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (391, 1, 184, N'Qatar - Mobile Vodafo', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (392, 1, 184, N'Qatar - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (393, 1, 184, N'Qatar -Mob VODAFONE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (394, 1, 219, N'Tajikistan - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (395, 1, 219, N'Tajikistan - Mobile Indigo', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (396, 1, 219, N'Tajikistan - Mobile TT-Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (397, 1, 219, N'Tajikistan - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (398, 1, 219, N'Tajikistan - Mobile Tacom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (399, 1, 219, N'Tajikistan - Mobile Babilon', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (400, 1, 219, N'Tajikistan - Mobile TK Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (401, 1, 219, N'Tajikistan - Mobile M-teko', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (402, 1, 219, N'Tajikistan - Mobile Telecom Ink', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (403, 1, 219, N'Tajikistan - Mobile Tojiktelecom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (404, 1, 94, N'Guatemala - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (405, 1, 94, N'Guatemala -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (406, 1, 94, N'Guatemala - Mobile COMCEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (407, 1, 94, N'Guatemala -Mob COMCEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (408, 1, 94, N'Guatemala - Mobile TELEFONICA', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (409, 1, 94, N'Guatemala - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (410, 1, 94, N'Guatemala - Mobile PCS', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (411, 1, 153, N'Montenegro - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (412, 1, 153, N'Montenegro - Mobile MTEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (413, 1, 153, N'Montenegro - Mobile PROMON', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (414, 1, 153, N'Montenegro - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (415, 1, 15, N'Austria - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (416, 1, 15, N'Austria - Fixed Vienna', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (417, 1, 15, N'Austria - Corporate Numbering', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (418, 1, 15, N'Austria - Mobile TMobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (419, 1, 15, N'Austria - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (420, 1, 15, N'Austria - Mobile H3G', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (421, 1, 15, N'Austria - Mobile Mobilkom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (422, 1, 15, N'Austria -Mob T MOBILE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (423, 1, 15, N'Austria -Mob HI3G', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (424, 1, 15, N'Austria -Mob ORANGE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (425, 1, 15, N'Austria -Mob MOBILKOM', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (426, 1, 50, N'Croatia - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (427, 1, 50, N'Croatia - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (428, 1, 50, N'Croatia - Mobile Vipnet', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (429, 1, 50, N'Croatia - Mobile HPT', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (430, 1, 112, N'International Toll Free', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (431, 1, 249, N'Viet Nam - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (432, 1, 249, N'Viet Nam - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (433, 1, 249, N'Vietnam -Mob VIETTEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (434, 1, 249, N'Viet Nam - Mobile Vietel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (435, 1, 249, N'Viet Nam - Fixed Hanoi', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (436, 1, 249, N'Viet Nam - Fixed Ho Chi Minh', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (437, 1, 190, N'Western Samoa - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (438, 1, 190, N'Samoa Western -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (439, 1, 190, N'Western Samoa - Mobile Digicel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (440, 1, 190, N'Western Samoa - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (441, 1, 56, N'Denmark - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (442, 1, 56, N'Denmark - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (443, 1, 56, N'Denmark -Mob TDC', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (444, 1, 56, N'Denmark - Mobile TDK', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (445, 1, 56, N'Denmark - Mobile Sonofon', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (446, 1, 56, N'Denmark -Mob TELENOR', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (447, 1, 56, N'Denmark - Mobile Telia', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (448, 1, 56, N'Denmark - Mobile HI3G', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (449, 1, 56, N'Denmark -Mob H3G', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (450, 1, 56, N'Denmark -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (451, 1, 56, N'Denmark -Mob TELIA AS', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (452, 1, 56, N'Denmark - Paid800', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (453, 1, 237, N'Ukraine - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (454, 1, 237, N'Ukraine - Fixed Lviv', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (455, 1, 237, N'Ukraine -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (456, 1, 237, N'Ukraine - Fixed Kiev', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (457, 1, 237, N'Ukraine - Fixed Odessa', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (458, 1, 237, N'Ukraine - Mobile UMC', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (459, 1, 237, N'Ukraine - Fixed Dniepropetrovsk', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (460, 1, 237, N'Ukraine - Fixed Kharkiv', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (461, 1, 237, N'Ukraine - Mobile Astelit', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (462, 1, 237, N'Ukraine - Mobile Kyivstar', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (463, 1, 237, N'Ukraine - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (464, 1, 237, N'Ukraine - Mobile INTTEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (465, 1, 191, N'San Marino - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (466, 1, 191, N'San Marino - Fixed TISM', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (467, 1, 191, N'San Marino - Mobile SMT', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (468, 1, 191, N'San Marino - Mobile Alternative', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (469, 1, 30, N'Brazil - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (470, 1, 30, N'Brazil -SAO PAULO', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (471, 1, 30, N'Brazil - Fixed Sao Paulo', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (472, 1, 30, N'Brazil -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (473, 1, 30, N'Brazil -Mob TIM', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (474, 1, 30, N'Brazil - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (475, 1, 30, N'Brazil - Mobile TIM', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (476, 1, 30, N'Brazil - Fixed State of Sao Paulo', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (477, 1, 30, N'Brazil -SAO PAULO STATE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (478, 1, 30, N'Brazil -SANTOS', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (479, 1, 30, N'Brazil - Fixed Campinas', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (480, 1, 30, N'Brazil - Fixed Rio', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (481, 1, 30, N'Brazil - Fixed Belo Horizonte', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (482, 1, 30, N'Brazil - Fixed Governador', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (483, 1, 30, N'Brazil - Fixed Curitiba', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (484, 1, 30, N'Brazil -CITIES', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (485, 1, 30, N'Brazil - Fixed Florianopolis', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (486, 1, 30, N'Brazil - Fixed Porto Alegre', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (487, 1, 30, N'Brazil - Fixed Brasilia', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (488, 1, 30, N'Brazil - Fixed Goiania', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (489, 1, 5, N'Andorra - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (490, 1, 5, N'Andorra - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (491, 1, 12, N'Armenia - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (492, 1, 12, N'Armenia -EREVAN', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (493, 1, 12, N'Armenia - Fixed Yerevan', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (494, 1, 12, N'Armenia - Mobile Orange', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (495, 1, 12, N'Armenia - Mobile Armentel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (496, 1, 12, N'Armenia - Fixed Nagorno Karabakh', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (497, 1, 12, N'Armenia - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (498, 1, 12, N'Armenia -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (499, 1, 12, N'Armenia -Mob K-TELECOM', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (500, 1, 12, N'Armenia - Mobile Nagorno Karabach', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (501, 1, 200, N'Slovenia - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (502, 1, 200, N'Slovenia - Fixed Type A', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (503, 1, 200, N'Slovenia - Fixed Type B', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (504, 1, 200, N'Slovenia -Fix ALTERNATIVE NETWORKS A', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (505, 1, 200, N'Slovenia - Mobile Simobil', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (506, 1, 200, N'Slovenia - Mobile Mobitel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (507, 1, 200, N'Slovenia - Mobile IPKO', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (508, 1, 200, N'Slovenia - Fixed IPKO', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (509, 1, 200, N'Slovenia - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (510, 1, 200, N'Slovenia -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (511, 1, 200, N'Slovenia - Mobile Tusmobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (512, 1, 53, N'Cyprus - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (513, 1, 53, N'Cyprus - Mobile MTN', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (514, 1, 53, N'Cyprus - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (515, 1, 53, N'Cyprus -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (516, 1, 53, N'Cyprus - Mobile CYTA', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (517, 1, 53, N'Cyprus - Personal Numbering', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (518, 1, 53, N'Cyprus - Mobility services', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (519, 1, 74, N'Finland - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (520, 1, 74, N'Finland - Mobility Services', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (521, 1, 74, N'Finland -SERVICES NUMBERS', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (522, 1, 74, N'Finland - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (523, 1, 74, N'Finland - Mobile Sonera', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (524, 1, 74, N'Finland - Mobile Finnet', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (525, 1, 74, N'Finland - Personal Numbering', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (526, 1, 195, N'Serbia - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (527, 1, 195, N'Serbia - Fixed Orion', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (528, 1, 195, N'Serbia - Fixed Belgrade', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (529, 1, 195, N'Serbia - Fixed Kosovo', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (530, 1, 195, N'Serbia - Fixed IPKO Kosovo', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (531, 1, 195, N'Serbia - Mobile Vipnet', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (532, 1, 195, N'Serbia - Mobile MOBTEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (533, 1, 195, N'Serbia - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (534, 1, 102, N'Iceland - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (535, 1, 102, N'Iceland - Mobile SIMINN', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (536, 1, 102, N'Iceland - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (537, 1, 102, N'Iceland -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (538, 1, 102, N'Iceland - Mobile Vodafone', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (539, 1, 102, N'Iceland -Mob VODAPHONE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (540, 1, 68, N'Estonia - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (541, 1, 68, N'Estonia -PREMIUM', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (542, 1, 68, N'Estonia - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (543, 1, 68, N'Estonia - Mobile EMT', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (544, 1, 68, N'Estonia -MOBILITY SERVICES', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (545, 1, 68, N'Estonia -Mob ELISA', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (546, 1, 68, N'Estonia - Mobile Tele2', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (547, 1, 68, N'Estonia - Mobile Radiolinja', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (548, 1, 68, N'Estonia - Mobility Services', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (549, 1, 68, N'Estonia -Mob TELE2', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (550, 1, 143, N'Malta - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (551, 1, 143, N'Malta -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (552, 1, 143, N'Malta - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (553, 1, 143, N'Malta -Mob VODAPHONE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (554, 1, 143, N'Malta - Mobile Vodafone', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (555, 1, 101, N'Hungary - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (556, 1, 101, N'Hungary - Fixed Budapest', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (557, 1, 101, N'Hungary - Mobile Pannon', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (558, 1, 101, N'Hungary - Mobile TMobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (559, 1, 101, N'Hungary - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (560, 1, 101, N'Hungary - Mobile Vodafone', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (561, 1, 135, N'Luxemburg - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (562, 1, 135, N'Luxemburg - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (563, 1, 135, N'Luxemburg - Mobile LUX GSM', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (564, 1, 135, N'Luxemburg - Mobile Vox', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (565, 1, 135, N'Luxemburg - Mobile Tangom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (566, 1, 135, N'Luxemburg - Non Geographical Number', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (567, 1, 116, N'Ireland - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (568, 1, 116, N'Ireland -Mob ACCESS', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (569, 1, 116, N'Ireland -VOIP', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (570, 1, 116, N'Ireland - Mobile Vodafone', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (571, 1, 116, N'Ireland - Mobile H3G', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (572, 1, 116, N'Ireland - Mobile Meteor', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (573, 1, 116, N'Ireland - Mobile O2', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (574, 1, 116, N'Ireland - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (575, 1, 116, N'Ireland - Mobile TESCO', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (576, 1, 34, N'Bulgaria - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (577, 1, 34, N'Bulgaria - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (578, 1, 34, N'Bulgaria - Fixed Sofia', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (579, 1, 34, N'Bulgaria - Fixed OLO', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (580, 1, 34, N'Bulgaria -Fix ALTERNATIVE NETWORKS', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (581, 1, 34, N'Bulgaria - Mobile Globul', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (582, 1, 34, N'Bulgaria - Mobile BTC', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (583, 1, 34, N'Bulgaria - Mobile Mobiltel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (584, 1, 34, N'Bulgaria - Mobility services Mobitel Type B', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (585, 1, 34, N'Bulgaria - Mobility services Mobitel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (586, 1, 34, N'Bulgaria - Mobility services TYPE_A', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (587, 1, 22, N'Belgium - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (588, 1, 22, N'Belgium - Mobile Proximus', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (589, 1, 22, N'Belgium - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (590, 1, 22, N'Belgium - Mobile Telenet', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (591, 1, 22, N'Belgium -Mob BASE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (592, 1, 22, N'Belgium - Mobile Base', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (593, 1, 22, N'Belgium - Mobile Mobistar', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (594, 1, 22, N'Belgium - Universal Access Number', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (595, 1, 134, N'Lithuania - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (596, 1, 134, N'Lithuania - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (597, 1, 134, N'Lithuania - Mobile Tele2', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (598, 1, 134, N'Lithuania - Mobile Omnitel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (599, 1, 134, N'Lithuania - Mobile BITE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (600, 1, 134, N'Lithuania -Mob CSC', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (601, 1, 134, N'Lithuania - Mobility Services', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (602, 1, 134, N'Lithuania -Mob OMNITEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (603, 1, 84, N'Gibraltar - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (604, 1, 84, N'Gibraltar - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (605, 1, 84, N'Gibraltar -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (606, 1, 84, N'Gibraltar -Mob EAZITEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (607, 1, 72, N'Faeroe - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (608, 1, 72, N'Faeroe - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (609, 1, 72, N'Faroe isl -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (610, 1, 89, N'Greenland - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (611, 1, 89, N'Greenland - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (612, 1, 179, N'Peru - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (613, 1, 179, N'Peru - Fixed Lima', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (614, 1, 179, N'Peru - Rural', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (615, 1, 179, N'Peru - High Cost', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (616, 1, 179, N'Peru - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (617, 1, 179, N'Peru - Mobile Claro', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (618, 1, 179, N'Peru -Mob MOVISTAR', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (619, 1, 179, N'Peru - Mobile Telefonica', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (620, 1, 179, N'Peru -Mob CLARO', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (621, 1, 13, N'Aruba - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (622, 1, 13, N'Aruba - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (623, 1, 13, N'Aruba - Mobile Digicel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (624, 1, 13, N'Aruba -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (625, 1, 207, N'St Helena', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (626, 1, 123, N'North Korea', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (627, 1, 205, N'Spain - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (628, 1, 205, N'Spain - Mobile Telefonica', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (629, 1, 205, N'Spain - Mobile Vodafone', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (630, 1, 205, N'Spain - Mobile Xfera', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (631, 1, 205, N'Spain - Mobile ORANGE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (632, 1, 205, N'Spain - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (633, 1, 205, N'Spain -Mob VODAFONE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (634, 1, 205, N'Spain -Mob TELEFONICA MOVIL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (635, 1, 205, N'Spain -Mob ORANGE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (636, 1, 205, N'SPAIN CANARY IS', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (637, 1, 205, N'Spain-Madrid Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (638, 1, 205, N'Spain-Barcelona Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (639, 1, 67, N'Eritrea - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (640, 1, 67, N'Eritrea - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (641, 1, 225, N'Thuraya Satellite Telecom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (642, 1, 47, N'Comoros', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (643, 1, 47, N'Comoros - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (644, 1, 47, N'Comoros - Mobility Services', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (645, 1, 151, N'Monaco - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (646, 1, 151, N'Monaco - Mobile Kosovo', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (647, 1, 151, N'Monaco -Mob KFOR', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (648, 1, 151, N'Monaco - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (649, 1, 151, N'Monaco - Fixed OLO', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (650, 1, 161, N'Netherlands - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (651, 1, 161, N'Netherlands Mobile KPN Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (652, 1, 161, N'Netherlands - Mobile Vodafone', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (653, 1, 161, N'Netherlands - Mobile T-Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (654, 1, 161, N'Netherlands - Mobile O2', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (655, 1, 161, N'Netherlands -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (656, 1, 161, N'Netherlands - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (657, 1, 161, N'Netherlands - Mobile Tismi', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (658, 1, 161, N'Netherlands - Mobile TELE2', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (659, 1, 161, N'Netherlands - Mobile Lyca', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (660, 1, 161, N'Netherlands -Mob LYCAMOBILE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (661, 1, 88, N'Greece - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (662, 1, 88, N'Greece - Fixed Athens', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (663, 1, 88, N'Greece - Mobile Wind', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (664, 1, 88, N'Greece - Mobile Vodafone', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (665, 1, 88, N'Greece - Mobile Cosmote', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (666, 1, 254, N'Zimbabwe - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (667, 1, 254, N'Zimbabwe - Mobile Netone', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (668, 1, 254, N'Zimbabwe - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (669, 1, 254, N'Zimbabwe - Mobile Econet', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (670, 1, 130, N'Lesotho - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (671, 1, 130, N'Lesotho - Mobile Vodacom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (672, 1, 130, N'Lesotho - Mobile ECONET', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (673, 1, 158, N'Namibia - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (674, 1, 158, N'Namibia - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (675, 1, 158, N'Namibia -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (676, 1, 156, N'Mozambique - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (677, 1, 156, N'Mozambique - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (678, 1, 156, N'Mozambique - Mobile Vodacom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (679, 1, 156, N'Mozambique - Mobile Movitel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (680, 1, 29, N'Botswana - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (681, 1, 29, N'Botswana -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (682, 1, 29, N'Botswana - Mobile MTN', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (683, 1, 29, N'Botswana - Mobile ORANGE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (684, 1, 29, N'Botswana - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (685, 1, 138, N'Madagascar - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (686, 1, 138, N'Madagascar - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (687, 1, 138, N'Madagascar - Mobile Orange', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (688, 1, 138, N'Madagascar - Mobile Madacom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (689, 1, 138, N'Madagascar - Mobility Services', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (690, 1, 214, N'Swaziland - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (691, 1, 214, N'Swaziland - Mobile MTN', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (692, 1, 214, N'Swaziland - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (693, 1, 36, N'Burundi - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (694, 1, 36, N'Burundi - Mobile Telecel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (695, 1, 36, N'Burundi - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (696, 1, 36, N'Burundi - Mobile Onatel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (697, 1, 36, N'Burundi - Mobile AFRICL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (698, 1, 220, N'Tanzania - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (699, 1, 220, N'Tanzania -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (700, 1, 220, N'Tanzania - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (701, 1, 220, N'Tanzania - Mobile Mobtel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (702, 1, 220, N'Tanzania - Mobile Celtza', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (703, 1, 220, N'Tanzania - Mobile Vodacom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (704, 1, 220, N'Tanzania - Mobile Zantel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (705, 1, 100, N'Hong Kong - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (706, 1, 100, N'Hong Kong - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (707, 1, 100, N'Hong Kong - Mobile CSL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (708, 1, 100, N'Hong Kong - Mobile Smartone', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (709, 1, 100, N'Hong Kong - Mobile Hutchison', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (710, 1, 100, N'Hong Kong -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (711, 1, 236, N'Uganda - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (712, 1, 236, N'Uganda - Fixed Warid', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (713, 1, 236, N'Uganda - Fixed Smile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (714, 1, 236, N'Uganda -Mob ORANGE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (715, 1, 236, N'Uganda - Fixed MTN', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (716, 1, 236, N'Uganda - Mobile MTN', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (717, 1, 236, N'Uganda - Mobile Gemtel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (718, 1, 236, N'Uganda -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (719, 1, 236, N'Uganda - Mobile Warid', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (720, 1, 236, N'Uganda - Mobile Ugatel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (721, 1, 236, N'Uganda - Mobile Smile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (722, 1, 236, N'Uganda - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (723, 1, 236, N'Uganda - Mobile Celtel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (724, 1, 236, N'Uganda - Mobile Orange', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (725, 1, 11, N'Argentina - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (726, 1, 11, N'Argentina - Fixed Buenos Aires', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (727, 1, 11, N'Argentina - Fixed Corridor', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (728, 1, 11, N'Argentina - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (729, 1, 28, N'Bosnia and Herzegovina - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (730, 1, 28, N'Bosnia and Herzegovina - Fixed BIH', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (731, 1, 28, N'Bosnia and Herzegovina - Fixed SRPSKE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (732, 1, 28, N'Bosnia and Herzegovina - Fixed Mostar', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (733, 1, 28, N'Bosnia Herz -Fix ALTERNATIVE NETWORKS', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (734, 1, 28, N'Bosnia and Herzegovina - Mobile BIH', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (735, 1, 28, N'Bosnia and Herzegovina - Mobile Eronet', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (736, 1, 28, N'Bosnia Herz -Mob ERONET', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (737, 1, 28, N'Bosnia and Herzegovina - Mobile SRPSKE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (738, 1, 28, N'Bosnia Herz -Mob MOBIS', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (739, 1, 186, N'Reunion', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (740, 1, 186, N'Reunion - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (741, 1, 186, N'Mayotte - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (742, 1, 186, N'Mayotte - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (743, 1, 186, N'Mayotte -Mob ORANGE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (744, 1, 186, N'Mayotte -Mob SRR MAYOTTE TEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (745, 1, 186, N'Reunion - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (746, 1, 186, N'Reunion - Mobile Orange', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (747, 1, 186, N'Reunion - Mobile SRR', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (748, 1, 186, N'Reunion -Mob SRR', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (749, 1, 253, N'Zambia - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (750, 1, 253, N'Zambia - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (751, 1, 253, N'Zambia - Mobile Zamtel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (752, 1, 253, N'Zambia - Mobile MTN', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (753, 1, 253, N'Zambia - Mobile Celtel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (754, 1, 14, N'Ascension', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (755, 1, 202, N'Somalia - Fixed Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (756, 1, 202, N'Somalia -HORMUD', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (757, 1, 202, N'Somalia - Fixed Telcom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (758, 1, 202, N'Somalia - Hormuud', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (759, 1, 202, N'Somalia -NATIONLINK', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (760, 1, 202, N'Somalia -TELESOM', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (761, 1, 202, N'Somalia -SOTELCO', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (762, 1, 202, N'Somalia - TELESO', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (763, 1, 202, N'Somalia - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (764, 1, 202, N'Somalia - Mobility Services', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (765, 1, 202, N'Somalia -GOLIS', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (766, 1, 202, N'Somalia - GOLIS', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (767, 1, 202, N'Somalia - Mobile Somafone', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (768, 1, 202, N'Somalia -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (769, 1, 31, N'Diego Garcia - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (770, 1, 31, N'Diego Garcia - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (771, 1, 96, N'Guinea-Bissau - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (772, 1, 96, N'Guinea-Bissau - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (773, 1, 96, N'Guinea-Bissau - Mobile MTN', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (774, 1, 57, N'Djibouti - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (775, 1, 57, N'Djibouti -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (776, 1, 57, N'Djibouti - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (777, 1, 212, N'Sudan - Fixed Sudatel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (778, 1, 212, N'Sudan - Mobile Sudani', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (779, 1, 212, N'Sudan - Fixed Kanartel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (780, 1, 212, N'Sudan - Mobile Zain', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (781, 1, 212, N'Sudan - Mobile MTN', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (782, 1, 212, N'Sudan North -Mob VIVACELL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (783, 1, 6, N'Angola - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (784, 1, 6, N'Angola -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (785, 1, 6, N'Angola - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (786, 1, 6, N'Angola - Mobile Unitel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (787, 1, 192, N'Sao Tome &amp; Principe - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (788, 1, 192, N'Sao Tome &amp; Principe - Mobile UNITEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (789, 1, 192, N'Sao Tome &amp; Principe - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (790, 1, 185, N'Congo - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (791, 1, 185, N'Congo - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (792, 1, 185, N'Congo - Mobile Warid', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (793, 1, 185, N'Congo - Mobile Celtel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (794, 1, 185, N'Congo - Mobile MTN', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (795, 1, 69, N'Ethiopia - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (796, 1, 69, N'Ethiopia - Fixed Addis Abeba', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (797, 1, 69, N'Ethiopia -ADDIS ABEBA', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (798, 1, 69, N'Ethiopia - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (799, 1, 69, N'Ethiopia - Mobile ETA Addis Abeba', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (800, 1, 69, N'Ethiopia -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (801, 1, 69, N'Ethiopia -Mob ADDIS ABEBA', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (802, 1, 79, N'Gabon - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (803, 1, 79, N'Gabon - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (804, 1, 79, N'Gabon - Mobile Usan', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (805, 1, 79, N'Gabon - Mobile CELTEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (806, 1, 79, N'Gabon - Mobile TELCEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (807, 1, 152, N'Mongolia - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (808, 1, 152, N'Mongolia - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (809, 1, 55, N'DR of Congo - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (810, 1, 55, N'DR of Congo - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (811, 1, 55, N'DR of Congo - Mobile Sait', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (812, 1, 55, N'DR of Congo - Mobility Services', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (813, 1, 55, N'DR of Congo - Mobile Vodacom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (814, 1, 55, N'DR of Congo - Mobile COCHTL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (815, 1, 55, N'DR of Congo - Mobility Services Type A', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (816, 1, 55, N'DR of Congo - Mobile TATEM', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (817, 1, 55, N'DR of Congo - Mobile AFRICELL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (818, 1, 55, N'DR of Congo - Mobile Celtel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (819, 1, 189, N'Rwanda - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (820, 1, 189, N'Rwanda - Fixed MTN', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (821, 1, 189, N'Rwanda - Mobile Tigo', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (822, 1, 189, N'Rwanda - Mobile AIRTEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (823, 1, 189, N'Rwanda - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (824, 1, 189, N'Rwanda - Mobile MTN', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (825, 1, 196, N'Seychelles', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (826, 1, 196, N'Seychelles - Mobil', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (827, 1, 196, N'Seychelles - Mobile Airtel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (828, 1, 196, N'Seychelles -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (829, 1, 196, N'Seychelles - Mobile Smartcom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (830, 1, 38, N'Cameroon - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (831, 1, 38, N'Cameroon - Fixed Douala', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (832, 1, 38, N'Cameroon - Mobile MTN', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (833, 1, 38, N'Cameroon - Mobile Orange', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (834, 1, 38, N'Cameroon - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (835, 1, 39, N'Cape Verde - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (836, 1, 39, N'Cape Verde -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (837, 1, 39, N'Cape Verde - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (838, 1, 39, N'Cape Verde - Mobile TPLUS', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (839, 1, 42, N'Chad - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (840, 1, 42, N'Chad -Mob CDMA', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (841, 1, 42, N'Chad - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (842, 1, 42, N'Chad - Mobile Celtel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (843, 1, 42, N'Chad - Mobile Millicom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (844, 1, 197, N'Sierra Leone - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (845, 1, 197, N'Sierra Leone -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (846, 1, 197, N'Sierra Leone - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (847, 1, 226, N'Togo - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (848, 1, 226, N'Togo - Interactive TV', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (849, 1, 226, N'Togo - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (850, 1, 226, N'Togo -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (851, 1, 226, N'Togo - Mobile ATL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (852, 1, 165, N'Niger - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (853, 1, 165, N'Niger -Mob ORANGE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (854, 1, 165, N'Niger - Mobile Orange', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (855, 1, 165, N'Niger - Mobile Celtel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (856, 1, 165, N'Niger - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (857, 1, 165, N'Niger -Mob SAHELCOM', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (858, 1, 165, N'Niger - Mobility services', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (859, 1, 165, N'Niger - Mobile Telecel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (860, 1, 24, N'Benin - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (861, 1, 24, N'Benin - Mobile MTN', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (862, 1, 24, N'Benin - Mobile TELCEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (863, 1, 24, N'Benin - Mobile GLOMOB', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (864, 1, 24, N'Benin - Mobile Libercom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (865, 1, 24, N'Benin - Mobile Bell Benin', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (866, 1, 146, N'Mauritius - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (867, 1, 146, N'Mauritius - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (868, 1, 146, N'Mauritius - Mobile Emtel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (869, 1, 35, N'Burkina Faso - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (870, 1, 35, N'Burkina Faso - Interactive TV', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (871, 1, 35, N'Burkina Faso - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (872, 1, 35, N'Burkina Faso - Mobile Celtel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (873, 1, 35, N'Burkina Faso - Mobile Telecel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (874, 1, 35, N'Burkina Faso -Mob AIRTEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (875, 1, 131, N'Liberia - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (876, 1, 131, N'Liberia - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (877, 1, 131, N'Liberia - Mobility services', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (878, 1, 131, N'Liberia - Mobile Comium', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (879, 1, 131, N'Liberia - Mobile MTN', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (880, 1, 131, N'Liberia - Mobile Cellcom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (881, 1, 117, N'Ivory Coast - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (882, 1, 117, N'Ivory Coast - Mobile Moov', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (883, 1, 117, N'Ivory Coast - Mobile MTN', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (884, 1, 117, N'Ivory Coast - Mobile Orange', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (885, 1, 117, N'Ivory Coast - Fixed MTN', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (886, 1, 117, N'Ivory Coast - Mobile Oricel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (887, 1, 117, N'Ivory Coast - Mobile COMIUM', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (888, 1, 117, N'Ivory Coast - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (889, 1, 117, N'Ivory Coast - Mobility Services', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (890, 1, 145, N'Mauritania', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (891, 1, 145, N'Mauritania - Mobile CHINGUITEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (892, 1, 145, N'Mauritania - Fixed CHITEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (893, 1, 145, N'Mauritania - Mobile MATTEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (894, 1, 145, N'Mauritania -Fix MATTEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (895, 1, 145, N'Mauritania - Mobile MAURITEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (896, 1, 95, N'Guinea - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (897, 1, 95, N'Guinea Conakry -Mob AREEBA', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (898, 1, 95, N'Guinea Conakry -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (899, 1, 95, N'Guinea Conakry -Mob SOTELGUI', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (900, 1, 95, N'Guinea Conakry -Mob ORANGE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (901, 1, 95, N'Guinea - Mobile Orange', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (902, 1, 95, N'Guinea - Mobile INTCEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (903, 1, 95, N'Guinea - Mobility services', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (904, 1, 95, N'Guinea - Mobile CELCOM', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (905, 1, 95, N'Guinea - Mobile MTN', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (906, 1, 95, N'Guinea Conakry -Mob CELLCOM', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (907, 1, 95, N'Guinea - Mobile GAMMA', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (908, 1, 95, N'Guinea - Mobile MAI', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (909, 1, 41, N'Central African Republic - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (910, 1, 41, N'Central African Republic - Mobile ACELL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (911, 1, 41, N'Central African Republic - Mobile ORANGE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (912, 1, 41, N'Central African Republic - Mobile TELCEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (913, 1, 41, N'Central African Republic - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (914, 1, 142, N'Mali - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (915, 1, 142, N'Mali - Fixed Bamako', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (916, 1, 142, N'Mali -ORANGE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (917, 1, 142, N'Mali - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (918, 1, 142, N'Mali - Mobile Ikatel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (919, 1, 132, N'Libya - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (920, 1, 132, N'Libya - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (921, 1, 80, N'Gambia - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (922, 1, 80, N'Gambia - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (923, 1, 80, N'Gambia -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (924, 1, 203, N'South Africa - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (925, 1, 203, N'South Africa - Fixed MTN Business VoIP', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (926, 1, 203, N'South Africa - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (927, 1, 203, N'South Africa - Mobile MTN', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (928, 1, 203, N'South Africa - Mobile Vodacom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (929, 1, 203, N'South Africa - Mobile Cell-C', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (930, 1, 203, N'South Africa - Mobile TELKOM', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (931, 1, 203, N'South Africa - Fixed OLO', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (932, 1, 139, N'Malawi Mobility Services', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (933, 1, 139, N'Malawi - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (934, 1, 139, N'Malawi - Mobile Access', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (935, 1, 139, N'Malawi - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (936, 1, 139, N'Malawi - Mobile Celtel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (937, 1, 194, N'Senegal - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (938, 1, 194, N'Senegal - Fixed Expresso', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (939, 1, 194, N'Senegal - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (940, 1, 194, N'Senegal - Mobile Sentel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (941, 1, 194, N'Senegal - Mobile Orange', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (942, 1, 231, N'Tunisia - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (943, 1, 231, N'Tunisia - Mobile Orascom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (944, 1, 231, N'Tunisia -Fix ORANGE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (945, 1, 231, N'Tunisia - Fixed ORANGE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (946, 1, 231, N'Tunisia - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (947, 1, 231, N'Tunisia - Mobile ORANGE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (948, 1, 231, N'Tunisia -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (949, 1, 217, N'Syria - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (950, 1, 217, N'Syria -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (951, 1, 217, N'Syria - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (952, 1, 217, N'Syria - Mobile MTN', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (953, 1, 66, N'Equatorial Guinea - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (954, 1, 66, N'Equatorial Guinea- Mobile Orange', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (955, 1, 66, N'Equatorial Guinea - Fixed ORANGE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (956, 1, 66, N'Equatorial Guinea - Fixed Greencom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (957, 1, 66, N'Equatorial Guinea- Mobile Greencom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (958, 1, 66, N'Equatorial Guinea - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (959, 1, 204, N'South Sudan - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (960, 1, 204, N'South Sudan - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (961, 1, 204, N'South Sudan - Mobile ZAIN', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (962, 1, 204, N'South Sudan - Mobile MTN', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (963, 1, 204, N'South Sudan - Mobile VIVCEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (964, 1, 204, N'South Sudan - Mobile GEMTEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (965, 1, 86, N'International Networks - DTAG', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (966, 1, 86, N'Maritime Communications Partner', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (967, 1, 86, N'International Networks Oration Technologies', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (968, 1, 86, N'International Networks Navita', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (969, 1, 86, N'International Networks - Ellipso', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (970, 1, 86, N'International Networks - VODAFO', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (971, 1, 86, N'International Networks - Intermatica', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (972, 1, 86, N'International Networks - Inphon', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (973, 1, 86, N'International Networks Onair', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (974, 1, 86, N'International Networks - AeroMobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (975, 1, 62, N'Egypt - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (976, 1, 62, N'Egypt - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (977, 1, 62, N'Egypt - Mobile Etisalat', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (978, 1, 62, N'Egypt - Mobile MOBINIL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (979, 1, 208, N'Saint Kitts and Nevis - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (980, 1, 208, N'Saint Kitts and Nevis - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (981, 1, 208, N'Saint Kitts and Nevis - Mobile Digicel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (982, 1, 92, N'Guam', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (983, 1, 183, N'Puerto Rico', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (984, 1, 58, N'Dominica', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (985, 1, 58, N'Dominica - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (986, 1, 58, N'Dominica - Mobile Digicel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (987, 1, 4, N'American Samoa', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (988, 1, 155, N'Morocco - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (989, 1, 155, N'Morocco - Fixed Meditel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (990, 1, 155, N'Morocco -CASABLANCA', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (991, 1, 155, N'Morocco - Mobile WANA Far Zone', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (992, 1, 155, N'Morocco - Fixed Wana', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (993, 1, 155, N'Morocco - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (994, 1, 155, N'Morocco - Mobile WANA', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (995, 1, 155, N'Morocco -Mob IAM', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (996, 1, 155, N'Morocco - Mobile Meditelecom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (997, 1, 155, N'Morocco - Mobile Hourri', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (998, 1, 155, N'Morocco - Mobility services', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (999, 1, 211, N'Saint Vincent and the Grenadines - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1000, 1, 211, N'Saint Vincent and the Grenadines - Mobile Digicel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1001, 1, 211, N'Saint Vincent and the Grenadines - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1002, 1, 229, N'Trinidad &amp; Tobago - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1003, 1, 229, N'Trinidad and Tobago - Mobile Digicel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1004, 1, 229, N'Trinidad Tobago -Mob DIGICEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1005, 1, 229, N'Trinidad and Tobago - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1006, 1, 229, N'Trinidad Tobago -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1007, 1, 166, N'Nigeria - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1008, 1, 166, N'Nigeria -Mob MULTI LINKS', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1009, 1, 166, N'Nigeria -Mob VISAFONE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1010, 1, 166, N'Nigeria -Mob RELTEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1011, 1, 166, N'Nigeria - Fixed MTN', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1012, 1, 166, N'Nigeria -Mob STARCOMMS', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1013, 1, 166, N'Nigeria -Fix GLOBACOM', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1014, 1, 166, N'Nigeria - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1015, 1, 166, N'Nigeria - Mobile Celtel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1016, 1, 166, N'Nigeria - Mobile Visafone', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1017, 1, 166, N'Nigeria - Mobile MTN', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1018, 1, 166, N'Nigeria - Mobile Globacom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1019, 1, 166, N'Nigeria -Mob ALPHATECH', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1020, 1, 166, N'Nigeria -Mob NITEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1021, 1, 166, N'Nigeria - Mobile Etisalat', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1022, 1, 154, N'Montserrat - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1023, 1, 154, N'Montserrat - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1024, 1, 209, N'Saint Lucia - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1025, 1, 209, N'Saint Lucia - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1026, 1, 209, N'Saint Lucia -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1027, 1, 209, N'Saint Lucia - Mobile Digicel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1028, 1, 64, N'Global Mobile Satellite System Ellipso', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1029, 1, 169, N'Saipan', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1030, 1, 83, N'Ghana - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1031, 1, 83, N'Ghana - Mobile Ghanat', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1032, 1, 83, N'Ghana - Mobile Glomob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1033, 1, 83, N'Ghana - Mobile MTN', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1034, 1, 83, N'Ghana - Mobile Zain', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1035, 1, 83, N'Ghana - Mobile Millicom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1036, 1, 83, N'Ghana - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1037, 1, 233, N'Turkmenistan - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1038, 1, 233, N'Turkmenistan - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1039, 1, 26, N'Bhutan - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1040, 1, 26, N'Bhutan - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1041, 1, 234, N'Turks and Caicos - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1042, 1, 234, N'Turks and Caicos - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1043, 1, 234, N'Turks and Caicos - Mobile Digicel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1044, 1, 113, N'International Networks - Telenor', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1045, 1, 113, N'International Networks - Mobistar', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1046, 1, 113, N'International Networks - MTT', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1047, 1, 113, N'International Networks - VOXBON', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1048, 1, 113, N'International Networks - ELLIPSAT', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1049, 1, 113, N'International Networks 1 Wins Aero', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1050, 1, 85, N'Iridium', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1051, 1, 127, N'Laos - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1052, 1, 127, N'Laos - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1053, 1, 21, N'Belarus - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1054, 1, 21, N'Belarus - Fixed Minsk', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1055, 1, 21, N'Belarus - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1056, 1, 21, N'Belarus - Mobile MDC', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1057, 1, 21, N'Belarus -Mob MTS', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1058, 1, 21, N'Belarus - Mobility services', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1059, 1, 7, N'Anguilla - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1060, 1, 7, N'Anguilla - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1061, 1, 7, N'Anguilla - Mobile Digicel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1062, 1, 10, N'Antigua and Barbuda - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1063, 1, 10, N'Antigua and Barbuda - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1064, 1, 244, N'United States Virgin Islands', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1065, 1, 162, N'New Caledonia - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1066, 1, 162, N'New Caledonia - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1067, 1, 251, N'Wallis and Futuna - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1068, 1, 251, N'Wallis and Futuna - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1069, 1, 90, N'Grenada - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1070, 1, 90, N'Grenada - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1071, 1, 90, N'Grenada -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1072, 1, 90, N'Grenada - Mobile Digicel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1073, 1, 136, N'Macao - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1074, 1, 136, N'Macao - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1075, 1, 239, N'United Kingdom - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1076, 1, 239, N'Jersey', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1077, 1, 239, N'United Kingdom - Fixed London', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1078, 1, 239, N'United Kingdom - Fixed Wide numbers', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1079, 1, 239, N'United Kingdom - Corporate Numbering', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1080, 1, 239, N'United Kingdom - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1081, 1, 239, N'United Kingdom - Personal Numbers', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1082, 1, 239, N'United Kingdom - Mobile O2', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1083, 1, 239, N'United Kingdom - Mobile H3G', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1084, 1, 239, N'United Kingdom - Mobile Lyca', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1085, 1, 239, N'United Kingdom - Mobile Vodafone', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1086, 1, 239, N'United Kingdom - Mobile Orange', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1087, 1, 239, N'United Kingdom - Mobile T-mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1088, 1, 239, N'United Kingdom - Mobile Alternative', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1089, 1, 239, N'United Kingdom - Mobile Tismi', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1090, 1, 239, N'United Kingdom - Mobility services TISMIB', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1091, 1, 239, N'Jersey -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1092, 1, 239, N'United Kingdom - Local Calls', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1093, 1, 239, N'United Kingdom -Fix NTS', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1094, 1, 239, N'United Kingdom - National Calls', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1095, 1, 18, N'Bahrain - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1096, 1, 18, N'Bahrain - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1097, 1, 18, N'Bahrain - Mobile STC', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1098, 1, 18, N'Bahrain - Mobile MTC Vodafone', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1099, 1, 18, N'Bahrain -Mob ZAIN', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1100, 1, 18, N'Bahrain - Fixed Wimax MENA', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1101, 1, 141, N'Maldives - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1102, 1, 141, N'Maldives - Mobile Dhiraagu', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1103, 1, 141, N'Maldives - Mobility services', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1104, 1, 141, N'Maldives -Mob WATANIYA', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1105, 1, 141, N'Maldives - Mobility Services Type A', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1106, 1, 141, N'Maldives - Mobile Wataniya', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1107, 1, 44, N'China - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1108, 1, 44, N'China - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1109, 1, 44, N'China - Mobile UNCCL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1110, 1, 111, N'Inmarsat B (SNAC) Except Pacific', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1111, 1, 111, N'Inmarsat HSD SNAC Except Pacific', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1112, 1, 111, N'Inmarsat AERO SNAC', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1113, 1, 111, N'Inmarsat M4 SNAC', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1114, 1, 111, N'Inmarsat M (SNAC) Except Pacific', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1115, 1, 111, N'Inmarsat Mini-M (SNAC) Except Pacific', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1116, 1, 111, N'Inmarsat BGAN SNAC', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1117, 1, 111, N'Inmarsat BGAN HSD SNAC', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1118, 1, 19, N'Bangladesh - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1119, 1, 19, N'Bangladesh - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1120, 1, 19, N'Bangladesh - Fixed Dhakka', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1121, 1, 19, N'Bangladesh - Fixed Chittagong', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1122, 1, 19, N'Bangladesh - Fixed Sylhet', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1123, 1, 40, N'Cayman Islands - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1124, 1, 40, N'Cayman Islands - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1125, 1, 40, N'Cayman Islands - Mobile Digicel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1126, 1, 120, N'Jordan - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1127, 1, 120, N'Jordan -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1128, 1, 120, N'Jordan - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1129, 1, 120, N'Jordan - Mobile Fastlink', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1130, 1, 120, N'Jordan -Mob ORANGE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1131, 1, 120, N'Jordan - Mobile Mobilecom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1132, 1, 120, N'Jordan - Mobile Umniah', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1133, 1, 120, N'Jordan -Mob ZAIN', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1134, 1, 252, N'Yemen - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1135, 1, 252, N'Yemen - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1136, 1, 252, N'Yemen - Mobile Sabafon', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1137, 1, 252, N'Yemen - Mobile MTN', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1138, 1, 252, N'Yemen - Mobile Yementel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1139, 1, 252, N'Yemen -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1140, 1, 118, N'Jamaica - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1141, 1, 118, N'Jamaica - Mobile CW', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1142, 1, 118, N'Jamaica - Mobile Digicel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1143, 1, 118, N'Jamaica -Mob DIGICEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1144, 1, 118, N'Jamaica -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1145, 1, 8, N'International Networks - Global Networks', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1146, 1, 8, N'International Networks - TPG', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1147, 1, 129, N'Lebanon - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1148, 1, 129, N'Lebanon - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1149, 1, 171, N'Oman - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1150, 1, 171, N'Oman - Mobile Nawras', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1151, 1, 171, N'Oman - Fixed Nawras', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1152, 1, 171, N'Oman - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1153, 1, 171, N'Oman -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1154, 1, 115, N'Iraq - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1155, 1, 115, N'Iraq - Fixed Baghdad', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1156, 1, 115, N'Iraq - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1157, 1, 115, N'Iraq -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1158, 1, 115, N'Iraq - Mobile Sanatel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1159, 1, 115, N'Iraq - Mobile Itisaluna', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1160, 1, 115, N'Iraq - Mobile Fanous', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1161, 1, 115, N'Iraq - Mobile Korek', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1162, 1, 115, N'Iraq - Mobile Asia Cell', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1163, 1, 115, N'Iraq - Mobile Zain', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1164, 1, 25, N'Bermuda', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1165, 1, 172, N'Pakistan Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1166, 1, 172, N'Pakistan-Karachi-Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1167, 1, 172, N'Multinet_Karachi    on-net', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1168, 1, 172, N'Multinet_Hyderabad    on-net', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1169, 1, 172, N'Pakistan-Other-Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1170, 1, 172, N'Pakistan-Mobilink-Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1171, 1, 172, N'Pakistan-Zong-Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1172, 1, 172, N'Pakistan-Warid-Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1173, 1, 172, N'Pakistan-Ufone-Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1174, 1, 172, N'Pakistan-Telenor-Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1175, 1, 172, N'Pakistan Mobile_SCO', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1176, 1, 172, N'Multinet_Sahiwal    on-net', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1177, 1, 172, N'Pakistan-Faisalabad-Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1178, 1, 172, N'Multinet_Faisalabad    on-net', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1179, 1, 172, N'Pakistan-Lahore-Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1180, 1, 172, N'Multinet_Lahore    on-net', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1181, 1, 172, N'Multinet_Kasur on-net', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1182, 1, 172, N'Pakistan-Islamabad-Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1183, 1, 172, N'Multinet_Islamabad    on-net', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1184, 1, 172, N'Multinet_Gujranwala    on-net', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1185, 1, 172, N'Multinet_Jhelum    on-net', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1186, 1, 172, N'Multinet_Shaikhupura on-net', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1187, 1, 172, N'Multinet_Multan    on-net', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1188, 1, 172, N'Multinet_Rahim Yar Khan on-net', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1189, 1, 172, N'Multinet_Sukkur    on-net', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1190, 1, 172, N'Multinet_Quetta    on-net', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1191, 1, 172, N'Multinet_Peshawar    on-net', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1192, 1, 172, N'Multinet_D. I. Khan    on-net', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1193, 1, 172, N'Multinet_Abbottabad    on-net', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1194, 1, 32, N'British Virgin Islands', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1195, 1, 32, N'British Virgin Islands - Mobile Digicel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1196, 1, 32, N'British Virgin Islands - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1197, 1, 16, N'Azerbaijan - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1198, 1, 16, N'Azerbaijan -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1199, 1, 16, N'Azerbaijan - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1200, 1, 16, N'Azerbaijan - Mobile Bakcell', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1201, 1, 157, N'Myanmar - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1202, 1, 157, N'Myanmar - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1203, 1, 157, N'Myanmar - Mobility Services', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1204, 1, 157, N'Myanmar - Mobile Telenor', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1205, 1, 157, N'Myanmar - Mobile Ooredoo', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1206, 1, 150, N'Moldova - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1207, 1, 150, N'Moldova - Fixed Pridnestrovje', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1208, 1, 150, N'Moldova -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1209, 1, 150, N'Moldova - Mobile Voxtel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1210, 1, 150, N'Moldova - Mobile Moldtelecom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1211, 1, 150, N'Moldova - Mobile Moldcell', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1212, 1, 150, N'Moldova - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1213, 1, 193, N'Saudi Arabia - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1214, 1, 193, N'Saudi Arabia - Other Mobiles', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1215, 1, 193, N'Saudi Arabia - Mobile STC', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1216, 1, 193, N'Saudi Arabia -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1217, 1, 193, N'Saudi Arabia - Mobile Mobily', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1218, 1, 193, N'Saudi Arabia - Mobile ZAIN', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1219, 1, 87, N'Globalstar TYPE_A', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1220, 1, 87, N'Globalstar TYPE_B', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1221, 1, 87, N'Globalstar TYPE_C', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1222, 1, 37, N'Cambodia - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1223, 1, 37, N'Cambodia - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1224, 1, 37, N'Cambodia - Mobile Smart', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1225, 1, 37, N'Cambodia - Mobile Mfone', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1226, 1, 37, N'Cambodia - Mobile Cellcard', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1227, 1, 37, N'Cambodia -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1228, 1, 37, N'Cambodia - Mobile Hello', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1229, 1, 37, N'Cambodia - Fixed Phnom Phen', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1230, 1, 37, N'Cambodia - Mobile Metfone', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1231, 1, 37, N'Cambodia -Mob SMART', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1232, 1, 37, N'Cambodia -Mob MFONE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1233, 1, 245, N'Uzbekistan - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1234, 1, 245, N'Uzbekistan - Fixed Tashkent', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1235, 1, 245, N'Uzbekistan - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1236, 1, 245, N'Uzbekistan -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1237, 1, 17, N'Bahamas', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1238, 1, 17, N'Bahamas - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1239, 1, 124, N'South Korea - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1240, 1, 124, N'Korea South -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1241, 1, 124, N'South Korea - Mobile SK', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1242, 1, 124, N'South Korea - Mobile KTF', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1243, 1, 124, N'South Korea - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1244, 1, 20, N'Barbados - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1245, 1, 20, N'Barbados - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1246, 1, 20, N'Barbados -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1247, 1, 20, N'Barbados - Mobile Digicel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1248, 1, 105, N'Indonesia - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1249, 1, 105, N'Indonesia - Fixed Jakarta', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1250, 1, 105, N'Indonesia - Fixed Bandung', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1251, 1, 105, N'Indonesia - Fixed Cirebon', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1252, 1, 105, N'Indonesia - Fixed Semarang', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1253, 1, 105, N'Indonesia - Fixed Bogor', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1254, 1, 105, N'Indonesia - Fixed Solo', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1255, 1, 105, N'Indonesia - Fixed Yogyakarta', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1256, 1, 105, N'Indonesia - Fixed Surabaya', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1257, 1, 105, N'Indonesia - Fixed Mojokerto', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1258, 1, 105, N'Indonesia - Fixed Jember', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1259, 1, 105, N'Indonesia - Fixed Banyuwangi', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1260, 1, 105, N'Indonesia - Fixed Malang', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1261, 1, 105, N'Indonesia - Fixed Blitar', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1262, 1, 105, N'Indonesia - Fixed Pasuruan', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1263, 1, 105, N'Indonesia - Fixed Madiun', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1264, 1, 105, N'Indonesia - Fixed Ponorogo', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1265, 1, 105, N'Indonesia - Fixed Tulungagung', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1266, 1, 105, N'Indonesia - Fixed Denpasar', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1267, 1, 105, N'Indonesia - Fixed Makassar', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1268, 1, 105, N'Indonesia - Fixed Manado', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1269, 1, 105, N'Indonesia - Fixed Banjarmasin', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1270, 1, 105, N'Indonesia - Fixed Balikpapan', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1271, 1, 105, N'Indonesia - Fixed Pontianak', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1272, 1, 105, N'Indonesia - Fixed Medan', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1273, 1, 105, N'Indonesia - Fixed Palembang', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1274, 1, 105, N'Indonesia - Fixed Bandarlampung', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1275, 1, 105, N'Indonesia - Fixed Jambi', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1276, 1, 105, N'Indonesia - Fixed Padang', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1277, 1, 105, N'Indonesia - Fixed Pekanbaru', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1278, 1, 105, N'Indonesia - Fixed Batam', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1279, 1, 105, N'Indonesia - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1280, 1, 105, N'Indonesia - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1281, 1, 105, N'Indonesia - Mobile Telkomsel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1282, 1, 105, N'Indonesia - Mobile Indosat', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1283, 1, 105, N'Indonesia - Mobile Excelcom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1284, 1, 105, N'Indonesia - Mobile Satellite', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1285, 1, 105, N'Indonesia -Mob SATELLITE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1286, 1, 2, N'Albania - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1287, 1, 2, N'Albania - Fixed OLO', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1288, 1, 2, N'Albania - Fixed AMC', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1289, 1, 2, N'Albania -Fix AMC', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1290, 1, 2, N'Albania - Interactive TV', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1291, 1, 2, N'Albania - Fixed Tirana', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1292, 1, 2, N'Albania - Fixed ALBTEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1293, 1, 2, N'Albania - Mobile Plus', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1294, 1, 2, N'Albania - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1295, 1, 2, N'Albania - Mobile AMC', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1296, 1, 2, N'Albania - Mobile Vodafone', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1297, 1, 121, N'Kenya - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1298, 1, 121, N'Kenya -NAIROBI', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1299, 1, 121, N'Kenya - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1300, 1, 121, N'Kenya - Mobile Safaricom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1301, 1, 121, N'Kenya -Mob SAFARICOM', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1302, 1, 121, N'Kenya - Mobile CELTEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1303, 1, 121, N'Kenya - Mobile Econet', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1304, 1, 121, N'Kenya -Mob ORANGE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1305, 1, 121, N'Kenya -Mob AIRTEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1306, 1, 241, N'Universal Personal Telecommunications - TPG', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1307, 1, 201, N'Solomon Islands - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1308, 1, 201, N'Solomon Islands - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1309, 1, 218, N'Taiwan - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1310, 1, 218, N'Taiwan -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1311, 1, 218, N'Taiwan - Mobile FarEastTone', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1312, 1, 218, N'Taiwan - Mobile Chungwa', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1313, 1, 218, N'Taiwan - Mobile Vibo', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1314, 1, 218, N'Taiwan - Mobile Taiwan Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1315, 1, 218, N'Taiwan - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1316, 1, 218, N'Taiwan -Mob CHUNGHWA', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1317, 1, 218, N'Taiwan - Mobility services', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1318, 1, 218, N'Taiwan -Mob FETG', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1319, 1, 3, N'Algeria - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1320, 1, 3, N'Algeria - Fixed CAT', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1321, 1, 3, N'Algeria -ALGER', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1322, 1, 3, N'Algeria - Mobile Wataniya', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1323, 1, 3, N'Algeria -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1324, 1, 3, N'Algeria - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1325, 1, 3, N'Algeria - Mobile Orascom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1326, 1, 3, N'Algeria -Mob ORASCOM', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1327, 1, 128, N'Latvia - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1328, 1, 128, N'Latvia - Mobile Tele2', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1329, 1, 128, N'Latvia - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1330, 1, 128, N'Latvia - Mobile Bite', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1331, 1, 128, N'Latvia - Mobile LMT', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1332, 1, 128, N'Latvia - Mobile Baltija', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1333, 1, 128, N'Latvia - Mobility services', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1334, 1, 128, N'Latvia - Mobile CAMEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1335, 1, 128, N'Latvia - Mobile Master', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1336, 1, 128, N'Latvia -Mob BITE LATVIJA', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1337, 1, 128, N'Latvia - Fixed OLO', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1338, 1, 128, N'Latvia - Fixed Riga', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1339, 1, 128, N'Latvia - Fixed OLO Zone3', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1340, 1, 1, N'Afghanistan - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1341, 1, 1, N'Afghanistan -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1342, 1, 1, N'Afghanistan - Mobile AWCC', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1343, 1, 1, N'Afghanistan - Mobile Roshan', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1344, 1, 1, N'Afghanistan - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1345, 1, 1, N'Afghanistan - Mobile WASEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1346, 1, 1, N'Afghanistan - Mobile MTN', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1347, 1, 1, N'Afghanistan - Mobile Etisalat', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1348, 1, 137, N'Macedonia - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1349, 1, 137, N'Macedonia - Fixed Skopje', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1350, 1, 137, N'Macedonia - Fixed Cosmofon', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1351, 1, 137, N'Macedonia -Fix COSMOFON', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1352, 1, 137, N'Macedonia - Mobile Mobimak', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1353, 1, 137, N'Macedonia -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1354, 1, 137, N'Macedonia - Mobile Cosmofon', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1355, 1, 137, N'Macedonia - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1356, 1, 206, N'Sri Lanka - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1357, 1, 206, N'Sri Lanka - Fixed SLT', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1358, 1, 206, N'Sri Lanka -Fix DIALOG', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1359, 1, 206, N'Sri Lanka - Fixed MTN', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1360, 1, 206, N'Sri Lanka -Fix SLT', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1361, 1, 206, N'Sri Lanka - Mobile Mobitel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1362, 1, 206, N'Sri Lanka - Mobile Celtel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1363, 1, 206, N'Sri Lanka - Mobile Bharti', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1364, 1, 206, N'Sri Lanka - Mobile Dialog Telekom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1365, 1, 206, N'Sri Lanka - Hutch Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1366, 1, 180, N'Philippines - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1367, 1, 180, N'Philippines -MANILA', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1368, 1, 180, N'Philippines - Fixed PLDT', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1369, 1, 180, N'Philippines - Fixed Digitel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1370, 1, 180, N'Philippines - Fixed Globetel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1371, 1, 180, N'Philippines - Fixed Bayantel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1372, 1, 180, N'Philippines - Mobile Smart', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1373, 1, 180, N'Philippines - Mobile Globetel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1374, 1, 180, N'Philippines - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1375, 1, 180, N'Philippines - Mobile Digitel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1376, 1, 240, N'United States', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1377, 1, 240, N'Canada - Fixed Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1378, 1, 240, N'Canada - Fixed Northwest Territories', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1379, 1, 240, N'Netherlands Antilles - Sint maarten', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1380, 1, 240, N'St Maarten -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1381, 1, 240, N'United States - 800', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1382, 1, 240, N'United States - Fixed Hawaii', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1383, 1, 240, N'United States - Fixed Alaska', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1384, 1, 59, N'Dominican Republic - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1385, 1, 59, N'Dominican Republic - Mobile Verizon', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1386, 1, 59, N'Dominican Republic - Santo Domingo', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1387, 1, 59, N'Dominican Republic - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1388, 1, 59, N'Dominican Rep -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1389, 1, 59, N'Dominican Republic - Mobile Orange', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1390, 1, 59, N'Dominican Rep -Mob ORANGE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1391, 1, 182, N'Portugal - Fixed OLO', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1392, 1, 182, N'Portugal - Mobile Vodafone Telecel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1393, 1, 182, N'Portugal - Mobile TMN', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1394, 1, 182, N'Portugal - Mobile Optimus', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1395, 1, 182, N'Portugal -LISBOA', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1396, 1, 182, N'Portugal - Fixed ONI', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1397, 1, 182, N'Portugal - Fixed Novis', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1398, 1, 182, N'Portugal -OLO', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1399, 1, 182, N'Portugal - Fixed Marconi Zone 1', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1400, 1, 182, N'Portugal -PORTO', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1401, 1, 182, N'Portugal - Fixed Marconi Zone 2', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1402, 1, 182, N'Portugal - Fixed Marconi Zone 3', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1403, 1, 182, N'Portugal - Fixed Marconi Zone 4', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1404, 1, 182, N'Portugal - Fixed Marconi Zone 5', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1405, 1, 182, N'Portugal - Madeira and Azores', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1406, 1, 182, N'Portugal -Mob TMNN ONYWAY', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1407, 1, 182, N'Portugal - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1408, 1, 182, N'Portugal -Mob VODAPHONE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1409, 1, 187, N'Romania - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1410, 1, 187, N'Romania - Fixed Bucharest', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1411, 1, 187, N'Romania -Fix ALTERNATIVE NETWORKS', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1412, 1, 187, N'Romania - Fixed OLO Zone1', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1413, 1, 187, N'Romania - Fixed Rodasy', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1414, 1, 187, N'Romania - Fixed Astral', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1415, 1, 187, N'Romania - fixed MOBFON', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1416, 1, 187, N'Romania - fixed ORANGE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1417, 1, 187, N'Romania - Fixed OLO Zone2', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1418, 1, 187, N'Romania - fixed COSMOT', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1419, 1, 187, N'Romania - Mobile Enigma', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1420, 1, 187, N'Romania - Mobile Romtelecom', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1421, 1, 187, N'Romania - Mobile Mobifon', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1422, 1, 187, N'Romania - Mobile Orange', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1423, 1, 187, N'Romania - Mobile COSMOT', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1424, 1, 187, N'Romania -Mob DIGI MOBIL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1425, 1, 187, N'Romania - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1426, 1, 187, N'Romania -Mob TELEMOBIL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1427, 1, 187, N'Romania - Mobile Telemobil', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1428, 1, 104, N'India - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1429, 1, 104, N'India - Fixed New Delhi', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1430, 1, 104, N'India -STATES', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1431, 1, 104, N'India - Fixed Punjab', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1432, 1, 104, N'India -CITIES', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1433, 1, 104, N'India - Fixed Jallandher', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1434, 1, 104, N'India - Fixed Pune', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1435, 1, 104, N'India - Fixed Mumbai', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1436, 1, 104, N'India - Fixed Gujarat', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1437, 1, 104, N'India - Fixed Calcutta', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1438, 1, 104, N'India - Fixed Hyderabad', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1439, 1, 104, N'India - Fixed Tamil Nadu', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1440, 1, 104, N'India - Fixed Madras', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1441, 1, 104, N'India - Fixed Kerala', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1442, 1, 104, N'India - Fixed Ernakulam', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1443, 1, 104, N'India - Mobile Bharti', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1444, 1, 104, N'India - Mobile Idea', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1445, 1, 104, N'India - Mobile Vodafone', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1446, 1, 104, N'India - Mobile Tata', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1447, 1, 104, N'India - Mobile AIRCEL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1448, 1, 104, N'India -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1449, 1, 104, N'India - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1450, 1, 104, N'India - Mobile Reliance', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1451, 1, 104, N'India - Mobile BSNL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1452, 1, 104, N'India - Fixed Ahmadabad', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1453, 1, 104, N'India - Fixed Bangalore', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1454, 1, 104, N'India -Mob BSNL', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1455, 1, 75, N'France - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1456, 1, 75, N'France - Fixed Paris', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1457, 1, 75, N'France - Fixed ILIAD', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1458, 1, 75, N'France - Fixed OLO', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1459, 1, 75, N'France - Fixed OLO Zone 1', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1460, 1, 75, N'France - Fixed OLO Zone 2', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1461, 1, 75, N'France - Fixed OLO Zone 3', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1462, 1, 75, N'France - Fixed OLO Zone 4', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1463, 1, 75, N'France - Fixed OLO Zone 5', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1464, 1, 75, N'France - Fixed OLO Zone 6', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1465, 1, 75, N'France - Fixed OLO Zone 7', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1466, 1, 75, N'France - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1467, 1, 75, N'France -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1468, 1, 75, N'France - Mobile Free', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1469, 1, 75, N'France - Mobile Orange', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1470, 1, 75, N'France - Mobile SFR', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1471, 1, 75, N'France - Mobile Bouygues', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1472, 1, 75, N'France -Mob ORANGE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1473, 1, 75, N'France -Mob BOUYGUES', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1474, 1, 75, N'France -Mob SFR', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1475, 1, 75, N'France - Mobile Virgin', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1476, 1, 75, N'France - Mobile Globalstar', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1477, 1, 75, N'France - Mobile NRJ', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1478, 1, 224, N'Thailand - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1479, 1, 224, N'Thailand -BANGKOK', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1480, 1, 224, N'Thailand Mobile AWN', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1481, 1, 224, N'Thailand - Mobile', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1482, 1, 224, N'Thailand - Mobile AIS', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1483, 1, 224, N'Thailand - Mobile RLFUTR', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1484, 1, 224, N'Thailand - Mobile DTAC Zone 1', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1485, 1, 224, N'Thailand -Mob TRUE', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1486, 1, 224, N'Thailand -Mob DTAC', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1487, 1, 224, N'Thailand - Mobile DTAC Zone 2', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1488, 1, 224, N'Thailand - Mobile DTAC Zone 3', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1489, 1, 224, N'Thailand - Mobile DTAC Zone 4', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1490, 1, 181, N'Poland - Fixed', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1491, 1, 181, N'Poland - Fixed POLTEL Zone 1', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1492, 1, 181, N'Poland -Fix ALTERNATIVE NETWORKS', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1493, 1, 181, N'Poland - Fixed POLTEL Zone 8', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1494, 1, 181, N'Poland - Fixed Warsaw', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1495, 1, 181, N'Poland - Fixed POLTEL Warsaw', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1496, 1, 181, N'Poland - Fixed POLTEL Zone 2', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1497, 1, 181, N'Poland - Fixed POLTEL Zone 3', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1498, 1, 181, N'Poland - Fixed POLTEL Zone 7', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1499, 1, 181, N'Poland - Mobile Centertel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1500, 1, 181, N'Poland - Fixed POLTEL Zone 4', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1501, 1, 181, N'Poland - Mobile Others', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1502, 1, 181, N'Poland - Mobile P4', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1503, 1, 181, N'Poland - Mobile Era', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1504, 1, 181, N'Poland - Mobile Polkomtel', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1505, 1, 181, N'Poland - Fixed POLTEL Zone 5', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1506, 1, 181, N'Poland -Mob', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1507, 1, 181, N'Poland -Mob CYFROWY', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1508, 1, 181, N'Poland -Mob P4', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1509, 1, 181, N'Poland -Mob GSM PLUS', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1510, 1, 181, N'Poland - Mobile Centernet', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO
INSERT [VR_NumberingPlan].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], [CreatedTime]) VALUES (1511, 1, 181, N'Poland - Fixed POLTEL Zone 6', CAST(N'2014-01-01 00:00:00.000' AS DateTime), NULL, NULL, CAST(N'2017-06-23 16:35:30.850' AS DateTime))
GO

