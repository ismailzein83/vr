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
GO--delete useless views from ClearVoice product such 'My Scheduler Service' since it is replace with 'Schedule Test Calls', 'Style Definitions', 'Organizational Charts','Countries'
delete from [sec].[View] where [Id] in ('C65ED28A-36D0-4047-BEC5-030D35B02308','66DE2441-8A96-41E7-94EA-9F8AF38A3515','DCF8CA21-852C-41B9-9101-6990E545509D','A1CE55FE-6CF4-4F15-9BC2-8E1F8DF68561','25994374-CB99-475B-8047-3CDB7474A083','9F691B87-4936-4C4C-A757-4B3E12F7E1D9', 'E5CA33D9-18AC-4BA1-8E8E-FB476ECAA9A9', '0F111ADC-B7F6-46A4-81BC-72FFDEB305EB', '4D7BF410-E4C6-4D6F-B519-D6B5C2C2F712','604B2CB5-B839-4E51-8D13-3C1C84D05DEE')

GO--delete useless [Setting] from ClearVoice product such 'System Currency', etc...
delete from [common].[Setting] where [Id] in ('1C833B2D-8C97-4CDD-A1C1-C1B4D9D299DE')
GO

--[sec].[View]--------------------------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank],[IsDeleted])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('7079BD63-BFE2-4519-9B1B-8158A2F3A12A','System Logs','System Logs',null,'BAAF681E-AB1C-4A64-9A35-3F3951398881',null,null,null,'{"$type":"Vanrise.Common.Business.MasterLogViewSettings, Vanrise.Common.Business","Items":[{"PermissionName":"VRCommon_System_Log: View General Logs","Directive":"vr-log-entry-search","Title":"General"},{"PermissionName":"VRCommon_System_Log: View Action Audit","Directive":"vr-common-actionaudit-search","Title":"Action Audit"}]}','372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',15,0)
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

Delete from [common].[Setting] where [ID] in (	'1CB20F2C-A835-4320-AEC7-E034C5A756E9',--'Bank Details'
												'1C833B2D-8C97-4CDD-A1C1-C1B4D9D299DE',--'System Currency'
												'81F62AC3-BAE4-4A2F-A60D-A655494625EA' )--'Company Setting'

--[common].[Setting]---------------------------801 to 900-------------------------------------------
begin
set nocount on;
;with cte_data([Id],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('509E467B-4562-4CA6-A32E-E50473B74D2C','Product Info','VR_Common_ProductInfoTechnicalSettings','General','{"Editor" : "vr-common-productinfotechnicalsettings-editor"}','{"$type":"Vanrise.Entities.ProductInfoTechnicalSettings, Vanrise.Entities","ProductInfo":{"$type":"Vanrise.Entities.ProductInfo, Vanrise.Entities","ProductName":"Carrier Portal","VersionNumber":"version #VersionNumber# ~ #VersionDate#"}}',1)
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

Delete from [runtime].[SchedulerTaskActionType] where Id in ('0A15BC35-A3A7-4ED3-B09B-1B41A7A9DDC9','7A35F562-319B-47B3-8258-EC1A704A82EB') --Exchange Rate, workflow

--common.ExtensionConfiguration---------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('48FA768C-7482-476D-9DB0-26C6A0CEB9A0','LiveBalance','Live Balance','VRCommon_VRTileExtendedSettings','{"Editor":"partnerportal-customeraccess-livebalancetiledefinitionsettings"}'),
('BBEF1C92-DCEC-441B-86A2-7FC0C67716F5','PartnerPortal_Invoice_InvoiceView','Portal Invoice','VR_Security_ViewTypeConfig','{"Editor":"/Client/Modules/Security/Views/View/GenericViewEditor.html","EnableAdd":true,"DirectiveEditor":"partnerportal-invoice-vieweditor"}'),
('3A02EEEA-6F38-4277-BAC4-9D8F88F71851','Invoice Viewer Type','Invoice Viewer Type','VR_Common_VRComponentType','{"Editor":"partnerportal-invoice-viewertypesettings"}'),
('06CD79C8-B1C0-4A33-A757-A36EBD96EA5B','Analytic','Analytic','VRCommon_VRTileExtendedSettings','{"Editor":"partnerportal-customeraccess-analytictiledefinitionsettings"}'),
('A8A6C730-9EF7-41E3-B875-C9FF7B6696FC','InvoiceTileDefinition','Invoice Tile','VRCommon_VRTileExtendedSettings','{"Editor":"partnerportal-invoice-invoicetiledefinitionsettings"}'),
('12371BE0-CF2C-4CDD-9F4C-E809D912A716','Partner Invoice Query','Partner Invoice Query','PartnerPortal_Invoice_InvoiceQueryInterceptor','{"Editor":"partnerportal-invoice-partnerinvoicequeryinterceptor"}'),
('3efaf2e0-bcd2-4c98-890e-6ec5b4e4dd10','DIDTileDefinition','DID Tile','VRCommon_VRTileExtendedSettings','{"Editor":"partnerportal-customeraccess-didtiledefinitionsettings"}'),
('ff8e7752-9df7-4017-83ab-6fa8a9cdb30f','RetailAccountInfoTileDefinition','Retail Account InfoTile','VRCommon_VRTileExtendedSettings','{"Editor":"partnerportal-customeraccess-retailaccountinfotiledefinitionsettings"}'),
('138b9459-67a5-4031-aea7-d86093f731d5','RetailSubAccountsInfoTileDefinition','Retail Sub Accounts Info Tile','VRCommon_VRTileExtendedSettings','{"Editor":"partnerportal-customeraccess-retailsubaccountsinfotiledefinitionsettings"}')
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