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


--[common].[extensionconfiguration]-----------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('EC99F6DB-2617-4AF4-8A20-AED83B404FCA','Invoice To VR Object Convertor','Invoice To VR Object Convertor','VR_BEBridge_BEConvertor','{"Editor":"vr-invoice-convertor-editor"}'),
('2241197C-B5B0-48E5-987A-B3C1949760CB','Order Details','Order Details', 'Retail_BE_AccountPartDefinition', '{"DefinitionEditor":"retail-zajil-accounttype-part-definition-orderdetails", "RuntimeEditor":"retail-zajil-accounttype-part-runtime-orderdetails"}')
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
