set nocount on;
;with cte_data([ID],[Name],[ActionTypeInfo])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(9,'Upload PriceList','{"URL":"","SystemType":false,"Editor":"cp-supplierpricelist-uploadsupplierpricelist","FQTN":"CP.SupplierPricelist.Business.PriceListTasks.UploadPriceListTaskAction, CP.SupplierPricelist.Business"}'),
(10,'Get PriceList Result','{"URL":"","SystemType":false,"Editor":"cp-supplierpricelist-resultsupplierpricelist","FQTN":"CP.SupplierPricelist.Business.PriceListTasks.ResultTaskAction, CP.SupplierPricelist.Business"}')
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
	values(s.[ID],s.[Name],s.[ActionTypeInfo])
when not matched by source then
	delete;