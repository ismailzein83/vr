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
--[sec].[Module]----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([ID],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('E7855563-9173-47F0-A8E7-4C47CD2A1F42','Numbering Plan','Numbering Plan',null,'/Client/Images/menu-icons/Business Entities.png',11,1),('30CB8D46-832D-438B-93CF-2267D5EE642F','Entities',null,'E7855563-9173-47F0-A8E7-4C47CD2A1F42',null,10,0)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic]))merge	[sec].[Module] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Url] = s.[Url],[ParentId] = s.[ParentId],[Icon] = s.[Icon],[Rank] = s.[Rank],[AllowDynamic] = s.[AllowDynamic]when not matched by target then	insert([ID],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])	values(s.[ID],s.[Name],s.[Url],s.[ParentId],s.[Icon],s.[Rank],s.[AllowDynamic]);

--[sec].[View]------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('1F11C51B-1EDB-4559-8EE2-25A44330CCA4','Selling Number Plans','Selling Number Plans','#/view/VR_NumberingPlan/Views/SellingNumberPlanManagement'	,'30CB8D46-832D-438B-93CF-2267D5EE642F','VR_NumberingPlan/SellingNumberPlan/GetFilteredSellingNumberPlans',null,null,null,'8DAA013C-3C9B-4E72-8A72-BD68608350B2',0,5),('11927F62-D139-452B-B23B-14EC58F87012','Code Groups','Code Groups','#/view/VR_NumberingPlan/Views/CodeGroup/CodeGroupManagement'					,'30CB8D46-832D-438B-93CF-2267D5EE642F','VR_NumberingPlan/CodeGroup/GetFilteredCodeGroups',null,null,null,'8DAA013C-3C9B-4E72-8A72-BD68608350B2',0,10),('29FE98DA-91D7-4442-8A35-3A721CD5656A','Sale Zones','Sale Zones','#/view/VR_NumberingPlan/Views/SaleZone/SaleZoneManagement'						,'30CB8D46-832D-438B-93CF-2267D5EE642F','VR_NumberingPlan/SaleZone/GetFilteredSaleZones',null,null,null,'8DAA013C-3C9B-4E72-8A72-BD68608350B2',0,15),('90331F0A-FB51-4DCF-8261-98D07B579938','Sale Codes','Sale Codes','#/view/VR_NumberingPlan/Views/SaleCode/SaleCodeManagement'						,'30CB8D46-832D-438B-93CF-2267D5EE642F','VR_NumberingPlan/SaleCode/GetFilteredSaleCodes',null,null,null,'8DAA013C-3C9B-4E72-8A72-BD68608350B2',0,20),('05CD2EE2-7374-4296-A532-CC273EE1C540','Numbering Plan','Numbering Plan Management','#/view/VR_NumberingPlan/Views/NumberingPlanManagement'		,'E7855563-9173-47F0-A8E7-4C47CD2A1F42','VR_NumberingPlan/CodePreparation/CheckCodePreparationState',null,null,null,'8DAA013C-3C9B-4E72-8A72-BD68608350B2',null,20)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank]))merge	[sec].[View] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[OldType] = s.[OldType],[Rank] = s.[Rank]when not matched by target then	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank])	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[OldType],s.[Rank]);

--[common].[Setting]------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([ID],[OldId],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('A4BBE063-AE8E-4AE3-AD42-5959E198E51B',null,'VR Numbering Plan','VR_NumberingPlan','Business Entities','{"Editor":"vr-np-settings-editor"}','{"$type":"Vanrise.NumberingPlan.Entities.CPSettingsData, Vanrise.NumberingPlan.Entities","EffectiveDateOffset":7}',0)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[OldId],[Name],[Type],[Category],[Settings],[Data],[IsTechnical]))merge	[common].[Setting] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[OldId] = s.[OldId],[Name] = s.[Name],[Type] = s.[Type],[Category] = s.[Category],[Settings] = s.[Settings],[Data] = s.[Data],[IsTechnical] = s.[IsTechnical]when not matched by target then	insert([ID],[OldId],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])	values(s.[ID],s.[OldId],s.[Name],s.[Type],s.[Category],s.[Settings],s.[Data],s.[IsTechnical]);
--[sec].[SystemAction]----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([Name],[RequiredPermissions])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('VR_NumberingPlan/SellingNumberPlan/GetSellingNumberPlans',null),('VR_NumberingPlan/SellingNumberPlan/GetSellingNumberPlan',null),('VR_NumberingPlan/SellingNumberPlan/GetMasterSellingNumberPlan',null),('VR_NumberingPlan/SellingNumberPlan/GetFilteredSellingNumberPlans','VR_NumberingPlan_SellingNumberPlan: View'),('VR_NumberingPlan/SellingNumberPlan/AddSellingNumberPlan','VR_NumberingPlan_SellingNumberPlan: Add'),('VR_NumberingPlan/SellingNumberPlan/UpdateSellingNumberPlan','VR_NumberingPlan_SellingNumberPlan: Edit'),('VR_NumberingPlan/CodeGroup/GetFilteredCodeGroups','VR_NumberingPlan_CodeGroup: View'),('VR_NumberingPlan/CodeGroup/GetAllCodeGroups',null),('VR_NumberingPlan/CodeGroup/GetCodeGroup',null),('VR_NumberingPlan/CodeGroup/AddCodeGroup','VR_NumberingPlan_CodeGroup: Add'),('VR_NumberingPlan/CodeGroup/UpdateCodeGroup','VR_NumberingPlan_CodeGroup: Edit'),('VR_NumberingPlan/CodeGroup/UploadCodeGroupList','VR_NumberingPlan_CodeGroup : UploadList'),('VR_NumberingPlan/CodeGroup/DownloadCodeGroupListTemplate',null),('VR_NumberingPlan/CodeGroup/DownloadCodeGroupLog',null),('VR_NumberingPlan/SaleZone/GetFilteredSaleZones','VR_NumberingPlan_SaleZone: View'),('VR_NumberingPlan/SaleZone/GetSaleZone',null),('VR_NumberingPlan/SaleZone/GetSellingNumberPlanIdBySaleZoneId',null),('VR_NumberingPlan/SaleZone/GetSaleZoneGroupTemplates',null),('VR_NumberingPlan/SaleCode/GetFilteredSaleCodes','VR_NumberingPlan_SaleCode: View'),('VR_NumberingPlan/CodePreparation/DownloadImportCodePreparationTemplate',null),('VR_NumberingPlan/CodePreparation/GetChanges',null),('VR_NumberingPlan/CodePreparation/SaveNewZone',null),('VR_NumberingPlan/CodePreparation/SaveNewCode',null),('VR_NumberingPlan/CodePreparation/MoveCodes',null),('VR_NumberingPlan/CodePreparation/CloseCodes',null),('VR_NumberingPlan/CodePreparation/CloseZone',null),('VR_NumberingPlan/CodePreparation/RenameZone',null),('VR_NumberingPlan/CodePreparation/GetZoneItems',null),('VR_NumberingPlan/CodePreparation/CheckCodePreparationState','VR_NumberingPlan_NumberingPlan: View'),('VR_NumberingPlan/CodePreparation/CancelCodePreparationState',null),('VR_NumberingPlan/CodePreparation/GetCodeItems',null),('VR_NumberingPlan/CodePreparation/GetCPSettings',null)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([Name],[RequiredPermissions]))merge	[sec].[SystemAction] as tusing	cte_data as son		1=1 and t.[Name] = s.[Name]when matched then	update set	[Name] = s.[Name],[RequiredPermissions] = s.[RequiredPermissions]when not matched by target then	insert([Name],[RequiredPermissions])	values(s.[Name],s.[RequiredPermissions]);--[sec].[BusinessEntityModule]------------------------201 to 300----------------------------------------------
begin
set nocount on;;with cte_data([ID],[OldId],[Name],[ParentId],[OldParentId],[BreakInheritance])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('35753BAD-CEEE-4F4B-9EBE-80EFFFBF67CA',null,'Numbering Plan','5A9E78AE-229E-41B9-9DBF-492997B42B61',1,0)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[OldId],[Name],[ParentId],[OldParentId],[BreakInheritance]))merge	[sec].[BusinessEntityModule] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[OldId] = s.[OldId],[Name] = s.[Name],[ParentId] = s.[ParentId],[OldParentId] = s.[OldParentId],[BreakInheritance] = s.[BreakInheritance]when not matched by target then	insert([ID],[OldId],[Name],[ParentId],[OldParentId],[BreakInheritance])	values(s.[ID],s.[OldId],s.[Name],s.[ParentId],s.[OldParentId],s.[BreakInheritance]);
--------------------------------------------------------------------------------------------------------------
end--[sec].[BusinessEntity]--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('5B732E53-C47C-4885-B516-35E60BAF214C',null,'VR_NumberingPlan_CodeGroup','CodeGroup'					,'35753BAD-CEEE-4F4B-9EBE-80EFFFBF67CA',null,0,'["View","Add","Edit","UploadList"]'),('DB827E90-C23E-4AB6-A725-4EB83C21F5FE',null,'VR_NumberingPlan_SellingNumberPlan','SellingNumberPlan'	,'35753BAD-CEEE-4F4B-9EBE-80EFFFBF67CA',null,0,'["View","Add","Edit"]'),('B84ACE6E-3A57-4582-81D7-A7F47FCC23C8',null,'VR_NumberingPlan_SaleZone','SaleZone'						,'35753BAD-CEEE-4F4B-9EBE-80EFFFBF67CA',null,0,'["View"]'),('A54A6B25-81A1-4A7D-A297-CCBEC44C336D',null,'VR_NumberingPlan_SaleCode','SaleCode'						,'35753BAD-CEEE-4F4B-9EBE-80EFFFBF67CA',null,0,'["View"]'),('B615F6EF-107E-469D-8DAE-407F208CF9B7',null,'VR_NumberingPlan_NumberingPlan','Numbering Plan'			,'35753BAD-CEEE-4F4B-9EBE-80EFFFBF67CA',null,0,'["View"]')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions]))merge	[sec].[BusinessEntity] as tusing	cte_data as son		1=1 and t.[Id] = s.[Id]when matched then	update set	[OldId] = s.[OldId],[Name] = s.[Name],[Title] = s.[Title],[ModuleId] = s.[ModuleId],[OleModuleId] = s.[OleModuleId],[BreakInheritance] = s.[BreakInheritance],[PermissionOptions] = s.[PermissionOptions]when not matched by target then	insert([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions])	values(s.[Id],s.[OldId],s.[Name],s.[Title],s.[ModuleId],s.[OleModuleId],s.[BreakInheritance],s.[PermissionOptions]);--to be updated wtih new records related to VR_NumberingPlan--[genericdata].[BusinessEntityDefinition]----------------------------------------------------------
BEGIN
set nocount on;;with cte_data([ID],[OldID],[Name],[Title],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('10740F30-5A20-4718-B5AF-0E2E160B21C2',4,'WHS_BE_SaleZone','Sale Zone'							,'{"$type":"Vanrise.GenericData.Entities.StaticBEDefinitionSettings, Vanrise.GenericData.Entities","SelectorUIControl":"vr-whs-be-salezone-masterplan-selector","ManagerFQTN":"TOne.WhS.BusinessEntity.Business.SaleZoneManager, TOne.WhS.BusinessEntity.Business","IdType":"System.Int64"}'),('2EC2FB2D-2343-40EB-B72A-9A90F99DF0C7',32,'WHS_BE_SellingNumberPlan','Selling Number Plan'		,'{"$type":"Vanrise.GenericData.Entities.StaticBEDefinitionSettings, Vanrise.GenericData.Entities","SelectorUIControl":"vr-whs-be-sellingnumberplan-selector","GroupSelectorUIControl":"","ManagerFQTN":"TOne.WhS.BusinessEntity.Business.SellingNumberPlanManager, TOne.WhS.BusinessEntity.Business","IdType":"System.Int32"}'),('F650D523-7ADB-4787-A2F6-C13168F7E8F7',31,'Whs_BE_SaleZoneMasterPlan','Master Sale Zone'		,'{"$type":"Vanrise.GenericData.Entities.StaticBEDefinitionSettings, Vanrise.GenericData.Entities","SelectorUIControl":"vr-whs-be-salezone-masterplan-selector","ManagerFQTN":"TOne.WhS.BusinessEntity.Business.SaleZoneManager, TOne.WhS.BusinessEntity.Business","IdType":"System.Int64"}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[OldID],[Name],[Title],[Settings]))merge	[genericdata].[BusinessEntityDefinition] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[OldID] = s.[OldID],[Name] = s.[Name],[Title] = s.[Title],[Settings] = s.[Settings]when not matched by target then	insert([ID],[OldID],[Name],[Title],[Settings])	values(s.[ID],s.[OldID],s.[Name],s.[Title],s.[Settings]);
----------------------------------------------------------------------------------------------------
END