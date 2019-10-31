﻿



















--Make sure to use same .json file using DEVTOOLS under http://192.168.110.185:8037





























--[sec].[View]--------------------------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank],[IsDeleted])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('a1d2e7a1-966d-473a-a7d5-ae3cdd3c48d1','Dashboard','Dashboard',NULL				,NULL,NULL,NULL,NULL,'{"$type":"Vanrise.Common.Business.VRTileViewSettings, Vanrise.Common.Business","VRTileViewData":{"$type":"Vanrise.Common.Business.VRTileViewData, Vanrise.Common.Business","VRTiles":{"$type":"System.Collections.Generic.List`1[[Vanrise.Entities.VRTile, Vanrise.Entities]], mscorlib","$values":[{"$type":"Vanrise.Entities.VRTile, Vanrise.Entities","VRTileId":"080d69b1-675a-30f1-f1ca-73fa52f3b66e","Name":"Last Invoice","Settings":{"$type":"Vanrise.Entities.VRTileSettings, Vanrise.Entities","ExtendedSettings":{"$type":"PartnerPortal.Invoice.Business.InvoiceTileDefinitionSettings, PartnerPortal.Invoice.Business","ConfigId":"a8a6c730-9ef7-41e3-b875-c9ff7b6696fc","RuntimeEditor":"partnerportal-invoice-invoicetileruntimesettings","VRConnectionId":"7a2044f4-b42c-44aa-bfb8-6538904e8e4c","InvoiceTypeId":"d86eaffb-5369-4535-8b23-200738f579a2","ViewId":"5687e4be-dc14-481c-ba28-a80822a769ce"}}},{"$type":"Vanrise.Entities.VRTile, Vanrise.Entities","VRTileId":"df65f7f4-be26-eacb-6909-28a56e71afc7","Name":"My Balance","Settings":{"$type":"Vanrise.Entities.VRTileSettings, Vanrise.Entities","ExtendedSettings":{"$type":"PartnerPortal.CustomerAccess.Business.LiveBalanceTileDefinitionSettings, PartnerPortal.CustomerAccess.Business","ConfigId":"48fa768c-7482-476d-9db0-26c6a0ceb9a0","RuntimeEditor":"partnerportal-customeraccess-livebalancetileruntimesettings","VRConnectionId":"7a2044f4-b42c-44aa-bfb8-6538904e8e4c","AccountTypeId":"20b0c83e-6f53-49c7-b52f-828a19e6dc2a","ViewId":"acfdfc22-fe0b-491e-b5d1-0a9541b822fe"}}},{"$type":"Vanrise.Entities.VRTile, Vanrise.Entities","VRTileId":"820e82c8-d05d-865c-74c5-e03ee36a0a69","Name":"Calls","Settings":{"$type":"Vanrise.Entities.VRTileSettings, Vanrise.Entities","ExtendedSettings":{"$type":"PartnerPortal.CustomerAccess.Business.AnalyticDefinitionSettings, PartnerPortal.CustomerAccess.Business","ConfigId":"06cd79c8-b1c0-4a33-a757-a36ebd96ea5b","RuntimeEditor":"partnerportal-customeraccess-analytictileruntimesettings","Queries":{"$type":"System.Collections.Generic.List`1[[PartnerPortal.CustomerAccess.Business.AnalyticQuery, PartnerPortal.CustomerAccess.Business]], mscorlib","$values":[{"$type":"PartnerPortal.CustomerAccess.Business.AnalyticQuery, PartnerPortal.CustomerAccess.Business","Name":"Today","UserDimensionName":"Company","VRConnectionId":"7a2044f4-b42c-44aa-bfb8-6538904e8e4c","TableId":"D722F557-9CDC-4634-A86E-A941BF51C035","Measures":{"$type":"System.Collections.Generic.List`1[[PartnerPortal.CustomerAccess.Business.MeasureItem, PartnerPortal.CustomerAccess.Business]], mscorlib","$values":[{"$type":"PartnerPortal.CustomerAccess.Business.MeasureItem, PartnerPortal.CustomerAccess.Business","MeasureItemId":"31d8d641-10eb-d569-fbba-384b45e5062c","MeasureName":"CountCDRs","MeasureTitle":"Today"}]},"TimePeriod":{"$type":"Vanrise.Common.MainExtensions.TodayTimePeriod, Vanrise.Common.MainExtensions","ConfigId":"de4f7720-5519-466c-8f14-e5f66a56dc42"}},{"$type":"PartnerPortal.CustomerAccess.Business.AnalyticQuery, PartnerPortal.CustomerAccess.Business","Name":"This Month","UserDimensionName":"Company","VRConnectionId":"7a2044f4-b42c-44aa-bfb8-6538904e8e4c","TableId":"D722F557-9CDC-4634-A86E-A941BF51C035","Measures":{"$type":"System.Collections.Generic.List`1[[PartnerPortal.CustomerAccess.Business.MeasureItem, PartnerPortal.CustomerAccess.Business]], mscorlib","$values":[{"$type":"PartnerPortal.CustomerAccess.Business.MeasureItem, PartnerPortal.CustomerAccess.Business","MeasureItemId":"3c10a72c-062f-665e-dfca-8b6325f4b173","MeasureName":"CountCDRs","MeasureTitle":"This Month"}]},"TimePeriod":{"$type":"Vanrise.Common.MainExtensions.CurrentMonthTimePeriod, Vanrise.Common.MainExtensions","ConfigId":"d03b778a-47da-474d-8a20-87f76c75145a"}}]},"OrderedMeasureIds":{"$type":"System.Collections.Generic.List`1[[System.Guid, mscorlib]], mscorlib","$values":["31d8d641-10eb-d569-fbba-384b45e5062c","3c10a72c-062f-665e-dfca-8b6325f4b173"]},"ViewId":"91164cb8-b3f3-4ce6-9b62-183ae3ee79cf"}}}]}}}','a10addbd-4a9f-4fbb-a54d-7ec0b671a38d',	10,0),
('ACFDFC22-FE0B-491E-B5D1-0A9541B822FE','Account Statement','Account Statement',null,'6471DA6F-E4DD-4B2A-BFB6-F8EA498CD37C',null,null,null,'{"$type":"PartnerPortal.CustomerAccess.Entities.AccountStatementViewSettings, PartnerPortal.CustomerAccess.Entities","AccountStatementViewData":{"$type":"PartnerPortal.CustomerAccess.Entities.AccountStatementViewData, PartnerPortal.CustomerAccess.Entities","VRConnectionId":"7a2044f4-b42c-44aa-bfb8-6538904e8e4c","AccountTypeId":"20b0c83e-6f53-49c7-b52f-828a19e6dc2a","ExtendedSettings":{"$type":"PartnerPortal.CustomerAccess.MainExtensions.DefaultAccountStatementExtendedSettings, PartnerPortal.CustomerAccess.MainExtensions","ConfigId":"bb59e6be-a590-4142-a5b4-f65daaf02ff6"},"AccountStatementHandler":{"$type":"PartnerPortal.CustomerAccess.MainExtensions.AccountStatement.RetailAccountUser, PartnerPortal.CustomerAccess.MainExtensions","ConfigId":"a8085279-37bf-40c5-941b-a1e46f83dfab"}}}','A0709FCC-0344-4B64-BC0D-50471D052D0F',20,0),
('5687E4BE-DC14-481C-BA28-A80822A769CE','My Invoices','My Invoices',null			,'6471DA6F-E4DD-4B2A-BFB6-F8EA498CD37C',null,null,null,'{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewSettings, PartnerPortal.Invoice.Entities","InvoiceViewItems":{"$type":"System.Collections.Generic.List`1[[PartnerPortal.Invoice.Entities.InvoiceViewItem, PartnerPortal.Invoice.Entities]], mscorlib","$values":[{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewItem, PartnerPortal.Invoice.Entities","InvoiceViewerTypeId":"bf148e4a-673c-4429-863b-5ba1ec20ff2e"}]}}','BBEF1C92-DCEC-441B-86A2-7FC0C67716F5',30,0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank],[IsDeleted]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank]
when not matched by target then
	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank],[IsDeleted])
	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank],s.[IsDeleted]);
----------------------------------------------------------------------------------------------------
END

--[sec].[Module]----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([ID],[Name],[Url],[DefaultViewId],[ParentId],[Icon],[Rank],[AllowDynamic])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('6471DA6F-E4DD-4B2A-BFB6-F8EA498CD37C','Billing Management',null,'a1d2e7a1-966d-473a-a7d5-ae3cdd3c48d1',null,'/Client/Images/menu-icons/billing.png',70,0)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Url],[DefaultViewId],[ParentId],[Icon],[Rank],[AllowDynamic]))merge	[sec].[Module] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[DefaultViewId] = s.[DefaultViewId]when not matched by target then	insert([ID],[Name],[Url],[DefaultViewId],[ParentId],[Icon],[Rank],[AllowDynamic])	values(s.[ID],s.[Name],s.[Url],s.[DefaultViewId],s.[ParentId],s.[Icon],s.[Rank],s.[AllowDynamic]);

--[common].[VRComponentType]------------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[ConfigID],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('BF148E4A-673C-4429-863B-5BA1EC20FF2E','Invoice Viewer','3A02EEEA-6F38-4277-BAC4-9D8F88F71851','{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeSettings, PartnerPortal.Invoice.Entities","VRComponentTypeConfigId":"3a02eeea-6f38-4277-bac4-9d8f88f71851","VRConnectionId":"7a2044f4-b42c-44aa-bfb8-6538904e8e4c","InvoiceTypeId":"d86eaffb-5369-4535-8b23-200738f579a2","GridSettings":{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridSettings, PartnerPortal.Invoice.Entities","InvoiceGridFields":{"$type":"System.Collections.Generic.List`1[[PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridField, PartnerPortal.Invoice.Entities]], mscorlib","$values":[{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridField, PartnerPortal.Invoice.Entities","Header":"Serial Number","Field":3},{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridField, PartnerPortal.Invoice.Entities","Header":"From Date","Field":4},{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridField, PartnerPortal.Invoice.Entities","Header":"To Date","Field":5},{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridField, PartnerPortal.Invoice.Entities","Header":"Due Date","Field":7},{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridField, PartnerPortal.Invoice.Entities","Header":"Issue Date","Field":6},{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridField, PartnerPortal.Invoice.Entities","Header":"Amount","Field":0,"CustomFieldName":"TotalAmount"},{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridField, PartnerPortal.Invoice.Entities","Header":"Duration","Field":0,"CustomFieldName":"TotalDuration"},{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridField, PartnerPortal.Invoice.Entities","Header":"Currency","Field":0,"CustomFieldName":"CurrencyId"},{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridField, PartnerPortal.Invoice.Entities","Header":"Paid","Field":8}]},"InvoiceGridActions":{"$type":"System.Collections.Generic.List`1[[PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridAction, PartnerPortal.Invoice.Entities]], mscorlib","$values":[{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridAction, PartnerPortal.Invoice.Entities","Title":"Download","InvoiceViewerTypeGridActionId":"7180da8b-3d34-6c8c-2c5c-15aa4d83ed31","Settings":{"$type":"PartnerPortal.Invoice.MainExtensions.InvoiceViewerTypeGridAction.DownloadAttachmentAction, PartnerPortal.Invoice.MainExtensions","ActionTypeName":"DownloadAttachment","ConfigId":"609a37c4-466a-40a9-8140-f23f5d80d3c3","InvoiceAttachmentId":"c19aff1b-7ada-6769-2eb3-5b5cd599f575"}}]}},"ExtendedSettings":{"$type":"PartnerPortal.Invoice.MainExtensions.DefaultInvoiceViewerExtendedSettings, PartnerPortal.Invoice.MainExtensions","ConfigId":"06377156-8265-4424-bd62-46fa5ab2ce41"},"InvoiceQueryInterceptor":{"$type":"PartnerPortal.Invoice.MainExtensions.Invoice.PartnerInvoiceQueryInterceptor, PartnerPortal.Invoice.MainExtensions","ConfigId":"12371be0-cf2c-4cdd-9f4c-e809d912a716"}}')
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