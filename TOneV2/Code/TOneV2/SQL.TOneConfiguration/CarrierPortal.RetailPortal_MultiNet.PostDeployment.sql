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


--common.ExtensionConfiguration---------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('da5653fc-7305-4ac8-b64c-1eb9b253d44b','AccountAdditionalInfoTileDefinition','Account Additional Info Tile','VRCommon_VRTileExtendedSettings','{"Editor":"cp-multinet-accountadditionalinfotiledefinitionsettings"}')
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
END

--[common].[VRComponentType]------------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[ConfigID],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('1924f586-8a46-477d-985f-537cbf5d517d','Multi Net Invoice Viewer','3a02eeea-6f38-4277-bac4-9d8f88f71851','{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeSettings, PartnerPortal.Invoice.Entities","VRComponentTypeConfigId":"3a02eeea-6f38-4277-bac4-9d8f88f71851","VRConnectionId":"7a2044f4-b42c-44aa-bfb8-6538904e8e4c","InvoiceTypeId":"c5f01977-67dd-4b62-b947-0b55f8bd2edb","GridSettings":{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridSettings, PartnerPortal.Invoice.Entities","InvoiceGridFields":{"$type":"System.Collections.Generic.List`1[[PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridField, PartnerPortal.Invoice.Entities]], mscorlib","$values":[{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridField, PartnerPortal.Invoice.Entities","Header":"Company","Field":0,"CustomFieldName":"CompanyId","GridColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Large"}},{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridField, PartnerPortal.Invoice.Entities","Header":"Branch","Field":0,"CustomFieldName":"BranchId","GridColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Large"}},{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridField, PartnerPortal.Invoice.Entities","Header":"Serial Number","Field":3},{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridField, PartnerPortal.Invoice.Entities","Header":"From Date","Field":4,"GridColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"FixedWidth","FixedWidth":80}},{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridField, PartnerPortal.Invoice.Entities","Header":"To Date","Field":5,"GridColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"FixedWidth","FixedWidth":80}},{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridField, PartnerPortal.Invoice.Entities","Header":"Doc Date","Field":6,"GridColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"FixedWidth","FixedWidth":80}},{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridField, PartnerPortal.Invoice.Entities","Header":"Due Date","Field":7,"GridColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"FixedWidth","FixedWidth":80}},{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridField, PartnerPortal.Invoice.Entities","Header":"Amount","Field":0,"CustomFieldName":"TotalCurrentCharges","GridColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"FixedWidth","FixedWidth":80}},{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridField, PartnerPortal.Invoice.Entities","Header":"Cur","Field":0,"CustomFieldName":"CurrencyId","GridColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"FixedWidth","FixedWidth":40}}]},"InvoiceGridActions":{"$type":"System.Collections.Generic.List`1[[PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridAction, PartnerPortal.Invoice.Entities]], mscorlib","$values":[{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridAction, PartnerPortal.Invoice.Entities","Title":"Download","InvoiceViewerTypeGridActionId":"00000000-0000-0000-0000-000000000000","Settings":{"$type":"PartnerPortal.Invoice.MainExtensions.InvoiceViewerTypeGridAction.DownloadAttachmentAction, PartnerPortal.Invoice.MainExtensions","ActionTypeName":"DownloadAttachment","ConfigId":"609a37c4-466a-40a9-8140-f23f5d80d3c3","InvoiceAttachmentId":"1530336d-0054-30b5-bce5-eef4dc902dda"}}]}},"ExtendedSettings":{"$type":"PartnerPortal.Invoice.MainExtensions.DefaultInvoiceViewerExtendedSettings, PartnerPortal.Invoice.MainExtensions","ConfigId":"06377156-8265-4424-bd62-46fa5ab2ce41"},"InvoiceQueryInterceptor":{"$type":"PartnerPortal.Invoice.MainExtensions.Invoice.PartnerInvoiceQueryInterceptor, PartnerPortal.Invoice.MainExtensions","ConfigId":"12371be0-cf2c-4cdd-9f4c-e809d912a716"}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[ConfigID],[Settings]))
merge	[common].[VRComponentType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[ConfigID] = s.[ConfigID],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[ConfigID],[Settings])
	values(s.[ID],s.[Name],s.[ConfigID],s.[Settings]);
----------------------------------------------------------------------------------------------------
END

--[sec].[View]--------------------------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank],[IsDeleted])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('82838420-fb3e-459d-88db-818d4d3dbd29','Dashboard','Dashboard',NULL					,NULL,NULL,NULL,NULL,'{"$type":"Vanrise.Common.Business.VRTileViewSettings, Vanrise.Common.Business","VRTileViewData":{"$type":"Vanrise.Common.Business.VRTileViewData, Vanrise.Common.Business","VRTiles":{"$type":"System.Collections.Generic.List`1[[Vanrise.Entities.VRTile, Vanrise.Entities]], mscorlib","$values":[{"$type":"Vanrise.Entities.VRTile, Vanrise.Entities","VRTileId":"ebebae13-da30-b45f-bff2-01d83a139cbc","Name":"Account Info","Settings":{"$type":"Vanrise.Entities.VRTileSettings, Vanrise.Entities","NumberOfColumns":4,"ExtendedSettings":{"$type":"PartnerPortal.CustomerAccess.Business.RetailAccountInfoTileDefinitionSettings, PartnerPortal.CustomerAccess.Business","ConfigId":"ff8e7752-9df7-4017-83ab-6fa8a9cdb30f","RuntimeEditor":"partnerportal-customeraccess-retailaccountinfotileruntimesettings","VRConnectionId":"7a2044f4-b42c-44aa-bfb8-6538904e8e4c"}}},{"$type":"Vanrise.Entities.VRTile, Vanrise.Entities","VRTileId":"c43d8ab3-4924-b9ea-5c37-8b8b68af03fe","Name":"Additional Info","Settings":{"$type":"Vanrise.Entities.VRTileSettings, Vanrise.Entities","NumberOfColumns":3,"ExtendedSettings":{"$type":"CP.MultiNet.Business.AccountAdditionalInfoTileDefinitionSettings, CP.MultiNet.Business","ConfigId":"da5653fc-7305-4ac8-b64c-1eb9b253d44b","RuntimeEditor":"cp-multinet-accountadditionalinfotileruntimesettings","VRConnectionId":"7a2044f4-b42c-44aa-bfb8-6538904e8e4c"}}},{"$type":"Vanrise.Entities.VRTile, Vanrise.Entities","VRTileId":"1ee5ab1b-0900-1b71-402b-0cd0b5f703ae","Name":"Sub Accounts","Settings":{"$type":"Vanrise.Entities.VRTileSettings, Vanrise.Entities","NumberOfColumns":4,"ExtendedSettings":{"$type":"PartnerPortal.CustomerAccess.Business.RetailSubAccountsInfoTileDefinitionSettings, PartnerPortal.CustomerAccess.Business","ConfigId":"138b9459-67a5-4031-aea7-d86093f731d5","RuntimeEditor":"partnerportal-customeraccess-retailsubaccountsinfotileruntimesettings","VRConnectionId":"7a2044f4-b42c-44aa-bfb8-6538904e8e4c","AccountGridFields":{"$type":"System.Collections.Generic.List`1[[PartnerPortal.CustomerAccess.Entities.AccountGridField, PartnerPortal.CustomerAccess.Entities]], mscorlib","$values":[{"$type":"PartnerPortal.CustomerAccess.Entities.AccountGridField, PartnerPortal.CustomerAccess.Entities","FieldName":"Name","FieldTitle":"Name","ColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Normal"}},{"$type":"PartnerPortal.CustomerAccess.Entities.AccountGridField, PartnerPortal.CustomerAccess.Entities","FieldName":"AccountType","FieldTitle":"Type","ColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Normal"}},{"$type":"PartnerPortal.CustomerAccess.Entities.AccountGridField, PartnerPortal.CustomerAccess.Entities","FieldName":"Status","FieldTitle":"Status","ColumnSettings":{"$type":"Vanrise.Entities.GridColumnSettings, Vanrise.Entities","Width":"Normal"}}]},"AccountBEDefinitionId":"9a427357-cf55-4f33-99f7-745206dee7cd"}}},{"$type":"Vanrise.Entities.VRTile, Vanrise.Entities","VRTileId":"cbe76202-125a-e7dc-0cca-c2e31191eb88","Name":"DIDs","Settings":{"$type":"Vanrise.Entities.VRTileSettings, Vanrise.Entities","NumberOfColumns":3,"ExtendedSettings":{"$type":"PartnerPortal.CustomerAccess.Business.DIDTileDefinitionSettings, PartnerPortal.CustomerAccess.Business","ConfigId":"3efaf2e0-bcd2-4c98-890e-6ec5b4e4dd10","RuntimeEditor":"partnerportal-customeraccess-didtileruntimesettings","VRConnectionId":"7a2044f4-b42c-44aa-bfb8-6538904e8e4c","WithSubAccounts":true}}}]}}}','a10addbd-4a9f-4fbb-a54d-7ec0b671a38d',10,0),
--('ACFDFC22-FE0B-491E-B5D1-0A9541B822FE','Account Statement','Account Statement',null	,'6471DA6F-E4DD-4B2A-BFB6-F8EA498CD37C',null,null,null,'{"$type":"PartnerPortal.CustomerAccess.Entities.AccountStatementViewSettings, PartnerPortal.CustomerAccess.Entities","AccountStatementViewData":{"$type":"PartnerPortal.CustomerAccess.Entities.AccountStatementViewData, PartnerPortal.CustomerAccess.Entities","VRConnectionId":"7a2044f4-b42c-44aa-bfb8-6538904e8e4c","AccountTypeId":"20b0c83e-6f53-49c7-b52f-828a19e6dc2a","ExtendedSettings":{"$type":"PartnerPortal.CustomerAccess.MainExtensions.DefaultAccountStatementExtendedSettings, PartnerPortal.CustomerAccess.MainExtensions","ConfigId":"bb59e6be-a590-4142-a5b4-f65daaf02ff6"},"AccountStatementHandler":{"$type":"PartnerPortal.CustomerAccess.MainExtensions.AccountStatement.RetailAccountUser, PartnerPortal.CustomerAccess.MainExtensions","ConfigId":"a8085279-37bf-40c5-941b-a1e46f83dfab"}}}','A0709FCC-0344-4B64-BC0D-50471D052D0F',20,0),
('5687E4BE-DC14-481C-BA28-A80822A769CE','My Invoices','My Invoices',null				,'6471DA6F-E4DD-4B2A-BFB6-F8EA498CD37C',null,null,null,'{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewSettings, PartnerPortal.Invoice.Entities","InvoiceViewItems":{"$type":"System.Collections.Generic.List`1[[PartnerPortal.Invoice.Entities.InvoiceViewItem, PartnerPortal.Invoice.Entities]], mscorlib","$values":[{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewItem, PartnerPortal.Invoice.Entities","InvoiceViewerTypeId":"1924f586-8a46-477d-985f-537cbf5d517d"}]}}','BBEF1C92-DCEC-441B-86A2-7FC0C67716F5',30,0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank],[IsDeleted]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank],[IsDeleted] = s.[IsDeleted]
when not matched by target then
	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank],[IsDeleted])
	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank],s.[IsDeleted]);
----------------------------------------------------------------------------------------------------
END

--[sec].[Module]----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([ID],[Name],[Url],[DefaultViewId],[ParentId],[Icon],[Rank],[AllowDynamic])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('6471DA6F-E4DD-4B2A-BFB6-F8EA498CD37C','Billing Management',null,'82838420-FB3E-459D-88DB-818D4D3DBD29',null,'/images/menu-icons/billing.png',70,0)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Url],[DefaultViewId],[ParentId],[Icon],[Rank],[AllowDynamic]))merge	[sec].[Module] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[DefaultViewId] = s.[DefaultViewId]when not matched by target then	insert([ID],[Name],[Url],[DefaultViewId],[ParentId],[Icon],[Rank],[AllowDynamic])	values(s.[ID],s.[Name],s.[Url],s.[DefaultViewId],s.[ParentId],s.[Icon],s.[Rank],s.[AllowDynamic]);
