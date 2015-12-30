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
--TOneWhS_BE.CarrierAccount-------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [TOneWhS_BE].[CarrierAccount] on;
;with cte_data([ID],[Name],[CarrierProfileID],[AccountType],[SupplierSettings],[CustomerSettings],[CarrierAccountSettings],[SellingNumberPlanID])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Protel',1,1,'{"$type":"TOne.WhS.BusinessEntity.Entities.CarrierAccountSupplierSettings, TOne.WhS.BusinessEntity.Entities","RoutingStatus":0}','{"$type":"TOne.WhS.BusinessEntity.Entities.CarrierAccountCustomerSettings, TOne.WhS.BusinessEntity.Entities","RoutingStatus":0}','{"$type":"TOne.WhS.BusinessEntity.Entities.CarrierAccountSettings, TOne.WhS.BusinessEntity.Entities","ActivationStatus":0}',1),
(2,'Spactron',2,3,'{"$type":"TOne.WhS.BusinessEntity.Entities.CarrierAccountSupplierSettings, TOne.WhS.BusinessEntity.Entities","RoutingStatus":0}','{"$type":"TOne.WhS.BusinessEntity.Entities.CarrierAccountCustomerSettings, TOne.WhS.BusinessEntity.Entities","RoutingStatus":0}','{"$type":"TOne.WhS.BusinessEntity.Entities.CarrierAccountSettings, TOne.WhS.BusinessEntity.Entities","ActivationStatus":0}',1),
(3,'Sama',3,2,'{"$type":"TOne.WhS.BusinessEntity.Entities.CarrierAccountSupplierSettings, TOne.WhS.BusinessEntity.Entities","RoutingStatus":0}','{"$type":"TOne.WhS.BusinessEntity.Entities.CarrierAccountCustomerSettings, TOne.WhS.BusinessEntity.Entities","RoutingStatus":0}','{"$type":"TOne.WhS.BusinessEntity.Entities.CarrierAccountSettings, TOne.WhS.BusinessEntity.Entities","ActivationStatus":0}',null),
(4,'MHD',4,2,'{"$type":"TOne.WhS.BusinessEntity.Entities.CarrierAccountSupplierSettings, TOne.WhS.BusinessEntity.Entities","RoutingStatus":0}','{"$type":"TOne.WhS.BusinessEntity.Entities.CarrierAccountCustomerSettings, TOne.WhS.BusinessEntity.Entities","RoutingStatus":0}','{"$type":"TOne.WhS.BusinessEntity.Entities.CarrierAccountSettings, TOne.WhS.BusinessEntity.Entities","ActivationStatus":0}',null),
(5,'Nettalk',5,2,'{"$type":"TOne.WhS.BusinessEntity.Entities.CarrierAccountSupplierSettings, TOne.WhS.BusinessEntity.Entities","RoutingStatus":0}','{"$type":"TOne.WhS.BusinessEntity.Entities.CarrierAccountCustomerSettings, TOne.WhS.BusinessEntity.Entities","RoutingStatus":0}','{"$type":"TOne.WhS.BusinessEntity.Entities.CarrierAccountSettings, TOne.WhS.BusinessEntity.Entities","ActivationStatus":0}',null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[CarrierProfileID],[AccountType],[SupplierSettings],[CustomerSettings],[CarrierAccountSettings],[SellingNumberPlanID]))
merge	[TOneWhS_BE].[CarrierAccount] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[CarrierProfileID] = s.[CarrierProfileID],[AccountType] = s.[AccountType],[SupplierSettings] = s.[SupplierSettings],[CustomerSettings] = s.[CustomerSettings],[CarrierAccountSettings] = s.[CarrierAccountSettings],[SellingNumberPlanID] = s.[SellingNumberPlanID]
when not matched by target then
	insert([ID],[Name],[CarrierProfileID],[AccountType],[SupplierSettings],[CustomerSettings],[CarrierAccountSettings],[SellingNumberPlanID])
	values(s.[ID],s.[Name],s.[CarrierProfileID],s.[AccountType],s.[SupplierSettings],s.[CustomerSettings],s.[CarrierAccountSettings],s.[SellingNumberPlanID])
when not matched by source then
	delete;
set identity_insert [TOneWhS_BE].[CarrierAccount] off;

--TOneWhS_BE.CarrierProfile-------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [TOneWhS_BE].[CarrierProfile] on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Protel Profile','{"$type":"TOne.WhS.BusinessEntity.Entities.CarrierProfileSettings, TOne.WhS.BusinessEntity.Entities","Company":"Protel","CountryId":1,"CityId":1,"CompanyLogo":0,"Contacts":{"$type":"System.Collections.Generic.List`1[[TOne.WhS.BusinessEntity.Entities.CarrierContact, TOne.WhS.BusinessEntity.Entities]], mscorlib","$values":[]}}'),
(2,'Spactron Profile','{"$type":"TOne.WhS.BusinessEntity.Entities.CarrierProfileSettings, TOne.WhS.BusinessEntity.Entities","Company":"Spactron LTD","CountryId":1,"CityId":1,"CompanyLogo":0,"Contacts":{"$type":"System.Collections.Generic.List`1[[TOne.WhS.BusinessEntity.Entities.CarrierContact, TOne.WhS.BusinessEntity.Entities]], mscorlib","$values":[]}}'),
(3,'Sama Profile','{"$type":"TOne.WhS.BusinessEntity.Entities.CarrierProfileSettings, TOne.WhS.BusinessEntity.Entities","Company":"Sama","CountryId":1,"CityId":1,"CompanyLogo":0,"Contacts":{"$type":"System.Collections.Generic.List`1[[TOne.WhS.BusinessEntity.Entities.CarrierContact, TOne.WhS.BusinessEntity.Entities]], mscorlib","$values":[]}}'),
(4,'MHD Profile','{"$type":"TOne.WhS.BusinessEntity.Entities.CarrierProfileSettings, TOne.WhS.BusinessEntity.Entities","Company":"MHD","CountryId":1,"CityId":1,"CompanyLogo":0,"Contacts":{"$type":"System.Collections.Generic.List`1[[TOne.WhS.BusinessEntity.Entities.CarrierContact, TOne.WhS.BusinessEntity.Entities]], mscorlib","$values":[]}}'),
(5,'Nettalk Profile','{"$type":"TOne.WhS.BusinessEntity.Entities.CarrierProfileSettings, TOne.WhS.BusinessEntity.Entities","Company":"Nettalk S.A.L","CountryId":1,"CityId":1,"CompanyLogo":0,"Contacts":{"$type":"System.Collections.Generic.List`1[[TOne.WhS.BusinessEntity.Entities.CarrierContact, TOne.WhS.BusinessEntity.Entities]], mscorlib","$values":[]}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings]))
merge	[TOneWhS_BE].[CarrierProfile] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Settings])
	values(s.[ID],s.[Name],s.[Settings])
when not matched by source then
	delete;
set identity_insert [TOneWhS_BE].[CarrierProfile] off;

--TOneWhS_BE.CodeGroup------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [TOneWhS_BE].[CodeGroup] on;
;with cte_data([ID],[CountryID],[Code])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(11,1,'961'),
(12,2,'963'),
(13,3,'49'),
(14,4,'33'),
(15,5,'98')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[CountryID],[Code]))
merge	[TOneWhS_BE].[CodeGroup] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[CountryID] = s.[CountryID],[Code] = s.[Code]
when not matched by target then
	insert([ID],[CountryID],[Code])
	values(s.[ID],s.[CountryID],s.[Code])
when not matched by source then
	delete;
set identity_insert [TOneWhS_BE].[CodeGroup] off;

--TOneWhS_BE.CustomerSellingProduct-----------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [TOneWhS_BE].[CustomerSellingProduct] on;
;with cte_data([ID],[CustomerID],[SellingProductID],[AllDestinations],[BED],[EED])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,1,1,null,'2015-12-28 14:28:00.000',null),
(2,2,1,null,'2015-12-28 14:30:00.000',null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[CustomerID],[SellingProductID],[AllDestinations],[BED],[EED]))
merge	[TOneWhS_BE].[CustomerSellingProduct] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[CustomerID] = s.[CustomerID],[SellingProductID] = s.[SellingProductID],[AllDestinations] = s.[AllDestinations],[BED] = s.[BED],[EED] = s.[EED]
when not matched by target then
	insert([ID],[CustomerID],[SellingProductID],[AllDestinations],[BED],[EED])
	values(s.[ID],s.[CustomerID],s.[SellingProductID],s.[AllDestinations],s.[BED],s.[EED])
when not matched by source then
	delete;
set identity_insert [TOneWhS_BE].[CustomerSellingProduct] off;

--TOneWhS_BE.RoutingProduct-------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [TOneWhS_BE].[RoutingProduct] on;
;with cte_data([ID],[Name],[SellingNumberPlanID],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Default Routing Product',1,'{"$type":"TOne.WhS.BusinessEntity.Entities.RoutingProductSettings, TOne.WhS.BusinessEntity.Entities","DefaultServicesFlag":0,"ZoneRelationType":0,"SupplierRelationType":0}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[SellingNumberPlanID],[Settings]))
merge	[TOneWhS_BE].[RoutingProduct] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[SellingNumberPlanID] = s.[SellingNumberPlanID],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[SellingNumberPlanID],[Settings])
	values(s.[ID],s.[Name],s.[SellingNumberPlanID],s.[Settings])
when not matched by source then
	delete;
set identity_insert [TOneWhS_BE].[RoutingProduct] off;

--TOneWhS_BE.SellingNumberPlan----------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [TOneWhS_BE].[SellingNumberPlan] on;
;with cte_data([ID],[Name])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Default')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name]))
merge	[TOneWhS_BE].[SellingNumberPlan] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name]
when not matched by target then
	insert([ID],[Name])
	values(s.[ID],s.[Name])
when not matched by source then
	delete;
set identity_insert [TOneWhS_BE].[SellingNumberPlan] off;

--TOneWhS_BE.SellingProduct-------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [TOneWhS_BE].[SellingProduct] on;
;with cte_data([ID],[Name],[DefaultRoutingProductID],[SellingNumberPlanID],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Default Selling Product',null,1,'null')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[DefaultRoutingProductID],[SellingNumberPlanID],[Settings]))
merge	[TOneWhS_BE].[SellingProduct] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[DefaultRoutingProductID] = s.[DefaultRoutingProductID],[SellingNumberPlanID] = s.[SellingNumberPlanID],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[DefaultRoutingProductID],[SellingNumberPlanID],[Settings])
	values(s.[ID],s.[Name],s.[DefaultRoutingProductID],s.[SellingNumberPlanID],s.[Settings])
when not matched by source then
	delete;
set identity_insert [TOneWhS_BE].[SellingProduct] off;

--TOneWhS_BE.Switch---------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [TOneWhS_BE].[Switch] on;
;with cte_data([ID],[Name])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Default Switch')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name]))
merge	[TOneWhS_BE].[Switch] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name]
when not matched by target then
	insert([ID],[Name])
	values(s.[ID],s.[Name])
when not matched by source then
	delete;
set identity_insert [TOneWhS_BE].[Switch] off;


DELETE FROM TOneWhS_BE.SalePriceList
DBCC CHECKIDENT ('TOneWhS_BE.SalePriceList',RESEED, 0)

TRUNCATE TABLE TOneWhS_BE.SupplierCode
TRUNCATE TABLE TOneWhS_BE.SupplierRate
DELETE FROM TOneWhS_BE.SupplierZone

DELETE FROM TOneWhS_BE.SupplierPriceList

TRUNCATE TABLE TOneWhS_BE.SaleCode
TRUNCATE TABLE TOneWhS_BE.SaleRate
DELETE FROM TOneWhS_BE.SaleZone

TRUNCATE TABLE TOneWhS_BE.CP_SaleCode_Changed
TRUNCATE TABLE TOneWhS_BE.CP_SaleCode_New
TRUNCATE TABLE TOneWhS_BE.CP_SaleZone_Changed
TRUNCATE TABLE TOneWhS_BE.CP_SaleZone_New
TRUNCATE TABLE TOneWhS_BE.CustomerZone


TRUNCATE TABLE TOneWhS_BE.SPL_SupplierCode_Changed
TRUNCATE TABLE TOneWhS_BE.SPL_SupplierCode_New
TRUNCATE TABLE TOneWhS_BE.SPL_SupplierRate_Changed
TRUNCATE TABLE TOneWhS_BE.SPL_SupplierRate_New
TRUNCATE TABLE TOneWhS_BE.SPL_SupplierZone_Changed
TRUNCATE TABLE TOneWhS_BE.SPL_SupplierZone_New


 
TRUNCATE TABLE TOneWhs_CP.CodePreparation
TRUNCATE TABLE TOneWhS_Routing.RoutingDatabase
TRUNCATE TABLE TOneWhS_Sales.RatePlan
TRUNCATE TABLE TOneWhS_SPL.SupplierCode_Preview
TRUNCATE TABLE TOneWhS_SPL.SupplierRate_Preview
TRUNCATE TABLE TOneWhS_SPL.SupplierZone_Preview