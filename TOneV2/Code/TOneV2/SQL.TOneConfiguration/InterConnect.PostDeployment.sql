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
--[sec].[Module]---------------------------1101 to 1200---------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[Module] on;
;with cte_data([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1101,'Operators',null,1,null,20,0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic]))
merge	[sec].[Module] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Url] = s.[Url],[ParentId] = s.[ParentId],[Icon] = s.[Icon],[Rank] = s.[Rank],[AllowDynamic] = s.[AllowDynamic]
when not matched by target then
	insert([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
	values(s.[Id],s.[Name],s.[Url],s.[ParentId],s.[Icon],s.[Rank],s.[AllowDynamic]);
set identity_insert [sec].[Module] off;

--[sec].[View]-----------------------------11001 to 12000-------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[View] on;
;with cte_data([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(11001,'Operator Profiles','Operator Profiles','#/view/InterConnect_BusinessEntity/Views/OperatorProfile/OperatorProfileManagement',1101,null,null,null,null,0,1),
(11002,'Operator Accounts','Operator Accounts','#/view/InterConnect_BusinessEntity/Views/OperatorAccount/OperatorAccountManagement',1101,null,null,null,null,0,2)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank]
when not matched by target then
	insert([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
	values(s.[Id],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank]);
set identity_insert [sec].[View] off;
--[sec].[BusinessEntityModule]-------------1101 to 1200---------------------------------------------------------
--------------------------------------------------------------------------------------------------------------

--[sec].[BusinessEntity]-------------------3001 to 3300-------------------------------------------------------
--------------------------------------------------------------------------------------------------------------

--[common].[TemplateConfig]----------40001 to 50000---------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------

--[sec].[SystemAction]----------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
--[genericdata].[BusinessEntityDefinition]----------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([Name],[Title],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('InterConnect_BE_OperatorProfile','Operator Profile','{"SelectorUIControl":"vr-interconnect-be-operatorprofile-selector","GroupSelectorUIControl":"","ManagerFQTN":"InterConnect.BusinessEntity.Business.OperatorProfileManager,InterConnect.BusinessEntity.Business", "IdType": "System.Int32"}'),
('InterConnect_BE_OperatorAccount','Operator Account','{"SelectorUIControl":"vr-interconnect-be-operatoraccount-selector","GroupSelectorUIControl":"","ManagerFQTN":"InterConnect.BusinessEntity.Business.OperatorAccountManager, InterConnect.BusinessEntity.Business", "IdType": "System.Int32"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Name],[Title],[Settings]))
merge	[genericdata].[BusinessEntityDefinition] as t
using	cte_data as s
on		1=1 and t.[Name] = s.[Name]
when matched then
	update set
	[Title] = s.[Title],[Settings] = s.[Settings]
when not matched by target then
	insert([Name],[Title],[Settings])
	values(s.[Name],s.[Title],s.[Settings]);

--[common].[Setting]---------------------------601 to 700-------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [common].[Setting] on;
;with cte_data([Id],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(601,'Product Info','VR_Common_ProductInfoTechnicalSettings','General','{"Editor" : "vr-common-productinfotechnicalsettings-editor"}','{"$type":"Vanrise.Entities.ProductInfoTechnicalSettings, Vanrise.Entities","ProductInfo":{"$type":"Vanrise.Entities.ProductInfo, Vanrise.Entities","ProductName":"InterConnect","VersionNumber":"version 0.9"}}',1)
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
set identity_insert [common].[Setting] off;