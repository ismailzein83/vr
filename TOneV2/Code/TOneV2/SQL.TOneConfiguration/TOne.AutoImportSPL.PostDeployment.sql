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
--[common].[ExtensionConfiguration]---------------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('7976A765-DD69-4A24-81B2-64C6C231BE79','Supplier Pricelist Received Mail Synchronizer','Supplier Pricelist Received Mail Synchronizer','VR_BEBridge_BESynchronizer','{"Editor":"vr-whs-be-receivedsupplierpricelistmessage-synchronizer"}'),
('05382832-5CBB-46C0-8214-B3B81769FB80','POP3 Supplier Pricelist Filter','POP3 Supplier Pricelist Filter','VRCommon_Pop3MessageFilter'								,'{"Editor":"vr-whs-be-pop3supplierpricelistmessagefilter"}'),
('0A5F1A1F-7A68-44EF-A71D-7BFD81D68F91','WhS_SupPL_VRObjectTypes_ReceivedPricelistObjectType','Received Pricelist','VR_Common_ObjectType'							,'{"Editor":"whs-spl-receivedpricelistobjecttype", "PropertyEvaluatorExtensionType": "WhS_SPL_ReceivedPricelist_PropertyEvaluator"}'),
('BEC4CF34-A716-4FD9-A38B-FCF1BB56265B','WhS_SPL_ReceivedPricelist_PropertyEvaluator','Received Pricelist Property','WhS_SPL_ReceivedPricelist_PropertyEvaluator'	,'{"Editor":"whs-spl-receivedpricelistpropertyevaluator"}')
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
end

--[common].[Setting]------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('1974526D-AFCB-4D4C-9BFA-92149102B625','Supplier Auto Import Pricelist','WhS_BE_AutoImport_Settings','Business Entities','{"Editor":"vr-whs-be-autoimport-settings-editor"}','{"$type":"TOne.WhS.BusinessEntity.Entities.AutoImportSPLSettings, TOne.WhS.BusinessEntity.Entities","PricelistTypeMappingList":{"$type":"System.Collections.Generic.List`1[[TOne.WhS.BusinessEntity.Entities.PricelistTypeMapping, TOne.WhS.BusinessEntity.Entities]], mscorlib","$values":[{"$type":"TOne.WhS.BusinessEntity.Entities.PricelistTypeMapping, TOne.WhS.BusinessEntity.Entities","Subject":"Full","PricelistType":2},{"$type":"TOne.WhS.BusinessEntity.Entities.PricelistTypeMapping, TOne.WhS.BusinessEntity.Entities","Subject":"Country","PricelistType":1},{"$type":"TOne.WhS.BusinessEntity.Entities.PricelistTypeMapping, TOne.WhS.BusinessEntity.Entities","Subject":"Rate Change","PricelistType":0}]},"SupplierAutoImportTemplateList":{"$type":"System.Collections.Generic.List`1[[TOne.WhS.BusinessEntity.Entities.SupplierAutoImportTemplate, TOne.WhS.BusinessEntity.Entities]], mscorlib","$values":[{"$type":"TOne.WhS.BusinessEntity.Entities.SupplierAutoImportTemplate, TOne.WhS.BusinessEntity.Entities","TemplateType":0,"EmailTemplateId":"30055174-9717-49f4-876e-4787c4f5e4ac","AttachPricelist":false},{"$type":"TOne.WhS.BusinessEntity.Entities.SupplierAutoImportTemplate, TOne.WhS.BusinessEntity.Entities","TemplateType":1,"EmailTemplateId":"343abd9f-a4db-404d-bf33-9f45575cd2a8","AttachPricelist":false},{"$type":"TOne.WhS.BusinessEntity.Entities.SupplierAutoImportTemplate, TOne.WhS.BusinessEntity.Entities","TemplateType":2,"EmailTemplateId":"0e1038be-0f8c-4027-a870-25c1f02b5f90","AttachPricelist":false}]},"InternalAutoImportTemplateList":{"$type":"System.Collections.Generic.List`1[[TOne.WhS.BusinessEntity.Entities.InternalAutoImportTemplate, TOne.WhS.BusinessEntity.Entities]], mscorlib","$values":[{"$type":"TOne.WhS.BusinessEntity.Entities.InternalAutoImportTemplate, TOne.WhS.BusinessEntity.Entities","TemplateType":0,"EmailTemplateId":"09ebd6e8-713e-411b-9d1f-37770b4e4241","AttachPricelist":false},{"$type":"TOne.WhS.BusinessEntity.Entities.InternalAutoImportTemplate, TOne.WhS.BusinessEntity.Entities","TemplateType":1,"EmailTemplateId":"675449be-1c52-4f51-a8c3-40e2f1e8e93f","AttachPricelist":false},{"$type":"TOne.WhS.BusinessEntity.Entities.InternalAutoImportTemplate, TOne.WhS.BusinessEntity.Entities","TemplateType":2,"EmailTemplateId":"ea7d247f-8b04-4608-aad3-e76578b576ca","AttachPricelist":false}]}}',0)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical]))merge	[common].[Setting] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]--when matched then--	update set--	[Name] = s.[Name],[Type] = s.[Type],[Category] = s.[Category],[Settings] = s.[Settings],[Data] = s.[Data],[IsTechnical] = s.[IsTechnical]when not matched by target then	insert([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])	values(s.[ID],s.[Name],s.[Type],s.[Category],s.[Settings],s.[Data],s.[IsTechnical]);

--[sec].[View]--------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank],[IsDeleted])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('4073D6EB-E9DD-47B2-9FF4-C3C0B5672BC5','Received Supplier Pricelists','Received Supplier Pricelists','#/view/WhS_SupplierPriceList/Views/ReceivedSupplierPriceLists','D66F9910-48EC-4FFD-8A09-C7960A6EE434','WhS_SupPL/ReceivedSupplierPriceList/GetFilteredReceivedSupplierPriceList',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',3,null)
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


--[sec].[SystemAction]------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([Name],[RequiredPermissions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('WhS_SupPL/ReceivedSupplierPriceList/GetFilteredReceivedSupplierPriceList','WhS_BE_SupplierPricelist: Start Process')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Name],[RequiredPermissions]))
merge	[sec].[SystemAction] as t
using	cte_data as s
on		1=1 and t.[Name] = s.[Name]
when matched then
	update set [RequiredPermissions] = s.[RequiredPermissions]
when not matched by target then
	insert([Name],[RequiredPermissions])
	values(s.[Name],s.[RequiredPermissions]);

--[common].[VRObjectTypeDefinition]----------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('A03BA5D0-F45C-4EBA-91B1-1DFA9959D280','Received Pricelist','{"$type":"Vanrise.Entities.VRObjectTypeDefinitionSettings, Vanrise.Entities","ObjectType":{"$type":"TOne.WhS.SupplierPriceList.MainExtensions.VRObjectTypes.ReceivedPricelistObjectType, TOne.WhS.SupplierPriceList.MainExtensions","ConfigId":"0a5f1a1f-7a68-44ef-a71d-7bfd81d68f91"},"Properties":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities]], mscorlib","Pricelist Type":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Pricelist Type","Description":"Pricelist Type","PropertyEvaluator":{"$type":"TOne.WhS.SupplierPriceList.MainExtensions.VRObjectTypes.ReceivedPricelistPropertyEvaluator, TOne.WhS.SupplierPriceList.MainExtensions","ConfigId":"bec4cf34-a716-4fd9-a38b-fcf1bb56265b","ReceivedPricelistField":0}},"Pricelist Status":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Pricelist Status","Description":"Pricelist Status","PropertyEvaluator":{"$type":"TOne.WhS.SupplierPriceList.MainExtensions.VRObjectTypes.ReceivedPricelistPropertyEvaluator, TOne.WhS.SupplierPriceList.MainExtensions","ConfigId":"bec4cf34-a716-4fd9-a38b-fcf1bb56265b","ReceivedPricelistField":1}},"Pricelist Name":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Pricelist Name","Description":"Pricelist Name","PropertyEvaluator":{"$type":"TOne.WhS.SupplierPriceList.MainExtensions.VRObjectTypes.ReceivedPricelistPropertyEvaluator, TOne.WhS.SupplierPriceList.MainExtensions","ConfigId":"bec4cf34-a716-4fd9-a38b-fcf1bb56265b","ReceivedPricelistField":2}},"Supplier Name":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Supplier Name","Description":"Supplier Name","PropertyEvaluator":{"$type":"TOne.WhS.SupplierPriceList.MainExtensions.VRObjectTypes.ReceivedPricelistPropertyEvaluator, TOne.WhS.SupplierPriceList.MainExtensions","ConfigId":"bec4cf34-a716-4fd9-a38b-fcf1bb56265b","ReceivedPricelistField":5}},"Supplier Email":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Supplier Email","Description":"Supplier Email","PropertyEvaluator":{"$type":"TOne.WhS.SupplierPriceList.MainExtensions.VRObjectTypes.ReceivedPricelistPropertyEvaluator, TOne.WhS.SupplierPriceList.MainExtensions","ConfigId":"bec4cf34-a716-4fd9-a38b-fcf1bb56265b","ReceivedPricelistField":6}},"Errors":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Errors","Description":"Errors","PropertyEvaluator":{"$type":"TOne.WhS.SupplierPriceList.MainExtensions.VRObjectTypes.ReceivedPricelistPropertyEvaluator, TOne.WhS.SupplierPriceList.MainExtensions","ConfigId":"bec4cf34-a716-4fd9-a38b-fcf1bb56265b","ReceivedPricelistField":7}}}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings]))
merge	[common].[VRObjectTypeDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Settings])
	values(s.[ID],s.[Name],s.[Settings]);

--[common].[MailMessageType]--------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('757F93C4-1886-4002-82AE-2FDA0E529A3C','Received Supplier Pricelist','{"$type":"Vanrise.Entities.VRMailMessageTypeSettings, Vanrise.Entities","Objects":{"$type":"Vanrise.Entities.VRObjectVariableCollection, Vanrise.Entities","Received Pricelist":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"Received Pricelist","VRObjectTypeDefinitionId":"a03ba5d0-f45c-4eba-91b1-1dfa9959d280"}}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings]))
merge	[common].[MailMessageType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Settings])
	values(s.[ID],s.[Name],s.[Settings]);

--[common].[MailMessageTemplate]--------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([ID],[Name],[MessageTypeID],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('0E1038BE-0F8C-4027-A870-25C1F02B5F90','Supplier Auto Import Failed','757F93C4-1886-4002-82AE-2FDA0E529A3C','{"$type":"Vanrise.Entities.VRMailMessageTemplateSettings, Vanrise.Entities","From":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities"},"To":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"@Model.GetVal(\"Received Pricelist\",\"Supplier Email\")"},"CC":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities"},"BCC":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities"},"Subject":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"@Model.GetVal(\"Received Pricelist\",\"Pricelist Name\") Failed"},"Body":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"Processing pricelist&nbsp;@Model.GetVal(\"Received Pricelist\",\"Pricelist Name\") failed<div>Status&nbsp;@Model.GetVal(\"Received Pricelist\",\"Pricelist Status\")</div><div>Errors :&nbsp;</div><div>@Model.GetVal(\"Received Pricelist\",\"Errors\")<br></div>"}}'),('09EBD6E8-713E-411B-9D1F-37770B4E4241','Internal Auto Import Received','757F93C4-1886-4002-82AE-2FDA0E529A3C','{"$type":"Vanrise.Entities.VRMailMessageTemplateSettings, Vanrise.Entities","From":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities"},"To":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"rodivanrise@gmail.com;mortada.test123@gmail.com"},"CC":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities"},"BCC":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities"},"Subject":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"Internal @Model.GetVal(\"Received Pricelist\",\"Pricelist Name\") received"},"Body":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"<div>Processing pricelist @Model.GetVal(\"Received Pricelist\",\"Pricelist Name\") received</div><div>Type @Model.GetVal(\"Received Pricelist\",\"Pricelist Type\")</div><div>Status @Model.GetVal(\"Received Pricelist\",\"Pricelist Status\")</div><div>Supplier @Model.GetVal(\"Received Pricelist\",\"Supplier Name\")</div><div><br></div>"}}'),('675449BE-1C52-4F51-A8C3-40E2F1E8E93F','Internal Auto Import Completed','757F93C4-1886-4002-82AE-2FDA0E529A3C','{"$type":"Vanrise.Entities.VRMailMessageTemplateSettings, Vanrise.Entities","From":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities"},"To":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"rodivanrise@gmail.com;mortada.test123@gmail.com"},"CC":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities"},"BCC":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities"},"Subject":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"Internal @Model.GetVal(\"Received Pricelist\",\"Pricelist Name\") completed"},"Body":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"Processing pricelist&nbsp;@Model.GetVal(\"Received Pricelist\",\"Pricelist Name\") completed&nbsp;<div>Type&nbsp;@Model.GetVal(\"Received Pricelist\",\"Pricelist Type\")<br><div>Status&nbsp;@Model.GetVal(\"Received Pricelist\",\"Pricelist Status\")</div><div>Supplier&nbsp;@Model.GetVal(\"Received Pricelist\",\"Supplier Name\")</div></div>"}}'),('30055174-9717-49F4-876E-4787C4F5E4AC','Supplier Auto Import Received','757F93C4-1886-4002-82AE-2FDA0E529A3C','{"$type":"Vanrise.Entities.VRMailMessageTemplateSettings, Vanrise.Entities","From":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities"},"To":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"@Model.GetVal(\"Received Pricelist\",\"Supplier Email\")"},"CC":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities"},"BCC":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities"},"Subject":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"Received @Model.GetVal(\"Received Pricelist\",\"Pricelist Name\")"},"Body":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"<div>Received pricelist&nbsp;@Model.GetVal(\"Received Pricelist\",\"Pricelist Name\") with type&nbsp;@Model.GetVal(\"Received Pricelist\",\"Pricelist Type\")<br></div><div><div><br></div></div>"}}'),('343ABD9F-A4DB-404D-BF33-9F45575CD2A8','Supplier Auto Import Completed','757F93C4-1886-4002-82AE-2FDA0E529A3C','{"$type":"Vanrise.Entities.VRMailMessageTemplateSettings, Vanrise.Entities","From":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities"},"To":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"@Model.GetVal(\"Received Pricelist\",\"Supplier Email\")"},"CC":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities"},"BCC":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities"},"Subject":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"@Model.GetVal(\"Received Pricelist\",\"Pricelist Name\") completed"},"Body":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"Processing pricelist&nbsp;@Model.GetVal(\"Received Pricelist\",\"Pricelist Name\") completed&nbsp;<div>Status&nbsp;@Model.GetVal(\"Received Pricelist\",\"Pricelist Status\")</div>"}}'),('EA7D247F-8B04-4608-AAD3-E76578B576CA','Internal Auto Import Failed','757F93C4-1886-4002-82AE-2FDA0E529A3C','{"$type":"Vanrise.Entities.VRMailMessageTemplateSettings, Vanrise.Entities","From":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities"},"To":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"rodivanrise@gmail.com;mortada.test123@gmail.com"},"CC":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities"},"BCC":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities"},"Subject":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"Internal @Model.GetVal(\"Received Pricelist\",\"Pricelist Name\") failed"},"Body":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"<div>Processing pricelist @Model.GetVal(\"Received Pricelist\",\"Pricelist Name\") failed</div><div>Type @Model.GetVal(\"Received Pricelist\",\"Pricelist Type\")</div><div>Status @Model.GetVal(\"Received Pricelist\",\"Pricelist Status\")</div><div>Supplier @Model.GetVal(\"Received Pricelist\",\"Supplier Name\")</div><div>Errors:@Model.GetVal(\"Received Pricelist\",\"Errors\")</div>"}}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[MessageTypeID],[Settings]))merge	[common].[MailMessageTemplate] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]--when matched then--	update set--	[Name] = s.[Name],[MessageTypeID] = s.[MessageTypeID],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[MessageTypeID],[Settings])	values(s.[ID],s.[Name],s.[MessageTypeID],s.[Settings]);

--[common].[Connection]-----------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('ACC19A52-9FC4-4436-BA94-555F47696F37','Pop 3 Reader','{"$type":"Vanrise.Common.Business.VRPop3Connection, Vanrise.Common.Business","ConfigId":"e3bf7c73-14ba-402b-9158-a67d03635447","UserName":"mailWherePricelistsAreReceived@sample.com","Password":"123456","Server":"smtp.gmail.com","Port":995,"SSL":true,"TimeToKeepReadMessageIdsInState":"00:00:00"}')
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

--[VR_BEBridge].[BEReceiveDefinition]---------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('375EA78E-2446-4469-89DD-01DC3173D4B7','Auto Import Supplier Pricelists (Pop3)','{"$type":"Vanrise.BEBridge.Entities.BEReceiveDefinitionSettings, Vanrise.BEBridge.Entities","SourceBEReader":{"$type":"Vanrise.BEBridge.MainExtensions.SourceBEReaders.Pop3SourceReader, Vanrise.BEBridge.MainExtensions","Setting":{"$type":"Vanrise.BEBridge.MainExtensions.SourceBEReaders.Pop3SourceReaderSetting, Vanrise.BEBridge.MainExtensions","VRConnectionId":"acc19a52-9fc4-4436-ba94-555f47696f37","Pop3MessageFilter":{"$type":"TOne.WhS.SupplierPriceList.MainExtensions.Pop3SupplierPricelistMessageFilter, TOne.WhS.SupplierPriceList.MainExtensions","ConfigId":"05382832-5cbb-46c0-8214-b3b81769fb80"},"BatchSize":10},"ConfigId":"05a15b37-43cb-4d0d-b453-803645d25c03"},"EntitySyncDefinitions":{"$type":"System.Collections.Generic.List`1[[Vanrise.BEBridge.Entities.EntitySyncDefinition, Vanrise.BEBridge.Entities]], mscorlib","$values":[{"$type":"Vanrise.BEBridge.Entities.EntitySyncDefinition, Vanrise.BEBridge.Entities","TargetBEConvertor":{"$type":"Vanrise.Common.MainExtensions.ReceivedMailMessageConvertor, Vanrise.Common.MainExtensions","Name":"Received Mail Message Convertor","ConfigId":"fd7a74f9-34dd-460a-87a4-288684a6c9b1","CompareBeforeUpdate":true},"TargetBESynchronizer":{"$type":"TOne.WhS.SupplierPriceList.Business.SupplierPricelistReceivedMailMessageSynchronizer, TOne.WhS.SupplierPriceList.Business","Name":"Supplier Pricelist Received Mail Message Synchronizer","ConfigId":"7976a765-dd69-4a24-81b2-64c6c231be79"}}]},"Security":{"$type":"Vanrise.BEBridge.Entities.BEReceiveDefinitionSecurity, Vanrise.BEBridge.Entities","ViewPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}},"StartInstancePermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}}}}')
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