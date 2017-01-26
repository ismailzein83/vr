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


--[common].[extensionconfiguration]-----------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('0897F25A-7BF4-4A4F-A97F-0EE33993633C','Zajil Payment Convertor','Zajil Payment Convertor','VR_BEBridge_BEConvertor'					,'{"Editor":"retail-zajil-payment-convertor-editor"}'),
('26DC208A-7954-4258-A8E5-48497C02EF19','Zajil Account Convertor','Zajil Account Convertor','VR_BEBridge_BEConvertor'					,'{"Editor":"retail-zajil-account-convertor-editor"}'),

('2241197C-B5B0-48E5-987A-B3C1949760CB','Order Details','Order Details', 'Retail_BE_AccountPartDefinition'								,'{"DefinitionEditor":"retail-zajil-accounttype-part-definition-orderdetails", "RuntimeEditor":"retail-zajil-accounttype-part-runtime-orderdetails"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[ConfigType],[Settings]))
merge	[common].[ExtensionConfiguration] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[ConfigType] = s.[ConfigType],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Title],[ConfigType],[Settings])
	values(s.[ID],s.[Name],s.[Title],s.[ConfigType],s.[Settings]);
----------------------------------------------------------------------------------------------------

END


--VR_BEBridge.BEReceiveDefinition-------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('9FABC7A4-DCC4-47B9-84F0-0CEDF8C26A07','Zajil Bridge Definition','{"$type":"Vanrise.BEBridge.Entities.BEReceiveDefinitionSettings, Vanrise.BEBridge.Entities","SourceBEReader":{"$type":"Vanrise.BEBridge.MainExtensions.SourceBEReaders.SqlSourceReader, Vanrise.BEBridge.MainExtensions","Setting":{"$type":"Vanrise.BEBridge.MainExtensions.SourceBEReaders.SqlSourceReaderSetting, Vanrise.BEBridge.MainExtensions","ConnectionString":"Server=192.168.110.195;Database=MVTSPRODEMO;User ID=Developers;Password=dev!123","Query":"select\nCompanyID,\nCompany_Name,\nArabic_Name,\nAddress,\nPhoneNo,\nFaxNo,\nEmail,\nContact_Person,\nMobile,\nSalesAgent,\nCreateDate,\nCRM_Company_ID,\nCRM_Company_AccountNo,\nfinance_contact_person,\nfinance_contact_email,\nfinance_contact_number,\nServiceType,\nRemarks,\nCharges,\nPayment,\nContractPeriod,\nContractRemain,\ncontract_days,\ntotal_contract,\nCharges_Year1,\nCharges_Year2,\nCharges_Year3,\nInstallation,\nThirdParty,\nDiscount,\nAchievement\n\t  from [dbo].[AccountsZeo]\nwhere CompanyID &gt; @LastImportedId\n\t  order by companyid","CommandTimeout":600,"BasedOnId":true,"IdField":"CompanyID"},"ConfigId":"287a031d-d476-4b03-9478-76dcb9076a95"},"EntitySyncDefinitions":{"$type":"System.Collections.Generic.List`1[[Vanrise.BEBridge.Entities.EntitySyncDefinition, Vanrise.BEBridge.Entities]], mscorlib","$values":[{"$type":"Vanrise.BEBridge.Entities.EntitySyncDefinition, Vanrise.BEBridge.Entities","TargetBEConvertor":{"$type":"Retail.Zajil.MainExtensions.AccountConvertor, Retail.Zajil.MainExtensions","Name":"Zajil Account Convertor","AccountBEDefinitionId":"9a427357-cf55-4f33-99f7-745206dee7cd","AccountTypeId":"046078a0-3434-4934-8f4d-272608cffebf","InitialStatusId":"0c5cb172-c579-423d-9bb9-e7a1ec754c37","FinancialPartDefinitionId":"82228be2-e633-4ef8-b383-9894f28c8cb0","CompanyProfilePartDefinitionId":"b0717c4f-e409-4ae2-8c00-5add4ca828c5","OrderDetailsPartDefinitionId":"b272b8b9-0501-4322-a4ad-360fdf5d933d","ConfigId":"26dc208a-7954-4258-a8e5-48497c02ef19","CompareBeforeUpdate":true},"TargetBESynchronizer":{"$type":"Retail.BusinessEntity.Business.AccountSynchronizer, Retail.BusinessEntity.Business","AccountBEDefinitionId":"9a427357-cf55-4f33-99f7-745206dee7cd","Name":"Accounts","ConfigId":"cd147065-88f3-4337-a625-8578708c4a53"}}]}}'),
('94EFB1A6-9478-4818-815A-22AE4079C76C','Invoice BE Bridge','{"$type":"Vanrise.BEBridge.Entities.BEReceiveDefinitionSettings, Vanrise.BEBridge.Entities","SourceBEReader":{"$type":"Vanrise.Invoice.MainExtensions.InvoiceReader.InvoiceSourceReader, Vanrise.Invoice.MainExtensions","Setting":{"$type":"Vanrise.Invoice.MainExtensions.InvoiceReader.InvoiceSourceReaderSetting, Vanrise.Invoice.MainExtensions","InvoiceTypeId":"384c819d-6e21-4e9a-9f08-11c7b81ad329","BatchSize":100},"ConfigId":"59551673-f340-4bde-8040-f213a24ab5a8"},"EntitySyncDefinitions":{"$type":"System.Collections.Generic.List`1[[Vanrise.BEBridge.Entities.EntitySyncDefinition, Vanrise.BEBridge.Entities]], mscorlib","$values":[{"$type":"Vanrise.BEBridge.Entities.EntitySyncDefinition, Vanrise.BEBridge.Entities","TargetBEConvertor":{"$type":"Vanrise.Invoice.MainExtensions.Convertors.InvoiceToVRObjectConvertor, Vanrise.Invoice.MainExtensions","PartnerInfoObjects":{"$type":"System.Collections.Generic.List`1[[Vanrise.Invoice.MainExtensions.Convertors.InvoicePartnerInfoObject, Vanrise.Invoice.MainExtensions]], mscorlib","$values":[]},"Name":"Invoice To VR Object Convertor","ConfigId":"ec99f6db-2617-4af4-8a20-aed83b404fca","CompareBeforeUpdate":true},"TargetBESynchronizer":{"$type":"Vanrise.BEBridge.MainExtensions.Synchronizers.DatabaseSynchronizer, Vanrise.BEBridge.MainExtensions","ConnectionString":"Server=192.168.110.185;Database=MVTSPRO;User ID=Development;Password=dev!123","InsertQueryTemplate":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"INSERT INTO [Invoice_Temp]\n           ([InvoiceId]\n           ,[SerialNumber]) \n     VALUES(@Model.GetVal(\"Invoice\",\"InvoiceId\"),\n\t\t\t''@Model.GetVal(\"Invoice\",\"Serial Number\")'')"},"Objects":{"$type":"Vanrise.Entities.VRObjectVariableCollection, Vanrise.Entities","Invoice":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"Invoice","VRObjectTypeDefinitionId":"12eb29c8-01a3-4786-af9c-af335be2ea19"}},"Name":"Target BE Synchronizer","ConfigId":"e761535c-9768-4047-8b61-fde5d83a9a4a"}}]}}'),
('2DF8D600-3259-44A1-9357-A7575B3B9FDA','Zajil Billing Transaction Bridge','{"$type":"Vanrise.BEBridge.Entities.BEReceiveDefinitionSettings, Vanrise.BEBridge.Entities","SourceBEReader":{"$type":"Vanrise.BEBridge.MainExtensions.SourceBEReaders.SqlSourceReader, Vanrise.BEBridge.MainExtensions","Setting":{"$type":"Vanrise.BEBridge.MainExtensions.SourceBEReaders.SqlSourceReaderSetting, Vanrise.BEBridge.MainExtensions","ConnectionString":"Server=192.168.110.185;Database=MVTSPRO;User ID=development;Password=dev!123","Query":"SELECT [PaymentId]\n      ,[AccountId]\n      ,[Amount]\n,CreatedOn\n  FROM [dbo].[Payment]","CommandTimeout":200,"BasedOnId":false},"ConfigId":"287a031d-d476-4b03-9478-76dcb9076a95"},"EntitySyncDefinitions":{"$type":"System.Collections.Generic.List`1[[Vanrise.BEBridge.Entities.EntitySyncDefinition, Vanrise.BEBridge.Entities]], mscorlib","$values":[{"$type":"Vanrise.BEBridge.Entities.EntitySyncDefinition, Vanrise.BEBridge.Entities","TargetBEConvertor":{"$type":"Retail.Zajil.MainExtensions.Convertors.PaymentConvertor, Retail.Zajil.MainExtensions","Name":"Zajil Payment Convertor","TransactionTypeId":"f178d94d-d622-4ebf-a1ba-2a4af1067d6b","AccountBEDefinitionId":"9a427357-cf55-4f33-99f7-745206dee7cd","SourceAccountIdColumn":"AccountId","AmountColumn":"Amount","TimeColumn":"CreatedOn","CurrencyId":0,"ConfigId":"0897f25a-7bf4-4a4f-a97f-0ee33993633c","CompareBeforeUpdate":true},"TargetBESynchronizer":{"$type":"Vanrise.AccountBalance.MainExtensions.BillingTransaction.BillingTransactionSynchronizer, Vanrise.AccountBalance.MainExtensions","Name":"Billing Transaction Synchronizer","BalanceAccountTypeId":"20b0c83e-6f53-49c7-b52f-828a19e6dc2a","ConfigId":"6336d88e-3460-4388-b56c-322fbc336129"}}]}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings]))
merge	[VR_BEBridge].[BEReceiveDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Settings])
	values(s.[ID],s.[Name],s.[Settings]);

--[genericdata].[GenericRuleDefinition]-------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[OldID],[Name],[Details])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('D0C4E67E-7D4B-4A31-B968-CE8F0341D297',null,'International Voice Rate Types','{"$type":"Vanrise.GenericData.Entities.GenericRuleDefinition, Vanrise.GenericData.Entities","GenericRuleDefinitionId":"d0c4e67e-7d4b-4a31-b968-ce8f0341d297","Name":"International Voice Rate Types","Title":"International Voice Rate Types","CriteriaDefinition":{"$type":"Vanrise.GenericData.Entities.GenericRuleDefinitionCriteria, Vanrise.GenericData.Entities","Fields":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.GenericRuleDefinitionCriteriaField, Vanrise.GenericData.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Entities.GenericRuleDefinitionCriteriaField, Vanrise.GenericData.Entities","FieldName":"Direction","Title":"Direction","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldChoicesType, Vanrise.GenericData.MainExtensions","ConfigId":"eabc41a9-e332-4120-ac85-f0b7e53c0d0d","Choices":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.MainExtensions.DataRecordFields.Choice, Vanrise.GenericData.MainExtensions]], mscorlib","$values":[{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.Choice, Vanrise.GenericData.MainExtensions","Value":1,"Text":"In"},{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.Choice, Vanrise.GenericData.MainExtensions","Value":2,"Text":"Out"}]},"IsNullable":false,"OrderType":1},"RuleStructureBehaviorType":0,"Priority":1,"ShowInBasicSearch":false,"ValueObjectName":"MappedCDR","ValuePropertyName":"TrafficDirection"},{"$type":"Vanrise.GenericData.Entities.GenericRuleDefinitionCriteriaField, Vanrise.GenericData.Entities","FieldName":"Zone","Title":"Zone","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","BusinessEntityDefinitionId":"F650D523-7ADB-4787-A2F6-C13168F7E8F7","IsNullable":false,"OrderType":1},"RuleStructureBehaviorType":0,"Priority":2,"ShowInBasicSearch":false,"ValueObjectName":"MappedCDR","ValuePropertyName":"Zone"}]}},"Objects":{"$type":"Vanrise.Entities.VRObjectVariableCollection, Vanrise.Entities","MappedCDR":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"MappedCDR","VRObjectTypeDefinitionId":"750432af-3229-4cd8-8599-8c5df0d70231"}},"SettingsDefinition":{"$type":"Vanrise.GenericData.Pricing.RateTypeRuleDefinitionSettings, Vanrise.GenericData.Pricing","ConfigId":"5969790e-1bd4-45e4-be39-b8d7fa6a1842"},"Security":{"$type":"Vanrise.GenericData.Entities.GenericRuleDefinitionSecurity, Vanrise.GenericData.Entities","ViewRequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"e4186240-7525-4b39-9b4d-48ccde5f2590","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"AddRequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"e4186240-7525-4b39-9b4d-48ccde5f2590","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Add"]}}]}},"EditRequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"e4186240-7525-4b39-9b4d-48ccde5f2590","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Edit"]}}]}}}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[OldID],[Name],[Details]))
merge  [genericdata].[GenericRuleDefinition] as t
using  cte_data as s
on            1=1 and t.[ID] = s.[ID]
when matched then
       update set
       [OldID] = s.[OldID],[Name] = s.[Name],[Details] = s.[Details]
when not matched by target then
       insert([ID],[OldID],[Name],[Details])
       values(s.[ID],s.[OldID],s.[Name],s.[Details]);
----------------------------------------------------------------------------------------------------
END

--[common].[Setting]---------------------------201 to 300-------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[OldID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('682C68EF-5687-47F1-957C-0150A6132F7E',null,'Business Entity Technical','Retail_BE_TechnicalSettings','Business Entities','{"Editor":"retail-be-technicalsettings"}','{"$type":"Retail.BusinessEntity.Entities.RetailBETechnicalSettings, Retail.BusinessEntity.Entities","IncludedAccountTypes":{"$type":"Retail.BusinessEntity.Entities.IncludedAccountTypes, Retail.BusinessEntity.Entities","AcountTypeIds":{"$type":"System.Collections.Generic.List`1[[System.Guid, mscorlib]], mscorlib","$values":["a26d4be8-d753-450d-8816-d19ab18f90f5","ed09fef6-c333-400b-8f92-14ff9f8ced7b","046078a0-3434-4934-8f4d-272608cffebf","cea8aba9-cdaa-4755-b30d-3734eae52e83","fa621626-8927-4ba6-ad5f-80a0a8fa6f06","5ff96aee-cdf0-4415-a643-6b275f47e791"]}}}',1)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[OldID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical]))
merge	[common].[Setting] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[OldID] = s.[OldID],[Name] = s.[Name],[Type] = s.[Type],[Category] = s.[Category],[Settings] = s.[Settings],[Data] = s.[Data],[IsTechnical] = s.[IsTechnical]
when not matched by target then
	insert([ID],[OldID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
	values(s.[ID],s.[OldID],s.[Name],s.[Type],s.[Category],s.[Settings],s.[Data],s.[IsTechnical]);
----------------------------------------------------------------------------------------------------
END