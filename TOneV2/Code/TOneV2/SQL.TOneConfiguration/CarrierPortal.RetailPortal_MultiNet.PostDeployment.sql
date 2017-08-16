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
--[common].[VRComponentType]------------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[ConfigID],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('1924f586-8a46-477d-985f-537cbf5d517d','Multi Net Invoice Viewer','3a02eeea-6f38-4277-bac4-9d8f88f71851','{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeSettings, PartnerPortal.Invoice.Entities","VRComponentTypeConfigId":"3a02eeea-6f38-4277-bac4-9d8f88f71851","VRConnectionId":"7a2044f4-b42c-44aa-bfb8-6538904e8e4c","InvoiceTypeId":"c5f01977-67dd-4b62-b947-0b55f8bd2edb","GridSettings":{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridSettings, PartnerPortal.Invoice.Entities","InvoiceGridFields":{"$type":"System.Collections.Generic.List`1[[PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridField, PartnerPortal.Invoice.Entities]], mscorlib","$values":[{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridField, PartnerPortal.Invoice.Entities","Header":"Company","Field":0,"CustomFieldName":"CompanyId"},{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridField, PartnerPortal.Invoice.Entities","Header":"Branch","Field":0,"CustomFieldName":"BranchId"},{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridField, PartnerPortal.Invoice.Entities","Header":"Serial Number","Field":3},{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridField, PartnerPortal.Invoice.Entities","Header":"From Date","Field":4},{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridField, PartnerPortal.Invoice.Entities","Header":"To Date","Field":5},{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridField, PartnerPortal.Invoice.Entities","Header":"Due Date","Field":7},{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridField, PartnerPortal.Invoice.Entities","Header":"Issue Date","Field":6},{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridField, PartnerPortal.Invoice.Entities","Header":"Amount","Field":0,"CustomFieldName":"TotalCurrentCharges"},{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridField, PartnerPortal.Invoice.Entities","Header":"Duration","Field":0,"CustomFieldName":"Quantity"},{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridField, PartnerPortal.Invoice.Entities","Header":"Currency","Field":0,"CustomFieldName":"CurrencyId"},{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridField, PartnerPortal.Invoice.Entities","Header":"Paid","Field":8}]},"InvoiceGridActions":{"$type":"System.Collections.Generic.List`1[[PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridAction, PartnerPortal.Invoice.Entities]], mscorlib","$values":[{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewerTypeGridAction, PartnerPortal.Invoice.Entities","Title":"Download","InvoiceViewerTypeGridActionId":"00000000-0000-0000-0000-000000000000","Settings":{"$type":"PartnerPortal.Invoice.MainExtensions.InvoiceViewerTypeGridAction.DownloadAttachmentAction, PartnerPortal.Invoice.MainExtensions","ActionTypeName":"DownloadAttachment","ConfigId":"609a37c4-466a-40a9-8140-f23f5d80d3c3","InvoiceAttachmentId":"1530336d-0054-30b5-bce5-eef4dc902dda"}}]}},"ExtendedSettings":{"$type":"PartnerPortal.Invoice.MainExtensions.DefaultInvoiceViewerExtendedSettings, PartnerPortal.Invoice.MainExtensions","ConfigId":"06377156-8265-4424-bd62-46fa5ab2ce41"},"InvoiceQueryInterceptor":{"$type":"PartnerPortal.Invoice.MainExtensions.Invoice.PartnerInvoiceQueryInterceptor, PartnerPortal.Invoice.MainExtensions","ConfigId":"12371be0-cf2c-4cdd-9f4c-e809d912a716"}}')
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

--[common].[Connection]-----------------------------------------------------------------------BEGINset nocount on;;with cte_data([ID],[Name],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('7A2044F4-B42C-44AA-BFB8-6538904E8E4C','Retail Connection','{"$type":"Vanrise.Common.Business.VRInterAppRestConnection, Vanrise.Common.Business","ConfigId":"5cd2aac3-1c74-401f-8010-8b9b5fd9c011","BaseURL":"http://retailapplicationURL","Username":"portalsystemaccount@nodomain.com","Password":"portalaccount@nodomain.com"}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Settings]))merge	[common].[Connection] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]--when matched then--	update set--	[Name] = s.[Name],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Settings])	values(s.[ID],s.[Name],s.[Settings]);----------------------------------------------------------------------------------------------------END
--[sec].[View]--------------------------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank],[IsDeleted])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('ACFDFC22-FE0B-491E-B5D1-0A9541B822FE','Account Statement','Account Statement',null,'6471DA6F-E4DD-4B2A-BFB6-F8EA498CD37C',null,null,null,'{"$type":"PartnerPortal.CustomerAccess.Entities.AccountStatementViewSettings, PartnerPortal.CustomerAccess.Entities","AccountStatementViewData":{"$type":"PartnerPortal.CustomerAccess.Entities.AccountStatementViewData, PartnerPortal.CustomerAccess.Entities","VRConnectionId":"7a2044f4-b42c-44aa-bfb8-6538904e8e4c","AccountTypeId":"20b0c83e-6f53-49c7-b52f-828a19e6dc2a","ExtendedSettings":{"$type":"PartnerPortal.CustomerAccess.MainExtensions.DefaultAccountStatementExtendedSettings, PartnerPortal.CustomerAccess.MainExtensions","ConfigId":"bb59e6be-a590-4142-a5b4-f65daaf02ff6"},"AccountStatementHandler":{"$type":"PartnerPortal.CustomerAccess.MainExtensions.AccountStatement.RetailAccountUser, PartnerPortal.CustomerAccess.MainExtensions","ConfigId":"a8085279-37bf-40c5-941b-a1e46f83dfab"}}}','A0709FCC-0344-4B64-BC0D-50471D052D0F',null,0),
('5687E4BE-DC14-481C-BA28-A80822A769CE','My Invoices','My Invoices',null			,'6471DA6F-E4DD-4B2A-BFB6-F8EA498CD37C',null,null,null,'{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewSettings, PartnerPortal.Invoice.Entities","InvoiceViewItems":{"$type":"System.Collections.Generic.List`1[[PartnerPortal.Invoice.Entities.InvoiceViewItem, PartnerPortal.Invoice.Entities]], mscorlib","$values":[{"$type":"PartnerPortal.Invoice.Entities.InvoiceViewItem, PartnerPortal.Invoice.Entities","InvoiceViewerTypeId":"1924f586-8a46-477d-985f-537cbf5d517d"}]}}','BBEF1C92-DCEC-441B-86A2-7FC0C67716F5',null,0)
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
----------------------------------------------------------------------------------------------------
END