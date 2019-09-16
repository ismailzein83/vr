﻿



















--Make sure to use same .json file using DEVTOOLS under http://192.168.110.185:8037




























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
*/--common.[extensionconfiguration]-------------------------------------------------------------------beginset nocount on;;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('096B0021-49A1-44F6-B601-406372DC5038','Constant','Constant'					,'VR_ExcelConversion_ConcatenatedPart'	,'{"Editor":"vr-excelconversion-concatenatedpart-constant","Type":1}'),('AB48FF16-7B04-42FF-8525-C68590A88799','Row Cell','Row Cell'					,'VR_ExcelConversion_FieldMapping'		,'{"Editor":"vr-excelconversion-fieldmapping-cellfieldmapping"}'),('17ABFA66-D659-4E5C-B1C9-F7B408E8DE00','Concatenate Text','Concatenate Text'	,'VR_ExcelConversion_FieldMapping'		,'{"Editor":"vr-excelconversion-fieldmapping-concatenatefieldmapping"}'),('18280F63-B0F1-42C8-A3CB-D487F17A8236','Row Cell','Row Cell'					,'VR_ExcelConversion_ConcatenatedPart'	,'{"Editor":"vr-excelconversion-concatenatedpart-cellfield","Type":0}'),('9F9D16F5-9E8B-4D01-8109-6DAABE59623F','Conditional Cell','Conditional Cell'	,'VR_ExcelConversion_FieldMapping'		,'{"Editor":"vr-excelconversion-fieldmapping-conditionalcell","hideCell":true}'),('CB6F97C0-8112-4954-9C80-B02AB1A1C69C','Fixed Cell','Fixed Cell'				,'VR_ExcelConversion_FieldMapping'		,'{"Editor":"vr-excelconversion-fieldmapping-fixedcell"}'),('F0EEEAA9-4FA6-4917-B860-CA285B4761BE','Constant Field','Constant Field'		,'VR_ExcelConversion_FieldMapping'		,'{"Editor":"vr-excelconversion-fieldmapping-constantfieldmapping"}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Title],[ConfigType],[Settings]))merge	[common].[extensionconfiguration] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[ConfigType] = s.[ConfigType],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Title],[ConfigType],[Settings])	values(s.[ID],s.[Name],s.[Title],s.[ConfigType],s.[Settings]);----------------------------------------------------------------------------------------------------end