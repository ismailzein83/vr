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

--[sec].[View]--------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank],[IsDeleted])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('4073D6EB-E9DD-47B2-9FF4-C3C0B5672BC5','Received Supplier Pricelist','Received Supplier Pricelist','#/view/WhS_SupplierPriceList/Views/ReceivedSupplierPriceList','D66F9910-48EC-4FFD-8A09-C7960A6EE434','WhS_SupPL/ReceivedSupplierPriceList/GetFilteredReceivedSupplierPriceList',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',20,null)
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

--[common].[VRObjectTypeDefinition]--------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([ID],[Name],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('A03BA5D0-F45C-4EBA-91B1-1DFA9959D280','Received Pricelist','{"$type":"Vanrise.Entities.VRObjectTypeDefinitionSettings, Vanrise.Entities","ObjectType":{"$type":"TOne.WhS.SupplierPriceList.MainExtensions.VRObjectTypes.ReceivedPricelistObjectType, TOne.WhS.SupplierPriceList.MainExtensions","ConfigId":"0a5f1a1f-7a68-44ef-a71d-7bfd81d68f91"},"Properties":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities]], mscorlib","Pricelist Type":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Pricelist Type","Description":"Pricelist Type","PropertyEvaluator":{"$type":"TOne.WhS.SupplierPriceList.MainExtensions.VRObjectTypes.ReceivedPricelistPropertyEvaluator, TOne.WhS.SupplierPriceList.MainExtensions","ConfigId":"bec4cf34-a716-4fd9-a38b-fcf1bb56265b","ReceivedPricelistField":0}},"Pricelist Status":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Pricelist Status","Description":"Pricelist Status","PropertyEvaluator":{"$type":"TOne.WhS.SupplierPriceList.MainExtensions.VRObjectTypes.ReceivedPricelistPropertyEvaluator, TOne.WhS.SupplierPriceList.MainExtensions","ConfigId":"bec4cf34-a716-4fd9-a38b-fcf1bb56265b","ReceivedPricelistField":1}},"Pricelist Name":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Pricelist Name","Description":"Pricelist Name","PropertyEvaluator":{"$type":"TOne.WhS.SupplierPriceList.MainExtensions.VRObjectTypes.ReceivedPricelistPropertyEvaluator, TOne.WhS.SupplierPriceList.MainExtensions","ConfigId":"bec4cf34-a716-4fd9-a38b-fcf1bb56265b","ReceivedPricelistField":2}},"Supplier Name":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Supplier Name","Description":"Supplier Name","PropertyEvaluator":{"$type":"TOne.WhS.SupplierPriceList.MainExtensions.VRObjectTypes.ReceivedPricelistPropertyEvaluator, TOne.WhS.SupplierPriceList.MainExtensions","ConfigId":"bec4cf34-a716-4fd9-a38b-fcf1bb56265b","ReceivedPricelistField":5}},"Supplier Email":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Supplier Email","Description":"Supplier Email","PropertyEvaluator":{"$type":"TOne.WhS.SupplierPriceList.MainExtensions.VRObjectTypes.ReceivedPricelistPropertyEvaluator, TOne.WhS.SupplierPriceList.MainExtensions","ConfigId":"bec4cf34-a716-4fd9-a38b-fcf1bb56265b","ReceivedPricelistField":6}},"Errors":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Errors","Description":"Errors","PropertyEvaluator":{"$type":"TOne.WhS.SupplierPriceList.MainExtensions.VRObjectTypes.ReceivedPricelistPropertyEvaluator, TOne.WhS.SupplierPriceList.MainExtensions","ConfigId":"bec4cf34-a716-4fd9-a38b-fcf1bb56265b","ReceivedPricelistField":7}}}}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Settings]))merge	[common].[VRObjectTypeDefinition] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Settings])	values(s.[ID],s.[Name],s.[Settings]);

--[common].[MailMessageType]------------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([ID],[Name],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('757F93C4-1886-4002-82AE-2FDA0E529A3C','Received Supplier Pricelist','{"$type":"Vanrise.Entities.VRMailMessageTypeSettings, Vanrise.Entities","Objects":{"$type":"Vanrise.Entities.VRObjectVariableCollection, Vanrise.Entities","Received Pricelist":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"Received Pricelist","VRObjectTypeDefinitionId":"a03ba5d0-f45c-4eba-91b1-1dfa9959d280"}}}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Settings]))merge	[common].[MailMessageType] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Settings])	values(s.[ID],s.[Name],s.[Settings]);