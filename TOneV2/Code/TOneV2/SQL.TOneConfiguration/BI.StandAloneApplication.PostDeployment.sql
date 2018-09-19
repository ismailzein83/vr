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
--[sec].[View]--------------------------------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('7079BD63-BFE2-4519-9B1B-8158A2F3A12A','System Logs','System Logs',null,'BAAF681E-AB1C-4A64-9A35-3F3951398881',null,null,null,'{"$type":"Vanrise.Common.Business.MasterLogViewSettings, Vanrise.Common.Business","Items":[{"PermissionName":"VRCommon_System_Log: View General Logs","Directive":"vr-log-entry-search","Title":"General"},{"PermissionName":"VRCommon_System_Log: View Action Audit","Directive":"vr-common-actionaudit-search","Title":"Action Audit"}]}','372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',15)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank]
when not matched by target then
	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank]);
--------------------------------------------------------------------------------------------------------------
end

DELETE FROM [sec].[BusinessEntity] WHERE ID IN('A91EFECE-00E1-4900-982F-68F01A7185D0',--'VRCommon_Country','Country'						
											   '174D4A58-4F41-469F-BAF0-B81FB64F6EA1',--'VRCommon_Region','Region'							
											   '9A285D4E-D4A6-4ABA-A5DA-22E7E237E808',--'VRCommon_City','City'								
											   '92EA996E-C5E9-4937-9157-7CD36EF0DA37',--'VRCommon_Currency','Currency'						
											   '8BE95D10-688E-40F3-99C1-86397A51AE9B',--'VRCommon_RateType','Rate Type'						
											   'A1DBA375-456A-4769-AD55-CC12C61C721F',--'VRCommon_TimeZone','Time Zone'
											   'BE46E2DC-CE86-46F5-A071-65656E8CCB25',--'VRCommon_VRConnection','Connections'				
											   'A611A651-B60B-483D-BC83-1C2B667A120A',--'TrafficData','Traffic Data'						
											   'AB794846-853C-4402-A8E4-6F5C3A75F5F2')--,'BillingData','Billing Data'						

DELETE FROM [sec].[BusinessEntityModule] WHERE ID IN ('B6B8F582-4759-43FB-9220-AA7662C366EA',--'System Processes'	
													  '8FAF6AA2-6C00-4C48-8EBF-68B87D2DC493',--'Lookups'			
													  'D9666AEA-9517-4DC5-A3D2-D074B2B99A1C',--'Business Entities'	
													  '520558FA-CF2F-440B-9B58-09C23B6A2E9B',--'Billing'			
													  '16419FE1-ED56-49BA-B609-284A5E21FC07',--'Traffic'		
													  '9BBD7C00-011D-4AC9-8B25-36D3E2A8F7CF')--,'Rules'			

DELETE FROM [sec].[View] WHERE ID IN ('604B2CB5-B839-4E51-8D13-3C1C84D05DEE',--'Countries','Countries','#/view/Co
									  'A1CE55FE-6CF4-4F15-9BC2-8E1F8DF68561',--'Regions','Regions','#/view/Common
									  '25994374-CB99-475B-8047-3CDB7474A083',--'Cities','Cities','#/view/Common/V
									  '9F691B87-4936-4C4C-A757-4B3E12F7E1D9',--'Currencies','Currencies','#/view/
									  'E5CA33D9-18AC-4BA1-8E8E-FB476ECAA9A9',--'Exchange Rates','Currency Exchang
									  '0F111ADC-B7F6-46A4-81BC-72FFDEB305EB',--'Time Zone','Time Zone Management'
									  '4D7BF410-E4C6-4D6F-B519-D6B5C2C2F712',--'Rate Types','Rate Types','#/view/
									  '66DE2441-8A96-41E7-94EA-9F8AF38A3515',--'Style','Style Definitions','#/vie
									  '8AC4B99E-01A0-41D1-AE54-09E679309086',--'Status Definitions','Status Defin
									  '52C580DE-C91F-45E2-8E3A-46E0BA9E7EFD',--'Component Types','Component Types
									  '2CF7E0BE-1396-4305-AA27-11070ACFC18F')--,'Application Visibilities','Applic

DELETE FROM [sec].[Module] WHERE ID IN('FC9D12D3-9CBF-4D99-8748-5C2BDD6C5ED9',--'System Processes',nul
									   'A459D3D0-35AE-4B0E-B267-54436FDA729A',--'Entities Definition',
									   '1037157D-BBC9-4B28-B53F-908936CEC137',--'System Processes'	
									   'E73C4ABA-FD03-4137-B047-F3FB4F7EED03',--'Business Entities'		
									   '89254E36-5D91-4DB1-970F-9BFEF404679A',--'Lookups'		
									   '1C7569FA-43C9-4853-AE4C-1152746A34FD',--'Rules'					
									   '6471DA6F-E4DD-4B2A-BFB6-F8EA498CD37C')--'Billing'

--DELETE FROM [common].[Setting] WHERE ID IN ('81F62AC3-BAE4-4A2F-A60D-A655494625EA',--'Company','VR_Common_Compan
--											'1CB20F2C-A835-4320-AEC7-E034C5A756E9')--'Bank Details','VR_Common_B

--Delete sa
--FROM [sec].[SystemAction] sa LEFT JOIN [sec].[BusinessEntity] be on be.Name like ''+substring(sa.RequiredPermissions,0,13)+'%'
--where  be.Name is null and sa.Name not like '%SEC%'

--DELETE FROM [common].[Currency]
INSERT INTO [common].[Currency]([Symbol],[Name])
SELECT 'USD','USD'

Declare @CurrencyID int
select top 1 @CurrencyID = ID from [common].[Currency] where [Symbol]='USD'
--[common].[Setting]------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('1C833B2D-8C97-4CDD-A1C1-C1B4D9D299DE','System Currency','VR_Common_BaseCurrency','General','{"Editor":"vr-common-currency-settings-editor"}','{"$type":"Vanrise.Entities.CurrencySettingData, Vanrise.Entities","CurrencyId":'+CONVERT(varchar(10),@CurrencyID)+'}',0)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical]))merge	[common].[Setting] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]--when matched then--	update set--	[Name] = s.[Name],[Type] = s.[Type],[Category] = s.[Category],[Settings] = s.[Settings],[Data] = s.[Data],[IsTechnical] = s.[IsTechnical]when not matched by target then	insert([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])	values(s.[ID],s.[Name],s.[Type],s.[Category],s.[Settings],s.[Data],s.[IsTechnical]);

/*
--for testing TOne BI
--[BI].[SchemaConfiguration]------------------------------------------------------------------------
begin 
set nocount on;
set identity_insert [BI].[SchemaConfiguration] on;
;with cte_data([ID],[Name],[DisplayName],[Type],[Configuration],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'ACD','ACD',1,'{"ColumnName":"[Measures].[ACD]","Expression":"","Unit":"min/call"}',null),
(2,'COST','COST',1,'{"ColumnName":"[Measures].[Cost Net]","Expression":"","Unit":"Currency"}',null),
(3,'SALE','SALE',1,'{"ColumnName":"[Measures].[Sale Net]","Expression":"","Unit":"Currency"}',null),
(5,'PROFIT','PROFIT',1,'{"ColumnName":"[Measures].[Profit_CALC]","Expression":"MEMBER [Measures].[Profit_CALC]  AS ([Measures].[Sale Net] - [Measures].[Cost Net])","Unit":"Currency"}',null),
(6,'SUCCESSFUL_ATTEMPTS','ATTEMPTS',1,'{"ColumnName":"[Measures].[SuccessfulAttempts]","Expression":"","Unit":"#Calls"}',null),
(7,'PDD','PDD',1,'{"ColumnName":"[Measures].[PDD]","Exepression":"","Unit":"sec"}',null),
(8,'Supplier','Supplier',0,'{"ColumnID":"[SupplierAccounts].[Carrier Account ID]","ColumnName":"[SupplierAccounts].[Profile Name]"}',3),
(9,'Customer','Customer',0,'{"ColumnID":"[CustomerAccounts].[Carrier Account ID]","ColumnName":"[CustomerAccounts].[Profile Name]"}',2),
(14,'SaleZone','SaleZone',0,'{"ColumnID":"[SaleZones].[Zone ID]","ColumnName":"[SaleZones].[Z One Name]","Expression":""}',null),
(16,'BWASR','BWASR',1,'{"ColumnName":"[Measures].[BWASR]","Expression":"","Unit":"%"}',null),
(19,'Facttable Count','Facttable',1,'{"ColumnName":"[Measures].[Facttable Count]","Expression":"","Unit":"min/call"}',null),
(20,'Duration In Minutes','Duration',1,'{"ColumnName":"[Measures].[Duration In Minutes]","Expression":"","Unit":"min"}',null),
(21,'ICABR','ICABR',1,'{"ColumnName":"[Measures].[ICABR]","Expression":"","Unit":"%"}',null),
(22,'ICASR','ICASR',1,'{"ColumnName":"[Measures].[ICASR]","Expression":"","Unit":"%"}',null),
(23,'ICCCR','ICCCR',1,'{"ColumnName":"[Measures].[ICCCR]","Expression":"","Unit":"%"}',null),
(27,'ICNER','ICNER',1,'{"ColumnName":"[Measures].[ICNER]","Expression":"","Unit":"%"}',null),
(28,'IIR','IIR',1,'{"ColumnName":"[Measures].[IIR]","Expression":"","Unit":"%"}',null),
(29,'MHT','MHT',1,'{"ColumnName":"[Measures].[MHT]","Expression":"","Unit":"sec"}',null),
(30,'MHT-Per-Call','MHT-Per-Call',1,'{"ColumnName":"[Measures].[MHT-Per-Call]","Expression":"","Unit":"sec"}',null),
(31,'OGABR','OGABR',1,'{"ColumnName":"[Measures].[OGABR]","Expression":"","Unit":"%"}',null),
(32,'OGASR','OGASR',1,'{"ColumnName":"[Measures].[OGASR]","Expression":"","Unit":"%"}',null),
(33,'OGCCR','OGCCR',1,'{"ColumnName":"[Measures].[OGCCR]","Expression":"","Unit":"%"}',null),
(34,'OGNER','OGNER',1,'{"ColumnName":"[Measures].[OGNER]","Expression":"","Unit":"%"}',null),
(35,'OIR','OIR',1,'{"ColumnName":"[Measures].[OIR]","Expression":"","Unit":"%"}',null),
(36,'PDD-Per-Call','PDD-Per-Call',1,'{"ColumnName":"[Measures].[PDD-Per-Call]","Expression":"","Unit":"sec"}',null),
(37,'Seizures','Seizures',1,'{"ColumnName":"[Measures].[Seizures]","Expression":"","Unit":"calls"}',null),
(41,'Switch','Switch',0,'{"ColumnID":"[Switch].[Switch ID]","ColumnName":"[Switch].[Name]","Expression":""}',null),
(42,'SaleRate','SaleRate',0,'{"ColumnID":"[SaleRate].[Rate ID]","ColumnName":"[SaleRate].[Rate]","Expression":""}',null),
(43,'CostZones','CostZones',0,'{"ColumnID":"[CostZones].[ID]","ColumnName":"[CostZones].[Z One Name]","Expression":""}',null),
(48,'Customer Priced Duration In Minutes','Priced sale duration ',1,'{"ColumnName":"[Measures].[Customer Priced Duration In Minutes]","Expression":"","Unit":"min"}',null),
(50,'Supplier Priced Duration In Minutes','Priced cost duration',1,'{"ColumnName":"[Measures].[Supplier Priced Duration In Minutes]","Expression":"","Unit":"min"}',null),
(88,'Default Time','Default Time',2,'{"Date":"[Date].[Date]","Year":"[Date].[Year]","MonthOfYear":"[Date].[Month Of Year]","WeekOfMonth":"[Date].[Week Of Month]","DayOfMonth":"[Date].[Day Of Month]","Hour":"[Date].[Hour]","IsDefault":"True"}',null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[DisplayName],[Type],[Configuration],[Rank]))
merge	[BI].[SchemaConfiguration] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[DisplayName] = s.[DisplayName],[Type] = s.[Type],[Configuration] = s.[Configuration],[Rank] = s.[Rank]
when not matched by target then
	insert([ID],[Name],[DisplayName],[Type],[Configuration],[Rank])
	values(s.[ID],s.[Name],s.[DisplayName],s.[Type],s.[Configuration],s.[Rank]);
set identity_insert [BI].[SchemaConfiguration] off;
----------------------------------------------------------------------------------------------------
end

*/
