-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Sales].[sp_CleanTemporaryTableRP]
@ProcessInstanceId bigint
AS
BEGIN
delete from [TOneWhS_Sales].RP_CustomerCountry_Changed
where ProcessInstanceID = @ProcessInstanceId

delete from [TOneWhS_Sales].RP_CustomerCountry_ChangedPreview
where ProcessInstanceID = @ProcessInstanceId

delete from [TOneWhS_Sales].RP_CustomerCountry_New
where ProcessInstanceID = @ProcessInstanceId

delete from [TOneWhS_Sales].RP_CustomerCountry_NewPreview
where ProcessInstanceID = @ProcessInstanceId

delete from [TOneWhS_Sales].RP_DefaultRoutingProduct_Changed
where ProcessInstanceID = @ProcessInstanceId

delete from [TOneWhS_Sales].RP_DefaultRoutingProduct_New
where ProcessInstanceID = @ProcessInstanceId

delete from [TOneWhS_Sales].RP_DefaultRoutingProduct_Preview
where ProcessInstanceID = @ProcessInstanceId

delete from [TOneWhS_Sales].RP_DefaultService_Changed
where ProcessInstanceID = @ProcessInstanceId

delete from [TOneWhS_Sales].RP_DefaultService_New
where ProcessInstanceID = @ProcessInstanceId

delete from [TOneWhS_Sales].RP_DefaultService_Preview
where ProcessInstanceID = @ProcessInstanceId

delete from [TOneWhS_Sales].RP_RatePlanPreview_Summary
where ProcessInstanceID = @ProcessInstanceId

delete from [TOneWhS_Sales].RP_SaleRate_Changed
where ProcessInstanceID = @ProcessInstanceId

delete from [TOneWhS_Sales].RP_SaleRate_New
where ProcessInstanceID = @ProcessInstanceId


delete from [TOneWhS_Sales].RP_SaleRate_Preview
where ProcessInstanceID = @ProcessInstanceId

delete from [TOneWhS_Sales].RP_SaleZoneRoutingProduct_Changed
where ProcessInstanceID = @ProcessInstanceId

delete from [TOneWhS_Sales].RP_SaleZoneRoutingProduct_New
where ProcessInstanceID = @ProcessInstanceId

delete from [TOneWhS_Sales].RP_SaleZoneRoutingProduct_Preview
where ProcessInstanceID = @ProcessInstanceId

delete from [TOneWhS_Sales].RP_SaleZoneService_Changed
where ProcessInstanceID = @ProcessInstanceId

delete from [TOneWhS_Sales].RP_SaleZoneService_New
where ProcessInstanceID = @ProcessInstanceId

delete from [TOneWhS_Sales].RP_SaleZoneService_Preview
where ProcessInstanceID = @ProcessInstanceId

delete  from  [TOneWhS_BE].SalePriceList_New
where ProcessInstanceID = @ProcessInstanceId

delete  from  [TOneWhS_BE].SalePricelistCodeChange_New
where BatchID = @ProcessInstanceId

delete  from  [TOneWhS_BE].SalePricelistCustomerChange_New
where BatchID = @ProcessInstanceId

delete  from  [TOneWhS_BE].SalePricelistRateChange_New
where ProcessInstanceID = @ProcessInstanceId

delete  from  [TOneWhS_BE].SalePricelistRPChange_New
where ProcessInstanceID = @ProcessInstanceId

END