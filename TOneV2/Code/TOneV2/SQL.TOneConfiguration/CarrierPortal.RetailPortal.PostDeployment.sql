--[common].[ExtensionConfiguration]---------------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('A8085279-37BF-40C5-941B-A1E46F83DFAB','PartnerPortal_CustomerAccess_RetailAccountUser','Retail Account User','PartnerPortal_CustomerAccess_AccountStatementContextHandler','{"Editor":"partnerportal-customeraccess-retailaccountuser"}'),
('A0709FCC-0344-4B64-BC0D-50471D052D0F','PartnerPortal_CustomerAccess_AccountStatementView','Portal Account Statement','VR_Security_ViewTypeConfig','{"Editor":"/Client/Modules/Security/Views/View/GenericViewEditor.html","EnableAdd":true,"DirectiveEditor":"partnerportal-customeraccess-accountstatement-vieweditor"}'),
('3a02eeea-6f38-4277-bac4-9d8f88f71851','Invoice Viewer Type','Invoice Viewer Type','VR_Common_VRComponentType','{"Editor":"partnerportal-invoice-viewertypesettings"}'),
('12371BE0-CF2C-4CDD-9F4C-E809D912A716','Partner Invoice Query','Partner Invoice Query','PartnerPortal_Invoice_InvoiceQueryInterceptor','{"Editor":"partnerportal-invoice-partnerinvoicequeryinterceptor"}'),
('BBEF1C92-DCEC-441B-86A2-7FC0C67716F5','PartnerPortal_Invoice_InvoiceView','Portal Invoice','VR_Security_ViewTypeConfig','{"Editor":"/Client/Modules/Security/Views/View/GenericViewEditor.html","EnableAdd":true,"DirectiveEditor":"partnerportal-invoice-vieweditor"}'),
('609A37C4-466A-40A9-8140-F23F5D80D3C3','DownloadAttachmentAction','Download Attachment','PartnerPortal_Invoice_InvoiceGridActionSettings','{"Editor":"partnerportal-invoice-gridaction-downloadattachment"}'),
('bb59e6be-a590-4142-a5b4-f65daaf02ff6','Default','Default','PartnerPortal_CustomerAccess_AccountStatementExtendedSettings','{"Editor":"pportal-custaccess-accountstatement-settings-default"}'),
('06377156-8265-4424-bd62-46fa5ab2ce41','Default','Default','PartnerPortal_Invoice_InvoiceViewerTypeExtendedSettings','{"Editor":"partnerportal-invoice-extendedsettings-default"}'),

('B3A94A20-92ED-47BF-86D6-1034B720BE73','Retail Account Query Interceptor','Retail Account Query Interceptor','VR_GenericData_RestAPIRecordQueryInterceptorConfig','{"Editor":"partnerportal-customeraccess-retailaccountqueryinterceptor"}'),
('1DC33B53-B625-4E51-B427-7952E3817708','Retail Account Query Interceptor','Retail Account Query Interceptor','VR_Analytic_RestAPIAnalyticQueryInterceptorConfig','{"Editor":"partnerportal-customeraccess-retailaccountanalyticqueryinterceptor"}')
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
('509E467B-4562-4CA6-A32E-E50473B74D2C','Product Info','VR_Common_ProductInfoTechnicalSettings','General','{"Editor" : "vr-common-productinfotechnicalsettings-editor"}','{"$type":"Vanrise.Entities.ProductInfoTechnicalSettings, Vanrise.Entities","ProductInfo":{"$type":"Vanrise.Entities.ProductInfo, Vanrise.Entities","ProductName":"Partner Portal","VersionNumber":"version #VersionNumber# ~ #VersionDate#"}}',1)
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


--[sec].[User]--------------------------------------------------------------------------------Beginset nocount on;;with cte_data([Name],[Password],[Email],[TenantId],[Description],[EnabledTill],[ExtendedSettings],[SecurityProviderId])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('Retail Account','JO9ZxpU4iuZ4iNY8PxfFHI/yQtJo','retailsystemaccount@nodomain.com',1,'System Account used to connect from the Retail Application',null,null,'9554069B-795E-4BB1-BFF3-9AF0F47FC0FF')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([Name],[Password],[Email],[TenantId],[Description],[EnabledTill],[ExtendedSettings],[SecurityProviderId]))merge	[sec].[User] as tusing	cte_data as son		1=1 and t.[Email] = s.[Email]--when matched then--	update set--	[Name] = s.[Name],[Password] = s.[Password],[TenantId] = s.[TenantId],[Description] = s.[Description],[EnabledTill] = s.[EnabledTill],[ExtendedSettings] = s.[ExtendedSettings]when not matched by target then	insert([Name],[Password],[Email],[TenantId],[Description],[EnabledTill],[ExtendedSettings],[SecurityProviderId])	values(s.[Name],s.[Password],s.[Email],s.[TenantId],s.[Description],s.[EnabledTill],s.[ExtendedSettings],s.[SecurityProviderId]);----------------------------------------------------------------------------------------------------ENDDECLARE @RetailAccountID int = (SELECT ID from [sec].[User] WHERE Email = 'retailsystemaccount@nodomain.com')--[sec].[Permission]--------------------------------------------------------------------------------BEGINset nocount on;;with cte_data([HolderType],[HolderId],[EntityType],[EntityId],[PermissionFlags])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////(0, CONVERT(varchar, @RetailAccountID),1,'b4158657-230e-40bf-b88c-f2b2ca8835de','[{"Name":"Add","Value":1},{"Name":"Reset Password","Value":1}]')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([HolderType],[HolderId],[EntityType],[EntityId],[PermissionFlags]))merge	[sec].[Permission] as tusing	cte_data as son		1=1 and t.[HolderType] = s.[HolderType] and t.[HolderId] = s.[HolderId] and t.[EntityType] = s.[EntityType] and t.[EntityId] = s.[EntityId]when matched then	update set	[PermissionFlags] = s.[PermissionFlags]when not matched by target then	insert([HolderType],[HolderId],[EntityType],[EntityId],[PermissionFlags])	values(s.[HolderType],s.[HolderId],s.[EntityType],s.[EntityId],s.[PermissionFlags]);----------------------------------------------------------------------------------------------------END	--[common].[Connection]-----------------------------------------------------------------------BEGINset nocount on;;with cte_data([ID],[Name],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('7A2044F4-B42C-44AA-BFB8-6538904E8E4C','Retail Connection','{"$type":"Vanrise.Common.Business.VRInterAppRestConnection, Vanrise.Common.Business","ConfigId":"5cd2aac3-1c74-401f-8010-8b9b5fd9c011","BaseURL":"http://retailapplicationURL","Username":"portalsystemaccount@nodomain.com","Password":"portalaccount@nodomain.com"}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Settings]))merge	[common].[Connection] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]--when matched then--	update set--	[Name] = s.[Name],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Settings])	values(s.[ID],s.[Name],s.[Settings]);----------------------------------------------------------------------------------------------------END

--[genericdata].[datastore]-------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('7403534B-628F-4114-B1C9-C9EDAB294219','VR Rest API','{"$type":"Vanrise.GenericData.MainExtensions.DataStorages.DataStore.VRRestAPIDataStoreSettings, Vanrise.GenericData.MainExtensions","ConfigId":"4829119d-f86f-4a6c-a6c0-cdb3fc8274c1","ConnectionId":"7a2044f4-b42c-44aa-bfb8-6538904e8e4c","IsRemoteDataStore":true}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings]))
merge	[genericdata].[datastore] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Settings])
	values(s.[ID],s.[Name],s.[Settings]);


--[genericdata].[BusinessEntityDefinition]----------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Title],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('56287CBB-376B-42F3-9BA0-04FFBE609A70','Remote_BE_ChargingPolicy','Charging Policy','{"$type":"Vanrise.GenericData.Business.VRRestAPIBEDefinitionSettings, Vanrise.GenericData.Business","ConfigId":"b4f22ffb-b663-4f5f-af53-acbef7224dfb","IdType":"System.Int32","DefinitionEditor":"vr-genericdata-restapibedefinitions-editor","SelectorUIControl":"vr-genericdata-businessentity-remoteselector","ManagerFQTN":"Vanrise.GenericData.Business.VRRestAPIBusinessEntityManager, Vanrise.GenericData.Business","ConnectionId":"7a2044f4-b42c-44aa-bfb8-6538904e8e4c","RemoteBEDefinitionId":"d0ee9bf8-385e-48ef-b989-a87666a00072","SingularTitle":"Charging Policy","PluralTitle":"Charging Policies"}'),
('AEB7BDC9-8A66-4297-8A11-0865515DF4E6','Remote_BE_Currency','Remote Currency','{"$type":"Vanrise.GenericData.Business.VRRestAPIBEDefinitionSettings, Vanrise.GenericData.Business","ConfigId":"b4f22ffb-b663-4f5f-af53-acbef7224dfb","IdType":"System.Int32","DefinitionEditor":"vr-genericdata-restapibedefinitions-editor","SelectorUIControl":"vr-genericdata-businessentity-remoteselector","ManagerFQTN":"Vanrise.GenericData.Business.VRRestAPIBusinessEntityManager, Vanrise.GenericData.Business","ConnectionId":"7a2044f4-b42c-44aa-bfb8-6538904e8e4c","RemoteBEDefinitionId":"d41ea344-c3c0-4203-8583-019b6b3edb76","SingularTitle":"Currency","PluralTitle":"Currencies"}'),
('48181232-9B13-4F76-8BA6-19AC19753C64','Remote_BE_SaleZone','Sale Zone','{"$type":"Vanrise.GenericData.Business.VRRestAPIBEDefinitionSettings, Vanrise.GenericData.Business","ConfigId":"b4f22ffb-b663-4f5f-af53-acbef7224dfb","IdType":"System.Int64","DefinitionEditor":"vr-genericdata-restapibedefinitions-editor","SelectorUIControl":"vr-genericdata-businessentity-remoteselector","ManagerFQTN":"Vanrise.GenericData.Business.VRRestAPIBusinessEntityManager, Vanrise.GenericData.Business","ConnectionId":"7a2044f4-b42c-44aa-bfb8-6538904e8e4c","RemoteBEDefinitionId":"10740f30-5a20-4718-b5af-0e2e160b21c2","SingularTitle":"Zone","PluralTitle":"Zones"}'),
('A902E235-9E7F-4CD4-9834-21B8C88CBE3B','Remote_BE_Country','Remote Country','{"$type":"Vanrise.GenericData.Business.VRRestAPIBEDefinitionSettings, Vanrise.GenericData.Business","ConfigId":"b4f22ffb-b663-4f5f-af53-acbef7224dfb","IdType":"System.Int32","DefinitionEditor":"vr-genericdata-restapibedefinitions-editor","SelectorUIControl":"vr-genericdata-businessentity-remoteselector","ManagerFQTN":"Vanrise.GenericData.Business.VRRestAPIBusinessEntityManager, Vanrise.GenericData.Business","ConnectionId":"7a2044f4-b42c-44aa-bfb8-6538904e8e4c","RemoteBEDefinitionId":"df5cdc08-ddf1-4d4e-b1f6-d17b3833452f","SingularTitle":"Country","PluralTitle":"Countries"}'),
('8E5EAEDF-F4F0-49FA-AB46-306C8044317D','Remote_BE_ServiceType','Service Type','{"$type":"Vanrise.GenericData.Business.VRRestAPIBEDefinitionSettings, Vanrise.GenericData.Business","ConfigId":"b4f22ffb-b663-4f5f-af53-acbef7224dfb","IdType":"System.Guid","DefinitionEditor":"vr-genericdata-restapibedefinitions-editor","SelectorUIControl":"vr-genericdata-businessentity-remoteselector","ManagerFQTN":"Vanrise.GenericData.Business.VRRestAPIBusinessEntityManager, Vanrise.GenericData.Business","ConnectionId":"7a2044f4-b42c-44aa-bfb8-6538904e8e4c","RemoteBEDefinitionId":"bfad446f-7129-45b1-94bf-febd290f394d","SingularTitle":"Service Type","PluralTitle":"Service Types"}'),
('06480AEF-4988-4688-A9D1-ADF9C131AF50','Remote_BE_SaleZoneMasterPlan','Master Sale Zone','{"$type":"Vanrise.GenericData.Business.VRRestAPIBEDefinitionSettings, Vanrise.GenericData.Business","ConfigId":"b4f22ffb-b663-4f5f-af53-acbef7224dfb","IdType":"System.Int64","DefinitionEditor":"vr-genericdata-restapibedefinitions-editor","SelectorUIControl":"vr-genericdata-businessentity-remoteselector","ManagerFQTN":"Vanrise.GenericData.Business.VRRestAPIBusinessEntityManager, Vanrise.GenericData.Business","ConnectionId":"7a2044f4-b42c-44aa-bfb8-6538904e8e4c","RemoteBEDefinitionId":"f650d523-7adb-4787-a2f6-c13168f7e8f7","SingularTitle":"Master Sale Zone","PluralTitle":"Master Sale Zones"}'),
('5F3C5C17-2D17-46EB-B7F2-DD44E979DBF9','Remote_BE_Package','Package','{"$type":"Vanrise.GenericData.Business.VRRestAPIBEDefinitionSettings, Vanrise.GenericData.Business","ConfigId":"b4f22ffb-b663-4f5f-af53-acbef7224dfb","IdType":"System.Int32","DefinitionEditor":"vr-genericdata-restapibedefinitions-editor","SelectorUIControl":"vr-genericdata-businessentity-remoteselector","ManagerFQTN":"Vanrise.GenericData.Business.VRRestAPIBusinessEntityManager, Vanrise.GenericData.Business","ConnectionId":"7a2044f4-b42c-44aa-bfb8-6538904e8e4c","RemoteBEDefinitionId":"c0c76db1-4876-4e0d-9b59-ca89120e6be9","SingularTitle":"Package","PluralTitle":"Packages"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Settings]))
merge	[genericdata].[BusinessEntityDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Title],[Settings])
	values(s.[ID],s.[Name],s.[Title],s.[Settings]);
