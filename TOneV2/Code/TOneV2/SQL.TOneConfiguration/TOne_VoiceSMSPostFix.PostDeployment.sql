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

------Set Voice Module under Traffic Analysis
UPDATE [sec].[View]
SET [Module] = 'D977F2C5-5A63-4B03-8F8A-C9CB1C5B5F15'
WHERE ID IN ('5233ADB2-1024-41ED-95D0-BDE8B9BD2A95',
			 'AC75CBEB-A5F5-4DD4-BE95-D0180165EF2A',
			 '5F3CE2E0-D153-423F-850B-2FB4578588B2',
			 'AF804DB2-9D2B-4F5C-8DE4-E80AA313E90B',
			 '7A4853F8-92E7-4FFF-8B77-875B94B19872',
			 'CFA90FA2-8013-484D-BF23-FE68C4237DDC',
			 '5D44BA46-5776-4C81-A6E6-5FC07BFDC729')



------Set SMS Module under Traffic Analysis
UPDATE [sec].[View]
SET [Module] = '93651496-1407-4B1B-B271-606D628F886C'
WHERE ID IN ('7BAF4C26-E684-4104-8D59-0D76F362A5F0',
			 '7C2BD918-EFE6-4D60-AA2A-52146514871F')



------Set Voice Module under Billing
UPDATE [sec].[View]
SET [Module] = '0AAA7D9E-EE0B-4AA6-8AA1-F03E2379D811'
WHERE ID IN ('D4C235C2-78D1-4398-AF80-1E254DB1FBB9',
			 'dea6a0b0-fdf7-45a4-87bf-6997f71377c9',
			 '99B26322-A96C-4F7A-961F-669F19BD546F')



--------Set SMS Module under Billing
UPDATE [sec].[View]
SET [Module] = '9ADD9AC2-D329-4834-BDEA-BA87C26C2A1D'
WHERE ID IN ('074C9841-2F44-4E05-8D7E-C2C58A8837C2')



------Set Voice Module under Rules (in Module Table)
UPDATE [sec].[Module]
SET [ParentId] = 'D2899D41-A6DB-4E5B-9C28-9FA69E74AAE2'
WHERE ID IN ('5EA95F6C-D3C8-426D-A8F0-8F4B0ECE478C')



------Set Voice Module under Rules (in View Table)
UPDATE [sec].[View]
SET [Module] = 'D2899D41-A6DB-4E5B-9C28-9FA69E74AAE2'
WHERE ID IN ('0D413348-8F8B-4282-8738-6F5178FE99E0',
			 '8C00BBB3-D71F-4A7A-A51B-007FDECD8211',
			 'B2FC9B65-A056-4E8A-91EA-B7014C5C59A7')




------Set SMS Module under Rules
UPDATE [sec].[View]
SET [Module] = '75C29F78-49B1-4968-9C4A-6233461C6897'
WHERE ID IN ('8BB4CBB4-49E9-4063-98C1-C3C1BE1DD12D')





------Set Voice Module under Reports & Dashboards
UPDATE [sec].[View]
SET [Module] = 'C33B407D-476E-4779-83F8-5F88CA1A4DF3'
WHERE ID IN ('6EDB23CF-5ECF-4020-80CE-21241D661720',
			 '0A028781-C435-4E45-B3DD-0F4FFAD80223')


			 
------Set SMS Module under Reports & Dashboards
UPDATE [sec].[View]
SET [Module] = '3B6E324F-4F1F-417A-AB60-963D4856B4CC'
WHERE ID IN ('73D4060B-8C67-41A7-88E4-9B53B794EE90',
			 '2D6ABFCC-B771-40E1-8EBF-012BBDE453B8','36C0D156-02C4-4D47-9103-02EAC621AD86',
													'0E01C4D3-2EBC-4609-A38D-1C7232AD5238',
													'3BF5215A-ACFE-4015-A3BE-BF633FE0DFB6')


------Set Voice Module under Sale Area (in Module Table)
UPDATE [sec].[Module]
SET [ParentId] = 'BBDEFF63-D16B-4936-B3B2-0232D8398B27'
WHERE ID IN ('9C3816A9-EABF-423B-9364-EEBADAD22EB5',
			 '1C8B893E-4DDA-4044-A1AE-D4E8536C3FBC')

			 
------Set Voice Module under Sale Area (in View Table)
UPDATE [sec].[View]
SET [Module] = 'BBDEFF63-D16B-4936-B3B2-0232D8398B27'
WHERE ID IN ('DBD01498-8194-4BA0-85FD-B9B8CD6C7529',
			 '5DC5C8ED-6D09-44DA-A77F-85C4DC4354D0',
			 '3A7700BB-4822-468D-BEBA-7404DE0ABD1F')



			 
------Set SMS Module under Sale Area
UPDATE [sec].[View]
SET [Module] = '194D667E-CFF2-4195-BAB1-29A57849D487'
WHERE ID IN ('8D81852F-1882-4583-9195-8CCEA0F3ECBC',
			 '1EDB75AF-0E4A-4516-BD71-AAC986DF4ECE',
			 '898A6F3D-BB14-4014-B13E-689C75AB6218',
			 '48F0F23E-D27C-4D08-91C7-5114CEC5C1B6')




------Set Voice Module under Purchase Area (in Module Table)
UPDATE [sec].[Module]
SET [ParentId] = '0EABABC6-354C-4F13-8028-8C0FF99620CA'
WHERE ID IN ('8F443685-3AC6-4C48-9298-017D74221EAF')

			 
------Set Voice Module under Purchase Area (in View Table)
UPDATE [sec].[View]
SET [Module] = '0EABABC6-354C-4F13-8028-8C0FF99620CA'
WHERE ID IN ('4073D6EB-E9DD-47B2-9FF4-C3C0B5672BC5',
			 'AB3952E4-AC37-471E-80D6-7DB6ED3DCC07')




			 
------Set SMS Module under Purchase Area
UPDATE [sec].[View]
SET [Module] = '63823963-5F3E-4463-A077-524D2229E6E0'
WHERE ID IN ('54C62DFA-0261-4E15-AB8A-02E66BBC470E',
			 '1CAC2CB0-F015-4E6A-A6F5-BA12055E2B76')


			 
			 
------Set Voice Module under Routing
UPDATE [sec].[View]
SET [Module] = '1C182996-2BE3-4461-815E-DB51C5F1E08B'
WHERE ID IN ('7EF6F3F4-CBC7-4BD2-85A7-011188A624D8',
			 'A644BAD6-408A-449A-B689-5027BA469609',
			 'BD504ABD-33C0-4890-8ADA-5685F6055013')

------Set SMS Module under Routing
UPDATE [sec].[View]
SET [Module] = '605D5A02-1170-4EB0-ABA7-BC07AD8EB858'
WHERE ID IN ('8D25B9BF-3972-4E9C-8F0A-3CB57C65E0E1')