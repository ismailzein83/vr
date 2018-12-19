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
--[common].[ExtensionConfiguration]-----------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('8A5B5D5F-68FE-4ED5-8A45-09F62EF3B817','CoordinatesParser','Coordinates Parser','VR_DataParser_HexTLVTagValueParser','{"Editor":"vr-dataparser-coordinates-hex-parser-settings"}'),
('4D2CA16E-01FA-4FFD-8DA7-0A0892AB6A65','NumberFromTextParser','Number From Text Parser','VR_DataParser_StringParser','{"Editor":"vr-dataparser-number-from-text-parser-settings"}'),
('F1FC606C-F862-41CB-A832-1C670F555EF9','ZonePackageParser','Zone Package Parser','VR_DataParser_HexTLVTagValueParser','{"Editor":"vr-dataparser-zone-package-parser-settings"}'),
('55FD305B-707F-4B98-A5DA-2CAEC314FC85','TBCDNumberParser','TBCD Number Parser','VR_DataParser_HexTLVTagValueParser','{"Editor":"vr-dataparser-tbcd-number-hex-parser-settings"}'),
('3BD9B2B7-9993-4664-9E08-3AF2C2819489','BoolParser','Bool Parser','VR_DataParser_HexTLVTagValueParser','{"Editor":"vr-dataparser-bool-hex-parser-settings"}'),
('F291C16D-74E2-4C25-B2A7-43301CD5C04F','StringParser','String Parser','VR_DataParser_HexTLVTagValueParser','{"Editor":"vr-dataparser-string-hex-parser-settings"}'),
('78168E82-0528-4F3A-9E7C-47AEF279CD49','BCDNumberParser','BCD Number Parser','VR_DataParser_HexTLVTagValueParser','{"Editor":"vr-dataparser-bcd-number-parser-settings"}'),
('BAE38D63-10CD-487C-9D74-4C2125F18B30','TimeFieldParser','Time Field Parser','VR_DataParser_HexTLVTagValueParser','{"Editor":"vr-dataparser-time-field-hex-parser-settings"}'),
('B60D9CC3-3C8C-494F-A272-4CADBF47BAFE','DateTimePackageParser','Date Time Package Parser','VR_DataParser_HexTLVTagValueParser','{"Editor":"vr-dataparser-date-time-package-parser-settings"}'),
('593CDE30-2D89-4303-972B-51F86378E30F','IPV4Parser','IPV4 Parser','VR_DataParser_HexTLVTagValueParser','{"Editor":"vr-dataparser-ipv4-hex-parser-settings"}'),
('5B97401A-8211-4E85-95AF-5C84D0CAA4E3','TrafficQualityDataPackageParser','Traffic Quality Data Package Parser','VR_DataParser_HexTLVTagValueParser','{"Editor":"vr-dataparser-traffic-quality-data-parser-settings"}'),
('D15F9394-E33A-4E13-BEC2-77670B650B81','PartyNumberPackageParser','Party Number Package Parser','VR_DataParser_HexTLVTagValueParser','{"Editor":"vr-dataparser-party-number-parser-settings"}'),
('3CA92CC0-EAA7-4AA4-A5FC-77EBE5D940B7','TimeFromTextParser','Time From Text Parser','VR_DataParser_StringParser','{"Editor":"vr-dataparser-time-from-text-hex-parser-settings"}'),
('0A6DABA3-CFEF-42E1-B81A-9F88DB7AFFA1','CallLocationInformationParser','Call Location Information Parser','VR_DataParser_HexTLVTagValueParser','{"Editor":"vr-dataparser-call-location-hex-parser-settings"}'),
('345EEBD2-630C-4490-98C8-BE7E78600B69','ConnectionIdentificationPackageParser','Connection Identification Package Parser','VR_DataParser_HexTLVTagValueParser','{"Editor":"vr-dataparser-connection-parser-settings"}'),
('73A133A2-D4A7-4BFD-A87C-CA78B435ED80','TimeParser','Time Parser','VR_DataParser_HexTLVTagValueParser','{"Editor":"vr-dataparser-time-hex-parser-settings"}'),
('F95D8834-4A38-4197-A6C7-D3D4BCD1B0FD','DateTimeParser','Date Time Parser','VR_DataParser_HexTLVTagValueParser','{"Editor":"vr-dataparser-date-time-hex-parser-settings"}'),
('11FCE310-6BFF-43BD-ACD8-F229C8F4ED8A','NumberFieldParser','Number Field Parser','VR_DataParser_HexTLVTagValueParser','{"Editor":"vr-dataparser-number-field-hex-parser-settings"}'),
('2ADCD3A8-18E4-4CD0-B4AC-F644E4A72408','DateFromTextParser','Date From Text Parser','VR_DataParser_StringParser','{"Editor":"vr-dataparser-data-from-text-parser-settings"}'),
('21B7367D-B5B8-41B2-95A5-2B8AF9071BEE','HexaParser','Hexa Parser','VR_DataParser_HexTLVTagValueParser','{"Editor":"vr-dataparser-hexa-hex-parser-settings"}'),
('213C598D-8800-41B4-9BB9-5FFEF706433A','TrunkIdentificationPackageParser','Trunk Identification Package Parser','VR_DataParser_HexTLVTagValueParser','{"Editor":"vr-dataparser-trunk-package-parser-settings"}')
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