--[common].[ExtensionConfiguration]---------------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('A8085279-37BF-40C5-941B-A1E46F83DFAB','PartnerPortal_CustomerAccess_RetailAccountUser','Retail Account User','PartnerPortal_CustomerAccess_AccountStatementContextHandler','{"Editor":"partnerportal-customeraccess-retailaccountuser"}'),
('A0709FCC-0344-4B64-BC0D-50471D052D0F','PartnerPortal_CustomerAccess_AccountStatementView','Portal Account Statement','VR_Security_ViewTypeConfig','{"Editor":"/Client/Modules/Security/Views/View/GenericViewEditor.html","EnableAdd":true,"DirectiveEditor":"partnerportal-customeraccess-accountstatement-vieweditor"}'),
('B3A94A20-92ED-47BF-86D6-1034B720BE73','Retail Account Query Interceptor','Retail Account Query Interceptor','VR_GenericData_RestAPIRecordQueryInterceptorConfig','{"Editor":"partnerportal-customeraccess-retailaccountqueryinterceptor"}'),
('3a02eeea-6f38-4277-bac4-9d8f88f71851','Invoice Viewer Type','Invoice Viewer Type','VR_Common_VRComponentType','{"Editor":"partnerportal-invoice-viewertypesettings"}'),
('12371BE0-CF2C-4CDD-9F4C-E809D912A716','Partner Invoice Query','Partner Invoice Query','PartnerPortal_Invoice_InvoiceQueryInterceptor','{"Editor":"partnerportal-invoice-partnerinvoicequeryinterceptor"}'),
('BBEF1C92-DCEC-441B-86A2-7FC0C67716F5','PartnerPortal_Invoice_InvoiceView','Portal Invoice','VR_Security_ViewTypeConfig','{"Editor":"/Client/Modules/Security/Views/View/GenericViewEditor.html","EnableAdd":true,"DirectiveEditor":"partnerportal-invoice-vieweditor"}'),
('609A37C4-466A-40A9-8140-F23F5D80D3C3','DownloadAttachmentAction','Download Attachment','PartnerPortal_Invoice_InvoiceGridActionSettings','{"Editor":"partnerportal-invoice-gridaction-downloadattachment"}')
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

--[common].[Setting]---------------------------801 to 900-------------------------------------------
begin
set nocount on;
;with cte_data([Id],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('509E467B-4562-4CA6-A32E-E50473B74D2C','Product Info','VR_Common_ProductInfoTechnicalSettings','General','{"Editor" : "vr-common-productinfotechnicalsettings-editor"}','{"$type":"Vanrise.Entities.ProductInfoTechnicalSettings, Vanrise.Entities","ProductInfo":{"$type":"Vanrise.Entities.ProductInfo, Vanrise.Entities","ProductName":"Partner Portal","VersionNumber":"version 0.9"}}',1)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Type],[Category],[Settings],[Data],[IsTechnical]))
merge	[common].[Setting] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Type] = s.[Type],[Category] = s.[Category],[Settings] = s.[Settings],[Data] = s.[Data],[IsTechnical] = s.[IsTechnical]
when not matched by target then
	insert([Id],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
	values(s.[Id],s.[Name],s.[Type],s.[Category],s.[Settings],s.[Data],s.[IsTechnical]);
----------------------------------------------------------------------------------------------------
end