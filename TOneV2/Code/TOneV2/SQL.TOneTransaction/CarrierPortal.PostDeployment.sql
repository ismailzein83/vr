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
--[runtime].[SchedulerTaskActionType]------------301 to 400-----------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[ActionTypeInfo])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(301,'Upload PriceList','{"URL":"","SystemType":false,"Editor":"cp-supplierpricelist-uploadsupplierpricelist","FQTN":"CP.SupplierPricelist.Business.PriceListTasks.UploadPriceListTaskAction, CP.SupplierPricelist.Business"}'),
(302,'Get PriceList Result','{"URL":"","SystemType":false,"Editor":"cp-supplierpricelist-resultsupplierpricelist","FQTN":"CP.SupplierPricelist.Business.PriceListTasks.ResultTaskAction, CP.SupplierPricelist.Business"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[ActionTypeInfo]))
merge	[runtime].[SchedulerTaskActionType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[ActionTypeInfo] = s.[ActionTypeInfo]
when not matched by target then
	insert([ID],[Name],[ActionTypeInfo])
	values(s.[ID],s.[Name],s.[ActionTypeInfo]);