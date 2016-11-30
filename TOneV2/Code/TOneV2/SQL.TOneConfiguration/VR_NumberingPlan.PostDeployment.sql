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
--[sec].[Module]----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([ID],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('E7855563-9173-47F0-A8E7-4C47CD2A1F42','Numbering Plan','Numbering Plan',null,'/Client/images/menu-icons/Business Entities.png',11,1),('30CB8D46-832D-438B-93CF-2267D5EE642F','Entities',null,'E7855563-9173-47F0-A8E7-4C47CD2A1F42',null,10,0)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic]))merge	[sec].[Module] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Url] = s.[Url],[ParentId] = s.[ParentId],[Icon] = s.[Icon],[Rank] = s.[Rank],[AllowDynamic] = s.[AllowDynamic]when not matched by target then	insert([ID],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])	values(s.[ID],s.[Name],s.[Url],s.[ParentId],s.[Icon],s.[Rank],s.[AllowDynamic]);

--[sec].[View]------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('1F11C51B-1EDB-4559-8EE2-25A44330CCA4','Selling Number Plans','Selling Number Plans','#/view/VR_NumberingPlan/Views/SellingNumberPlanManagement'	,'30CB8D46-832D-438B-93CF-2267D5EE642F','VR_NumberingPlan/SellingNumberPlan/GetFilteredSellingNumberPlans',null,null,null,'8DAA013C-3C9B-4E72-8A72-BD68608350B2',0,5),('11927F62-D139-452B-B23B-14EC58F87012','Code Groups','Code Groups','#/view/VR_NumberingPlan/Views/CodeGroup/CodeGroupManagement'					,'30CB8D46-832D-438B-93CF-2267D5EE642F','VR_NumberingPlan/CodeGroup/GetFilteredCodeGroups',null,null,null,'8DAA013C-3C9B-4E72-8A72-BD68608350B2',0,10),('29FE98DA-91D7-4442-8A35-3A721CD5656A','Sale Zones','Sale Zones','#/view/VR_NumberingPlan/Views/SaleZone/SaleZoneManagement'						,'30CB8D46-832D-438B-93CF-2267D5EE642F','VR_NumberingPlan/SaleZone/GetFilteredSaleZones',null,null,null,'8DAA013C-3C9B-4E72-8A72-BD68608350B2',0,15),('90331F0A-FB51-4DCF-8261-98D07B579938','Sale Codes','Sale Codes','#/view/VR_NumberingPlan/Views/SaleCode/SaleCodeManagement'						,'30CB8D46-832D-438B-93CF-2267D5EE642F','VR_NumberingPlan/SaleCode/GetFilteredSaleCodes',null,null,null,'8DAA013C-3C9B-4E72-8A72-BD68608350B2',0,20),('05CD2EE2-7374-4296-A532-CC273EE1C540','Numbering Plan','Numbering Plan Management','#/view/VR_NumberingPlan/Views/NumberingPlanManagement'		,'E7855563-9173-47F0-A8E7-4C47CD2A1F42',null,null,null,null,'A3AD3B1D-B56A-49D0-BC25-0F1EF7DFB03D',null,20)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank]))merge	[sec].[View] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[OldType] = s.[OldType],[Rank] = s.[Rank]when not matched by target then	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank])	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[OldType],s.[Rank]);

--[sec].[SystemAction]----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([Name],[RequiredPermissions])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('VR_NumberingPlan/SellingNumberPlan/GetSellingNumberPlans',null),('VR_NumberingPlan/SellingNumberPlan/GetSellingNumberPlan',null),('VR_NumberingPlan/SellingNumberPlan/GetMasterSellingNumberPlan',null),('VR_NumberingPlan/SellingNumberPlan/GetFilteredSellingNumberPlans','VR_NumberingPlan_SellingNumberPlan: View'),('VR_NumberingPlan/SellingNumberPlan/AddSellingNumberPlan','VR_NumberingPlan_SellingNumberPlan: Add'),('VR_NumberingPlan/SellingNumberPlan/UpdateSellingNumberPlan','VR_NumberingPlan_SellingNumberPlan: Edit'),('VR_NumberingPlan/CodeGroup/GetFilteredCodeGroups','VR_NumberingPlan_CodeGroup: View'),('VR_NumberingPlan/CodeGroup/GetAllCodeGroups',null),('VR_NumberingPlan/CodeGroup/GetCodeGroup',null),('VR_NumberingPlan/CodeGroup/AddCodeGroup','VR_NumberingPlan_CodeGroup: Add'),('VR_NumberingPlan/CodeGroup/UpdateCodeGroup','VR_NumberingPlan_CodeGroup: Edit'),('VR_NumberingPlan/CodeGroup/UploadCodeGroupList','VR_NumberingPlan_CodeGroup : UploadList'),('VR_NumberingPlan/CodeGroup/DownloadCodeGroupListTemplate',null),('VR_NumberingPlan/CodeGroup/DownloadCodeGroupLog',null),('VR_NumberingPlan/SaleZone/GetFilteredSaleZones','VR_NumberingPlan_SaleZone: View'),('VR_NumberingPlan/SaleZone/GetSaleZone',null),('VR_NumberingPlan/SaleZone/GetSellingNumberPlanIdBySaleZoneId',null),('VR_NumberingPlan/SaleZone/GetSaleZoneGroupTemplates',null),('VR_NumberingPlan/SaleCode/GetFilteredSaleCodes','VR_NumberingPlan_SaleCode: View')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([Name],[RequiredPermissions]))merge	[sec].[SystemAction] as tusing	cte_data as son		1=1 and t.[Name] = s.[Name]when matched then	update set	[Name] = s.[Name],[RequiredPermissions] = s.[RequiredPermissions]when not matched by target then	insert([Name],[RequiredPermissions])	values(s.[Name],s.[RequiredPermissions]);--[sec].[BusinessEntity]--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('5B732E53-C47C-4885-B516-35E60BAF214C',null,'VR_NumberingPlan_CodeGroup','CodeGroup'					,'8FAF6AA2-6C00-4C48-8EBF-68B87D2DC493',null,0,'["View","Add","Edit","UploadList"]'),('DB827E90-C23E-4AB6-A725-4EB83C21F5FE',null,'VR_NumberingPlan_SellingNumberPlan','SellingNumberPlan'	,'8FAF6AA2-6C00-4C48-8EBF-68B87D2DC493',null,0,'["View","Add","Edit"]'),('B84ACE6E-3A57-4582-81D7-A7F47FCC23C8',null,'VR_NumberingPlan_SaleZone','SaleZone'						,'8FAF6AA2-6C00-4C48-8EBF-68B87D2DC493',null,0,'["View"]'),('A54A6B25-81A1-4A7D-A297-CCBEC44C336D',null,'VR_NumberingPlan_SaleCode','SaleCode'						,'8FAF6AA2-6C00-4C48-8EBF-68B87D2DC493',null,0,'["View"]')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions]))merge	[sec].[BusinessEntity] as tusing	cte_data as son		1=1 and t.[Id] = s.[Id]when matched then	update set	[OldId] = s.[OldId],[Name] = s.[Name],[Title] = s.[Title],[ModuleId] = s.[ModuleId],[OleModuleId] = s.[OleModuleId],[BreakInheritance] = s.[BreakInheritance],[PermissionOptions] = s.[PermissionOptions]when not matched by target then	insert([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions])	values(s.[Id],s.[OldId],s.[Name],s.[Title],s.[ModuleId],s.[OleModuleId],s.[BreakInheritance],s.[PermissionOptions]);