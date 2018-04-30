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
--[common].[extensionconfiguration]---------------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('5A8308B0-BA99-4002-939B-6D76A1DF1FA7','VR_Invoice_GenericInvoice','Invoice','VR_Security_ViewTypeConfig','{"Editor":"/Client/Modules/VR_Invoice/Views/InvoiceViewEditor.html","EnableAdd":true}'),

('EC99F6DB-2617-4AF4-8A20-AED83B404FCA','Invoice To VR Object Convertor','Invoice To VR Object Convertor','VR_BEBridge_BEConvertor'								,'{"Editor":"vr-invoice-convertor-editor"}'),

('59551673-F340-4BDE-8040-F213A24AB5A8','Invoice Reader','Invoice Reader','VR_BEBridge_SourceBeReaders'															,'{"Editor":"vr-invoice-reader-directive"}'),

('EDA771DE-D1CC-4137-B2AD-1647D4C50B81','Invoice Item Field','Invoice Item Field','VR_InvoiceType_ItemSetNameParts'												,'{"Editor":"vr-invoicetype-itemsetnameparts-invoiceitemfield"}'),
('17832921-2BD5-4D2B-B952-C45BDBA25B33','Current ItemSetName','Current ItemSetName','VR_InvoiceType_ItemSetNameParts'											,'{"Editor":"vr-invoicetype-itemsetnameparts-currentitemsetname"}'),
('9E0B783D-FB05-4173-A137-CF01F2CDC83A','Constant','Constant','VR_InvoiceType_ItemSetNameParts'																	,'{"Editor":"vr-invoicetype-itemsetnameparts-constant"}'),

('B9CB6032-438E-42FD-9520-E3451FAD6A71','Date','Date','VR_InvoiceType_SerialNumberParts'																		,'{"Editor":"vr-invoicetype-serialnumber-invoicedate"}'),
('034D0103-916C-48DC-8A7F-986DEB09FE3F','Sequence','Sequence','VR_InvoiceType_SerialNumberParts'																,'{"Editor":"vr-invoicetype-serialnumber-invoicesequence"}'),
('5C47A8C8-240C-41DA-9A3C-C671BC03D478','Invoice Field','Invoice Field','VR_InvoiceType_SerialNumberParts'														,'{"Editor":"vr-invoicetype-serialnumber-invoicefield"}'),

('71F19A9B-0FAD-4370-B390-3F28137DE1EE','Carrier Partner','Carrier Partner','VR_Invoice_InvoiceType_InvoicePartnerSettings'										,'{"Editor":"vr-invoicetype-partnersettings-carrier"}'),

('20359B38-3A22-4D31-ACAA-CEB099D5A62C','PaidInvoice','Paid Invoice','VR_Invoice_InvoiceType_InvoiceGridFilterConditionConfig'									,'{"Editor":"vr-invoicetype-invoicefiltercondition-ispaidinvoice"}'),
('62b90539-aaab-4a76-9967-40feaa07a3c1','SentInvoice','Sent Invoice','VR_Invoice_InvoiceType_InvoiceGridFilterConditionConfig'									,'{"Editor":"vr-invoicetype-invoicefiltercondition-issentinvoice"}'),
('3764B09D-3F5A-4121-91D3-4AEF308AD437','FilterGroup','Filter Group','VR_Invoice_InvoiceType_InvoiceGridFilterConditionConfig'									,'{"Editor":"vr-invoicetype-invoicefiltercondition-filtergroup"}'),

('B220DD18-D38E-4170-8E80-7508854C29F6','InvoiceLocked','Invoice Locked','VR_Invoice_InvoiceType_InvoiceGridFilterConditionConfig'								,'{"Editor":"vr-invoicetype-invoicefiltercondition-islockedinvoice"}'),
('C61DE7AE-34D3-457B-9C00-78F002AF508B','Generic','Generic','VR_Invoice_ItemsFilter'																			,'{"Editor":"vr-invoicetype-datasource-itemsfilter-generic"}'),

('B0F8D3C8-33C3-4FA6-AB7C-016C14575F47','Taxes','Taxes','VR_Invoice_InvoiceType_RDLCDataSourceSettings'															,'{"Editor":"vr-invoicetype-datasourcesettings-taxes"}'),
('DE6F2641-A4A8-4F56-AEB4-2A0A25000408','Bank Details','Bank Details','VR_Invoice_InvoiceType_RDLCDataSourceSettings'											,'{"Editor":"vr-invoicetype-datasourcesettings-bankdetails"}'),
('6721BF29-D257-47D9-8D56-4EE10538BFDC','Items','Items','VR_Invoice_InvoiceType_RDLCDataSourceSettings'															,'{"Editor":"vr-invoicetype-datasourcesettings-rdlcitems"}'),
('65D64951-AD27-475C-81D2-FAC7816E6E4B','Invoice','Invoice','VR_Invoice_InvoiceType_RDLCDataSourceSettings'														,'{"Editor":"vr-invoicetype-datasourcesettings-invoice"}'),
('F41D670E-F079-4D38-A2B5-9B6394299EFA','Item Grouping','Item Grouping','VR_Invoice_InvoiceType_RDLCDataSourceSettings'											,'{"Editor":"vr-invoicetype-datasourcesettings-itemgrouping"}'),

('786EDDD6-1EC7-4C44-889C-E7246B51AED0','CustomField','Custom Field','VR_Invoice_InvoiceType_DLCParameterSettings'												,'{"Editor":"vr-invoicetype-openrdlcreport-parametersettings-customfield"}'),
('7A1A9991-EC84-4E92-B1D1-1C5140DF8FF4','Field','Field','VR_Invoice_InvoiceType_DLCParameterSettings'															,'{"Editor":"vr-invoicetype-openrdlcreport-parametersettings-field"}'),

('D74A64B6-FDFA-4095-B6CD-6FE0E31E0BE1','Invoice','Invoice','VR_Common_ObjectType'																				,'{"Editor":"vr-invoice-invoiceobjecttype", "PropertyEvaluatorExtensionType": "VR_Invoice_InvoiceField_PropertyEvaluator"}'),
('259F1D17-09A0-4BDA-A83A-BFC5624AD73B','InvoiceField','Invoice Field','VR_Invoice_InvoiceField_PropertyEvaluator'												,'{"Editor":"vr-invoice-invoicefieldpropertyevaluator"}'),

('CE1699D9-2696-4931-B332-D519AECF526E','InvoiceField','Invoice RDLC File','VR_InvoiceActions_SendEmailAttachmentTypeConfig'									,'{"Editor":"vr-invoiceactions-sendemail-attachmenttype-rdlfileconverter"}'),
('c24ce2de-ddd2-44bd-a2b0-a9133113d7c0','Conditional','Conditional','VR_InvoiceActions_SendEmailAttachmentTypeConfig'									,'{"Editor":"vr-invoiceactions-sendemail-attachmenttype-conditionalinvoicefile"}'),

('6F31ACAD-1FD8-4350-AC3B-1EEAE60A1D5D','Partner','Partner','VR_Invoice_InvoiceType_InvoiceGeneratorFilterConditionConfig'										,'{"Editor":"vr-invoicetype-invoicegeneratorfiltercondition-partner"}'),
('A72E2AAA-E837-47BD-8BF4-C41EA07893EA','FilterGroup','Filter Group','VR_Invoice_InvoiceType_InvoiceGeneratorFilterConditionConfig'								,'{"Editor":"vr-invoicetype-invoicegeneratorfiltercondition-filtergroup"}'),

('82230E8D-680B-4362-8C00-D14D6E8E8AC1','Set Lock','Set Lock','VR_Invoice_InvoiceType_InvoiceGridActionSettings'												,'{"Editor":"vr-invoicetype-gridactionsettings-setinvoicelock"}'),
('ebf6d7eb-eec7-44b9-b1d4-f4a435680c14','Set Deleted','Set Deleted','VR_Invoice_InvoiceType_InvoiceGridActionSettings'												,'{"Editor":"vr-invoicetype-gridactionsettings-setinvoicedeleted"}'),
('6AB1BA29-F57F-439E-AEA6-7A98AF3FE184','Send Email','Send Email','VR_Invoice_InvoiceType_InvoiceGridActionSettings'											,'{"Editor":"vr-invoicetype-gridactionsettings-sendemail"}'),
('6B7841D1-8ECF-48CE-88D6-440342654ADC','Recreate','Recreate','VR_Invoice_InvoiceType_InvoiceGridActionSettings'												,'{"Editor":"vr-invoicetype-gridactionsettings-recreate"}'),
('56DB2DBA-3A2C-4D62-A782-5BE05B35EF46','Notes','Notes','VR_Invoice_InvoiceType_InvoiceGridActionSettings'														,'{"Editor":"vr-invoicetype-gridactionsettings-note"}'),
('F974FC02-9C97-4C04-9DD3-2501A3807BFE','SetInvoicePaidAction','Set Invoice Paid','VR_Invoice_InvoiceType_InvoiceGridActionSettings'							,'{"Editor":"vr-invoicetype-gridactionsettings-setinvoicepaid"}'),
('5B4BD540-832E-46E4-8C18-49073775D002','OpenRDLCReportAction','Open RDLC Report','VR_Invoice_InvoiceType_InvoiceGridActionSettings'							,'{"Editor":"vr-invoicetype-gridactionsettings-openrdlcreport"}'),

('94AA2673-04E4-4EFC-9913-CD95C40CD600','BiMonthly','BiMonthly','VR_Invoice_BillingPeriodConfig'																,'{"Editor":"vr-invoice-billingperiod-bimonthly"}'),
('37F03848-3F78-4A00-BA56-E6C7E2F5F3A2','Monthly Customized','Monthly Customized','VR_Invoice_BillingPeriodConfig'												,'{"Editor":"vr-invoice-billingperiod-monthly"}'),
('66B9FD8C-DACB-4F35-9853-F6C0C9DFE4F1','BiWeekly','BiWeekly','VR_Invoice_BillingPeriodConfig'																	,'{"Editor":"vr-invoice-billingperiod-biweekly"}'),
('A08230D0-317E-4B30-A5EA-5ED72E2604D8','Weekly','Weekly','VR_Invoice_BillingPeriodConfig'																		,'{"Editor":"vr-invoice-billingperiod-weekly"}'),
('3D0DD9A7-3422-4311-B16F-08B03F175FAE','Period','Period','VR_Invoice_BillingPeriodConfig'																		,'{"Editor":"vr-invoice-billingperiod-period"}'),
('E0EDEB7A-FE1D-4207-A1E6-0AE2A42ED452','SemiMonthly','Semi Monthly','VR_Invoice_BillingPeriodConfig'															,'{"Editor":"vr-invoice-billingperiod-semimonthly"}'),

('5F9DDA2C-860D-42E6-BDF2-904B1B8FF287','Billing Period Accumulation','Billing Period Accumulation','VR_Invoice_StartDateCalculationMethodConfig'				,'{"Editor":"vr-invoice-startdatecalculationmethod-periodaccumulation"}'),

('E46CBB79-5448-460E-A94A-3C6405C5BB5F','Invoice Item Section','Invoice Item Section','VR_Invoice_InvoiceType_InvoiceUISubSectionSettings'						,'{"Editor":"vr-invoicetype-invoiceuisubsectionsettings-invoiceitem","RuntimeEditor":"vr-invoice-subsection-grid"}'),
('8A958396-18C2-4913-BABB-FF31683C6A17','Item Grouping Section','Item Grouping Section','VR_Invoice_InvoiceType_InvoiceUISubSectionSettings'					,'{"Editor":"vr-invoicetype-invoicesubsectionsettings-itemgrouping","RuntimeEditor":"vr-invoice-groupingsubsection-grid"}'),

('5BAC5D43-DA8F-4996-9730-72132FD83ECB','Send Email','Send Email','VR_Invoice_InvoiceType_AutomaticInvoiceSettingsConfig'										,'{"Editor":"vr-invoicetype-automaticinvoiceaction-sendemail"}'),
('ffecce75-d9ca-4357-94ad-7bd643b633f8','RecreateInvoice','Recreate','VR_Invoice_InvoiceType_AutomaticInvoiceSettingsConfig'										,'{"Editor":"vr-invoicetype-invoicebulkaction-recreateinvoice"}'),
('c5f9cdcc-fc61-4f0b-9b72-b5de53d8520b','Save Invoice File','Save Invoice File','VR_Invoice_InvoiceType_AutomaticInvoiceSettingsConfig'										,'{"Editor":"vr-invoicetype-automaticinvoiceaction-saveinvoicetofile"}'),
('D75CD1BA-F26B-4E17-BC3C-59996A00495A','InitialSequence','Initial Sequence','VR_Invoice_InvoiceSettingPartConfig'												,'{"RuntimeEditor":"vr-invoice-invoicesetting-runtime-initialsequencevaluepart"}'),
('49EC6BB0-0C46-4610-93D0-4ED0BE537265','DuePeriod','Due Period','VR_Invoice_InvoiceSettingPartConfig'															,'{"RuntimeEditor":"vr-invoice-invoicesetting-runtime-dueperiodpart"}'),
('0356AB64-58D4-4645-AF56-CCB19585D7C3','Automatic Invoice','Automatic Invoice','VR_Invoice_InvoiceSettingPartConfig'											,'{"RuntimeEditor":"vr-invoice-invoicesetting-runtime-automaticinvoicesettingpart"}'),
('D8730E6B-EF3E-4043-99FF-D5FA8F4EF812','Billing Period','Billing Period','VR_Invoice_InvoiceSettingPartConfig'													,'{"RuntimeEditor":"vr-invoice-invoicesetting-runtime-billingperiodinvoicesettingpart"}'),
('BB32D601-A1F7-478E-A5D9-F554DE35C85C','Serial Number Pattern','Serial Number Pattern','VR_Invoice_InvoiceSettingPartConfig'									,'{"RuntimeEditor":"vr-invoice-invoicesetting-runtime-serialnumberpatterninvoicesettingpart"}'),
('a3c1239e-2c00-4236-96ec-690a5b294586','Automatic Invoice Actions','Automatic Invoice Actions','VR_Invoice_InvoiceSettingPartConfig'							,'{"RuntimeEditor":"vr-invoice-invoicesetting-runtime-automaticinvoiceactionspart"}'),

('88D62CA6-AABF-4059-A008-279DDFBCEC9C','VR_BEBridge_BESynchronizer_Invoice','Invoice Synchronizer','VR_BEBridge_BESynchronizer','{"Editor":"vr-invoice-synchronizer-editor"}'),
('A06AF611-6996-488D-B6D9-75BFC775947C','Last Invoices','Last Invoices','VR_Invoice_InvoiceType_RDLCDataSourceSettings','{"Editor":"vr-invoicetype-datasourcesettings-lastinvoices"}'),
('72314BA6-93BE-4662-9AE7-AACC0E965F3B','Default','Default','VR_Invoice_InvoiceType_ItemSetNameStorageRule','{"Editor":"vr-invoicetype-itemsetnamestoragerulesettings-default"}'),
('228497ae-5dbe-4a00-88a5-6dc6c5b8535a'	,'Condition Group',	'Condition Group'	,'VR_Invoice_InvoiceType_InvoiceGridFilterConditionConfig'	,'{"Editor":"vr-invoicetype-invoicefiltercondition-conditiongroup"}'),
('5bc6cf2e-5a0f-47a4-a939-6137fb8e1e1a'	,'AccountActivation',	'Account Activation'	,'VR_Invoice_InvoiceType_InvoiceGridFilterConditionConfig'	,'{"Editor":"vr-invoicetype-invoicefiltercondition-accountactivation"}'),
('01153407-1ec9-4d9d-9722-d4be5cd419fe'	,'File Name Pattern',	'File Name Pattern'	,'VR_Invoice_InvoiceSettingPartConfig'	,'{"RuntimeEditor":"vr-invoice-invoicesetting-runtime-filenamepatterninvoicesettingpart"}'),

('203d36dd-f867-48d9-a748-0cac483b5407'	,'Invoice Field',	'Invoice Field'	,'VR_InvoiceType_FileNameParts'	,'{"Editor":"vr-invoicetype-filename-invoicefield"}'),
('b63ad817-6776-4883-a522-4f6d045dc67c'	,'Time',	'Time'	,'VR_InvoiceType_FileNameParts'	,'{"Editor":"vr-invoicetype-filename-time"}'),
('f36a2c18-9033-4f9e-ab7d-0730f630a81a'	,'DownloadFile',	'Download File'	,'VR_Invoice_InvoiceType_InvoiceGridActionSettings'	,'{"Editor":"vr-invoicetype-gridactionsettings-downloadfile"}'),

('CC4DFF84-25C3-4BF7-93BF-48E2F7438476','VR_InvoiceSettings','Specific Accounts','VR_Invoice_Partnergroup'	,'{"Editor":"vr-invoice-partnergroup-specificaccounts","RuntimeEditor":""}'),
('CF988AC3-FF1A-49F7-9293-4E7FD1E8E270','VR_InvoiceSettings','Invoice Setting','VR_Invoice_Partnergroup'	,'{"Editor":"vr-invoice-partnergroup-invoicesetting","RuntimeEditor":""}'),
('fcfb637a-a6a4-4b2f-a817-9ff9c3af3bab','Min Amount','Min Amount','VR_Invoice_InvoiceSettingPartConfig'	,'{"RuntimeEditor":"vr-invoice-invoicesetting-runtime-minamountinvoicesettingpart"}'),
('F2DBA69C-B86B-48DB-A49B-A30A72C50D1E','Approve Invoice','Approve Invoice','VR_Invoice_InvoiceType_InvoiceGridActionSettings'	,'{"Editor":"vr-invoicetype-gridactionsettings-approveinvoice"}'),
('18d88a5e-b148-4d2e-b2ae-f411757f5591'	,'InvoiceApproved',	'Invoice Approved'	,'VR_Invoice_InvoiceType_InvoiceGridFilterConditionConfig'	,'{"Editor":"vr-invoicetype-invoicefiltercondition-approvalinvoice"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[ConfigType],[Settings]))
merge	[common].[extensionconfiguration] as t
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

--[sec].[SystemAction]------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([Name],[RequiredPermissions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('VR_Invoice/InvoiceType/GetInvoiceType',null),
('VR_Invoice/InvoiceType/GetInvoiceTypeRuntime',null),
('VR_Invoice/InvoiceType/GetGeneratorInvoiceTypeRuntime',null),
('VR_Invoice/InvoiceType/GetFilteredInvoiceTypes','VR_SystemConfiguration: View'),
('VR_Invoice/InvoiceType/AddInvoiceType','VR_SystemConfiguration: Add'),
('VR_Invoice/InvoiceType/UpdateInvoiceType','VR_SystemConfiguration: Edit'),
('VR_Invoice/InvoiceType/ConvertToGridColumnAttribute',null),
('VR_Invoice/InvoiceType/GetInvoiceTypesInfo',null),
('VR_Invoice/InvoiceType/GetInvoiceGeneratorActions',null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Name],[RequiredPermissions]))
merge	[sec].[SystemAction] as t
using	cte_data as s
on		1=1 and t.[Name] = s.[Name]
when matched then
	update set
	[Name] = s.[Name],[RequiredPermissions] = s.[RequiredPermissions]
when not matched by target then
	insert([Name],[RequiredPermissions])
	values(s.[Name],s.[RequiredPermissions]);

--[sec].[View]-----------------------------20001 to 21000--------------------------------------------------------
begin 
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('E24A36DA-C72E-49B4-AFC6-885DDA003E1A','Invoice Types','Invoice Types','#/view/VR_Invoice/Views/InvoiceTypeManagement'							,'D018C0CD-F15F-486D-80C3-F9B87C3F47B8','VR_Invoice/InvoiceType/GetFilteredInvoiceTypes',null,null,null												,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',40),
('85868106-9188-4AA7-9191-321BE53F32B0','Invoice Settings','Invoice Settings','#/view/VR_Invoice/Views/InvocieSettingManagement'		,'BAAF681E-AB1C-4A64-9A35-3F3951398881',null,null,null,'{"$type":"Vanrise.Invoice.Entities.InvoiceSettingsViewSettings, Vanrise.Invoice.Entities"}'	,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',35)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank]
when not matched by target then
	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank]);
---------------------------------------------------------------------------------------------------------------
end

--[bp].[BPDefinition]----------------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[FQTN],[Config])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('553EF81C-5442-4E8E-B3AB-C2C108500A4C','Vanrise.Invoice.BP.Arguments.AutomaticInvoiceProcessInput','Automatic Invoice Process','Vanrise.Invoice.BP.AutomaticInvoiceProcess, Vanrise.Invoice.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"ManualExecEditor":"vr-invoice-automaticinvoiceprocess-manual","ScheduledExecEditor":"vr-invoice-automaticinvoiceprocess-scheduled","IsPersistable":false,"HasChildProcesses":true,"HasBusinessRules":false,"NotVisibleInManagementScreen":false,"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"1F72D8FD-7988-4BB4-9AF4-B0CE96930A4E","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View Process Logs"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"1F72D8FD-7988-4BB4-9AF4-B0CE96930A4E","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Start Process"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"1F72D8FD-7988-4BB4-9AF4-B0CE96930A4E","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Start Process"]}}]}}}}'),
('4c9b0a57-ad37-4b14-94e9-255777a0def8','Vanrise.Invoice.BP.Arguments.InvoiceBulkActionProcessInput','Invoice Bulk Action Process','Vanrise.Invoice.BP.InvoiceBulkActionProcess,Vanrise.Invoice.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":false,"ExtendedSettings":{"$type":"Vanrise.Invoice.Business.InvoiceBulkActionsBPDefinitionSettings, Vanrise.Invoice.Business"},"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"1F72D8FD-7988-4BB4-9AF4-B0CE96930A4E","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View Process Logs"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"1F72D8FD-7988-4BB4-9AF4-B0CE96930A4E","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Start Process"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"1F72D8FD-7988-4BB4-9AF4-B0CE96930A4E","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Start Process"]}}]}}}}'),
('72D6821B-370A-4582-87D8-A35C363423DA','Vanrise.Invoice.BP.Arguments.InvoiceGenerationProcessInput','Invoice Generation Process','Vanrise.Invoice.BP.InvoiceGenerationProcess,Vanrise.Invoice.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"IsPersistable":false,"HasChildProcesses":true,"HasBusinessRules":false,"NotVisibleInManagementScreen":false,"ExtendedSettings":{"$type":"Vanrise.Invoice.Business.InvoiceGenerationBPDefinitionSettings, Vanrise.Invoice.Business"},"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"1F72D8FD-7988-4BB4-9AF4-B0CE96930A4E","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View Process Logs"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"1F72D8FD-7988-4BB4-9AF4-B0CE96930A4E","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Start Process"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"1F72D8FD-7988-4BB4-9AF4-B0CE96930A4E","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Start Process"]}}]}}}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[FQTN],[Config]))
merge	[bp].[BPDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[FQTN] = s.[FQTN],[Config] = s.[Config]
when not matched by target then
	insert([ID],[Name],[Title],[FQTN],[Config])
	values(s.[ID],s.[Name],s.[Title],s.[FQTN],s.[Config]);
----------------------------------------------------------------------------------------------------
end

--[sec].[BusinessEntity]------------------301 to 600----------------------------------------------------------
begin
set nocount on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('1F72D8FD-7988-4BB4-9AF4-B0CE96930A4E','VR_Invoice_AutomaticInvoiceProcess','Automatic Invoice','B6B8F582-4759-43FB-9220-AA7662C366EA',0,'["Start Process", "View Process Logs"]'),
('DC03506D-42A6-46E1-977B-EF781D8941E6','VR_Invoice_Settings','Invoice Settings'						,'520558FA-CF2F-440B-9B58-09C23B6A2E9B',0,'["View","Add","Edit","Delete","ManagePartner"]')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions]))
merge	[sec].[BusinessEntity] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[ModuleId] = s.[ModuleId],[BreakInheritance] = s.[BreakInheritance],[PermissionOptions] = s.[PermissionOptions]
when not matched by target then
	insert([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
	values(s.[Id],s.[Name],s.[Title],s.[ModuleId],s.[BreakInheritance],s.[PermissionOptions]);
--------------------------------------------------------------------------------------------------------------
end

--[logging].[LoggableEntity]------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[UniqueName],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('125CFCA7-8538-4751-9B9E-3146D7CB3DF8','VR_Invoice_InvoiceType','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Invoice_InvoiceType_ViewHistoryItem"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[UniqueName],[Settings]))
merge	[logging].[LoggableEntity] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[UniqueName] = s.[UniqueName],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[UniqueName],[Settings])
	values(s.[ID],s.[UniqueName],s.[Settings]);