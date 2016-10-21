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
--common.ExtensionConfiguration---------------------------------------------------------------------beginset nocount on;;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('5A8308B0-BA99-4002-939B-6D76A1DF1FA7','VR_Invoice_GenericInvoice','Invoice','VR_Security_ViewTypeConfig','{"Editor":"/Client/Modules/VR_Invoice/Views/InvoiceViewEditor.html","EnableAdd":true}'),

('65D64951-AD27-475C-81D2-FAC7816E6E4B','Invoice','Invoice','VR_Invoice_InvoiceType_RDLCDataSourceSettings','{"Editor":"vr-invoicetype-datasourcesettings-invoice"}'),
('7A1A9991-EC84-4E92-B1D1-1C5140DF8FF4','Field','Field','VR_Invoice_InvoiceType_DLCParameterSettings','{"Editor":"vr-invoicetype-openrdlcreport-parametersettings-field"}'),
('DE6F2641-A4A8-4F56-AEB4-2A0A25000408','Bank Details','Bank Details','VR_Invoice_InvoiceType_RDLCDataSourceSettings','{"Editor":"vr-invoicetype-datasourcesettings-bankdetails"}'),
('E46CBB79-5448-460E-A94A-3C6405C5BB5F','Invoice Item Section','Invoice Item Section','VR_Invoice_InvoiceType_InvoiceUISubSectionSettings','{"Editor":"vr-invoicetype-invoiceuisubsectionsettings-invoiceitem","RuntimeEditor":"vr-invoice-subsection-grid"}'),
('786EDDD6-1EC7-4C44-889C-E7246B51AED0','CustomField','Custom Field','VR_Invoice_InvoiceType_DLCParameterSettings','{"Editor":"vr-invoicetype-openrdlcreport-parametersettings-customfield"}'),
('C61DE7AE-34D3-457B-9C00-78F002AF508B','Generic','Generic','VR_Invoice_ItemsFilter','{"Editor":"vr-invoicetype-datasource-itemsfilter-generic"}'),
('6721BF29-D257-47D9-8D56-4EE10538BFDC','Items','Items','VR_Invoice_InvoiceType_RDLCDataSourceSettings','{"Editor":"vr-invoicetype-datasourcesettings-rdlcitems"}'),
('5B4BD540-832E-46E4-8C18-49073775D002','OpenRDLCReportAction','Open RDLC Report','VR_Invoice_InvoiceType_InvoiceGridActionSettings','{"Editor":"vr-invoicetype-gridactionsettings-openrdlcreport"}'),
('71F19A9B-0FAD-4370-B390-3F28137DE1EE','Carrier Partner','Carrier Partner','VR_Invoice_InvoiceType_InvoicePartnerSettings','{"Editor":"vr-invoicetype-partnersettings-carrier"}'),
('B9CB6032-438E-42FD-9520-E3451FAD6A71','Date','Date','VR_InvoiceType_SerialNumberParts','{"Editor":"vr-invoicetype-serialnumber-invoicedate"}'),
('9E0B783D-FB05-4173-A137-CF01F2CDC83A','Constant','Constant','VR_InvoiceType_ItemSetNameParts','{"Editor":"vr-invoicetype-itemsetnameparts-constant"}'),
('5C47A8C8-240C-41DA-9A3C-C671BC03D478','Invoice Field','Invoice Field','VR_InvoiceType_SerialNumberParts','{"Editor":"vr-invoicetype-serialnumber-invoicefield"}'),
('17832921-2BD5-4D2B-B952-C45BDBA25B33','Current ItemSetName','Current ItemSetName','VR_InvoiceType_ItemSetNameParts','{"Editor":"vr-invoicetype-itemsetnameparts-currentitemsetname"}'),
('034D0103-916C-48DC-8A7F-986DEB09FE3F','Counter','Counter','VR_InvoiceType_SerialNumberParts','{"Editor":"vr-invoicetype-serialnumber-invoicecounter"}'),
('EDA771DE-D1CC-4137-B2AD-1647D4C50B81','Invoice Item Field','Invoice Item Field','VR_InvoiceType_ItemSetNameParts','{"Editor":"vr-invoicetype-itemsetnameparts-invoiceitemfield"}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Title],[ConfigType],[Settings]))merge	[common].[extensionconfiguration] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[ConfigType] = s.[ConfigType],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Title],[ConfigType],[Settings])	values(s.[ID],s.[Name],s.[Title],s.[ConfigType],s.[Settings]);----------------------------------------------------------------------------------------------------end

--[sec].[View]-----------------------------20001 to 21000--------------------------------------------------------
begin 
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank])as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('E24A36DA-C72E-49B4-AFC6-885DDA003E1A','Invoice Types','Invoice Types','#/view/VR_Invoice/Views/InvoiceTypeManagement','D018C0CD-F15F-486D-80C3-F9B87C3F47B8',null,null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,40)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank]))merge	[sec].[View] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[OldType] = s.[OldType],[Rank] = s.[Rank]when not matched by target then	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank])	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[OldType],s.[Rank]);
---------------------------------------------------------------------------------------------------------------
end