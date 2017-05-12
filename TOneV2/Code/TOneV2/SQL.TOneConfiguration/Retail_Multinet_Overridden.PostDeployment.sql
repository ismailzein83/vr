

-------------- START Entity '[Retail_BE].[AccountType]' -------------------
-----------------------------------------------------------------------------------------
BEGIN

set nocount on;
;with cte_data([ID],[Name],[Title],[AccountBEDefinitionID],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('5ff96aee-cdf0-4415-a643-6b275f47e791','Centrex_Site','Branch','9a427357-cf55-4f33-99f7-745206dee7cd','{"$type":"Retail.BusinessEntity.Entities.AccountTypeSettings, Retail.BusinessEntity.Entities","ShowConcatenatedName":true,"CanBeRootAccount":false,"SupportedParentAccountTypeIds":{"$type":"System.Collections.Generic.List`1[[System.Guid, mscorlib]], mscorlib","$values":["046078a0-3434-4934-8f4d-272608cffebf"]},"PartDefinitionSettings":{"$type":"System.Collections.Generic.List`1[[Retail.BusinessEntity.Entities.AccountTypePartSettings, Retail.BusinessEntity.Entities]], mscorlib","$values":[{"$type":"Retail.BusinessEntity.Entities.AccountTypePartSettings, Retail.BusinessEntity.Entities","AvailabilitySettings":0,"RequiredSettings":2,"PartDefinitionId":"82228be2-e633-4ef8-b383-9894f28c8cb0"}]},"InitialStatusId":"dadc2977-a348-4504-89c9-c92f8f9008dd"}'),
('046078a0-3434-4934-8f4d-272608cffebf','Centrex_Company','Company','9a427357-cf55-4f33-99f7-745206dee7cd','{"$type":"Retail.BusinessEntity.Entities.AccountTypeSettings, Retail.BusinessEntity.Entities","ShowConcatenatedName":false,"CanBeRootAccount":true,"PartDefinitionSettings":{"$type":"System.Collections.Generic.List`1[[Retail.BusinessEntity.Entities.AccountTypePartSettings, Retail.BusinessEntity.Entities]], mscorlib","$values":[{"$type":"Retail.BusinessEntity.Entities.AccountTypePartSettings, Retail.BusinessEntity.Entities","AvailabilitySettings":0,"RequiredSettings":0,"PartDefinitionId":"82228be2-e633-4ef8-b383-9894f28c8cb0"}]},"InitialStatusId":"dadc2977-a348-4504-89c9-c92f8f9008dd"}'),
('fa621626-8927-4ba6-ad5f-80a0a8fa6f06','Centrex_Residential','Residential','9a427357-cf55-4f33-99f7-745206dee7cd','{"$type":"Retail.BusinessEntity.Entities.AccountTypeSettings, Retail.BusinessEntity.Entities","ShowConcatenatedName":false,"CanBeRootAccount":true,"PartDefinitionSettings":{"$type":"System.Collections.Generic.List`1[[Retail.BusinessEntity.Entities.AccountTypePartSettings, Retail.BusinessEntity.Entities]], mscorlib","$values":[{"$type":"Retail.BusinessEntity.Entities.AccountTypePartSettings, Retail.BusinessEntity.Entities","AvailabilitySettings":0,"RequiredSettings":0,"PartDefinitionId":"d7e9ad2f-a458-421f-911b-8b8c3e6451f8"},{"$type":"Retail.BusinessEntity.Entities.AccountTypePartSettings, Retail.BusinessEntity.Entities","AvailabilitySettings":0,"RequiredSettings":0,"PartDefinitionId":"82228be2-e633-4ef8-b383-9894f28c8cb0"}]},"InitialStatusId":"dadc2977-a348-4504-89c9-c92f8f9008dd"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[AccountBEDefinitionID],[Settings]))
merge	[Retail_BE].[AccountType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[AccountBEDefinitionID] = s.[AccountBEDefinitionID],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Title],[AccountBEDefinitionID],[Settings])
	values(s.[ID],s.[Name],s.[Title],s.[AccountBEDefinitionID],s.[Settings]);

END
-----------------------------------------------------------------------------------------
-------------- END Entity '[Retail_BE].[AccountType]' -------------------




-------------- START Entity '[Retail_BE].[AccountPartDefinition]' -------------------
-----------------------------------------------------------------------------------------
BEGIN

set nocount on;
;with cte_data([ID],[Title],[Name],[Details])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('b0717c4f-e409-4ae2-8c00-5add4ca828c5','Company Profile','Centrex_Company_Profile','{"$type":"Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartCompanyProfileDefinition, Retail.BusinessEntity.MainExtensions","ConfigId":"1aff2bf7-1f15-4e0b-accf-457edf36a342","IncludeArabicName":false,"ContactTypes":{"$type":"System.Collections.Generic.List`1[[Retail.BusinessEntity.MainExtensions.AccountParts.CompanyProfileContactType, Retail.BusinessEntity.MainExtensions]], mscorlib","$values":[{"$type":"Retail.BusinessEntity.MainExtensions.AccountParts.CompanyProfileContactType, Retail.BusinessEntity.MainExtensions","Name":"Main","Title":"Main"},{"$type":"Retail.BusinessEntity.MainExtensions.AccountParts.CompanyProfileContactType, Retail.BusinessEntity.MainExtensions","Name":"Finance","Title":"Finance"}]}}'),
('d7e9ad2f-a458-421f-911b-8b8c3e6451f8','Residential Profile','Centrex_Residential_Profile','{"$type":"Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartPersonalInfoDefinition, Retail.BusinessEntity.MainExtensions","ConfigId":"3900317c-b982-4d8b-bd0d-01215ac1f3d9"}'),
('82228be2-e633-4ef8-b383-9894f28c8cb0','Financial','Centrex_Financial','{"$type":"Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartFinancialDefinition, Retail.BusinessEntity.MainExtensions","ConfigId":"ba425fa1-13ca-4f44-883a-2a12b7e3f988"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Title],[Name],[Details]))
merge	[Retail_BE].[AccountPartDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Title] = s.[Title],[Name] = s.[Name],[Details] = s.[Details]
when not matched by target then
	insert([ID],[Title],[Name],[Details])
	values(s.[ID],s.[Title],s.[Name],s.[Details]);

END
-----------------------------------------------------------------------------------------
-------------- END Entity '[Retail_BE].[AccountPartDefinition]' -------------------


